using UnityEngine;

public class PreviewSystem : MonoBehaviour
{
    public static PreviewSystem Instance;
    
    [Header("Preview Settings")]
    [SerializeField] private float previewYOffset = 0.06f;
    [SerializeField] private GameObject cellIndicator;
    [SerializeField] private Material pfPreviewMaterial;
    
    private GameObject _previewObject;
    private Material _previewMaterialInstance;
    private Renderer _cellIndicatorRenderer;
    
    [Header("Tool Settings")]
    [SerializeField] private Material validRadiusMaterial;
    [SerializeField] private Material invalidRadiusMaterial;
    [SerializeField] private Grid grid;
    
    private GameObject _toolIndicator;
    private Renderer _toolIndicatorRenderer;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else 
            Destroy(gameObject);
    }
    
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
    
    public void ShowToolIndicator(Vector2Int size)
    {
        if (_toolIndicator == null)
        {
            _toolIndicator = Instantiate(cellIndicator);
            _toolIndicatorRenderer = _toolIndicator.GetComponentInChildren<Renderer>();
        }
        
        _toolIndicator.transform.localScale = new Vector3(size.x, size.y, 1);
        _toolIndicator.SetActive(true);
        
        if (_cellIndicatorRenderer != null)
        {
            _cellIndicatorRenderer.material.mainTextureScale = size;
        }
    }
    
    public void UpdateToolIndicator(Vector3 pos, bool validity)
    {
        if (_toolIndicator == null) return;
        
        _toolIndicator.transform.position = pos;
        
        Color color = validity ? Color.white : Color.red;
        // color.a = 0.5f;
        
        if (_toolIndicatorRenderer != null)
        {
            _toolIndicatorRenderer.material.color = color;
        }
    }
    
    public void HideToolIndicator()
    {
        if (_toolIndicator != null)
        {
            _toolIndicator.SetActive(false);
        }
    }
}
