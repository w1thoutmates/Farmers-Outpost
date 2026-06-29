using System.Linq;
using UnityEngine;

public class WateringAction : IGridAction
{
    private float _waterAmount;
    
    public WateringAction(float waterAmount = 25f)
    {
        _waterAmount = waterAmount;
    }
    
    public bool CanActOn(Vector3Int gridPosition, GridData gridData, ToolUseType useType)
    {
        switch (useType)
        {
            case ToolUseType.Primary:
                return gridData.IsPositionHoed(gridPosition);
            case ToolUseType.Secondary:
                return gridData.IsPositionWater(gridPosition);
            default:
                return false;
        }
    }

    public bool Execute(Vector3Int gridPosition, GridData gridData,
        Grid grid, ObjectPlacer objectPlacer, Database database,
        InventorySlot slot, ToolUseType useType)
    {
        switch (useType)
        {
            case ToolUseType.Primary:
                return WateringFarmland(gridPosition, grid, slot);
            case ToolUseType.Secondary:
                return RefillWatering(grid, slot);
            default: 
                return false;
        }
    }

    private bool WateringFarmland(Vector3Int gridPosition, Grid grid, InventorySlot slot)
    {
        if (slot.ItemData is not WateringTool wateringTool)
            return false;
        
        if (slot.Watering <= 0)
            return false;
        
        var allFarmlands = GameObject.FindObjectsByType<Farmland>();
        foreach (var farmland in allFarmlands)
        {
            Vector3Int farmlandCell = grid.WorldToCell(farmland.transform.position);
            if (farmlandCell == gridPosition)
            {
                if (!slot.ReduceWatering(wateringTool.reduceWaterByUse, wateringTool))
                    return false;
                farmland.UpdateWateringLevel(_waterAmount);
                return true;
            }
        }
        
        return false;
    }

    private bool RefillWatering(Grid grid, InventorySlot slot)
    {
        if (slot.ItemData is not WateringTool) return false;

        var allGrounds = GameObject.FindObjectsByType<Ground>();
        var allWaters = allGrounds.Where(ground => ground.groundType == GroundType.Water).ToList();
        
        Vector3Int playerCell = grid.WorldToCell(Player.Instance.transform.position);

        foreach (var water in allWaters)
        {
            Vector3Int waterCell = grid.WorldToCell(water.transform.position);
            
            int dx = Mathf.Abs(waterCell.x - playerCell.x);
            int dz = Mathf.Abs(waterCell.z - playerCell.z);
            
            if (dx <= 1 && dz <= 1)
            {
                if (slot.RefillWatering(slot.GetMaxWaterCapacity()))
                {
                    EventBus.NotifyThatUINeedToRefresh();
                    return true;
                }
            }
        }
        return false;
    }
    
    public int GetRadius(ToolUseType useType)
    {
        switch (useType)
        {
            case ToolUseType.Primary:
                return 3;
            case ToolUseType.Secondary:
                return 1;
        }

        return 0;
    }
}
