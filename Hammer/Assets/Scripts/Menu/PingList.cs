using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingList : MonoBehaviour
{
    public string[] servers;
    public int[] pings;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
