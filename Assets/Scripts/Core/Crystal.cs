using UnityEngine;

public class Crystal : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Collider2D collider;

    [Header ("Parameters")]
    [SerializeField] private float moveSpeed = 1.0f;
    //[SerializeField] private float scaleSpeed = 1.0f;

    [Header("Sprites")]
    [SerializeField] private Sprite[] crystalSprites;


    private Vector2Int gridPosition;
    private Vector3 worldPosition;
    private bool isMoving = false;

    public int type 
    {
        get;
        private set;
    }


    public void Init(int type, Vector2Int gridPosition, Vector3 worldPosition)
    {
        this.type = type;

        SetPositionInGrid(gridPosition, worldPosition);
        UpdateType(this.type);
    }


    public void UpdateType(int newType)
    {
        type = newType;

        if (spriteRenderer != null && newType >= 0 && newType < crystalSprites.Length)
        {
            spriteRenderer.sprite = crystalSprites[newType];
        }
    }

    public void SetPositionInGrid(Vector2Int newGridPosition, Vector3 newWordPostion, bool animate = false)
    {
        this.gridPosition = newGridPosition;
        this.worldPosition = newWordPostion;

        if (animate)
        {
            isMoving = true;
        }
        else
        {
            transform.position = worldPosition;
        }
    }

    public Vector2Int GetGridPostion()
    {
        return gridPosition;
    }

    public void Destroy() 
    {
        gameObject.SetActive(false);
    }

    public void VisualSelect() 
    {
        transform.localScale += Vector3.one * 0.1f;
    }
    public void VisualDeselect() 
    {
        transform.localScale -= Vector3.one * 0.1f;
    }
}
