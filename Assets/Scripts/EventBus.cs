using UnityEngine;
using UnityEngine.Events;

// 1. Создаем статический класс-шину (или синглтон-менеджер)
public static class EventBus
{
    private static UnityAction _onPlacementItemWasUsed;
    
    public static void Subscribe(UnityAction listener)
    {
        _onPlacementItemWasUsed += listener;
    }

    public static void Unsubscribe(UnityAction listener)
    {
        _onPlacementItemWasUsed -= listener;
    }

    public static void NotifyPlacementItemUsed()
    {
        _onPlacementItemWasUsed?.Invoke();
    }
}