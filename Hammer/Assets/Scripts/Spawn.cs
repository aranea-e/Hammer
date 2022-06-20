using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Spawn : MonoBehaviour
{
    public GameObject prefab;

    private void Start()
    {
        PhotonNetwork.Instantiate(prefab.name, Vector2.zero, Quaternion.identity);
    }
}
