using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Spawn : MonoBehaviour
{
    public GameObject prefab;
    void Start()
    {
                if (PhotonNetwork.IsMasterClient)
        {    
            PhotonNetwork.Instantiate(prefab.name, Vector2.zero, Quaternion.identity); //instantiates the deck object on the server
        }
    }
}
