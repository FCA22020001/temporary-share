/////////////////////////////////////////////////
//      Shields_Project
//            Created by FCA22020001
//            Since 2022/10/25
/////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelection : MonoBehaviour
{
    [SerializeField]
    RectTransform selectionBox;  // box selector image

    private Vector2 lClickPos; // left click position
    private Vector2 lClickingPos; // current position

    private void Update()
    {
        // Mouse down
        if (Input.GetMouseButtonDown(0))
        {
            // Get mouse position
            lClickPos = Input.mousePosition;
            Debug.Log("lClickPos: " + lClickPos);

            // Show the selection box
            selectionBox.gameObject.SetActive(true);

            // Set selectionBox anchored position
            selectionBox.anchoredPosition = lClickPos;
        }

        // Mouse held down
        if (Input.GetMouseButton(0))
        {
            // Get current mouse position
            lClickingPos = Input.mousePosition;

            // Calculate selectionBox width and height
            float width = (lClickingPos.x - lClickPos.x);
            float height = (lClickingPos.y - lClickPos.y);

            // Calculate and set selectionBox scale
            selectionBox.sizeDelta = new Vector2(width, height);
        }

        // Mouse up
        if (Input.GetMouseButtonUp(0))
        {
            // Hide the selectionBox
            selectionBox.gameObject.SetActive(false);
        }
    }
}
