using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Spawn : MonoBehaviour
{
    public GameObject deckPrefab;
    public GameObject cardPrefab;
    public GameObject userPrefab;
    public Vector2[] cardLocations;
    void Awake() //uses Awake to make sure references get assigned properly in Start functions
    {
        if (PhotonNetwork.IsMasterClient)
        {    
            PhotonNetwork.Instantiate(deckPrefab.name, Vector2.zero, Quaternion.identity); //instantiates the deck object on the server
            for (int i = 0; i < 12; i++)
            {
                PhotonNetwork.Instantiate(cardPrefab.name, cardLocations[i], Quaternion.identity);
                /*
                instantiates 12 cards, with locations found in cardLocations
                locations will be used to determine which card is which
                cards will add themselves to cardObjects in DeckManager
                */
            }
        }
        PhotonNetwork.Instantiate(userPrefab.name, Vector2.zero, Quaternion.identity); //creates the user data manager
    }
}
