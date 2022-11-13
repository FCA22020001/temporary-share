using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace First_Fps
{
    public class Gravity : MonoBehaviour
    {
        private Variables var;

        private void Start()
        {
            var = GetComponent<Variables>();
        }

        public void playerGravity()
        {
            var.playerBody.AddForce(Vector3.down * Time.deltaTime * 10);
        }
    }
}
