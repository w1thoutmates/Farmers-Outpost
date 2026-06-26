using UnityEngine;

public class PreviewSystem : MonoBehaviour
{
    [SerializeField] private float previewYOffset = 0.06f;
    [SerializeField] private GameObject cellIndicator;
    [SerializeField] private Material pfPreviewMaterial;
    
    private GameObject _previewObject;
    private Material _previewMaterialInstance;
    private Renderer _cellIndicatorRenderer;

    void Start()
    {
        _previewMaterialInstance = new Material(pfPreviewMaterial);
        cellIndicator.gameObject.SetActive(false);
        _cellIndicatorRenderer = cellIndicator.GetComponentInChildren<Renderer>();
    }

    public void StartShowingPlacementPreview(GameObject prefab, Vector2Int size) 
    {
        _previewObject = Instantiate(prefab);
        
        Collider objectCollider = _previewObject.GetComponent<Collider>();
        if (objectCollider != null)
            objectCollider.enabled = false;
        
        PreparePreview(_previewObject);
        PrepareCursor(size);
        cellIndicator.gameObject.SetActive(true);
    }

    void PrepareCursor(Vector2Int size)
    {
        if (size.x > 0 || size.y > 0)
        {
            cellIndicator.transform.localScale = new Vector3(size.x, size.y, 1);
            _cellIndicatorRenderer.material.mainTextureScale = size;
        }
    }

    void PreparePreview(GameObject previewObject)
    {
        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            Material[] mats = r.materials;
            for (int i = 0; i < mats.Length; i++)
            {
                mats[i] = _previewMaterialInstance;
            }
            r.materials = mats;
        }
    }

    public void StopShowingPlacementPreview()
    {
        cellIndicator.gameObject.SetActive(false);
        Destroy(_previewObject);
        _previewObject = null;
    }

    public void UpdatePosition(Vector3 pos, bool validity)
    {
        if (_previewObject == null)
            return;

        MovePreview(pos);
        MoveCursor(pos);
        ApplyFeedback(validity);
    }

    void ApplyFeedback(bool validity)
    {
        Color color = validity ? Color.white : Color.red;
        _cellIndicatorRenderer.material.color = color;
        color.a = 0.5f;
        _previewMaterialInstance.color = color;
    }

    void MoveCursor(Vector3 pos)
    {
        cellIndicator.transform.position = pos;
    }

    void MovePreview(Vector3 pos)
    {
        if (_previewObject == null)
            return;

        _previewObject.transform.position =
            new Vector3(
                pos.x,
                pos.y + previewYOffset,
                pos.z
            );
    }
}
