using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public bool IsPaused;

    public Photon.Realtime.Player[] playerList;
    public int[] playerNumberList;
    public int selfPlayerNumber;
    
    public Spawn spawner;
    public HandManager hm;
    public DeckManager dm;
    public TableCard[] tableCards;
    public HandCard[] handCards;
    public PhotonView pv;

    void Awake()
    {
        spawner.SpawnStart();
        UpdatePlayerList();
    }

    [ContextMenu("Update Player List")]
    public void UpdatePlayerList()
    {
        if (pv.IsMine)
        {
            playerList = new Photon.Realtime.Player[25];
            playerNumberList = new int[25];
            for (int i = 0; i < 25; i++)
            {
                if (i < PhotonNetwork.PlayerList.Length)
                { //sets list of players
                    playerList[i] = PhotonNetwork.PlayerList[i];
                } else
                {
                    playerList[i] = null;
                }
                if (playerList[i] == null)
                { //sets list of player numbers to be used for things such as symbols
                    playerNumberList[i] = -1;
                } else
                {
                    playerNumberList[i] = playerList[i].ActorNumber;
                }
                if (playerNumberList[i] == PhotonNetwork.LocalPlayer.ActorNumber)
                { //sets number of client
                    selfPlayerNumber = i;
                }
            }
        }
        
        UserManager[] UMS = new UserManager[25];
        UMS = Object.FindObjectsOfType<UserManager>();
        foreach (UserManager UM in UMS)
        {
            UM.updateUser();
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                stream.SendNext(IsPaused); //sends game pause state to clients
                for (int i = 0; i < playerNumberList.Length; i++)
                {
                    stream.SendNext(playerNumberList[i]);
                }
            }
        } else
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                IsPaused = (bool)stream.ReceiveNext(); //recieves game pause state
                playerNumberList = new int[25];
                for (int i = 0; i < playerNumberList.Length; i++)
                {
                    playerNumberList[i] = (int)stream.ReceiveNext();
                }
            }
        }
    }
}
