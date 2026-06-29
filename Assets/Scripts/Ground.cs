using System;
using UnityEngine;

public enum GroundType
{
    Water,
    Grass,
    Wall,
    Farmland
}

public class Ground : MonoBehaviour
{
    [NonSerialized] public bool isOccupied = false;
    [SerializeField] protected GroundType groundType = GroundType.Grass;

    protected virtual void Awake()
    {
        switch (groundType)
        {
            case GroundType.Grass:
                isOccupied = false;
                break;
            case GroundType.Wall:
                isOccupied = true;
                break;
            case GroundType.Water:
                isOccupied = true;
                break;
            case GroundType.Farmland:
                isOccupied = true;
                break;
            default:
                isOccupied = true;
                break;
        }
    }
    
}
