using System;
using UnityEngine;

namespace First_Fps
{
    public class GroundChecker : MonoBehaviour
    {
        private Variables var;
        private Ray ray;
        private RaycastHit hit;
        private Vector3 rayPosition;

        private float distance = 1.1f;
        //[SerializeField] private LayerMask whatIsGround; 

        private void Start()
        {
            var = GetComponent<Variables>();
        }

        
        public void groundChecker()
        {
            rayPosition = var.playerBody.transform.localPosition; // Ray shot position
            ray = new Ray(rayPosition, -var.playerBody.transform.up); // Shot ray down
            Debug.DrawRay(ray.origin, ray.direction * distance, Color.red); // Set ray color

            if (Physics.Raycast(ray, out hit, distance))
            {
                if (hit.collider.tag == "Ground")
                {
                    var.grounded = true;
                    Debug.Log(var.grounded);

                    var.readyJump = true;
                    var.readyDoubleJump = false;
                }
            }
        }

        /*
        private bool IsFloor(Vector3 v)
        {
            float angle = Vector3.Angle(Vector3.up, v);
            return angle < 25f;
        }
        private void OnCollisionStay(Collision collision)
        {
            int layer = collision.gameObject.layer;

            if (whatIsGround != (whatIsGround | (1 << layer))) return;

            for (int i = 0; i < collision.contactCount; i++)
            {
                Vector3 normal = collision.contacts[i].normal;

                if (IsFloor(normal))
                {
                    var.grounded = true;
                    var.readyJump = true;
                    var.readyDoubleJump = false;
                }
            }
        } */
    }
}
