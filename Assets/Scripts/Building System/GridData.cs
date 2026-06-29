using System;
using System.Collections.Generic;
using UnityEngine;

public class GridData
{
    private Dictionary<Vector3Int, PlacementData> placementObjects = new();
    private Grid _grid;
    
    public GridData(Grid grid = null)
    {
        _grid = grid;
    }

    public void AddObjectAt(Vector3Int gridPosition, Vector2Int objectSize, int ID, int placedObjectIndex)
    {
        List<Vector3Int> positionsToOccupy = CalculatePositions(gridPosition, objectSize);
        PlacementData data = new PlacementData(positionsToOccupy, ID, placedObjectIndex);
        foreach (var pos in positionsToOccupy)
        {
            if (placementObjects.ContainsKey(pos))
            {
                throw new Exception($"Dictionary already contains this cell position {pos}");
            }
            
            placementObjects[pos] = data;
        }
    }

    private List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> returnVal = new();
        for (int x = 0; x < objectSize.x; x++)
        {
            for (int y = 0; y < objectSize.y; y++)
            {
                returnVal.Add(gridPosition + new Vector3Int(x, 0, y));
            }
        }
        
        return returnVal;
    }

    public bool CanPlaceObjectAt(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> positionsToOccupy = CalculatePositions(gridPosition, objectSize);
        foreach (var pos in positionsToOccupy)
        {
            if (placementObjects.ContainsKey(pos))
                return false;
        }
        
        return true;
    }
    
    public bool IsPositionOccupied(Vector3Int gridPosition)
    {
        return placementObjects.ContainsKey(gridPosition);
    }

    public bool IsPositionHoed(Vector3Int gridPosition)
    {
        if (placementObjects.TryGetValue(gridPosition, out PlacementData data))
        {
            return data.ID == PlacementSystem.Instance.FarmlandId;
        }
        return false;
    }
    
    public bool IsPositionWater(Vector3Int gridPosition)
    {
        if (_grid == null) return false;
        
        var allGrounds = GameObject.FindObjectsByType<Ground>();
        
        foreach (var ground in allGrounds)
        {
            Vector3Int groundCell = _grid.WorldToCell(ground.transform.position);
            groundCell.y = 0;   // Нормализация координат 
            gridPosition.y = 0; // потому что используются только X и Z
                                // В альтернативе можно исп. 
                                // if (ground.groundType == GroundType.Water &&
                                //    groundCell.x == gridPosition.x &&
                                //    groundCell.z == gridPosition.z)
            if (groundCell == gridPosition && ground.groundType == GroundType.Water)
            {
                return true;
            }
        }
        
        return false;
    }
    
    public void RegisterOccupiedPosition(Vector3Int gridPosition, int ID)
    {
        if (placementObjects.ContainsKey(gridPosition)) return;
    
        PlacementData data = new PlacementData(
            new List<Vector3Int> { gridPosition }, 
            ID, 
            -1
        );
        placementObjects[gridPosition] = data;
    }
}

public class PlacementData
{
    public List<Vector3Int> occupiedPositions;
    public int ID { get; private set; }
    public int PlacedObjectIndex { get; private set; }
    
    public PlacementData(List<Vector3Int> occupiedPositions, int id, int placedObjectIndex)
    {
        this.occupiedPositions = occupiedPositions;
        ID = id;
        PlacedObjectIndex = placedObjectIndex;
    }
}
