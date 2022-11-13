using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace First_Fps
{
    public class InputOriginManager : MonoBehaviour
    {

        public static bool Camera()
        {
            // マウスの場合

            // タッチパネルの場合

            return false;
        }

        public void GetHorizontalAxis()
        {
            float x = Input.GetAxis("Horizontal");
            Debug.Log(x);
        }
    }
}
