using UnityEngine;

public class HoeAction : IGridAction
{
    private readonly GameObject _farmlandPrefab;

    public HoeAction(Database database)
    {
        var farmland = database.ItemPlacements.Find(d => d.displayName == "Farmland");
        _farmlandPrefab = farmland?.prefab;
    }

    public bool CanActOn(Vector3Int gridPosition, GridData gridData, ToolUseType useType)
    {
        return !gridData.IsPositionOccupied(gridPosition);
    }

    public bool Execute(Vector3Int gridPosition, GridData gridData,
        Grid grid, ObjectPlacer objectPlacer, Database database, 
        InventorySlot slot, ToolUseType useType)
    {
        switch (useType)
        {
            case ToolUseType.Primary:
                int index = objectPlacer.PlaceObject(_farmlandPrefab, grid.CellToWorld(gridPosition));
                gridData.AddObjectAt(gridPosition, Vector2Int.one, 
                    PlacementSystem.Instance.FarmlandId, index);
                return true;
            default:
                return false;
        }
    }
    
    public int GetRadius(ToolUseType useType)
    {
        return 3;
    }
}