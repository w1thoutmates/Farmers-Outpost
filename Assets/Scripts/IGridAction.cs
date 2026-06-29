using UnityEngine;

public interface IGridAction
{
    bool CanActOn(Vector3Int gridPosition, GridData gridData, ToolUseType useType);
    
    bool Execute(Vector3Int gridPosition, GridData gridData, 
        Grid grid, ObjectPlacer objectPlacer, Database database, 
        InventorySlot slot, ToolUseType useType);
    
    int GetRadius(ToolUseType useType);
}
