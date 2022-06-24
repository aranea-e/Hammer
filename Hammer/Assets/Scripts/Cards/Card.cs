using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Card : MonoBehaviourPunCallbacks, IPunObservable
{
    #region vars
    public string cardCurrent;
    public int cardNumber;
    public Vector2 cardLocation;
    public Sprite sprite;
    public Sprite[] sprites;
    #endregion

    #region references
    public DeckManager dm;
    public Spawn Spawn;
    public Transform trans;
    public SpriteRenderer sr;
    public PhotonView pv;
    #endregion

    void Start()
    {
        dm = Object.FindObjectOfType<DeckManager>();
        Spawn = Object.FindObjectOfType<Spawn>();
        bool cont = false;
        cardLocation.x = trans.position.x; //takes Vector2 cardLocation to determine which number card this is
        cardLocation.y = trans.position.y;
        for (int i = 0; !cont; i++)
        {
            if (Spawn.cardLocations[i] == cardLocation) //compares cardLocation to array of locations in Spawn
            {
                cardNumber = i; //sets cardNumber to whichever number in the array matched the location
                dm.cardObjects[i] = this; //sets reference in DeckManager
                cont = true;
            }
        }
        sprites = Resources.LoadAll<Sprite>("Sprites/Cards"); //loads spritesheet of cards for later use
    }

    #region functions
    public void drawCard() //draws a card from DeckManager and updates the sprite and current card accordingly
    {
        cardCurrent = dm.drawCard();
        updateSprite();
    }

    public void updateSprite()
    {
        if (cardCurrent == "none") //if no card was drawn, hides the sprite
        {
            sr.enabled = false;
        } else
        {
            for (int i = 0; i < sprites.Length; i++) //finds sprite that matches cardCurrent
            {
                if (cardCurrent == sprites[i].name)
                {
                    sprite = sprites[i];
                }
            }
            sr.sprite = sprite; //sets sprite
            sr.enabled = true; //enables the sprite in case it was previously none
        }
    }
    #endregion

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) //if client is owner of cards, sends current card
        {
            stream.SendNext(cardCurrent);
        } else //if client is not owner of cards, recieves cards from server and updates the sprite
        {
            cardCurrent = (string)stream.ReceiveNext();
            updateSprite();
        }
    }
}