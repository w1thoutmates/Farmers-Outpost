using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image itemSprite;
    [SerializeField] private TextMeshProUGUI itemCount;
    [SerializeField] private InventorySlot assignedInventorySlot;
    [SerializeField] private GameObject _slotHightlight;
    
    [SerializeField] private Image durabilityFillImage;
    [SerializeField] private GameObject durabilitySlider;

    private Button button;

    public InventorySlot AssignedInventorySlot => assignedInventorySlot;
    public InventoryDisplay ParentDisplay { get; private set; }

    void Awake()
    {
        ClearSlot();
        
        itemSprite.preserveAspect = true;

        button = GetComponent<Button>();
        button?.onClick.AddListener(OnUISlotClick);

        ParentDisplay = transform.GetComponentInParent<InventoryDisplay>();
    }

    public void Init(InventorySlot slot)
    {
        assignedInventorySlot = slot;
        UpdateUISlot(slot);
    }

    public void UpdateUISlot(InventorySlot slot)
    {
        if (slot.ItemData != null)
        {
            itemSprite.sprite = slot.ItemData.icon;
            itemSprite.color = new Color(1, 1, 1, 1);

            if (slot.StackSize > 1) itemCount.text = slot.StackSize.ToString();
            else itemCount.text = string.Empty;
            
            UpdateDurabilityDisplay(slot);
        } else
        {
            ClearSlot();
            durabilitySlider.gameObject.SetActive(false);
        }
    }

    public void UpdateUISlot()
    {
        if (assignedInventorySlot != null) UpdateUISlot(assignedInventorySlot);
    }

    public void ClearSlot()
    {
        assignedInventorySlot?.ClearSlot();
        itemSprite.sprite = null;
        itemSprite.color = Color.clear;
        itemCount.text = string.Empty;
    }

    public void OnUISlotClick()
    {
        ParentDisplay?.SlotClicked(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            ParentDisplay?.SlotRightClicked(this);
        }
    }

    public void ToggleHighlight()
    {
        _slotHightlight.SetActive(!_slotHightlight.activeInHierarchy);
    }

    public void UpdateDurabilityDisplay(InventorySlot slot)
    {
        if (slot == null || slot.ItemData == null)
        {
            durabilitySlider.gameObject.SetActive(false);
            return;
        }
    
        if (slot.IsWateringTool())
        {
            float maxWater = slot.GetMaxWaterCapacity();
            if (Mathf.Approximately(slot.Watering, maxWater))
            {
                durabilitySlider.gameObject.SetActive(false);
            }
            
            else if (maxWater > 0)
            {
                durabilitySlider.gameObject.SetActive(true);
                durabilityFillImage.fillAmount = slot.Watering / maxWater;
                durabilityFillImage.color = slot.Watering / maxWater > 0.25f ?
                    new Color(108 / 255f, 139 / 255f, 159 / 255f,1f) :
                    new Color(200 / 255f, 90 / 255f, 90 / 255f, 1f);
            }
            else
            {
                durabilitySlider.gameObject.SetActive(false);
            }
            return;
        }
    
        if (slot.IsToolWithDurability())
        {
            float maxDurability = slot.GetMaxDurability();
            if (Mathf.Approximately(slot.Durability, maxDurability))
            {
                durabilitySlider.gameObject.SetActive(false);
            }
            
            else if (maxDurability > 0)
            {
                durabilitySlider.gameObject.SetActive(true);
                durabilityFillImage.fillAmount = slot.Durability / maxDurability;
                float durabilityPercent = slot.Durability / maxDurability;
                durabilityFillImage.color = durabilityPercent > 0.5f ?
                    new Color(106 / 255f, 191 / 255f, 105 / 255f, 1f) : 
                    durabilityPercent > 0.25f ?
                        new Color(229 / 255f, 166 / 255f, 58 / 255f, 1f) :
                    new Color(200 / 255f, 90 / 255f, 90 / 255f, 1f);
            }
            else
            {
                durabilitySlider.gameObject.SetActive(false);
            }
            return;
        }
    
        durabilitySlider.gameObject.SetActive(false);
    }
}
