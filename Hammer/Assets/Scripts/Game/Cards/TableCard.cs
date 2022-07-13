using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TableCard : MonoBehaviourPunCallbacks, IPunObservable
{
    #region vars
    public string cardCurrent;
    public string[] cardStack;
    public int cardNumber;
    public Vector2 cardLocation;
    public Sprite sprite;
    public Sprite[] sprites;
    public Sprite playerMarkerSprite;
    public Sprite[] playerMarkerSprites;
    public bool isPlayable;
    public int lastPlayer;
    #endregion

    #region references
    public DeckManager dm;
    public HandManager hm;
    public Spawn Spawn;
    public UserManager um;
    public Transform trans;
    public SpriteRenderer sr;
    public PhotonView pv;
    public SpriteRenderer cardGlow;
    public SpriteRenderer playerMarker;
    #endregion

    void Start()
    {
        dm = Object.FindObjectOfType<DeckManager>(); //sets references
        hm = Object.FindObjectOfType<HandManager>();
        Spawn = Object.FindObjectOfType<Spawn>();
        bool cont = false;
        cardLocation.x = trans.position.x; //takes Vector2 cardLocation to determine which number card this is
        cardLocation.y = trans.position.y;
        trans.Translate(Vector3.forward); //moves card to Z = 1 in order to appear behind hand
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
        playerMarkerSprites = Resources.LoadAll<Sprite>("Sprites/PlayerMarkers");
    }

    public void Update()
    {
        um = Object.FindObjectOfType<UserManager>(); //finds any user manager to get own player number
        if (cardCurrent != "none" && hm.cardHover != null && um.selfPlayerNumber != lastPlayer && (cardCurrent.Substring(0, 1) == "J" || cardCurrent.Remove (0, 1) == "1" || hm.cardHover.cardCurrent.Substring(0, 1) == "J" || hm.cardHover.cardCurrent.Remove (0, 1) == "1" || cardCurrent.Remove(0, 1) == hm.cardHover.cardCurrent.Remove(0, 1)) && !((cardCurrent.Substring(0, 1) == "J" || cardCurrent.Remove (0, 1) == "1") && lastPlayer == -1))
        { //determines if this is a Joker or Ace, or if selected card is a Joker, Ace, or same value as cardCurrent
            isPlayable = true; //allows card to be played on this card
        } else
        {
            isPlayable = false; //disallows card to be played on this card
        }
        cardGlow.enabled = isPlayable; //glows if a card can be played, does not glow if a card cannot be played
        if (isPlayable && Input.GetKeyDown(KeyCode.Mouse0) && isMouseOnCard())
        { //checks if a card can be played and the player clicked on this card
            pv.RPC("playCard", RpcTarget.All, hm.cardHover.cardCurrent, um.selfPlayerNumber); //calls playCard rpc
            hm.removeCard(hm.cardHover);
            hm.cardSelected = false;
        }
        updateSprite();
    }

    #region functions

    [PunRPC]
    public void playCard(string Card, int Number)
    {
        cardCurrent = Card; //sets card
        lastPlayer = Number; //sets last player
    }
    public void drawCard() //draws a card from DeckManager and updates the sprite and current card accordingly
    {
        cardCurrent = dm.drawCard();
        lastPlayer = -1;
        updateSprite();
    }

    public bool isMouseOnCard()
    {
        return (Camera.main.ScreenToWorldPoint(Input.mousePosition).x >= trans.position.x - .5 && Camera.main.ScreenToWorldPoint(Input.mousePosition).x <= trans.position.x + .5 && Camera.main.ScreenToWorldPoint(Input.mousePosition).y >= trans.position.y - .75 && Camera.main.ScreenToWorldPoint(Input.mousePosition).y <= trans.position.y + .75);
        //returns true if mouse is on this card
    }

    public void updateSprite()
    {
        if (cardCurrent == "none") //if no card was drawn, hides the sprite
        {
            sr.enabled = false;
            playerMarker.enabled = false;
        } else
        {
            for (int i = 0; i < sprites.Length; i++) //finds sprite that matches cardCurrent
            {
                if (cardCurrent == sprites[i].name)
                {
                    sprite = sprites[i];
                }
            }
            if (lastPlayer != -1) //checks if a player played this card
            {
                for (int i = 0; i < playerMarkerSprites.Length; i++) //finds the correct marker based on player who played card
                {
                    if ("PlayerMarkers_" + lastPlayer.ToString() == playerMarkerSprites[i].name)
                    {
                        playerMarkerSprite = playerMarkerSprites[i];
                    }
                }
                playerMarker.enabled = true; //enables player marker
            } else
            {
                playerMarker.enabled = false; //disables player marker
            }
            sr.sprite = sprite; //sets sprite
            playerMarker.sprite = playerMarkerSprite;
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