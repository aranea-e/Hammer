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
    public Card[] cardObjects;
    public PhotonView pv;
    public SpriteRenderer sr;
    public TextMeshProUGUI textmesh;
    #endregion

    void Start()
    {
        pv = GetComponent<PhotonView>();
        ResetDeck();
        Shuffle();
    }

    void Update()
    {

    }

    #region functions
    [ContextMenu("shuffle")]
    public void Shuffle() //randomizes order of cards in deck
    {
        for (int i = 0; i < 54; i++) 
        {
            int rnd = Random.Range(i, deck.Length);
            temp = deck[rnd]; //temporary variable temp stores values during change
            deck[rnd] = deck[i];
            deck[i] = temp;
        }
    }

    [ContextMenu("reset")]
    public void ResetDeck() //resets the deck back to default
    {
        for (int i = 0; i < 54; i++)
        {
            deck[i] = deckFull[i]; //copies values from deckFull, a full sorted deck
        }
    }

    public string drawCard()
    {
        bool cont = false;
        temp = "none";
        for (int i = 0; !cont && i < 54; i++)
        {
            if (deck[i] != "none")
            {
                temp = deck[i];
                deck[i] = "none";
                cont = true;
            }
        }
        updateDeckSprite();
        return temp;
    }

    public int countDeck()
    {
        cards = 0;
        for (int i = 0; i < deck.Length; i++)
        {
            if (deck[i] != "none")
            {
                cards++;
            }
        }
        return cards;
    }

    public void updateDeckSprite()
    {
        textmesh.text = countDeck().ToString();
        sr.enabled = countDeck() != 0;
    }

    [ContextMenu("fill table")]
    public void drawTable()
    {
        for (int i = 0; i < 12; i++)
        {
            cardObjects[i].drawCard();
        }
    }
    #endregion

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            for (int i = 0; i < 54; i++)
            {
                stream.SendNext(deck[i]);
            }
        } else
        {
            for (int i = 0; i < 54; i++)
            {
                deck[i] = (string)stream.ReceiveNext();
            }
        }
    }
}
