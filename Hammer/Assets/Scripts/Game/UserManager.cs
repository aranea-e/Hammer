using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class UserManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public int playerNumber;
    public string username;
    public int cards;
    public PhotonView pv;
    public HandManager hm;

    void Start()
    {
        if (pv.IsMine)
        {
            playerNumber = PhotonNetwork.LocalPlayer.ActorNumber;
            username = PlayerPrefs.GetString("Username");
        }
        hm = Object.FindObjectOfType<HandManager>();
    }

    void Update()
    {
        if (pv.IsMine)
        {
            cards = hm.countHand();
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) //if client is owner of UserManager, sends all data
        {
            stream.SendNext(playerNumber);
            stream.SendNext(username);
            stream.SendNext(cards);
        } else //if client is not owner of UserManager, recieves data
        {
            playerNumber = (int)stream.ReceiveNext();
            username = (string)stream.ReceiveNext();
            cards = (int)stream.ReceiveNext();
        }
    }
}
