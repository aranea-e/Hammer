using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public Canvas buttons;

    public void toLobby()
    { //loads lobby
        SceneManager.LoadScene(1);
    }

    public void exit()
    { //leaves app
        Application.Quit();
    }

    public void loadInstructions()
    {
        /* 
        TODO: add tutorial
        Temporarily disables menu buttons
        */
        buttons.enabled = false;
    }
}
