using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuOpenClose : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameMenu.isOpen)
            {
                GameMenu.closeMenu();
            } else
            {
                GameMenu.openMenu();
            }
        }
    }
}
