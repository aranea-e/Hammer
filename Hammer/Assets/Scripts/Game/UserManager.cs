using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class UserManager : MonoBehaviourPunCallbacks, IPunObservable
{
    #region vars
    public int symbolNumber;
    public string username;
    public int cards;
    public Vector2 pos;
    public Vector2[] posList;
    public Sprite[] sprites;
    public Sprite sprite;
    public bool IsPlaying;
    public int playerActorNumber;
    public int playerNumber;
    public bool newJoin;
    public bool isHost;
    #endregion

    #region refs
    public PhotonView pv;
    public HandManager hm;
    public RectTransform rt;
    public RectTransform canvasRT;
    public TextMeshProUGUI cardsUI;
    public TextMeshProUGUI usernameUI;
    public Image cardUI;
    public Image symbolUI;
    public Image cardGlow;
    public Canvas canvas;
    public GameManager Game;
    public Sprite cardSprite;
    public Sprite graySprite;
    #endregion

    public void Start()
    {
        if (pv.IsMine)
        { //loads username and determines if client is host
            username = PlayerPrefs.GetString("Username");
            isHost = PhotonNetwork.IsMasterClient;
        }
        playerActorNumber = pv.OwnerActorNr;
        hm = Object.FindObjectOfType<HandManager>(); //sets references
        sprites = Resources.LoadAll<Sprite>("Sprites/PlayerMarkers");
        Game = Object.FindObjectOfType<GameManager>();
    }

    public void Update()
    {
        if (pv.IsMine)
        { //counts cards in hand and hides UserManager
            cards = hm.countHand();
            canvas.enabled = false;
        } else
        { //displays player info
            symbolNumber = playerNumber;
            canvas.enabled = true;
            usernameUI.text = username; //displays username
            usernameUI.enabled = true;
            cardUI.enabled = true;
            if (IsPlaying)
            {
                cardsUI.text = cards.ToString(); //displays number of cards
                cardsUI.enabled = true;
                for (int i = 0; i < sprites.Length; i++)
                {
                    if ("PlayerMarkers_" + symbolNumber.ToString() == sprites[i].name)
                    { //displays player marker sprite
                        sprite = sprites[i];
                    }
                }
                symbolUI.sprite = sprite;
                symbolUI.enabled = true;
                cardUI.sprite = cardSprite;
            } else 
            {
                cardsUI.enabled = false;
                symbolUI.enabled = false;
                cardUI.sprite = graySprite;
            }
            if (newJoin)
            {
                cardsUI.enabled = false;
                symbolUI.enabled = false;
                cardUI.enabled = false;
                usernameUI.enabled = false;
            } else
            {
                if (playerNumber > Game.selfPlayerNumber && !Game.isUserNewJoin)
                { //moves to appropriate location
                    pos = posList[playerNumber - 1];
                } else
                {
                    pos = posList[playerNumber];
                }
                rt.anchoredPosition = pos;
            }
        }
    }

    public void updateUser()
    {
        Game = Object.FindObjectOfType<GameManager>();
        for (int i = 0; i < Game.playerNumberList.Length; i++)
        {
            if (playerActorNumber == Game.playerNumberList[i])
            {
                playerNumber = i;
            }
        }
        symbolNumber = playerNumber;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) //if client is owner of UserManager, sends all data
        {
            stream.SendNext(playerNumber);
            stream.SendNext(username);
            stream.SendNext(cards);
            stream.SendNext(IsPlaying);
            stream.SendNext(newJoin);
            stream.SendNext(isHost);
        } else //if client is not owner of UserManager, recieves data
        {
            playerNumber = (int)stream.ReceiveNext();
            username = (string)stream.ReceiveNext();
            cards = (int)stream.ReceiveNext();
            IsPlaying = (bool)stream.ReceiveNext();
            newJoin = (bool)stream.ReceiveNext();
            isHost = (bool)stream.ReceiveNext();
        }
    }
}
