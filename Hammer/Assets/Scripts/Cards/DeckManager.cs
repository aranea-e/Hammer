using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DeckManager : MonoBehaviour
{
    public string[] deck;
    public string[] deckFull;
    private string temp;
    public GameObject prefab;
    public Vector2 deckLocation;

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {    
            PhotonNetwork.Instantiate(prefab.name, deckLocation, Quaternion.identity);
            ResetDeck(deck);
            Shuffle(deck);
        }
    }

    public void Shuffle(string[] deckToShuffle) 
    {
        for (int i = 0; i < 54; i++) 
        {
            int rnd = Random.Range(i, deckToShuffle.Length);
            temp = deckToShuffle[rnd];
            deckToShuffle[rnd] = deckToShuffle[i];
            deckToShuffle[i] = temp;
        }
    }

    public void ResetDeck(string[] deckToReset)
    {
        for (int i = 0; i < 54; i++)
        {
            deckToReset[i] = deckFull[i];
        }
    }
}
