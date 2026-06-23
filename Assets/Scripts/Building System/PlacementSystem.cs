using System;
using System.Collections.Generic;
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

    [SerializeField] public Database database;
    [SerializeField] private GameObject gridVisualisation;

    private GridData objectPlacementData;
    private Renderer previewRenderer;

    private List<GameObject> placedGameObjects = new();

    private int _selectedObjectIndex { get; set; } = -1;

    void Update()
    {
        if (_selectedObjectIndex < 0)
            return;
        
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        float snappedY = Mathf.Round(mousePosition.y * 2f) / 2f;
        Vector3 correctedPosition = new Vector3(mousePosition.x, snappedY, mousePosition.z);
        Vector3Int gridPosition = grid.WorldToCell(correctedPosition);
        
        bool placementValidity = CheckPlacementValidity(gridPosition, _selectedObjectIndex);
        previewRenderer.material.color = placementValidity ? Color.white : Color.red;
        
        mouseIndicator.transform.position = mousePosition;
        Vector3 cellWorldPos = grid.CellToWorld(gridPosition);
        cellIndicator.transform.position = cellWorldPos;
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        return objectPlacementData.CanPlaceObjectAt(gridPosition, database.ItemPlacements[selectedObjectIndex].size);
    }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        previewRenderer = cellIndicator.GetComponentInChildren<Renderer>();
    }
    
    void Start()
    {
        objectPlacementData = new();
        StopPlacement();
    }
    
    public void StartPlacement(int id)
    {
        if (InventoryUIController.Instance != null &&
            InventoryUIController.Instance.IsAnyInventoryOpen)
            return;
        
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

        bool placementValidity = CheckPlacementValidity(gridPosition, _selectedObjectIndex);
        if (!placementValidity)
            return;
        
        GameObject newObject = Instantiate(database.ItemPlacements[_selectedObjectIndex].prefab);
        Vector3 cellWorldPos = grid.CellToWorld(gridPosition);
        newObject.transform.position = cellWorldPos;
        placedGameObjects.Add(newObject);
        
        objectPlacementData.AddObjectAt(
            gridPosition,
            database.ItemPlacements[_selectedObjectIndex].size,
            database.ItemPlacements[_selectedObjectIndex].id,
            placedGameObjects.Count - 1
        );
        
        EventBus.NotifyPlacementItemUsed();
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
    
    public bool CanPlaceCurrentObject()
    {
        if (_selectedObjectIndex < 0)
            return false;

        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        float snappedY = Mathf.Round(mousePosition.y * 2f) / 2f;
        Vector3 correctedPosition = new Vector3(mousePosition.x, snappedY, mousePosition.z);
        Vector3Int gridPosition = grid.WorldToCell(correctedPosition);

        return CheckPlacementValidity(gridPosition, _selectedObjectIndex);
    }

    public void ChangeSelectedObjectIndex(int newIndex)
    {
        _selectedObjectIndex = newIndex;
    }
}
