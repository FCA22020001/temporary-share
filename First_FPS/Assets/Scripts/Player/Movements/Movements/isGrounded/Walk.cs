using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace First_Fps
{
    public class Walk : MonoBehaviour
    {
        private Variables var;
        //private float playerWalkSpeed = 5f;

        private void Start()
        {
            var = GetComponent<Variables>();
        }

        public void playerWalk()
        {
            Vector3 forward = var.playerBody.transform.forward.normalized;
            Vector3 back = -var.playerBody.transform.forward.normalized;
            Vector3 right = var.playerBody.transform.right.normalized;
            Vector3 left = -var.playerBody.transform.right.normalized;

            // Moving
            if (Input.GetKey(var.forwardInput))
            {
                var.playerBody.AddForce(forward * 10.0f, ForceMode.Force);
            }
            if (Input.GetKey(var.backwardInput))
            {
                var.playerBody.AddForce(back * 10.0f, ForceMode.Force);
            }
            if (Input.GetKey(var.leftInput))
            {
                var.playerBody.AddForce(left * 10.0f, ForceMode.Force);
            }
            if (Input.GetKey(var.rightInput))
            {
                var.playerBody.AddForce(right * 10.0f, ForceMode.Force);
            }
        }
    }
}
