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
    public int remainingPlayers;
    public int lastPlayer;

    public int pauseReason;
    public bool isFirstPauseFrame;
    public float pauseDuration;
    public bool isFirstPregame;
    public bool isWinscreen;
    public string winner;
    public int quakeTrigger;

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
            
            for (int i = 0; i < hm.cards.Length; i++)
            {
                hm.cards[i] = "none";
            }
            hm.cardSelected = false; //resents HandManager variables
            hm.jSelected = false;
            hm.cardsToDiscard = 0;
            hm.cardsToDraw = 0;
            hm.quakesInGame = 0;
            for (int i = 0; i < tableCards.Length; i++)
            {
                tableCards[i].resetCard(); //resets all table cards
            }
            dm.discardCard("none"); //resets discards
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
        { //keeps track of if players are playing or not
            if (!UM.newJoin)
            {
                if (UM.IsPlaying)
                {
                    playerStatusList[UM.playerNumber] = 1; //sets number to 1 if playing
                } else
                {
                    playerStatusList[UM.playerNumber] = 2; //sets number to 2 if player lost
                }
            }
        }
        remainingPlayers = 0;
        for (int i = 0; i < playerStatusList.Length; i++)
        {
            if (playerStatusList[i] == 1)
            {
                remainingPlayers++;
                lastPlayer = i;
            }
        }
        if (remainingPlayers == 0 && PhotonNetwork.IsMasterClient)
        {
            IsPregame = true;
            winner = "nobody";
        }
        if (remainingPlayers == 1 && PhotonNetwork.IsMasterClient)
        {
            IsPregame = true;
            foreach (UserManager UM in UMS)
            {
                if (UM.playerNumber == lastPlayer)
                {
                    winner = UM.username;
                }
            }
        }
        #endregion
        #region pause
        if (IsPaused)
        {
            pauseDuration += Time.deltaTime; //increases pauseDuration by time since last frame
            if (isFirstPauseFrame && pauseReason == 1)
            { //if this is the first frame after the deck ran out
                dm.requestCardsFromHands(); //resets deck
                pv.RPC("checkCards", RpcTarget.All); //checks if players lost
            }
            isWinscreen = (pauseDuration <= 3f && pauseReason == 2 && !isFirstPregame); //displays winscreen for first 3 seconds of new pregame
            if (pauseReason == 1 && pauseDuration >= 3f && dm.isDone)
            { //checks if it is time to unpause
                IsPaused = false;
                if (pv.IsMine)
                {
                    pv.RPC("drawCardsAfterPause", RpcTarget.All); //draws cards from during pause
                    for (int i = 0; i < 12; i++)
                    {
                        if (tableCards[i].cardCurrent == "none")
                        { //makes sure each table card has a card
                            tableCards[i].drawCard();
                        }
                    }
                }
            }
            if (pauseReason == 3 && pauseDuration >= 3f)
            { //if 3 seconds have passed from a quake
                IsPaused = false;
                if (pv.IsMine)
                {
                    pv.RPC("drawCardsFromQuake", RpcTarget.All, quakeTrigger); //forces all to draw cards
                }
            }
            isFirstPauseFrame = false;
        } else
        {
            isFirstPauseFrame = true;
            pauseReason = 0; //resets pause variables
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
        isFirstPregame = false;
    }

    public void Lose()
    {
        UserManager[] UMS = Object.FindObjectsOfType<UserManager>();
        foreach (UserManager UM in UMS)
        {
            if (UM.playerNumber == selfPlayerNumber)
            { //finds user's UserManager and removes them from play
                UM.IsPlaying = false;
            }
        }
        isUserPlaying = false;
    }

    [PunRPC]
    public void checkCards()
    {
        if (hm.countHand() > 14)
        { //loses if user has more than 15 cards
            Lose();
        }
    }

    [PunRPC]
    public void drawCardsAfterPause()
    {
        for (int i = 0; i < hm.cardsToDraw; i++)
        { //forces users to draw cards unable to be drawn due to pause
            hm.drawCard();
        }
        hm.cardsToDraw = 0; //resets cardsToDraw
    }

    [PunRPC]
    public void quake(int Number)
    {
        if (pv.IsMine)
        { //pauses and tells the game it was a quake
            IsPaused = true;
            pauseReason = 3;
            quakeTrigger = Number;
        }
    }

    [PunRPC]
    public void drawCardsFromQuake(int Number)
    {
        for (int i = 0; i < 3; i++)
        { //draw 3
            hm.drawCard();
        }
        if (selfPlayerNumber == Number)
        {
            for (int i = 0; i < hm.quakesInGame; i++)
            { //draw 1 more for each time user has triggered quake
                hm.drawCard();
            }
            hm.quakesInGame++; //increments counter for quakes
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                stream.SendNext(IsPaused); //sends game pause state to clients
                stream.SendNext(IsPregame);
                stream.SendNext(isFirstPregame);
                stream.SendNext(pauseReason);
                stream.SendNext(winner);
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
                isFirstPregame = (bool)stream.ReceiveNext();
                pauseReason = (int)stream.ReceiveNext();
                winner = (string)stream.ReceiveNext();
                playerNumberList = new int[25];
                for (int i = 0; i < playerNumberList.Length; i++)
                {
                    playerNumberList[i] = (int)stream.ReceiveNext();
                }
            }
        }
    }
}
