using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LeaveReason : MonoBehaviour
{
    public Color color;
    float delay;
    public TextMeshProUGUI tmp;

    void Start()
    {
        color.a = GameMenu.leaveReason;
        delay = 5.0f;
    }

    void Update()
    {
        if (delay > 0)
        {
            delay -= Time.deltaTime;
        }
        else
        {
            color.a = (color.a - Time.deltaTime / 3) * GameMenu.leaveReason;
        }
        tmp.color = color;
    }
}
