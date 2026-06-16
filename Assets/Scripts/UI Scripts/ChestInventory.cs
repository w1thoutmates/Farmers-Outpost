using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections.Generic;

public class ChestInventory : InventoryHolder, IInteractable
{
    [SerializeField] private float availableInventoryTimeOutOfRadius = 2f;

    private bool isInAvailableRadius = false;
    private float timer;
    private bool isOpened;

    public UnityAction<IInteractable> OnInteractionComplete { get; set; }

    protected override void Awake()
    {
        base.Awake();

        timer = availableInventoryTimeOutOfRadius;
    }

    public void Interact(Interactor interactor, out bool isInteractWasSuccessful)
    {
        OnDynamicInventoryDisplayRequested?.Invoke(primaryInventorySystem);
        isInteractWasSuccessful = true;
        isOpened = true;

        timer = availableInventoryTimeOutOfRadius;
    }

    public void EndInteraction()
    {
        if (!isOpened) return;

        OnDynamicInventoryDisplayRequested?.Invoke(null);

        isOpened = false;
        OnInteractionComplete?.Invoke(this);
    }

    void Update()
    {
        if (isOpened && !isInAvailableRadius)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                EndInteraction();
            }
        }
    }

    public void SetPlayerInRadius(bool inRadius)
    {
        isInAvailableRadius = inRadius;

        if (inRadius)
        {
            timer = availableInventoryTimeOutOfRadius;
        }
    }
}
