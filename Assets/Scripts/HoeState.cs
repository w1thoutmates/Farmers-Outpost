using UnityEngine;
using System.Collections.Generic;

public class HoeState : IToolState
{
    private Grid _grid;
    private PreviewSystem _previewSystem;
    private GridData _gridData;
    private ObjectPlacer _objectPlacer;
    private Database _database;
    private int _toolId;
    private int _hoeRadius = 3;
    
    private Dictionary<Vector3Int, bool> _hoedPositions = new ();
    
    private GameObject _hoedGroundPrefab;
    
    public HoeState(
        int toolId,
        Grid grid,
        PreviewSystem previewSystem,
        GridData gridData,
        ObjectPlacer objectPlacer,
        Database database)
    {
        _toolId = toolId;
        _grid = grid;
        _previewSystem = previewSystem;
        _gridData = gridData;
        _objectPlacer = objectPlacer;
        _database = database;
        
        var hoedGround = database.ItemPlacements.Find(data => data.displayName == "Farmland");
        if (hoedGround != null)
        {
            _hoedGroundPrefab = hoedGround.prefab;
        }
        else
        {
            Debug.LogError("Hoed ground prefab not found in database!");
        }
    }
    
    public void EnterState()
    {
        _previewSystem.ShowToolIndicator(Vector2Int.one);
    }
    
    public void UpdateState(Vector3Int gridPosition)
    {
        bool primary = CheckActionValidity(gridPosition, ToolUseType.Primary);
        bool secondary = CheckActionValidity(gridPosition, ToolUseType.Secondary);
        
        Vector3 worldPos = _grid.CellToWorld(gridPosition);
        bool isValid = primary || secondary;
        _previewSystem.UpdateToolIndicator(worldPos, isValid);
    }
    
    public bool OnAction(Vector3Int gridPosition, InventorySlot slot, ToolUseType useType)
    {
        if (!CheckActionValidity(gridPosition, useType))
            return false;

        return HoePosition(gridPosition);
    }
    
    public bool CheckActionValidity(Vector3Int gridPosition,ToolUseType useType)
    {
        Vector3Int playerCell = _grid.WorldToCell(Player.Instance.transform.position);

        int dx = Mathf.Abs(gridPosition.x - playerCell.x);
        int dz = Mathf.Abs(gridPosition.z - playerCell.z);

        if (dx > _hoeRadius || dz > _hoeRadius)
            return false;

        return CanHoePosition(gridPosition);
    }
    
    public void ExitState()
    {
        _previewSystem.HideToolIndicator();
    }
    
    private bool CanHoePosition(Vector3Int gridPosition)
    {
        if (_gridData.IsPositionOccupied(gridPosition))
            return false;

        // Проверка типа поверхности

        return true;
    }
    
    private bool HoePosition(Vector3Int gridPosition)
    {
        if (!CanHoePosition(gridPosition))
            return false;

        Vector3 worldPos = _grid.CellToWorld(gridPosition);

        int placedIndex = _objectPlacer.PlaceObject(_hoedGroundPrefab, worldPos);

        _gridData.AddObjectAt(
            gridPosition,
            Vector2Int.one,
            PlacementSystem.Instance.FarmlandId,
            placedIndex);

        _hoedPositions[gridPosition] = true;

        return true;
    }
}