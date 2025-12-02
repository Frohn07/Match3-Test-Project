using UnityEngine;
using System;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using System.Collections;
using UnityEngine.UIElements;

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
                //StartCoroutine();
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


    /*
    private IEnumerator SwapProcces(Vector2Int position1, Vector2Int position2)
    {
        isProcesssing = true;

        gridModel.Swap(position1, position2);

    }
    */
}
