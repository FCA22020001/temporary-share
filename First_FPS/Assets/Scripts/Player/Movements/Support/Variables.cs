using System;
using UnityEngine;

namespace First_Fps
{
    public class Variables : MonoBehaviour
    {
        public Rigidbody playerBody;
        public Transform playerHead;

        // PlayerStatus
        public bool grounded;
        public bool readyJump;
        public bool readyDoubleJump;
        public bool wallRunning;
        public bool rightWall;
        public bool leftWall;

        // Keybind
        public KeyCode forwardInput = KeyCode.W;
        public KeyCode backwardInput = KeyCode.S;
        public KeyCode leftInput = KeyCode.A;
        public KeyCode rightInput = KeyCode.D;
        public KeyCode sprintInput = KeyCode.LeftShift;
        public KeyCode crouchInput = KeyCode.LeftControl;
        public KeyCode jumpInput = KeyCode.Space;
        public KeyCode tabMenuInput = KeyCode.Tab;
        public KeyCode escMenuInput = KeyCode.Escape;

        // Player input config
        public float sensitivity = 50f;
        public float sensMultiplier = 1.0f;
        public bool sprintHoldSwitch;

        // Other
        public Transform playerCamera;
        public Transform orientation;
    }
}
