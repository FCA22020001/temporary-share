using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace First_Fps
{
    public class CoreInputController : MonoBehaviour
    {
        [SerializeField] private Variables var;
        [SerializeField] private Gravity gravity;
        [SerializeField] private Look look;
        [SerializeField] private Walk walk;
        [SerializeField] private Jump jump;

        [SerializeField] private GroundChecker groundChecker;

        //-----------------------------------------------------------------------------//

        private void Awake()
        {
            playerRigidGet();
        }
        private void Start()
        {
            getComponents();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        private void FixedUpdate()
        {
            gravity.playerGravity();
        }
        private void Update()
        {
            playerStatus();

            playerLookAround();
            playerMovement();
        }

        //-----------------------------------------------------------------------------//

        public void playerRigidGet()
        {
            var.playerBody.GetComponent<Rigidbody>();
        }
        private void getComponents()
        {
            var = GetComponent<Variables>();

            gravity = GetComponent<Gravity>();
            look = GetComponent<Look>();
            walk = GetComponent<Walk>();
            jump = GetComponent<Jump>();

            groundChecker = GetComponent<GroundChecker>();
        }
        private void playerStatus()
        {
            groundChecker.groundChecker();
        }
        private void playerMovement()
        {
            walk.playerWalk();
            jump.playerJump();
        }
        private void playerLookAround()
        {
            look.playerLook();
        }
    }
}
