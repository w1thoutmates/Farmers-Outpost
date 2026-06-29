using UnityEngine;

public interface IGridAction
{
    bool CanActOn(Vector3Int gridPosition, GridData gridData);
    
    bool Execute(Vector3Int gridPosition, GridData gridData, 
        Grid grid, ObjectPlacer objectPlacer, Database database, InventorySlot slot);
}
