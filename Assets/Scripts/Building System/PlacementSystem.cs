using System;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    public static PlacementSystem Instance;
    
    private bool _isPlacementModeActive = false;
    
    public bool IsPlacementModeActive => _isPlacementModeActive;
    
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Grid grid;

    [SerializeField] public Database database;
    [SerializeField] private GameObject gridVisualisation;

    [SerializeField] private PreviewSystem preview;
    
    [SerializeField] private ObjectPlacer objectPlacer;

    private Vector3Int _lastDetectedPosition = Vector3Int.zero; 
    private GridData _objectPlacementData;
    private IBuildingState _buildingState;

    void Update()
    {
        if (_buildingState == null)
            return;
        
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        float snappedY = Mathf.Round(mousePosition.y * 2f) / 2f;
        Vector3 correctedPosition = new Vector3(mousePosition.x, snappedY, mousePosition.z);
        Vector3Int gridPosition = grid.WorldToCell(correctedPosition);

        if (_lastDetectedPosition != gridPosition)
        {
            _buildingState.UpdateState(gridPosition);
            _lastDetectedPosition = gridPosition;
        }
    }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    void Start()
    {
        _objectPlacementData = new();
        gridVisualisation.gameObject.SetActive(false);
        StopPlacement();
    }
    
    public void StartPlacement(int id)
    {
        if (InventoryUIController.Instance != null &&
            InventoryUIController.Instance.IsAnyInventoryOpen)
            return;
        
        StopPlacement();
        _isPlacementModeActive = true;
        gridVisualisation.SetActive(true);

        _buildingState = new PlacementState(id, grid, preview, database, _objectPlacementData, objectPlacer);
        
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

        bool placed = _buildingState.OnAction(gridPosition);
        
        if (placed)
        {
            EventBus.NotifyPlacementItemUsed();
        }
    }

    public void StopPlacement()
    {
        if (_buildingState == null) 
            return;
        _isPlacementModeActive = false;
        gridVisualisation.SetActive(false);
        _buildingState.EndState();
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
        _lastDetectedPosition = Vector3Int.zero;
        _buildingState = null;
    }
    
    public bool CanPlaceCurrentObject()
    {
        if (_buildingState == null)
            return false;

        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        float snappedY = Mathf.Round(mousePosition.y * 2f) / 2f;
        Vector3 correctedPosition = new Vector3(mousePosition.x, snappedY, mousePosition.z);
        Vector3Int gridPosition = grid.WorldToCell(correctedPosition);

        return _buildingState.CheckPlacementValidity(gridPosition);
    }
}
