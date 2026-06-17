using System.Collections;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class ItemWorld : MonoBehaviour
{
    public float pickUpRadius = 1f;
    public ItemData itemData;
    public int stackAmount;
    [SerializeField] private float rotateSpeed = 20f;

    private SphereCollider _collider;
    private GameObject _player;

    private void Awake()
    {
        _collider = GetComponent<SphereCollider>();
        _collider.isTrigger = true;
        _collider.radius = pickUpRadius;
        
        _player = GameObject.FindWithTag("Player");
        Physics.IgnoreCollision(GetComponent<BoxCollider>(), _player.GetComponent<Collider>(), true);
        
        var myLayer = gameObject.layer;
        Physics.IgnoreLayerCollision(myLayer, myLayer, true);
    }
    
    public void SetupItem(ItemData data, int amount)
    {
        itemData = data;
        stackAmount = amount;
    }

    public void Update()
    {
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        var inventory = other.transform.GetComponent<PlayerInventoryHolder>();

        if (!inventory) return;
        
        _ = TakeItemWithAnim(inventory);
    }

    void OnDestroy()
    {
        transform.DOKill();
    }
    
    private void AnimateTake(ItemWorld itemWorld)
    {
        if (itemWorld == null || itemWorld.gameObject == null) return;
        
        Transform iwTransform = itemWorld.transform;
        if (iwTransform == null) return;
        
        iwTransform.DOKill();
        
        Vector3 startScale = iwTransform.localScale;
        Vector3 finalScale = startScale / 5;
        float animDuration = 0.15f;

        iwTransform.DOScale(finalScale, animDuration)
            .From(startScale)
            .SetEase(Ease.InBack)
            .SetAutoKill(true);
    }

    private void TakeItem(PlayerInventoryHolder inventory)
    {
        inventory.PrimaryInventorySystem.AddToInventory(itemData, stackAmount, out int remainingAmount);

        if (remainingAmount <= 0)
        {
            Destroy(this.gameObject);
        }
        else
        {
            stackAmount = remainingAmount;
        }
    }

    private async Awaitable TakeItemWithAnim(PlayerInventoryHolder inventory)
    {
        _collider.enabled = false;
        AnimateTake(this);
        await Awaitable.WaitForSecondsAsync(0.15f, destroyCancellationToken);
        TakeItem(inventory);
    }

}
