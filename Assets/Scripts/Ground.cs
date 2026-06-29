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
    public const int STATIC_OBSTACLE_ID = -99;
    
    [NonSerialized] public bool isOccupied = false;
    [SerializeField] protected GroundType groundType = GroundType.Grass;

    protected virtual void Awake()
    {
        switch (groundType)
        {
            case GroundType.Grass:
                isOccupied = false;
                break;
            default:
                isOccupied = true;
                break;
        }
    }
    
    protected virtual void Start()
    {
        if (isOccupied)
            RegisterInGrid();
    }

    private void RegisterInGrid()
    {
        var placementSystem = PlacementSystem.Instance;
        if (placementSystem == null) return;

        Grid grid = placementSystem.grid;
        
        Vector3Int cell = grid.WorldToCell(transform.position);
        int id = groundType == GroundType.Farmland 
            ? placementSystem.FarmlandId 
            : STATIC_OBSTACLE_ID;
            
        placementSystem.RegisterGroundObject(cell, id);
    }
}
