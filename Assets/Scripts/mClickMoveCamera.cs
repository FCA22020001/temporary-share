/////////////////////////////////////////////////
//      Shields_Project
//            Created by FCA22020001
//            Since 2022/10/25
/////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mClickMoveCamera : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;

    private Vector2 curCameraPos; // Current camera's position
    private Vector2 mClickPos;    // Middle click position
    private Vector2 mClickingPos; // Middle clicking position
    private Vector2 mClickPosDif; // Middle click first mouse position - Middle clicking current mouse position


    void Update()
    {
        // If mouse is don't move, lock camera position
        mClickPos = mClickingPos;

        // Get mouse start posision
        if (Input.GetMouseButtonDown(2))
        {
            mClickPos = Input.mousePosition;
        }

        // Calculate mouse position diff and camera position
        if (Input.GetMouseButton(2))
        {
            // Get current camera position
            curCameraPos = mainCamera.transform.position;
            // Get mouse position
            mClickingPos = Input.mousePosition;
            // Calculate mouse position diff
            mClickPosDif = mClickPos - mClickingPos;
            // Calculate camera position
            mainCamera.transform.position = new Vector3(mClickPosDif.x + curCameraPos.x, mClickPosDif.y + curCameraPos.y, -10);
        }

        // Reset camera's position
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetMouseButtonDown(2))
            {
                mainCamera.transform.position = new Vector3(0, 0, -10);
            }
        }
    }
}
