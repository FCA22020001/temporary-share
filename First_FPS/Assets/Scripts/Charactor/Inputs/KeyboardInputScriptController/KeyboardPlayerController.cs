using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace First_Fps
{
    public class KeyboardPlayerController : MonoBehaviour
    {

        [Header("Movements")]
        public float playerSpeed;
        public float playerNotGroundedSpeed;
        public float playerMaxSpeed;

        [Header("Friction")]
        public float playerGroundedFriction;
        public float playerNotGroundedFriction;

        [Header("Rotation")]
        public float playerCameraRotationSensitivity;
        public float playerBodyRotationBounds;

        [Header("Ground Detection")]
        public LayerMask mapWhatIsGround;
        public float playerHighOffset;
        public float playerRadius;
        public float playerOnGroundTimer;

        [Header("Jump")]
        public float playerJumpForce;
        public float playerJumpCooldown;

        [Header("Wall Running")]
        [SerializeField] private float _wallRunUpForce;
        [SerializeField] private float _wallRunPushForce;
        // Boolean that is used for adding forces when jumping off the walls, used to determine which direction.
        private bool isRightWall;
        private bool isLeftWall;

        //Used for effects etc.
        public static bool isWallRunning;
        // Checks the distance from walls and takes the wall that is the closest to the player
        private float distanceFromLeftWall;
        private float distanceFromRightWall;

        [Header("Others")]
        public Transform playerCameraHolder;
        public Rigidbody playerRigidBody;
        private Transform playerHead;

        private float _xRotation;
        private float _yRotation;
        private float _grounded;
        private bool _realGrounded;
        private float _jumpCooldown;

        
        private void Awake()
        {
            playerRigidBody = GetComponent<Rigidbody>();
        }
        
        private void FixedUpdate()
        {
            PlayerMovement();
            PlayerFriction();

        }

        private void Update()
        {
            PlayerGroundOn();
            Jumping();
            Rotation();
            wallChecker();
        }

        // Check the player is on ground
        private void PlayerGroundOn()
        {
            _grounded -= Time.deltaTime;
            var colliderList = new Collider[100];
            var size = Physics.OverlapSphereNonAlloc(transform.position + new Vector3(0, playerHighOffset, 0), playerRadius, colliderList, mapWhatIsGround);
            _realGrounded = size > 0;
            if (_realGrounded)
                _grounded = playerOnGroundTimer;
        }

        // WASD movements
        private void PlayerMovement()
        {
            var axis = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

            var speed = _realGrounded ? playerSpeed : playerNotGroundedSpeed;
            var vertical = axis.y * speed * Time.fixedDeltaTime * transform.forward;
            var horizontal = axis.x * speed * Time.fixedDeltaTime * transform.right;

            if (CanApplyForce(vertical, axis))
                playerRigidBody.velocity += vertical;

            if (CanApplyForce(horizontal, axis))
                playerRigidBody.velocity += horizontal;
        }

        // Check player <--> wall distance
        private void wallChecker()
        {
            RaycastHit rightRaycast;
            RaycastHit leftRaycast;

            if (Physics.Raycast(playerHead.transform.position, playerHead.transform.right, out rightRaycast))
            {
                distanceFromRightWall = Vector3.Distance(playerHead.transform.position, rightRaycast.point);
                if (distanceFromRightWall <= 3f)
                {
                    isRightWall = true;
                    isLeftWall = false;
                }
            }
            if (Physics.Raycast(playerHead.transform.position, -playerHead.transform.right, out leftRaycast))
            {
                distanceFromLeftWall = Vector3.Distance(playerHead.transform.position, leftRaycast.point);
                if (distanceFromLeftWall <= 3f)
                {
                    isRightWall = false;
                    isLeftWall = true;
                }
            }
        }

        private void PlayerFriction()
        {
            var vel = playerRigidBody.velocity;
            var target = _realGrounded ? playerGroundedFriction : playerNotGroundedFriction;
            vel.x = Mathf.Lerp(vel.x, 0f, target * Time.fixedDeltaTime);
            vel.z = Mathf.Lerp(vel.z, 0f, target * Time.fixedDeltaTime);
            playerRigidBody.velocity = vel;
        }

        private void Rotation()
        {
            if (Cursor.lockState != CursorLockMode.Locked)
                Cursor.lockState = CursorLockMode.Locked;

            var mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            _xRotation -= mouseDelta.y * playerCameraRotationSensitivity;
            _xRotation = Mathf.Clamp(_xRotation, -playerBodyRotationBounds, playerBodyRotationBounds);
            _yRotation += mouseDelta.x * playerCameraRotationSensitivity;

            transform.rotation = Quaternion.Euler(0, _yRotation, 0);
            playerCameraHolder.localRotation = Quaternion.Euler(_xRotation, 0, 0);
        }

        private void Jumping()
        {
            _jumpCooldown -= Time.deltaTime;
            if (!(_grounded >= 0) || !(_jumpCooldown <= 0) || !Input.GetKeyDown(KeyCode.Space)) return;
            var vel = playerRigidBody.velocity;
            vel.y = playerJumpForce;
            playerRigidBody.velocity = vel;
            _jumpCooldown = playerJumpCooldown;
        }
        
        // Wall running
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.transform.CompareTag("RunnableWall"))
            {
                isWallRunning = true;
                playerRigidBody.useGravity = false;

                if (isLeftWall)
                {
                    playerCameraHolder.transform.localEulerAngles = new Vector3(0f, 0f, -10f);
                }
                if (isRightWall)
                {
                    playerCameraHolder.transform.localEulerAngles = new Vector3(0f, 0f, 10f);
                }
            }
        }
        private void OnCollisionStay(Collision collision)
        {
            if (collision.transform.CompareTag("RunnableWall"))
            {
                if (Input.GetKey(KeyCode.Space) && isLeftWall)
                {
                    playerRigidBody.AddForce(Vector3.up * _wallRunUpForce, ForceMode.Impulse);
                    playerRigidBody.AddForce(playerHead.transform.right * _wallRunUpForce, ForceMode.Impulse);
                }
                if (Input.GetKey(KeyCode.Space) && isRightWall)
                {
                    playerRigidBody.AddForce(Vector3.up * _wallRunUpForce, ForceMode.Impulse);
                    playerRigidBody.AddForce(-playerHead.transform.right * _wallRunUpForce, ForceMode.Impulse);
                }
            }
        }
        private void OnCollisionExit(Collision collision)
        {

            if (collision.transform.CompareTag("RunnableWall"))
            {
                playerCameraHolder.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
                isWallRunning = false;
                playerRigidBody.useGravity = true;
            }
        }
        
        private bool CanApplyForce(Vector3 target, Vector2 axis)
        {
            var targetC = Get2DVec(target).normalized;
            var velocityC = Get2DVec(playerRigidBody.velocity).normalized;
            var dotProduct = Vector2.Dot(velocityC, targetC);
            return dotProduct <= 0 || dotProduct * Get2DVec(playerRigidBody.velocity).magnitude < playerMaxSpeed * GetAxisForce(axis);
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
}
