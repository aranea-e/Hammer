using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    public TMP_InputField createInput;
    public TMP_InputField joinInput;

    public void CreateRoom() //called when create room button is pressed
    {
        PhotonNetwork.CreateRoom(createInput.text); //creates and joins room from input field
    }

    public void JoinRoom() //called when join room button is pressed
    {
        PhotonNetwork.JoinRoom(joinInput.text); //joins room from input field
    }

    public override void OnJoinedRoom() //activates upon joining room, creating rooms automatically joins them
    {
        PhotonNetwork.LoadLevel("Game Table"); //move to table scene
    }
}
