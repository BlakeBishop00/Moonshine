using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 6f;

    public float gravity = -20f;

    [Header("Ground Movement")]
    public float groundAccelTime = 0.08f;

    public float groundStopTime = 0.08f;

    public float stopSharpness = 4.6f;

    public Vector2 WorldBorder;
    public float worldFloor;

    [Header("Air Movement")]
    public float airAccelTime = 0.5f;

    public float airFriction = 0.5f;
    [Header("Jumping")]
    public float jumpForce = 8f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 2f;
    public Transform cameraTransform;

    public float minPitch = -80f;
    public float maxPitch = 80f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCollideDist = 0.1f;
    public float allowJumpDist = 0.1f;
    public LayerMask groundMask;

    // New Input System
    [Header("Input")]
    public PlayerInputActions inputActions;

    private CharacterController controller;
    private Vector3 velocity;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float pitch = 0f;
    private bool isGrounded;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (controller == null)
        {
            enabled = false;
            return;
        }

        if (cameraTransform == null)
        {
            cameraTransform = Camera.main?.transform;
        }


        inputActions = new PlayerInputActions();

        inputActions.Player.Enable();

        // Subscribe (you can also use PlayerInput component + messages / UnityEvents)
        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        inputActions.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Look.canceled += ctx => lookInput = Vector2.zero;

        inputActions.Player.Jump.performed += ctx => DoJump();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDestroy()
    {
        if (inputActions != null)
        {
            inputActions.Player.Disable();
        }
    }

    void Update()
    {

        // send to rigid body and we shouldn't need two checks.
        // Rigid body is probably better too because you can send objects flying around, I don't think you can with char controller

        // allowJump because isGrounded will make them stop that much distance above the ground
        allowJump = Physics.CheckSphere(groundCheck.position, allowJumpDist, groundMask);
        // the legit floor colliding distance
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCollideDist, groundMask);
        

        Vector2 lookDelta = lookInput * mouseSensitivity;

        transform.Rotate(0f, lookDelta.x, 0f);

        pitch -= lookDelta.y;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        Vector3 wishDir = (transform.right * moveInput.x + transform.forward * moveInput.y).normalized;
        bool hasInput = moveInput.sqrMagnitude > 0.01f;

        float groundAccel = moveSpeed / Mathf.Max(groundAccelTime, 0.001f);
        float groundFriction = stopSharpness / Mathf.Max(groundStopTime, 0.001f);
        float airAccel = moveSpeed / Mathf.Max(airAccelTime, 0.001f);

        Vector3 horizVel = new Vector3(velocity.x, 0f, velocity.z);

        if (isGrounded)
        {
            if (velocity.y != 0 && !iReallyWantJump) // use I want jump to not reset the player's velocity to 0 if they're trying to jump
            {
                velocity.y = 0;
            }

            horizVel = ApplyFriction(horizVel, groundFriction);

            if (hasInput)
            {
                horizVel = ApplyAcceleration(horizVel, wishDir, moveSpeed, groundAccel);
            }
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;

            horizVel = ApplyFriction(horizVel, airFriction);

            if (hasInput)
            {
                horizVel = ApplyAcceleration(horizVel, wishDir, moveSpeed, airAccel);
            }
        }

        velocity.x = horizVel.x;
        velocity.z = horizVel.z;

        if (horizVel.sqrMagnitude < 0.0001f)
        {
            velocity.x = 0f;
            velocity.z = 0f;
        }

        controller.Move(velocity * Time.deltaTime);

        Vector3 pos = transform.position;

        pos.x = Mathf.Clamp(pos.x, -WorldBorder.x, WorldBorder.x);
        pos.z = Mathf.Clamp(pos.z, -WorldBorder.y, WorldBorder.y);
        if(pos.y < worldFloor)
        {
            pos.y = worldFloor;
        }

        transform.position = pos;
    }

    bool allowJump;
    bool iReallyWantJump;
    public float pauseGroundChecksTime = 0.05f;

    private void DoJump()
    {
        if (!allowJump)
            return;

        iReallyWantJump = true;
        allowJump = false;
        velocity.y = jumpForce;
        Invoke("PauseBeforeCheckingGroundAgain", pauseGroundChecksTime);
    }

    void PauseBeforeCheckingGroundAgain()
    {
        // not the best way to do this but it will just stop doing grounded checks momentarily to allow the player to get off the ground,
        // since I've made it reset the player's velocity if they hit the ground (before it would keep their velocity, and they would have something like -1000 velocity even when on the ground)

        iReallyWantJump = false;

    }

    private Vector3 ApplyFriction(Vector3 vel, float friction)
    {
        float speed = vel.magnitude;
        if (speed < 0.01f) return Vector3.zero;

        float drop = speed * friction * Time.deltaTime;
        float newSpeed = Mathf.Max(speed - drop, 0f);
        return vel * (newSpeed / speed);
    }

    private Vector3 ApplyAcceleration(Vector3 vel, Vector3 wishDir, float wishSpeed, float accel)
    {
        float currentSpeed = Vector3.Dot(vel, wishDir);
        float addSpeed = wishSpeed - currentSpeed;
        if (addSpeed <= 0) return vel;

        float accelSpeed = Mathf.Min(accel * Time.deltaTime, addSpeed);
        return vel + accelSpeed * wishDir;
    }
}