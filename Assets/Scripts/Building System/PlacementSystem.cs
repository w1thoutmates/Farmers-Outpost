using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    public static PlacementSystem Instance;
    
    private bool _isPlacementModeActive = false;
    
    public bool IsPlacementModeActive => _isPlacementModeActive;
    
    [SerializeField] private GameObject mouseIndicator;
    [SerializeField] private GameObject cellIndicator;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Grid grid;

    [SerializeField] private Database database;
    [SerializeField] private GameObject gridVisualisation;
    
    private int _selectedObjectIndex = -1;

    void Update()
    {
        if (_selectedObjectIndex < 0)
            return;
        
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        float snappedY = Mathf.Round(mousePosition.y * 2f) / 2f;
        Vector3 correctedPosition = new Vector3(mousePosition.x, snappedY, mousePosition.z);
        Vector3Int gridPosition = grid.WorldToCell(correctedPosition);
        mouseIndicator.transform.position = mousePosition;
        Vector3 cellWorldPos = grid.CellToWorld(gridPosition);
        cellIndicator.transform.position = cellWorldPos;
    }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    void Start()
    {
        StopPlacement();
    }
    
    public void StartPlacement(int id)
    {
        StopPlacement();
        _selectedObjectIndex = database.ItemPlacements.FindIndex(data => data.id == id);

        if (_selectedObjectIndex < 0)
        {
            Debug.LogError($"No ID found {id}");
            return;
        }
        
        _isPlacementModeActive = true;
        gridVisualisation.SetActive(true);
        cellIndicator.SetActive(true);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    void PlaceStructure()
    {
        if (!_isPlacementModeActive || inputManager.IsPointerOverUIObject())
        {
            return;
        }
        
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        float snappedY = Mathf.Round(mousePosition.y * 2f) / 2f;
        Vector3 correctedPosition = new Vector3(mousePosition.x, snappedY, mousePosition.z);
        Vector3Int gridPosition = grid.WorldToCell(correctedPosition);
        GameObject newObject = Instantiate(database.ItemPlacements[_selectedObjectIndex].prefab);
        Vector3 cellWorldPos = grid.CellToWorld(gridPosition);
        newObject.transform.position = cellWorldPos;
    }

    public void StopPlacement()
    {
        _selectedObjectIndex = -1;
        _isPlacementModeActive = false;
        gridVisualisation.SetActive(false);
        cellIndicator.SetActive(false);
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
    }
}
