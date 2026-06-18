using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField] private Camera sceneCamera;
    [SerializeField] private LayerMask placementLayerMask;
    public event Action OnClicked, OnExit;

    private Vector3 _lastMousePosition;
    
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame) 
            OnClicked?.Invoke();
        
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            OnExit?.Invoke();
    }

    public bool IsPointerOverUIObject()
    {
        return MouseItemData.IsPointerOverUIObject();
    }

    public Vector3 GetSelectedMapPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = sceneCamera.nearClipPlane;
        Ray ray = sceneCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, placementLayerMask))
        {
            _lastMousePosition = hit.point;
        }
        return _lastMousePosition;
    }
}
