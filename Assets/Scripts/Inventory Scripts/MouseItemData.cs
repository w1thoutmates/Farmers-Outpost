using System.Collections;
using DG.Tweening;
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

    private Transform _playerTransform;

    void Awake()
    {
        image.color = Color.clear;
        image.preserveAspect = true;
        count.text = string.Empty;

        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        if (_playerTransform == null) Debug.Log("Player not found");
    }

    public void UpdateMouseSlot(InventorySlot invSlot)
    {
        // assignedInventorySlot.AssignItem(invSlot);
        assignedInventorySlot = new InventorySlot(invSlot.ItemData, invSlot.StackSize);

        UpdateMouseSlot();
    }

    private void UpdateMouseSlot()
    {
        image.sprite = assignedInventorySlot.ItemData.icon;
        count.text = assignedInventorySlot.StackSize.ToString();
        image.color = new Color(1, 1, 1, 1);
    }
    
    private void UpdateMouseSlotDisplay()
    {
        if (assignedInventorySlot.ItemData != null)
        {
            image.sprite = assignedInventorySlot.ItemData.icon;
            count.text = assignedInventorySlot.StackSize.ToString();
            image.color = Color.white;
        }
    }

    void Update()
    {
        if (assignedInventorySlot == null || assignedInventorySlot.ItemData == null) return;
        
        transform.position = Mouse.current.position.ReadValue();

        if (Mouse.current.leftButton.wasPressedThisFrame && !IsPointerOverUIObject())
        {
            ThrowItemFromMouseLMB();
            return;
        }

        if (Mouse.current.rightButton.wasPressedThisFrame && !IsPointerOverUIObject())
        {
            ThrowItemFromMouseRMB();
            return;
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

    private void AnimateDrop(ItemWorld itemWorld)
    {
        if (itemWorld == null || itemWorld.gameObject == null) return;
        
        Transform iwTransform = itemWorld.transform;
        if (iwTransform == null) return;
        
        iwTransform.DOKill();
        
        Vector3 finalScale = iwTransform.localScale;
        Vector3 startScale = finalScale / 5;
        float animDuration = 0.3f;

        iwTransform.DOScale(finalScale, animDuration)
            .From(startScale)
            .SetEase(Ease.OutBounce)
            .SetAutoKill(true);
    }
    
    private Vector3 CalculateSpawnPosition()
    {
        Vector3 spawnPos = _playerTransform.position + _playerTransform.forward * 1.0f;
        spawnPos.y = _playerTransform.position.y + 0.8f;
    
        return spawnPos;
    }
    
    private void ThrowItemFromMouseLMB()
    {
        if (assignedInventorySlot.ItemData.itemWorld != null)
        {
            Vector3 spawnPosition = CalculateSpawnPosition();
            ItemWorld itemWorld = Instantiate(assignedInventorySlot.ItemData.itemWorld, spawnPosition, Quaternion.identity);
            itemWorld.SetupItem(assignedInventorySlot.ItemData, assignedInventorySlot.StackSize);
        
            AnimateDrop(itemWorld);
        
            if (itemWorld.TryGetComponent<Rigidbody>(out var rb))
            {
                Vector3 throwDirection = _playerTransform.forward + Vector3.up * 0.25f + _playerTransform.right * Random.Range(-0.1f, 0.1f);
                rb.AddForce(throwDirection.normalized * 2f, ForceMode.Impulse);
            }

            ClearSlot();
        }
    }
    
    private void ThrowItemFromMouseRMB()
    {
        if (assignedInventorySlot.ItemData.itemWorld != null)
        {
            Vector3 spawnPosition = CalculateSpawnPosition();
    
            ItemWorld itemWorld = Instantiate(assignedInventorySlot.ItemData.itemWorld, spawnPosition, Quaternion.identity);
            itemWorld.SetupItem(assignedInventorySlot.ItemData, 1);
                
            AnimateDrop(itemWorld);

            if (itemWorld.TryGetComponent<Rigidbody>(out var rb))
            {
                Vector3 throwDirection = _playerTransform.forward + Vector3.up * 0.25f + _playerTransform.right * Random.Range(-0.1f, 0.1f);
                rb.AddForce(throwDirection.normalized * 2f, ForceMode.Impulse);
            }
                
            if (assignedInventorySlot.StackSize > 1)
            {
                assignedInventorySlot.AddToStack(-1);
                UpdateMouseSlotDisplay(); 
            }
            else
            {
                ClearSlot();
                return;
            }
        }
    }
}
