using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollowTarget : MonoBehaviour
{
    public Transform target;
    public Vector3 offsetPos;
    public float moveSpeed = 5;
    public float rotateSpeed = 360f;

    Vector3 _velocity = Vector3.zero;
    Vector3 _targetPos;

    float smooth = 0f;
    float _targetY = 45f;

    private PlayerInputActions _inputActions;

    private void Awake()
    {
        _inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        _inputActions.Enable();
        _inputActions.Camera.RotateLeft.performed += OnRotateLeft;
        _inputActions.Camera.RotateRight.performed += OnRotateRight;
    }

    private void OnDisable()
    {
        _inputActions.Camera.RotateLeft.performed -= OnRotateLeft;
        _inputActions.Camera.RotateRight.performed -= OnRotateRight;
        _inputActions.Disable();
    }

    void LateUpdate()
    {
        MoveWithTarget();

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            Quaternion.Euler(30f, _targetY, 0f),
            360f * Time.deltaTime
        );
    }

    void MoveWithTarget()
    {
        _targetPos = target.transform.position + offsetPos;
        transform.position = Vector3.SmoothDamp(transform.position, _targetPos, ref _velocity, smooth);
    }

    private void OnRotateLeft(InputAction.CallbackContext context)
    {
        ToIso.RotateTarget(90);
        _targetY += 90f;
    }

    private void OnRotateRight(InputAction.CallbackContext context)
    {
        ToIso.RotateTarget(-90);
        _targetY -= 90f;
    }
}