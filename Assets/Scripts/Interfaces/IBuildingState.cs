using UnityEngine;

public interface IBuildingState
{
    public bool OnAction(Vector3Int gridPosition);
    public void EndState();
    public void UpdateState(Vector3Int gridPosition);
    
    public bool CheckPlacementValidity(Vector3Int gridPosition);
}
