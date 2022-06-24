using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    void Start() //automatic process with no user input
    {
        PhotonNetwork.ConnectUsingSettings(); //connects to servers, allowing user to later join a room
    }

    public override void OnConnectedToMaster() //activates when connected to server
    {
        PhotonNetwork.JoinLobby(); //joins the lobby, allowing users to now join rooms
    }

    public override void OnJoinedLobby() //activates when user joins the lobby
    {
        SceneManager.LoadScene("Lobby"); //loads lobby scene with room selection
    }
}
