using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class DetectAndLeaveRoom : MonoBehaviourPunCallbacks
{
    float buffer = 1.0f;

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
        GameMenu.isOpen = false;
        PhotonNetwork.Disconnect();
        GameMenu.leavingRoom = false;
    }

    void Update()
    {
        if (buffer > 0)
        {
            buffer -= Time.deltaTime;
        }
        UserManager[] ums = FindObjectsOfType<UserManager>();
        bool foundMasterClient = false;
        foreach (UserManager um in ums)
        {
            if (um.isHost)
            {
                foundMasterClient = true;
            }
        }
        if (!foundMasterClient && buffer < 0)
        {
            GameMenu.exit(1);
        }
    }
}
