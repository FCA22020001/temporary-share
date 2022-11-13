using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace First_Fps
{
    public class Look : MonoBehaviour
    {
        private Variables var;

        private float xRotation;
        private float yRotation;

        private void Start()
        {
            var = GetComponent<Variables>();
        }

        public void playerLook()
        {
            if (var.grounded == true && var.wallRunning == false)
            {
                // Get mouse input axis
                float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * var.sensitivity;
                float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * var.sensitivity;

                yRotation += mouseX;

                xRotation -= mouseY;
                xRotation = Mathf.Clamp(xRotation, -90f, 90f);

                var.playerBody.rotation = Quaternion.Euler(0, yRotation, 0);
                var.playerCamera.rotation = Quaternion.Euler(xRotation, yRotation, 0);
            }
        }
    }
}
