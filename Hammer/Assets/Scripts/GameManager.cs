using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public bool IsPaused;
    public bool isUserPlaying;
    public bool IsPregame;
    public bool isUserNewJoin;
    public int[] playerStatusList;

    public int pauseReason;
    public bool isFirstPauseFrame;
    public float pauseDuration;

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
        if (PhotonNetwork.IsMasterClient)
        {
            IsPaused = true;
            IsPregame = true;
        }
        spawner.SpawnStart();
        UpdatePlayerList();
    }

    void Update()
    {
        #region pregame update
        if (IsPregame)
        { //pauses and constantly updates player list during pregame
            IsPaused = true;
            pauseReason = 2;
            UpdatePlayerList();
        }
        #endregion
        #region isplaying update
        UserManager[] UMS = Object.FindObjectsOfType<UserManager>();
        foreach (UserManager UM in UMS)
        {
            if (UM.playerNumber == selfPlayerNumber)
            {
                if (IsPregame)
                { //sets all users to playing during pregame
                    UM.IsPlaying = true;
                    isUserPlaying = true;
                    UM.newJoin = false;
                    isUserNewJoin = false;
                } else
                { //sets isUserPlaying to IsPlaying of own usermanager
                    isUserPlaying = UM.IsPlaying;
                    isUserNewJoin = UM.newJoin;
                }
            }
        }
        #endregion
        #region player status
        playerStatusList = new int[PhotonNetwork.PlayerList.Length];
        foreach (UserManager UM in UMS)
        {
            if (!UM.newJoin)
            {
                if (UM.IsPlaying)
                {
                    playerStatusList[UM.playerNumber] = 1;
                } else
                {
                    playerStatusList[UM.playerNumber] = 2;
                }
            }
        }
        #endregion
        #region pause
        if (IsPaused)
        {
            pauseDuration += Time.deltaTime;
            if (isFirstPauseFrame && pauseReason == 1)
            {
                dm.requestCardsFromHands();
                pv.RPC("checkCards", RpcTarget.All);
            }
            if (pauseReason == 1 && pauseDuration >= 3f && dm.isDone)
            {
                IsPaused = false;
                pv.RPC("drawCardsAfterPause", RpcTarget.All);
                for (int i = 0; i < 12; i++)
                {
                    if (tableCards[i].cardCurrent == "none")
                    {
                        tableCards[i].drawCard();
                    }
                }
            }
            isFirstPauseFrame = false;
        } else
        {
            isFirstPauseFrame = true;
            pauseReason = 0;
            pauseDuration = 0f;
        }
        #endregion
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
            }
        }

        for (int i = 0; i < playerNumberList.Length; i++)
        {
            if (playerNumberList[i] == PhotonNetwork.LocalPlayer.ActorNumber)
            { //sets number of client
                selfPlayerNumber = i;
            }
        }
        
        UserManager[] UMS = new UserManager[25];
        UMS = Object.FindObjectsOfType<UserManager>();
        foreach (UserManager UM in UMS)
        { //updates userManagers
            UM.updateUser();
        }
    }

    public void startGame()
    { //starts a new round
        UpdatePlayerList(); //updates player list
        IsPaused = false; //unpauses
        IsPregame = false;
        dm.drawStartingCards(); //draws nessecary cards
    }

    public void Lose()
    {
        UserManager[] UMS = Object.FindObjectsOfType<UserManager>();
        foreach (UserManager UM in UMS)
        {
            if (UM.playerNumber == selfPlayerNumber)
            {
                UM.IsPlaying = false;
            }
        }
        isUserPlaying = false;
    }

    [PunRPC]
    public void checkCards()
    {
        if (hm.countHand() > 14)
        {
            Lose();
        }
    }

    [PunRPC]
    public void drawCardsAfterPause()
    {
        for (int i = 0; i < hm.cardsToDraw; i++)
        {
            hm.drawCard();
        }
        hm.cardsToDraw = 0;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                stream.SendNext(IsPaused); //sends game pause state to clients
                stream.SendNext(IsPregame);
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
                IsPregame = (bool)stream.ReceiveNext();
                playerNumberList = new int[25];
                for (int i = 0; i < playerNumberList.Length; i++)
                {
                    playerNumberList[i] = (int)stream.ReceiveNext();
                }
            }
        }
    }
}
