using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private GameObject mouseIndicator;
    [SerializeField] private GameObject cellIndicator;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Grid grid;
    
    private void Update()
    {
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        float snappedY = Mathf.Round(mousePosition.y * 2f) / 2f;
        Vector3 correctedPosition = new Vector3(mousePosition.x, snappedY, mousePosition.z);
        Vector3Int gridPosition = grid.WorldToCell(correctedPosition);
        mouseIndicator.transform.position = mousePosition;
        Vector3 cellWorldPos = grid.CellToWorld(gridPosition);
        cellIndicator.transform.position = cellWorldPos;
    }
}
