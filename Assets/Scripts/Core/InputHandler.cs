using Unity.VisualScripting;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    private GridController gridController;
    private Camera mainCamera;

    public void Init(GridController gridController)
    {
        this.gridController = gridController;
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            HandleClick(Input.mousePosition);
        }

        //Mobile Input
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            HandleClick(Input.GetTouch(0).position);
        }
    }



    private void HandleClick(Vector2 screenPosition) 
    {
        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

        if(hit.collider != null)
        {
            Crystal crystal = hit.collider.gameObject.GetComponent<Crystal>();

            if (hit.collider.TryGetComponent(out Crystal targetCrystal))
            {
                gridController.OnCrystalClicked(targetCrystal);
                //dfs
            }
        }
    }
}
