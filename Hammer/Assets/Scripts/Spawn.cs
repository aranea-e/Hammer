using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Spawn : MonoBehaviour
{
    public GameObject deckPrefab;
    public GameObject cardPrefab;
    public Vector2[] cardLocations;
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {    
            PhotonNetwork.Instantiate(deckPrefab.name, Vector2.zero, Quaternion.identity); //instantiates the deck object on the server
            for (int i = 0; i < 12; i++)
            {
                PhotonNetwork.Instantiate(cardPrefab.name, cardLocations[i], Quaternion.identity);
                /*
                Instantiates 12 cards, with locations found in cardLocations
                Locations will be used to determine which card is which
                Cards will add themselves to cardObjects in DeckManager
                */
            }
        }
    }
}
