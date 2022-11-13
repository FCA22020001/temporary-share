using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    [Header("Movement")]
    public float movementSpeed;
    public float airMovementSpeed;
    public float maxSpeed;

    [Header("Friction")]
    public float friction;
    public float airFriction;

    [Header("Rotation")]
    public float rotationSensitivity;
    public float rotationBounds;

    [Header("Ground Detection")]
    public LayerMask whatIsGround;
    public float checkYOffset;
    public float checkRadius;
    public float groundTimer;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;

    [Header("Data")]
    public Transform cameraHolder;
    public Rigidbody rb;

    private float _xRotation;
    private float _yRotation;
    private float _grounded;
    private bool _realGrounded;
    private float _jumpCooldown;

    private void FixedUpdate()
    {
        ApplyMovement();
        ApplyFriction();
    }

    private void Update()
    {
        GroundCheck();
        Jumping();
        Rotation();
    }

    private void GroundCheck()
    {
        _grounded -= Time.deltaTime;
        var colliderList = new Collider[100];
        var size = Physics.OverlapSphereNonAlloc(transform.position + new Vector3(0, checkYOffset, 0), checkRadius, colliderList, whatIsGround);
        _realGrounded = size > 0;
        if (_realGrounded)
            _grounded = groundTimer;
    }

    private void ApplyMovement()
    {
        var axis = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;

        var speed = _realGrounded ? movementSpeed : airMovementSpeed;
        var vertical = axis.y * speed * Time.fixedDeltaTime * transform.forward;
        var horizontal = axis.x * speed * Time.fixedDeltaTime * transform.right;

        if (CanApplyForce(vertical, axis))
            rb.velocity += vertical;

        if (CanApplyForce(horizontal, axis))
            rb.velocity += horizontal;
    }

    private void ApplyFriction()
    {
        var vel = rb.velocity;
        var target = _realGrounded ? friction : airFriction;
        vel.x = Mathf.Lerp(vel.x, 0f, target * Time.fixedDeltaTime);
        vel.z = Mathf.Lerp(vel.z, 0f, target * Time.fixedDeltaTime);
        rb.velocity = vel;
    }

    private void Rotation()
    {
        if (Cursor.lockState != CursorLockMode.Locked)
            Cursor.lockState = CursorLockMode.Locked;

        var mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        _xRotation -= mouseDelta.y * rotationSensitivity;
        _xRotation = Mathf.Clamp(_xRotation, -rotationBounds, rotationBounds);
        _yRotation += mouseDelta.x * rotationSensitivity;

        transform.rotation = Quaternion.Euler(0, _yRotation, 0);
        cameraHolder.localRotation = Quaternion.Euler(_xRotation, 0, 0);
    }

    private void Jumping()
    {
        _jumpCooldown -= Time.deltaTime;
        if (!(_grounded >= 0) || !(_jumpCooldown <= 0) || !Input.GetKeyDown(KeyCode.Space)) return;
        var vel = rb.velocity;
        vel.y = jumpForce;
        rb.velocity = vel;
        _jumpCooldown = jumpCooldown;
    }

    private bool CanApplyForce(Vector3 target, Vector2 axis)
    {
        var targetC = Get2DVec(target).normalized;
        var velocityC = Get2DVec(rb.velocity).normalized;
        var dotProduct = Vector2.Dot(velocityC, targetC);
        return dotProduct <= 0 || dotProduct * Get2DVec(rb.velocity).magnitude < maxSpeed * GetAxisForce(axis);
    }

    private static float GetAxisForce(Vector2 axis)
    {
        return (int)axis.x != 0 ? Mathf.Abs(axis.x) : Mathf.Abs(axis.y);
    }

    private static Vector2 Get2DVec(Vector3 vec)
    {
        return new Vector2(vec.x, vec.z);
    }
}


