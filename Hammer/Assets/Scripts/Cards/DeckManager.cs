using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DeckManager : MonoBehaviour
{
    #region vars
    public string[] deck;
    public string[] deckFull;
    private string temp;
    public PhotonView pv;
    #endregion

    void Start()
    {
        pv = GetComponent<PhotonView>();
        ResetDeck(deck);
        Shuffle(deck);
        pv.RPC("syncDeck", RpcTarget.All, deck);
    }

    void Update()
    {
        if (Input.GetKeyDown("space") && PhotonNetwork.IsMasterClient)
        {
            Shuffle(deck);
            pv.RPC("syncDeck", RpcTarget.All, deck);

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

    [PunRPC]
    void syncDeck(string[] deckToSync)
    {
        deck = deckToSync;
    }
    #endregion
}
