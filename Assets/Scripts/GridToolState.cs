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
    private readonly InventorySlot _slot;

    public GridToolState(
        IGridAction action,
        Grid grid,
        PreviewSystem previewSystem,
        GridData gridData,
        ObjectPlacer objectPlacer,
        Database database,
        InventorySlot slot,
        int radius = 3)
    {
        _action = action;
        _grid = grid;
        _previewSystem = previewSystem;
        _gridData = gridData;
        _objectPlacer = objectPlacer;
        _database = database;
        _radius = radius;
        _slot = slot;
    }

    public void EnterState() => _previewSystem.ShowToolIndicator(Vector2Int.one);
    public void ExitState()  => _previewSystem.HideToolIndicator();

    public void UpdateState(Vector3Int gridPosition)
    {
        bool isValid = CheckActionValidity(gridPosition);
        _previewSystem.UpdateToolIndicator(_grid.CellToWorld(gridPosition), isValid);
    }

    public bool OnAction(Vector3Int gridPosition)
    {
        bool valid = CheckActionValidity(gridPosition);
        Debug.Log($"GridToolState.OnAction: valid={valid}, pos={gridPosition}");
        if (!valid) return false;
        return _action.Execute(gridPosition, _gridData, _grid, _objectPlacer, _database, _slot);
    }

    public bool CheckActionValidity(Vector3Int gridPosition)
    {
        Vector3Int playerCell = _grid.WorldToCell(Player.Instance.transform.position);
        int dx = Mathf.Abs(gridPosition.x - playerCell.x);
        int dz = Mathf.Abs(gridPosition.z - playerCell.z);
        Debug.Log($"CheckValidity: dx={dx}, dz={dz}, radius={_radius}");
        if (dx > _radius || dz > _radius) return false;

        bool canAct = _action.CanActOn(gridPosition, _gridData);
        Debug.Log($"CanActOn: {canAct}");
        return canAct;
    }
}