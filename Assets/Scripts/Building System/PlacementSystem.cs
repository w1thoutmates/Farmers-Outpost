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
    
    [SerializeField] private HotbarDisplay hotbarDisplay;
    
    [SerializeField] private ToolState toolState;
    private bool _isToolModeActive = false;
    
    public bool IsToolModeActive => _isToolModeActive;
    public int FarmlandId => 0;

    private Vector3Int _lastDetectedPosition = Vector3Int.zero; 
    private Vector3Int _lastPlayerCell;
    private GridData _objectPlacementData;
    private PlacementState _buildingState;
    
    private int _currentPlacementId = -1;
    private int _currentToolId = -1;

    void Update()
    {
        if (_buildingState == null && !_isToolModeActive)
            return;
    
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        float snappedY = Mathf.Round(mousePosition.y * 2f) / 2f;
        Vector3 correctedPosition = new Vector3(mousePosition.x, snappedY, mousePosition.z);
        Vector3Int gridPosition = grid.WorldToCell(correctedPosition);

        Vector3Int playerCell = grid.WorldToCell(Player.Instance.transform.position);

        bool shouldUpdate = false;
    
        if (_buildingState != null)
        {
            shouldUpdate = (_lastDetectedPosition != gridPosition || _lastPlayerCell != playerCell);
        }
        else if (_isToolModeActive)
        {
            shouldUpdate = (_lastDetectedPosition != gridPosition || _lastPlayerCell != playerCell);
        }
    
        if (shouldUpdate)
        {
            if (_buildingState != null)
            {
                _buildingState.UpdateState(gridPosition);
            }
            else if (_isToolModeActive)
            {
                toolState.UpdateState(gridPosition);
            }

            _lastDetectedPosition = gridPosition;
            _lastPlayerCell = playerCell;
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
        toolState = new ToolState(grid, preview, _objectPlacementData, objectPlacer, database, 4);
        StopPlacement();
    }
    
    public void StartPlacement(int id)
    {
        if (InventoryUIController.Instance != null &&
            InventoryUIController.Instance.IsAnyInventoryOpen)
            return;
        
        if (_isPlacementModeActive && _currentPlacementId == id)
            return;

        StopToolMode();
        StopPlacement();
        
        _currentPlacementId = id;
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
            InventorySlot activeSlot = hotbarDisplay.GetActiveSlot();
            if (activeSlot != null)
            {
                EventBus.NotifyPlacementItemUsed(activeSlot);
            }
        }
    }
    
    public void StopPlacement()
    {
        _isPlacementModeActive = false;

        if (_buildingState != null)
        {
            _buildingState.EndState();
            _buildingState = null;
        }

        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;

        gridVisualisation.SetActive(false);
        _lastDetectedPosition = Vector3Int.zero;
    }
    
    public void StartToolMode(int toolId)
    {
        if (InventoryUIController.Instance != null &&
            InventoryUIController.Instance.IsAnyInventoryOpen)
            return;
        
        if (_isToolModeActive && _currentToolId == toolId)
            return;

        StopPlacement();
        StopToolMode();

        _currentToolId = toolId;
        _isPlacementModeActive = false;
        _isToolModeActive = true;
        gridVisualisation.SetActive(true);
        
        toolState.SetTool(toolId);
        toolState.EnterState();
        
        inputManager.OnClicked += UseTool;
        inputManager.OnExit += StopToolMode;
    }
    
    void UseTool()
    {
        if (!_isToolModeActive || inputManager.IsPointerOverUIObject())
        {
            return;
        }
        
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        float snappedY = Mathf.Round(mousePosition.y * 2f) / 2f;
        Vector3 correctedPosition = new Vector3(mousePosition.x, snappedY, mousePosition.z);
        Vector3Int gridPosition = grid.WorldToCell(correctedPosition);
        
        bool used = toolState.OnAction(gridPosition);
        
        if (used)
        {
            InventorySlot activeSlot = hotbarDisplay.GetActiveSlot();
            if (activeSlot != null)
            {
                EventBus.NotifyToolWasUsed(activeSlot);
            }
        }
    }
    
    public void StopToolMode()
    {
        if (!_isToolModeActive) return;
        
        _isToolModeActive = false;
        gridVisualisation.SetActive(false);
        toolState.ExitState();
        
        PreviewSystem.Instance.HideToolIndicator();
        
        inputManager.OnClicked -= UseTool;
        inputManager.OnExit -= StopToolMode;
        _lastDetectedPosition = Vector3Int.zero;
    }
}
