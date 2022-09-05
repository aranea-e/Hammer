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

    void Start()
    {
        hm = Object.FindObjectOfType<HandManager>(); //sets references
        hm.dm = this;
        hm.dmPV = pv;
        Game = Object.FindObjectOfType<GameManager>();
        discard = Object.FindObjectOfType<Discard>();
        um = Object.FindObjectOfType<UserManager>();
        Game.dm = this;
        Game.tableCards = cardObjects;
        if (pv.IsMine)
        {
            ResetDeck(); //resets and shuffles deck
            Shuffle();
        }
        hm.HMStart(); //calls HandManager start function
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

    #region draw card

    [PunRPC]
    public void returnDrawnCard(int PlayerNumber, string Card)
    { //returns card from requestDrawCard to player who requested card
        um = Object.FindObjectOfType<UserManager>();
        if (Game.selfPlayerNumber == PlayerNumber)
        { //checks if this is the player who requested card
            hm.addCardToHand(Card); //gives card
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

    public string drawCard()
    {
        bool cont = false;
        temp = "none";
        for (int i = 0; !cont && i < 54; i++)
        {
            /*
            finds the topmost card which != none
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
            Game.IsPaused = true; //pauses if deck runs out of cards
        }
        return temp;
    }

    #endregion

    [PunRPC]
    public void discardCard(string Card)
    { //discards card
        discard.discardCard(Card);
    }
    
    public int countDeck()
    { //counts remaining cards
        cards = 0;
        for (int i = 0; i < deck.Length; i++)
        {
            //checks the number of cards in the deck which != none
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
