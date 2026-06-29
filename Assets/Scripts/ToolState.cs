using UnityEngine;

public class ToolState : IToolState
{
    private IToolState _currentToolState;
    private Grid _grid;
    private PreviewSystem _previewSystem;
    private GridData _gridData;
    private ObjectPlacer _objectPlacer;
    private Database _database;
    private int _hoeId;
    
    public ToolState(Grid grid, PreviewSystem previewSystem, GridData gridData, ObjectPlacer objectPlacer, Database database, int hoeId)
    {
        _grid = grid;
        _previewSystem = previewSystem;
        _gridData = gridData;
        _objectPlacer = objectPlacer;
        _database = database;
        _hoeId = hoeId;
    }
    
    public void SetTool(int toolId)
    {
        ExitState();

        if (toolId == _hoeId)
        {
            _currentToolState = new HoeState(toolId, _grid, _previewSystem, _gridData, _objectPlacer, _database);
        }
        // else if () // Другие инструменты
        // {
        //     // _currentToolState = new OtherToolState(...);
        //     //     break;
        // }
        else
        {
            _currentToolState = null;
            return;
        }
        
        _currentToolState?.EnterState();
    }
    
    public void EnterState()
    {
        _currentToolState?.EnterState();
    }
    
    public void UpdateState(Vector3Int gridPosition)
    {
        _currentToolState?.UpdateState(gridPosition);
    }
    
    public bool OnAction(Vector3Int gridPosition)
    {
        return _currentToolState?.OnAction(gridPosition) ?? false;
    }
    
    public bool CheckActionValidity(Vector3Int gridPosition)
    {
        return _currentToolState?.CheckActionValidity(gridPosition) ?? false;
    }
    
    public void ExitState()
    {
        _currentToolState?.ExitState();
        _currentToolState = null;
    }
}