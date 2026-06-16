using UnityEngine;

public class ChestZoneTrigger : MonoBehaviour
{
    private ChestInventory parentChest;

    void Awake()
    {
        parentChest = GetComponentInParent<ChestInventory>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && parentChest != null)
        {
            parentChest.SetPlayerInRadius(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && parentChest != null)
        {
            parentChest.SetPlayerInRadius(false);
        }
    }
}