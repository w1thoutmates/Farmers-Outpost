using UnityEngine;

public class PlacementState : IBuildingState
{
    public int selectedObjectIndex = -1;
    private int _id;
    private Grid _grid;
    private PreviewSystem _previewSystem;
    private Database _database;
    private GridData _gridData;
    private ObjectPlacer _objectPlacer;
    
    public PlacementState(
        int id,
        Grid grid,
        PreviewSystem previewSystem,
        Database database,
        GridData gridData,
        ObjectPlacer objectPlacer
    )
    {
        _id = id;
        _grid = grid;
        _previewSystem = previewSystem;
        _database = database;
        _gridData = gridData;
        _objectPlacer = objectPlacer;
        
        selectedObjectIndex = database.ItemPlacements.FindIndex(data => data.id == id);

        if (selectedObjectIndex > -1)
        {
            previewSystem.StartShowingPlacementPreview(
                database.ItemPlacements[selectedObjectIndex].prefab,
                database.ItemPlacements[selectedObjectIndex].size
            );
        }
        else
        {
            throw new System.Exception($"No object with id {id}");
        }
    }

    public void EndState()
    {
        _previewSystem.StopShowingPlacementPreview();
    }

    public bool OnAction(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition);
        if (!placementValidity)
            return false;

        int placedObjectIndex = _objectPlacer.PlaceObject(
            _database.ItemPlacements[selectedObjectIndex].prefab,
            _grid.CellToWorld(gridPosition)
        );
        
        _gridData.AddObjectAt(
            gridPosition,
            _database.ItemPlacements[selectedObjectIndex].size,
            _database.ItemPlacements[selectedObjectIndex].id,
            placedObjectIndex
        );
        
        _previewSystem.UpdatePosition(_grid.CellToWorld(gridPosition), false);
        
        return true;
    }
    
    public bool CheckPlacementValidity(Vector3Int gridPosition)
    {
        return _gridData.CanPlaceObjectAt(
            gridPosition,
            _database.ItemPlacements[selectedObjectIndex].size
        );
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition);
        Vector3 cellWorldPos = _grid.CellToWorld(gridPosition);
        _previewSystem.UpdatePosition(cellWorldPos, placementValidity);
    }
}
