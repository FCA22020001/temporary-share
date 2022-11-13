using System;
using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
    [Header("Assignables")]
    [SerializeField] private Transform playerCam; // Player's camera(main camera)
    [SerializeField] private Transform playerBody; // Player's body
    [SerializeField] private Transform orientation; // Player's body controll joint
    [SerializeField, HideInInspector] private Rigidbody rb; // Player's rigidbody

    [Header("Mouse input body rotation & look around")]
    [SerializeField] private float xRotation;
    [SerializeField] private float sensitivity = 50f;
    [SerializeField] private float sensMultiplier = 1.0f;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 4500;
    [SerializeField] private float maxSpeed = 20;
    [SerializeField] private bool grounded;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private float counterMovement = 0.175f;
    [SerializeField] private float threshold = 0.01f;
    [SerializeField] private float maxSlopeAngle = 35f;

    [Header("Crouch & slide")]
    [SerializeField] private Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    [SerializeField] private Vector3 playerScale;
    [SerializeField] public float slideForce = 400;
    [SerializeField] public float slideCounterMovement = 0.2f;

    [Header("Jump")]
    [SerializeField] private bool readyToJump = true;
    [SerializeField] private float jumpCooldown = 0.25f;
    [SerializeField] public float jumpForce = 137.5f;

    [Header("Sliding")]
    [SerializeField] private Vector3 normalVector = Vector3.up;
    [SerializeField] private Vector3 wallNormalVector;

    [Header("Wall Running")]
    [SerializeField] private float wallRunUpForce;
    [SerializeField] private float wallRunPushForce;

    // Boolean that is used for adding forces when jumping off the walls, used to determine which direction.
    private bool isRightWall;
    private bool isLeftWall;

    // Used for effects etc.
    public static bool isWallRunning;
    // Checks the distance from walls and takes the wall that is the closest to the player
    private float distanceFromLeftWall;
    private float distanceFromRightWall;

    //Used to add forces.
    public Transform head;

    [Header("Input")]
    [SerializeField] private float x, y;
    [SerializeField] private bool jumping, sprinting, crouching;

    private float desiredX;
    private bool cancellingGrounded;

    //---------------------------------------------------------------------------------//

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        playerScale = transform.localScale;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void FixedUpdate()
    {
        PlayerMovement();
    }
    private void Update()
    {
        PlayerInput();
        Look();
        wallChecker();
    }

    //---------------------------------------------------------------------------------//

    private void PlayerInput()
    {
        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");
        jumping = Input.GetButton("Jump");
        crouching = Input.GetKey(KeyCode.LeftControl);

        // Crouching
        if (Input.GetKeyDown(KeyCode.LeftControl))
            StartCrouch();
        if (Input.GetKeyUp(KeyCode.LeftControl))
            StopCrouch();
    }
    private void PlayerMovement()
    {
        // Gravity
        rb.AddForce(Vector3.down * Time.deltaTime * 10);

        //Find actual velocity relative to where player is looking
        Vector2 mag = FindVelRelativeToLook();
        float xMag = mag.x, yMag = mag.y;

        //Counteract sliding and sloppy movement
        CounterMovement(x, y, mag);

        //If holding jump && ready to jump, then jump
        if (readyToJump && jumping) PlayerJump();

        //Set max speed
        float maxSpeed = this.maxSpeed;

        //If sliding down a ramp, add force down so player stays grounded and also builds speed
        if (crouching && grounded && readyToJump)
        {
            rb.AddForce(Vector3.down * Time.deltaTime * 3000);
            return;
        }

        //If speed is larger than maxspeed, cancel out the input so you don't go over max speed
        if (x > 0 && xMag > maxSpeed) x = 0;
        if (x < 0 && xMag < -maxSpeed) x = 0;
        if (y > 0 && yMag > maxSpeed) y = 0;
        if (y < 0 && yMag < -maxSpeed) y = 0;

        //Some multipliers
        float multiplier = 1f, multiplierV = 1f;

        // Movement in air
        if (!grounded)
        {
            multiplier = 0.5f;
            multiplierV = 0.5f;
        }

        // Movement while sliding
        if (grounded && crouching) multiplierV = 0f;

        //Apply forces to move player
        rb.AddForce(orientation.transform.forward * y * moveSpeed * Time.deltaTime * multiplier * multiplierV);
        rb.AddForce(orientation.transform.right * x * moveSpeed * Time.deltaTime * multiplier);
    }
    private void PlayerJump()
    {
        if (grounded && readyToJump)
        {
            readyToJump = false;

            //Add jump forces
            rb.AddForce(Vector2.up * jumpForce * 1.5f);
            rb.AddForce(normalVector * jumpForce * 0.5f);

            //If jumping while falling, reset y velocity.
            Vector3 vel = rb.velocity;
            if (rb.velocity.y < 0.5f)
                rb.velocity = new Vector3(vel.x, 0, vel.z);
            else if (rb.velocity.y > 0)
                rb.velocity = new Vector3(vel.x, vel.y / 2, vel.z);

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }
    private void ResetJump()
    {
        readyToJump = true;
    }
    private void StartCrouch()
    {
        transform.localScale = crouchScale;
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
        if (rb.velocity.magnitude > 0.5f)
        {
            if (grounded)
            {
                rb.AddForce(orientation.transform.forward * slideForce);
            }
        }
    }
    private void StopCrouch()
    {
        transform.localScale = playerScale;
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
    }
    private void CounterMovement(float x, float y, Vector2 mag)
    {
        if (!grounded || jumping) return;

        //Slow down sliding
        if (crouching)
        {
            rb.AddForce(moveSpeed * Time.deltaTime * -rb.velocity.normalized * slideCounterMovement);
            return;
        }

        //Counter movement
        if (Math.Abs(mag.x) > threshold && Math.Abs(x) < 0.05f || (mag.x < -threshold && x > 0) || (mag.x > threshold && x < 0))
        {
            rb.AddForce(moveSpeed * orientation.transform.right * Time.deltaTime * -mag.x * counterMovement);
        }
        if (Math.Abs(mag.y) > threshold && Math.Abs(y) < 0.05f || (mag.y < -threshold && y > 0) || (mag.y > threshold && y < 0))
        {
            rb.AddForce(moveSpeed * orientation.transform.forward * Time.deltaTime * -mag.y * counterMovement);
        }

        //Limit diagonal running. This will also cause a full stop if sliding fast and un-crouching, so not optimal.
        if (Mathf.Sqrt((Mathf.Pow(rb.velocity.x, 2) + Mathf.Pow(rb.velocity.z, 2))) > maxSpeed)
        {
            float fallspeed = rb.velocity.y;
            Vector3 n = rb.velocity.normalized * maxSpeed;
            rb.velocity = new Vector3(n.x, fallspeed, n.z);
        }
    }
    private void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.fixedDeltaTime * sensMultiplier;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.fixedDeltaTime * sensMultiplier;

        //Find current look rotation
        Vector3 rot = playerCam.transform.localRotation.eulerAngles;
        desiredX = rot.y + mouseX;

        //Rotate, and also make sure we dont over- or under-rotate.
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //Perform the rotations
        playerCam.transform.localRotation = Quaternion.Euler(xRotation, desiredX, 0);
        orientation.transform.localRotation = Quaternion.Euler(0, desiredX, 0);
    }
    private void wallChecker()
    {
        //Debug.Log("wallChecker");
        RaycastHit rightRaycast;
        RaycastHit leftRaycast;

        Debug.Log("HeadPosition: " + head.transform.position + "\nHeadRight: " + head.transform.right);

        if (Physics.Raycast(head.transform.position, head.transform.right, out rightRaycast))
        {
            distanceFromRightWall = Vector3.Distance(head.transform.position, rightRaycast.point);
            //Debug.Log(rightRaycast.collider.gameObject.transform.position);
            Debug.Log(distanceFromRightWall);
            if (distanceFromRightWall <= 0.5f)
            {
                isRightWall = true;
                isLeftWall = false;
            }
        }
        if (Physics.Raycast(head.transform.position, -head.transform.right, out leftRaycast))
        {
            distanceFromLeftWall = Vector3.Distance(head.transform.position, leftRaycast.point);
            Debug.Log(distanceFromLeftWall);
            if (distanceFromLeftWall <= 0.5f)
            {
                isRightWall = false;
                isLeftWall = true;
            }
        }
    }
    public Vector2 FindVelRelativeToLook()
    {
        float lookAngle = orientation.transform.eulerAngles.y;
        float moveAngle = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg;

        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;

        float magnitue = rb.velocity.magnitude;
        float yMag = magnitue * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitue * Mathf.Cos(v * Mathf.Deg2Rad);

        return new Vector2(xMag, yMag);
    }

    private bool IsFloor(Vector3 v)
    {
        float angle = Vector3.Angle(Vector3.up, v);
        return angle < maxSlopeAngle;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (grounded == false)
        {
            if (collision.transform.CompareTag("RunnableWall"))
            {
                isWallRunning = true;
                rb.useGravity = false;

                Vector3 rot = playerCam.transform.localRotation.eulerAngles;

                if (isLeftWall)
                {
                    playerCam.transform.localRotation = Quaternion.Euler(rot.x, rot.y, -10f);
                    //playerCam.transform.localEulerAngles = new Vector3(rot.x, rot.y, -90f);
                    Debug.Log("isLeft");
                }
                if (isRightWall)
                {
                    playerCam.transform.localRotation = Quaternion.Euler(rot.x, rot.y, 10f);
                    //playerCam.transform.localEulerAngles = new Vector3(rot.x, rot.y, 90f);
                    Debug.Log("isRight");
                }
            }
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        //Make sure we are only checking for walkable layers
        int layer = collision.gameObject.layer;
        if (whatIsGround != (whatIsGround | (1 << layer))) return;

        //Iterate through every collision in a physics update
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.contacts[i].normal;
            //FLOOR
            if (IsFloor(normal))
            {
                grounded = true;
                cancellingGrounded = false;
                normalVector = normal;
                CancelInvoke(nameof(StopGrounded));
            }
        }

        //Invoke ground/wall cancel, since we can't check normals with CollisionExit
        float delay = 3f;
        if (!cancellingGrounded)
        {
            cancellingGrounded = true;
            Invoke(nameof(StopGrounded), Time.deltaTime * delay);
        }

        if (collision.transform.CompareTag("RunnableWall"))
        {
            if (Input.GetKeyDown(KeyCode.Space) && isLeftWall)
            {
                rb.AddForce(Vector3.up * wallRunUpForce, ForceMode.Impulse);
                rb.AddForce(head.transform.right * wallRunUpForce, ForceMode.Impulse);
            }
            if (Input.GetKeyDown(KeyCode.Space) && isRightWall)
            {
                rb.AddForce(Vector3.up * wallRunUpForce, ForceMode.Impulse);
                rb.AddForce(-head.transform.right * wallRunUpForce, ForceMode.Impulse);
            }
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        Vector3 rot = playerCam.transform.localRotation.eulerAngles;

        if (collision.transform.CompareTag("RunnableWall"))
        {
            playerCam.transform.eulerAngles = new Vector3(rot.x, rot.y, 0f);
            isWallRunning = false;
            rb.useGravity = true;
        }
    }
    private void StopGrounded()
    {
        grounded = false;
    }
}