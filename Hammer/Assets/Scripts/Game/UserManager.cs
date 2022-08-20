using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class UserManager : MonoBehaviourPunCallbacks, IPunObservable
{
    #region vars
    public int playerActorNumber;
    public int selfPlayerNumber;
    public int playerNumber;
    public int symbolNumber;
    public string username;
    public int cards;
    public Vector2 pos;
    public Vector2[] posList;
    public Sprite[] sprites;
    public Sprite sprite;
    public Photon.Realtime.Player[] playerList;
    public int[] playerNumberList;
    #endregion

    #region refs
    public PhotonView pv;
    public HandManager hm;
    public RectTransform rt;
    public RectTransform canvasRT;
    public TextMeshProUGUI cardsUI;
    public TextMeshProUGUI usernameUI;
    public Image symbolUI;
    public Image cardGlow;
    public Canvas canvas;
    #endregion

    void Start()
    {
        playerList = new Photon.Realtime.Player[25];
        if (pv.IsMine)
        {
            username = PlayerPrefs.GetString("Username");
            playerActorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        }
        hm = Object.FindObjectOfType<HandManager>();
        sprites = Resources.LoadAll<Sprite>("Sprites/PlayerMarkers");
    }

    void Update()
    {
        for (int i = 0; i < 25; i++)
        {
            if (i < PhotonNetwork.PlayerList.Length)
            {
                playerList[i] = PhotonNetwork.PlayerList[i];
            } else
            {
                playerList[i] = null;
            }
            if (playerList[i] == null)
            {
                playerNumberList[i] = -1;
            } else
            {
                playerNumberList[i] = playerList[i].ActorNumber;
            }
            if (playerNumberList[i] == playerActorNumber)
            {
                playerNumber = i;
            }
            if (playerNumberList[i] == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                selfPlayerNumber = i;
            }
        }
        symbolNumber = playerNumber;
        if (pv.IsMine)
        {
            cards = hm.countHand();
            canvas.enabled = false;
        } else
        {
            canvas.enabled = true;
            if (playerNumber > selfPlayerNumber)
            {
                pos = posList[playerNumber - 1];
            } else
            {
                pos = posList[playerNumber];
            }
            rt.anchoredPosition = pos;
            usernameUI.text = username;
            cardsUI.text = cards.ToString();
            for (int i = 0; i < sprites.Length; i++)
            {
                if ("PlayerMarkers_" + symbolNumber.ToString() == sprites[i].name)
                {
                    sprite = sprites[i];
                }
            }
            symbolUI.sprite = sprite;
        }
        
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) //if client is owner of UserManager, sends all data
        {
            stream.SendNext(playerActorNumber);
            stream.SendNext(username);
            stream.SendNext(cards);
        } else //if client is not owner of UserManager, recieves data
        {
            playerActorNumber = (int)stream.ReceiveNext();
            username = (string)stream.ReceiveNext();
            cards = (int)stream.ReceiveNext();
        }
    }
}
