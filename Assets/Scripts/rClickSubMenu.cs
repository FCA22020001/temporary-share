/////////////////////////////////////////////////
//      Shields_Project
//            Created by FCA22020001
//            Since 2022/10/25
/////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rClickSubMenu : MonoBehaviour
{
    [SerializeField]
    RectTransform rClickSubMenuBox;

    private Vector2 upLeftPos;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            // Get menu up left position
            upLeftPos = Input.mousePosition;

            // Set menu position
            rClickSubMenuBox.anchoredPosition = new Vector2(upLeftPos.x, upLeftPos.y);

            // Show the sub menu
            if (!rClickSubMenuBox.gameObject.activeInHierarchy)
            {
                rClickSubMenuBox.gameObject.SetActive(true);
            }
        }

        if (rClickSubMenuBox.gameObject.activeInHierarchy)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // Hide the sub menu
                rClickSubMenuBox.gameObject.SetActive(false);
            }
        }
    }
}
