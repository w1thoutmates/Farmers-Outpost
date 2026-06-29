using UnityEngine;

public class WateringAction : IGridAction
{
    private readonly float _waterAmount;
    
    public WateringAction(float waterAmount = 25f)
    {
        _waterAmount = waterAmount;
    }
    
    public bool CanActOn(Vector3Int gridPosition, GridData gridData)
    {
        bool hoed = gridData.IsPositionHoed(gridPosition);
        Debug.Log($"WateringAction.CanActOn: isHoed={hoed}, pos={gridPosition}");
        return hoed;
    }

    public bool Execute(Vector3Int gridPosition, GridData gridData,
        Grid grid, ObjectPlacer objectPlacer, Database database, InventorySlot slot)
    {
        if (slot.ItemData is not WateringTool wateringTool) return false;
        
        Vector3 worldPos = grid.CellToWorld(gridPosition);

        var colliders = Physics.OverlapBox(worldPos, new Vector3(0.5f, 0.25f, 0.5f));
        foreach (var col in colliders)
        {
            if (col.TryGetComponent<Farmland>(out var farmland))
            {
                if (!slot.ReduceWatering(wateringTool.reduceWaterByUse, wateringTool)) 
                    return false;
                
                farmland.UpdateWateringLevel(_waterAmount);
                return true;
            }
        }
        
        return false;
    }
}
