using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float speedTurn;
    public bool matrix = false;

    private Rigidbody _rb;
    private Vector3 _input;
    private Vector3 _relative;

    private PlayerInputActions _inputActions;

    private void Awake()
    {
        _inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        _inputActions.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Disable();
    }

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        InputGet();
    }

    void InputGet()
    {
        Vector2 inputVector = _inputActions.Player.Move.ReadValue<Vector2>();
        _input = new Vector3(inputVector.x, 0, inputVector.y);
    }

    void FixedUpdate()
    {
        if (_input != Vector3.zero)
        {
            Look();
            Move();
        }
        else
        {
            _rb.linearVelocity = new Vector3(
                0,
                _rb.linearVelocity.y,
                0
            );
        }
    }

    void Move()
    {
        Vector3 moveDir = matrix
            ? _input.SetToIso().normalized
            : _input.normalized;

        Vector3 targetVelocity = new Vector3(
            moveDir.x * speed,
            _rb.linearVelocity.y,
            moveDir.z * speed
        );

        _rb.linearVelocity = Vector3.Lerp(
            _rb.linearVelocity,
            targetVelocity,
            10f * Time.fixedDeltaTime
        );
    }

    void Look()
    {
        Vector3 lookDir = matrix
            ? _input.SetToIso()
            : _input;

        if (lookDir.sqrMagnitude < 0.01f)
            return;

        Quaternion targetRot =
            Quaternion.LookRotation(lookDir, Vector3.up);

        //transform.rotation = Quaternion.RotateTowards(
        //    transform.rotation,
        //    targetRot,
        //    speedTurn * Time.deltaTime
        //);

        _rb.MoveRotation(
            Quaternion.RotateTowards(
                _rb.rotation,
                targetRot,
                speedTurn * Time.fixedDeltaTime
            )
        );
    }
}