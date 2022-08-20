using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SetUsername : MonoBehaviour
{
    public TMP_InputField field;

    public void Start()
    {
        field.text = PlayerPrefs.GetString("Username"); //loads username from PlayerPrefs
    }

    public void UpdateUsername(string name) //called when username input field is changed
    {
        PlayerPrefs.SetString("Username", name); //sets username to PlayerPrefs, saving it between sessions and allowing for easy access later
    }
}
