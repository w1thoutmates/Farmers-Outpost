using System;
using UnityEngine;
using UnityEngine.Events;

public static class EventBus
{
    public static event Action<InventorySlot> onPlacementItemWasUsed;
    public static event Action<InventorySlot> onToolWasDestroyed;
    public static event Action<InventorySlot> onToolWasUsed;
    public static event Action onNewDayStarted;
    public static event Action onUINeedToRefresh;

    public static void NotifyPlacementItemUsed(InventorySlot slot)
    {
        onPlacementItemWasUsed?.Invoke(slot);
    }

    public static void NotifyThatToolWasDestroyed(InventorySlot slot)
    {
        onToolWasDestroyed?.Invoke(slot);
    }
    
    public static void NotifyToolWasUsed(InventorySlot slot)
    {
        onToolWasUsed?.Invoke(slot);
    }

    public static void NotifyNewDayStarted()
    {
        Debug.Log("Новый день начался");
        onNewDayStarted?.Invoke();
    }

    public static void NotifyThatUINeedToRefresh()
    {
        onUINeedToRefresh?.Invoke();
    }
}