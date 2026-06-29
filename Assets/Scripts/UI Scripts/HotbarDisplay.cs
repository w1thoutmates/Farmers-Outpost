using UnityEngine;
using UnityEngine.InputSystem;

public class HotbarDisplay : StaticInventoryDisplay
{
    private int _maxIndexSize = 6;
    private int _currentIndex = 0;
    
    private PlayerInputActions _playerInputActions;

    void Awake()
    {
        _playerInputActions = new PlayerInputActions();
    }

    protected override void Start()
    {
        base.Start();

        _currentIndex = 0;
        _maxIndexSize = slots.Length - 1;

        slots[_currentIndex].ToggleHighlight();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _playerInputActions.Enable();

        _playerInputActions.Player.Hotbar1.performed += Hotbar1;
        _playerInputActions.Player.Hotbar2.performed += Hotbar2;
        _playerInputActions.Player.Hotbar3.performed += Hotbar3;
        _playerInputActions.Player.Hotbar4.performed += Hotbar4;
        _playerInputActions.Player.Hotbar5.performed += Hotbar5;
        _playerInputActions.Player.Hotbar6.performed += Hotbar6;
        _playerInputActions.Player.Hotbar7.performed += Hotbar7;
        _playerInputActions.Player.UseItem.performed += UseItem;
        
        EventBus.onPlacementItemWasUsed += UpdateHotbarSlotAfterPlacementItemUsed;
        EventBus.onToolWasDestroyed += UpdateHotbarAfterToolDestroyed;
        EventBus.onToolWasUsed += OnToolWasUsed;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        _playerInputActions.Disable();
        
        _playerInputActions.Player.Hotbar1.performed -= Hotbar1;
        _playerInputActions.Player.Hotbar2.performed -= Hotbar2;
        _playerInputActions.Player.Hotbar3.performed -= Hotbar3;
        _playerInputActions.Player.Hotbar4.performed -= Hotbar4;
        _playerInputActions.Player.Hotbar5.performed -= Hotbar5;
        _playerInputActions.Player.Hotbar6.performed -= Hotbar6;
        _playerInputActions.Player.Hotbar7.performed -= Hotbar7;
        _playerInputActions.Player.UseItem.performed -= UseItem;

        EventBus.onPlacementItemWasUsed -= UpdateHotbarSlotAfterPlacementItemUsed;
        EventBus.onToolWasDestroyed -= UpdateHotbarAfterToolDestroyed;
        EventBus.onToolWasUsed -= OnToolWasUsed;
    }

    #region Hotbar Select Methods

    void Hotbar1(InputAction.CallbackContext context)
    {
        SetIndex(0);
    }

    void Hotbar2(InputAction.CallbackContext context)
    {
        SetIndex(1);
    }

    void Hotbar3(InputAction.CallbackContext context)
    {
        SetIndex(2);
    }

    void Hotbar4(InputAction.CallbackContext context)
    {
        SetIndex(3);
    }

    void Hotbar5(InputAction.CallbackContext context)
    {
        SetIndex(4);
    }

    void Hotbar6(InputAction.CallbackContext context)
    {
        SetIndex(5);
    }

    void Hotbar7(InputAction.CallbackContext context)
    {
        SetIndex(6);
    }

    #endregion
    
    void Update()
    {
        if (_playerInputActions.Player.MouseWheel.ReadValue<float>() > 0.1f) ChangeIndex(1);
        if (_playerInputActions.Player.MouseWheel.ReadValue<float>() < -0.1f) ChangeIndex(-1);
        
        if (InventoryUIController.Instance.IsAnyInventoryOpen)
        {
            if (PlacementSystem.Instance.IsPlacementModeActive)
            {
                PlacementSystem.Instance.StopPlacement();
            }
            
            if (PlacementSystem.Instance.IsToolModeActive)
            {
                PlacementSystem.Instance.StopToolMode();
            }

            return;
        }
        
        var currentSlot = slots[_currentIndex].AssignedInventorySlot;
    
        if (currentSlot.ItemData is ItemPlacement itemPlacement)
        {
            PlacementSystem.Instance.StartPlacement(itemPlacement.id);
        }
        else if (currentSlot.ItemData is ItemTool tool)
        {
            PlacementSystem.Instance.StartToolMode(tool.id);
        }
        else
        {
            PlacementSystem.Instance.StopPlacement();
            PlacementSystem.Instance.StopToolMode();
        }
    }

    void UseItem(InputAction.CallbackContext context)
    {
        //slots[_currentIndex].AssignedInventorySlot.Use();
    }
    
    private void UpdateHotbarAfterToolDestroyed(InventorySlot obj)
    {
        RefreshStaticDisplay();
    }
    
    private void OnToolWasUsed(InventorySlot slot)
    {
        slot.Use();
        RefreshStaticDisplay();
    }

    void UpdateHotbarSlotAfterPlacementItemUsed(InventorySlot slot)
    {
        slot.Use();
        slot.RemoveFromStack(1);
        RefreshStaticDisplay();
    }

    void ChangeIndex(int direction)
    {
        slots[_currentIndex].ToggleHighlight();
        _currentIndex += direction;

        if (_currentIndex > _maxIndexSize) _currentIndex = 0;
        if (_currentIndex < 0) _currentIndex = _maxIndexSize;
        
        slots[_currentIndex].ToggleHighlight();
        
        var currentSlot = slots[_currentIndex].AssignedInventorySlot;
    
        if (currentSlot.ItemData is ItemPlacement itemPlacement)
        {
            PlacementSystem.Instance.StartPlacement(itemPlacement.id);
        }
        else if (currentSlot.ItemData is ItemTool tool)
        {
            PlacementSystem.Instance.StartToolMode(tool.id);
        }
        else
        {
            PlacementSystem.Instance.StopPlacement();
            PlacementSystem.Instance.StopToolMode();
        }
    }
    
    private void SetIndex(int index)
    {
        slots[_currentIndex].ToggleHighlight();
        
        if (index < 0) index = 0;
        if (index > _maxIndexSize) index = _maxIndexSize;
        
        _currentIndex = index;
        slots[_currentIndex].ToggleHighlight();
        
        // if (slots[_currentIndex].AssignedInventorySlot.ItemData is ItemPlacement itemPlacement)
        // {
        //     PlacementSystem.Instance.ChangeSelectedObjectIndex(
        //         PlacementSystem.Instance.database.ItemPlacements.FindIndex(data => data.id == itemPlacement.id));
        // }
        
        var currentSlot = slots[_currentIndex].AssignedInventorySlot;
    
        if (currentSlot.ItemData is ItemPlacement itemPlacement)
        {
            PlacementSystem.Instance.StartPlacement(itemPlacement.id);
        }
        else if (currentSlot.ItemData is ItemTool tool)
        {
            PlacementSystem.Instance.StartToolMode(tool.id);
        }
        else
        {
            PlacementSystem.Instance.StopPlacement();
            PlacementSystem.Instance.StopToolMode();
        }
    }
    
    public InventorySlot GetActiveSlot()
    {
        if (slots != null && _currentIndex >= 0 && _currentIndex < slots.Length)
        {
            return slots[_currentIndex].AssignedInventorySlot;
        }
        return null;
    }
}
