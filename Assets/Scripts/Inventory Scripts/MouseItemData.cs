using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MouseItemData : MonoBehaviour
{
    public Image image;
    public TextMeshProUGUI count;
    public InventorySlot assignedInventorySlot;

    void Awake()
    {
        image.color = Color.clear;
        count.text = string.Empty;
    }

    public void UpdateMouseSlot(InventorySlot invSlot)
    {
        // assignedInventorySlot.AssingItem(invSlot);

        assignedInventorySlot = new InventorySlot(invSlot.ItemData, invSlot.StackSize);

        image.sprite = invSlot.ItemData.icon;
        count.text = invSlot.StackSize.ToString();
        image.color = new Color(1, 1, 1, 1);
    }

    void Update()
    {
        if (assignedInventorySlot.ItemData != null)
        {
            transform.position = Mouse.current.position.ReadValue();

            if (Mouse.current.leftButton.wasPressedThisFrame && !IsPointerOverUIObject())
            {
                ClearSlot();
                // TODO: drop item on the ground;
            }
        }
    }

    public void ClearSlot()
    {
        assignedInventorySlot.ClearSlot();
        count.text = string.Empty;
        image.sprite = null;
        image.color = Color.clear;
    }

    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = Mouse.current.position.ReadValue();
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
