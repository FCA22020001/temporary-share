using System;
using UnityEngine;

namespace First_Fps
{
    public class Jump : MonoBehaviour
    {
        private Variables var;

        [SerializeField] public float jumpForce = 137.5f;
        [SerializeField] private Vector3 normalVector = Vector3.up;

        private void Start()
        {
            var = GetComponent<Variables>();
        }

        public void playerJump()
        {
            if (Input.GetKeyDown(var.jumpInput) && var.grounded == true && var.readyJump == true)
            {
                var.grounded = false;
                Debug.Log(var.grounded);
                var.readyJump = false;
                var.readyDoubleJump = true;

                // Add jump force
                var.playerBody.AddForce(Vector2.up * jumpForce * 1.5f);
            }
        }
    }
}
