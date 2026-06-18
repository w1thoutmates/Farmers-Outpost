using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
    public Transform interactionPoint;
    public LayerMask interactionLayer;
    public float interactionPointRadius = 1f;
    public bool isIntecating { get; private set; }

    void Update()
    {
        if (InventoryUIController.Instance != null && 
            (InventoryUIController.Instance.inventoryPanel.gameObject.activeInHierarchy || 
             InventoryUIController.Instance.playerBackpackPanel.gameObject.activeInHierarchy)) return; 
        
        var colliders = Physics.OverlapSphere(interactionPoint.position, interactionPointRadius, interactionLayer);

        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                var interactable = colliders[i].GetComponent<IInteractable>();

                if (interactable != null)
                {
                    StartInteraction(interactable);
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
}
