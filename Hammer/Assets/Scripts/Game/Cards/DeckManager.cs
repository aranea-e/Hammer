using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class DeckManager : MonoBehaviourPunCallbacks, IPunObservable
{
    #region vars
    public string[] deck;
    public string[] deckFull;
    public int cards;
    private string temp;
    #endregion

    #region references
    public TableCard[] cardObjects;
    public PhotonView pv;
    public SpriteRenderer sr;
    public HandManager hm;
    public TextMeshProUGUI textmesh;
    public UserManager um;
    public GameManager Game;
    public Discard discard;
    #endregion

    void Awake()
    {
        hm = Object.FindObjectOfType<HandManager>();
        hm.dm = this;
        hm.dmPV = pv;
        Game = Object.FindObjectOfType<GameManager>();
        discard = Object.FindObjectOfType<Discard>();
        ResetDeck();
        Shuffle();
    }

    void Update()
    {
        if (Input.GetKeyDown("e") && pv.IsMine)
        {
            drawTable();
        }
        um = Object.FindObjectOfType<UserManager>();
    }

    #region functions
    [ContextMenu("shuffle")]
    public void Shuffle() //randomizes order of cards in deck
    {
        for (int i = 0; i < 54; i++) 
        {
            int rnd = Random.Range(i, deck.Length); //chooses random card from deck
            temp = deck[rnd]; //temporary variable temp stores values during change
            deck[rnd] = deck[i]; //swaps random card with current card
            deck[i] = temp; //sets current card to temp value
        }
    }

    [ContextMenu("reset")]
    public void ResetDeck() //resets the deck back to default
    {
        for (int i = 0; i < 54; i++)
        {
            deck[i] = deckFull[i]; //copies values from deckFull, a full sorted deck
        }
        updateDeckSprite(); //updates the deck to reflect change
    }

    [PunRPC]
    public void returnDrawnCard(int PlayerNumber, string Card)
    {
        if (um.selfPlayerNumber == PlayerNumber)
        {
            hm.addCardToHand(Card);
        }
    }

    [PunRPC]
    public void requestDrawCard(int PlayerNumber)
    {
        if (pv.IsMine)
        {
            pv.RPC("returnDrawnCard", RpcTarget.All, PlayerNumber, drawCard()); //draws a card and returns to client who requested
        }
    }

    [PunRPC]
    public void discardCard(string Card)
    {
        discard.discardCard(Card);
    }

    public string drawCard()
    {
        bool cont = false;
        temp = "none";
        for (int i = 0; !cont && i < 54; i++)
        {
            /*
            finds the topmost card != none
            if all cards == none, returns none
            */
            if (deck[i] != "none")
            {
                temp = deck[i];
                deck[i] = "none";
                cont = true;
            }
        }
        updateDeckSprite(); //update deck to reflect drawn card
        if (countDeck() == 0)
        {
            Game.IsPaused = true;
        }
        return temp;
    }

    public int countDeck()
    {
        cards = 0;
        for (int i = 0; i < deck.Length; i++)
        {
            //checks the number of cards in the deck != none
            if (deck[i] != "none")
            {
                cards++;
            }
        }
        return cards;
    }

    public void updateDeckSprite()
    {
        textmesh.text = countDeck().ToString(); //counts deck and updates number accordingly
        sr.enabled = countDeck() != 0; //if there are no cards in the deck, hides blank card sprite
    }

    [ContextMenu("fill table")]
    public void drawTable() //draws a full table of cards
    {
        for (int i = 0; i < 12; i++)
        {
            cardObjects[i].drawCard(); //calls drawCard() from each table card object
        }
    }
    #endregion

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) //if client is owner of deck, sends deck data
        {
            for (int i = 0; i < 54; i++)
            {
                stream.SendNext(deck[i]);
            }
        } else //if client is not owner of deck, looks for deck data and updates the deck sprite and number
        {
            for (int i = 0; i < 54; i++)
            {
                deck[i] = (string)stream.ReceiveNext();
                updateDeckSprite();
            }
        }
    }
}
