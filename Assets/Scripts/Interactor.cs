using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
    public Transform interactionPoint;
    public LayerMask interactionLayer;
    public float interactionPointRadius = 1f;
    public bool isIntecating { get; private set; }
    
    private InventorySystem _currentOpenInventory;
    
    void OnEnable()
    {
        InventoryUIController.OnChestInventoryOpened += HandleChestOpened;
        InventoryUIController.OnChestInventoryClosed += HandleChestClosed;
    }
    
    void OnDisable()
    {
        InventoryUIController.OnChestInventoryOpened -= HandleChestOpened;
        InventoryUIController.OnChestInventoryClosed -= HandleChestClosed;
    }
    
    private void HandleChestOpened(InventorySystem openedInventory)
    {
        _currentOpenInventory = openedInventory;
    }

    private void HandleChestClosed()
    {
        _currentOpenInventory = null;
    }

    void Update()
    {
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            var colliders = Physics.OverlapSphere(interactionPoint.position, interactionPointRadius, interactionLayer);
            
            IInteractable targetInteractable = null;
            InventoryHolder targetInventoryHolder = null;

            for (int i = 0; i < colliders.Length; i++)
            {
                var interactable = colliders[i].GetComponent<IInteractable>();
                if (interactable != null)
                {
                    targetInteractable = interactable;
                    targetInventoryHolder = colliders[i].GetComponent<InventoryHolder>();
                    
                    if (targetInventoryHolder != null && _currentOpenInventory != null)
                    {
                        if (targetInventoryHolder.PrimaryInventorySystem != _currentOpenInventory)
                        {
                            break; 
                        }
                    }
                }
            }

            if (targetInteractable != null)
            {
                if (targetInventoryHolder != null && _currentOpenInventory != null && 
                    targetInventoryHolder.PrimaryInventorySystem == _currentOpenInventory)
                {
                    InventoryUIController.Instance.CloseAllInventories();
                    return;
                }

                StartInteraction(targetInteractable);
            }
            else
            {
                if (_currentOpenInventory != null)
                {
                    InventoryUIController.Instance.CloseAllInventories();
                }
            }
        }
    }

    void StartInteraction(IInteractable interactable)
    {
        interactable.Interact(this, out bool isInteractionSuccessful);
       
        if (isInteractionSuccessful)
        {
            isIntecating = true;
            interactable.OnInteractionComplete += HandleInteractionComplete;
        }
    }

    void EndInteraction()
    {
        isIntecating = false;
    }

    void HandleInteractionComplete(IInteractable interactable)
    {
        interactable.OnInteractionComplete -= HandleInteractionComplete;
        EndInteraction();
    }
    
    public bool CanInteractWithNewObject(InventorySystem currentOpenInventory)
    {
        var colliders = Physics.OverlapSphere(interactionPoint.position, interactionPointRadius, interactionLayer);

        for (int i = 0; i < colliders.Length; i++)
        {
            var inventoryHolder = colliders[i].GetComponent<InventoryHolder>(); // or child of InventoryHolder, for example - chest holder

            if (inventoryHolder != null)
            {
                if (inventoryHolder.PrimaryInventorySystem != currentOpenInventory)
                {
                    return true; 
                }
            }
        }

        return false;
    }
}
