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
            InteractClosest();
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            InteractMouse();
        }
    }
    
    void InteractMouse()
    {
        Plane plane = new Plane(Vector3.up, interactionPoint.position);
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!plane.Raycast(ray, out float enter))
            return;

        Vector3 worldPoint = ray.GetPoint(enter);

        Collider[] colliders = Physics.OverlapSphere(
            interactionPoint.position,
            interactionPointRadius,
            interactionLayer);

        IInteractable best = null;
        InventoryHolder bestInventory = null;
        float bestDist = float.MaxValue;

        foreach (var col in colliders)
        {
            var interactable = col.GetComponent<IInteractable>();
            if (interactable == null)
                continue;

            float dist = Vector3.Distance(worldPoint, col.transform.position);

            if (dist < bestDist)
            {
                bestDist = dist;
                best = interactable;
                bestInventory = col.GetComponent<InventoryHolder>();
            }
        }

        if (best == null)
        {
            if (_currentOpenInventory != null)
            {
                InventoryUIController.Instance.CloseAllInventories();
            }
            return;
        }

        if (bestInventory != null && _currentOpenInventory != null)
        {
            if (bestInventory.PrimaryInventorySystem == _currentOpenInventory)
            {
                if (MouseItemData.IsPointerOverUIObject()) return;
                
                InventoryUIController.Instance.CloseAllInventories();
                return;
            }
        }

        if (!MouseItemData.IsPointerOverUIObject())
            StartInteraction(best);
    }

    void InteractClosest()
    {
        var colliders = Physics.OverlapSphere(
            interactionPoint.position,
            interactionPointRadius,
            interactionLayer);

        IInteractable target = null;
        InventoryHolder targetInventory = null;

        foreach (var col in colliders)
        {
            var interactable = col.GetComponent<IInteractable>();
            if (interactable == null)
                continue;

            target = interactable;
            targetInventory = col.GetComponent<InventoryHolder>();

            break;
        }

        if (target == null)
        {
            if (_currentOpenInventory != null)
            {
                InventoryUIController.Instance.CloseAllInventories();
            }
            return;
        }

        if (targetInventory != null && _currentOpenInventory != null)
        {
            if (targetInventory.PrimaryInventorySystem == _currentOpenInventory)
            {
                InventoryUIController.Instance.CloseAllInventories();
                return;
            }
        }

        StartInteraction(target);
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
