using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    #endregion

    void Start()
    {
        pv = GetComponent<PhotonView>();
        ResetDeck(deck);
        Shuffle(deck);
    }

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            if (PhotonNetwork.IsMasterClient)
            {
                for (int i = 0; i < 12; i++)
                {
                    cardObjects[i].drawCard();
                }
            } else
            {
                for (int i = 0; i < 12; i++)
                {
                    cardObjects[i].updateSprite(cardObjects[i].cardCurrent);
                }
            }
        }
    }

    #region functions
    public void Shuffle(string[] deckToShuffle) //randomizes order of cards in deckToShuffle
    {
        for (int i = 0; i < 54; i++) 
        {
            int rnd = Random.Range(i, deckToShuffle.Length);
            temp = deckToShuffle[rnd]; //temporary variable temp stores values during change
            deckToShuffle[rnd] = deckToShuffle[i];
            deckToShuffle[i] = temp;
        }
    }

    public void ResetDeck(string[] deckToReset) //resets the deck back to default
    {
        for (int i = 0; i < 54; i++)
        {
            deckToReset[i] = deckFull[i]; //copies values from deckFull, a full sorted deck
        }
    }

    public string drawCard(string[] deckToDraw)
    {
        bool cont = false;
        temp = "none";
        for (int i = 0; !cont && i < 54; i++)
        {
            if (deckToDraw[i] != "none")
            {
                temp = deckToDraw[i];
                deckToDraw[i] = "none";
                cont = true;
            }
        }
        return temp;
    }

    public int countDeck(string[] deckToCount)
    {
        cards = 0;
        for (int i = 0; i < deckToCount.Length; i++)
        {
            if (deckToCount[i] != "none")
            {
                cards++;
            }
        }
        return cards;
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
