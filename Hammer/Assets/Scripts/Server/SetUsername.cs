using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SetUsername : MonoBehaviour
{
    public TMP_InputField field;

    public void Start()
    {
        field.text = PlayerPrefs.GetString("Username");
    }

    public void UpdateUsername(string name)
    {
        PlayerPrefs.SetString("Username", name);
    }
}
