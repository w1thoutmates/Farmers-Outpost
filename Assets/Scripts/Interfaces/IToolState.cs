using UnityEngine;

public interface IToolState
{
    void EnterState();
    void UpdateState(Vector3Int gridPosition);
    bool OnAction(Vector3Int gridPosition);
    void ExitState();
    bool CheckActionValidity(Vector3Int gridPosition);
}