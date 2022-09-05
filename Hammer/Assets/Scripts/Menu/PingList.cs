using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingList : MonoBehaviour
{
    public string[] servers;
    public int[] pings;

    void Awake()
    {
        //prevents list from being destroyed, allowing ping data to be accessed at any time
        DontDestroyOnLoad(this.gameObject);
    }
}
