using UnityEngine;

public class GridToolState : IToolState
{
    private readonly IGridAction _action;
    private readonly Grid _grid;
    private readonly PreviewSystem _previewSystem;
    private readonly GridData _gridData;
    private readonly ObjectPlacer _objectPlacer;
    private readonly Database _database;
    private readonly int _radius;

    public GridToolState(
        IGridAction action,
        Grid grid,
        PreviewSystem previewSystem,
        GridData gridData,
        ObjectPlacer objectPlacer,
        Database database,
        int radius = 3)
    {
        _action = action;
        _grid = grid;
        _previewSystem = previewSystem;
        _gridData = gridData;
        _objectPlacer = objectPlacer;
        _database = database;
        _radius = radius;
    }

    public void EnterState() => _previewSystem.ShowToolIndicator(Vector2Int.one);
    public void ExitState()  => _previewSystem.HideToolIndicator();

    public void UpdateState(Vector3Int gridPosition)
    {
        bool primary = CheckActionValidity(gridPosition, ToolUseType.Primary);
        bool secondary = CheckActionValidity(gridPosition, ToolUseType.Secondary);

        bool isValid = primary || secondary;

        _previewSystem.UpdateToolIndicator(
            _grid.CellToWorld(gridPosition),
            isValid);
    }

    public bool OnAction(Vector3Int gridPosition, InventorySlot slot, ToolUseType useType)
    {
        if (!CheckActionValidity(gridPosition, useType)) return false;
        bool result = _action.Execute(gridPosition, _gridData, _grid, _objectPlacer, _database, slot, useType);
        return result;
    }

    public bool CheckActionValidity(Vector3Int gridPosition, ToolUseType useType)
    {
        Vector3Int playerCell = _grid.WorldToCell(Player.Instance.transform.position);
        int radius = _action.GetRadius(useType);
        
        int dx = Mathf.Abs(gridPosition.x - playerCell.x);
        int dz = Mathf.Abs(gridPosition.z - playerCell.z);
        if (dx > radius || dz > radius) return false;

        bool canAct = _action.CanActOn(gridPosition, _gridData, useType);
        return canAct;
    }
}