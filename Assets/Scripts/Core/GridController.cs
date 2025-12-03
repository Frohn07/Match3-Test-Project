using UnityEngine;
using System;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using System.Collections;
using UnityEngine.UIElements;
using System.Collections.Generic;
using UnityEditor.Rendering;

public class GridController : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int gridWidth = 5;
    [SerializeField] private int gridHeight = 5;
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private Vector2 gridOffcet = Vector2.zero;

    [Header("References")]
    [SerializeField] private Crystal crystalPrefab;
    [SerializeField] private Transform gridParent;

    [Header("Game Settings")]
    [SerializeField] private float swapDuration = 1f;
    [SerializeField] private float dropDuration = 1f;
    [SerializeField] private float swapDelay = 1f;
    [SerializeField] private float spawnDelay = 1f;


    private GridModel gridModel;
    private InputHandler inputHandler;
    private PoolManager poolManager;

    private bool isProcesssing = false;
    private Crystal selectCrystal;

    public Action<int> OnMatchFound;
    public Action OnMoveComplete;





    private void Start()
    {
        Init();
        CreateGrid();
    }

    public void Init() 
    {
        gridModel = new GridModel(gridWidth, gridHeight);
        gridModel.InitGrid();

        poolManager = new PoolManager(crystalPrefab, gridParent, gridHeight * gridWidth + 10); // зачем + 10? уточнить

        inputHandler = gameObject.AddComponent<InputHandler>();
        inputHandler.Init(this);
    }

    private void CreateGrid()
    {
        for(int i = 0; i < gridWidth; i++)
        {
            for(int j = 0; j < gridHeight; j++)
            {
                Vector3 worldPosition = GetWorldPostion(i, j);
                Crystal crystal = poolManager.GetCrystal();

                crystal.Init(gridModel.grid[i, j].type, new Vector2Int(i, j), worldPosition);
                gridModel.grid[i, j].view = crystal;

            }
        }
    }


    public Vector3 GetWorldPostion(int x, int y)
    {
        Vector3 result = new Vector3( x * cellSize + gridOffcet.x, y * cellSize + gridOffcet.y, 0);
        return result;
    }

    public void OnCrystalClicked(Crystal crystal)
    {
        if (isProcesssing) return;

        if(selectCrystal == null)
        {
            selectCrystal = crystal;
            crystal.VisualSelect();
        }
        else if(selectCrystal == crystal)
        {
            crystal.VisualDeselect();
            selectCrystal = null;
        }
        else 
        {
            Vector2Int position1 = selectCrystal.GetGridPostion();
            Vector2Int position2 = crystal.GetGridPostion();

            if (AreNeighbors(position1, position2)) 
            {
                StartCoroutine(SwapProcces(position1, position2));
            }
        }
    }

    private bool AreNeighbors(Vector2Int position1, Vector2Int postion2)
    {
      
        int dx = Mathf.Abs(position1.x - postion2.x);
        int dy = Mathf.Abs(position1.y - postion2.y);
        bool result = (dx == 1 && dy == 0) || (dx == 0 && dy == 1);

        return result;
    }


    
    private IEnumerator SwapProcces(Vector2Int position1, Vector2Int position2)
    {
        isProcesssing = true;

        gridModel.Swap(position1, position2);


        Crystal crystal1 = gridModel.grid[position1.x, position1.y].view;
        Crystal crystal2 = gridModel.grid[position2.x, position2.y].view;

        Vector3 target1 = GetWorldPostion(position2.x, position2.y);
        Vector3 target2 = GetWorldPostion(position1.x, position1.y);

        StartCoroutine(MoveCrystal(crystal1, target1, swapDuration));
        StartCoroutine(MoveCrystal(crystal2, target2, swapDuration));

        yield return new WaitForSeconds(swapDuration);

        crystal1.SetPositionInGrid(position2, target1);
        crystal2.SetPositionInGrid(position1, target2);

        List<Vector2Int> mathes = gridModel.FindAllMathes();

        if(mathes.Count > 0)
        {
            yield return StartCoroutine(ProcessMatches(mathes));
        }
        else
        {
            gridModel.Swap(position1, position2);

            StartCoroutine(MoveCrystal(crystal1, GetWorldPostion(position1.x, position1.y), swapDuration));
            StartCoroutine(MoveCrystal(crystal2, GetWorldPostion(position2.x, position2.y), swapDuration));

            yield return new WaitForSeconds(swapDuration);

            crystal1.SetPositionInGrid(position1, GetWorldPostion(position1.x, position1.y));
            crystal1.SetPositionInGrid(position2, GetWorldPostion(position2.x, position2.y));



        }

        isProcesssing = false;
        OnMoveComplete?.Invoke();
    }
    

    private IEnumerator MoveCrystal(Crystal crystal, Vector3 targetPosition, float duration) 
    {
        float elapsed = 0;
        Vector3 start = crystal.transform.position;

        while (elapsed < duration)
        {
            crystal.transform.position = Vector3.Lerp(start, targetPosition, elapsed/duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        crystal.transform.position = targetPosition;

    }

    private IEnumerator ProcessMatches(List<Vector2Int> matches) 
    {
        yield return StartCoroutine(DestroyMathces(matches));

        yield return StartCoroutine(ApplayGravity());

        yield return StartCoroutine(FillEmptySlots());

        List<Vector2Int> newMatches = gridModel.FindAllMathes();

        if(newMatches.Count > 0)
        {
            yield return StartCoroutine(ProcessMatches(newMatches));
        }

        OnMatchFound?.Invoke(matches.Count);
    }

    private IEnumerator DestroyMathces(List<Vector2Int> matches) 
    {
        for(int i = 0; i < matches.Count; i++)
        {
            Vector2Int position = matches[i];
            GridModel.Cell cell = gridModel.grid[position.x, position.y];

            if(cell.view != null)
            {
                cell.view.Destroy();
                cell.view = null;
                cell.isEmpty = true;
            }

        }

        yield return null;
    }

    private IEnumerator ApplayGravity() 
    {
        bool moved = false;

        for(int i = 0; i < gridModel.width; i++)
        {
            int emptyCount = 0;

            for(int j = gridModel.height - 1; j >= 0; j--)
            {
                GridModel.Cell cell = gridModel.grid[i, j];

                if(cell.isEmpty)
                {
                    emptyCount++;
                }

                else if(emptyCount > 0)
                {
                    GridModel.Cell targetCell = gridModel.grid[i, j + emptyCount];

                    targetCell.isEmpty = false;
                    targetCell.view = cell.view;
                    targetCell.type = cell.type;


                    if(targetCell.view != null)
                    {
                        Vector3 targetPosition = GetWorldPostion(i, j + emptyCount);
                        StartCoroutine(MoveCrystal(targetCell.view, targetPosition, dropDuration));
                        targetCell.view.SetPositionInGrid(new Vector2Int(i, j + emptyCount), targetPosition);
                    }

                    cell.type = 0;
                    cell.view = null;
                    cell.isEmpty = true;

                    moved = true;
                }
            }

        }

        if(moved)
        {
            yield return new WaitForSeconds(dropDuration);
        }
    }

    private IEnumerator FillEmptySlots() 
    {
        for(int i = 0; i < gridModel.width; i++)
        {
            for(int j = 0; j < gridModel.height; j++)
            {
                if (gridModel.grid[i, j].isEmpty)
                {
                    int newCellType = UnityEngine.Random.Range(0, gridModel.colorCount);
                    Crystal crystal = poolManager.GetCrystal();

                    Vector3 newPosition = GetWorldPostion(i, gridModel.height);
                    crystal.Init(newCellType, new Vector2Int(i, j), newPosition);

                    Vector3 targetPosition = GetWorldPostion(i, j);
                    StartCoroutine(MoveCrystal(crystal, targetPosition, dropDuration));


                    gridModel.grid[i, j].type = newCellType;
                    gridModel.grid[i, j].view = crystal;
                    gridModel.grid[i, j].isEmpty = false;

                    yield return new WaitForSeconds(spawnDelay);

                }
            }
        }

        yield return new WaitForSeconds(dropDuration);
    }

    private void OnDrawGizmos()
    {
        if(!Application.isPlaying)
        {
            Gizmos.color = Color.white;

            for(int x = 0; x < gridHeight; x++)
            {
                for(int y = 0; y < gridWidth; y++)
                {
                    Vector3 position = GetWorldPostion(x, y);
                    Gizmos.DrawWireCube(position, new Vector3(cellSize * 0.8f, cellSize * 0.8f, 0));
                }
            }
        }
    }
}
