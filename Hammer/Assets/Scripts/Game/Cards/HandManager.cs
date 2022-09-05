using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HandManager : MonoBehaviour
{
    public float speed;
    public Vector3 targetUp;
    public Vector3 targetDown;
    public HandCard cardHover;
    public bool cardSelected;
    public bool jSelected;
    public int jCard;
    public int cardCount;
    public int cardsToDiscard;

    public HandCard[] cardObjects;
    public string[] cards;
    public DeckManager dm;
    public SpriteRenderer cardGlowSR;
    public Transform cardGlowTR;
    public PhotonView dmPV;
    public UserManager um;
    public UserManager[] ums;
    public TableCard[] tcs;
    public Trash trash;
    public GameManager Game;

    public void HMStart() //called by DeckManager to make sure references are set properly
    {
        um = Object.FindObjectOfType<UserManager>(); //updates UserManager to use for drawCard()
        um.Start();
        um.Update();
        for (int i = 0; i < 21; i++)
        {
            cardObjects[i].cardNumber = i; //sets references for card objects
        }
        for (int i = 0; i < 7; i++)
        {
            drawCard(); //draws 7 cards
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && cardHover != null && cardHover.cardCurrent != "none")
        { //if the user clicks while a card is selected
        #region card deselect
            bool handCardHover = false;
            for (int i = 0; i < 21; i++)
            {
                if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x <= cardObjects[i].trans.position.x + 1.0f/6.0f && Camera.main.ScreenToWorldPoint(Input.mousePosition).x >= cardObjects[i].trans.position.x - .5 && Camera.main.ScreenToWorldPoint(Input.mousePosition).y <= -3)
                { //checks if card i is being hovered over
                    if (cardObjects[i].cardCurrent != "none")
                    { //checks if card i is being displayed
                        handCardHover = true;
                    }
                }
            }
            if (handCardHover)
            { //if user clicked on a card in their hand, deselects that card
                cardSelected = !cardSelected;
                jSelected = false;
            }
        #endregion
        
        #region discard
            if (!Game.IsPaused && Camera.main.ScreenToWorldPoint(Input.mousePosition).x <= trash.trans.position.x + 0.5f && Camera.main.ScreenToWorldPoint(Input.mousePosition).x >= trash.trans.position.x - 0.5f && Camera.main.ScreenToWorldPoint(Input.mousePosition).y <= trash.trans.position.y + 0.5f && Camera.main.ScreenToWorldPoint(Input.mousePosition).y >= trash.trans.position.y - 0.5f)
            { //checks if user clicked on trash while game is unpaused
                if (cardsToDiscard > 0)
                {
                    cardsToDiscard--; //subtracts 1 from cardsToDiscard 
                    dmPV.RPC("discardCard", RpcTarget.All, cardHover.cardCurrent);
                    removeCard(cardHover.cardNumber); //discards card
                }
            }
        #endregion
        }

        #region card selection
        if (cardSelected == false) // detects if a card has already been selected
        {
            cardGlowSR.enabled = false; //hides the glow around selected cards
            cardHover = null;
            for (int i = 0; i < 21; i++)
            {
                if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x <= cardObjects[i].trans.position.x + 1.0f/6.0f && Camera.main.ScreenToWorldPoint(Input.mousePosition).x >= cardObjects[i].trans.position.x - .5 && Camera.main.ScreenToWorldPoint(Input.mousePosition).y <= -3)
                { //determines if cursor is hovering over each card, sends the card up if it's being hovered over
                    var step = speed * Time.deltaTime;
                    targetUp.x = cardObjects[i].trans.position.x;
                    targetUp.z = cardObjects[i].trans.position.z;
                    cardObjects[i].trans.position = Vector3.MoveTowards(cardObjects[i].trans.position, targetUp, step); //moves card up
                    cardHover = cardObjects[i];
                } else
                { //sends cardObjects not hovered over down
                    var step = speed * Time.deltaTime;
                    targetDown.x = cardObjects[i].trans.position.x;
                    targetDown.z = cardObjects[i].trans.position.z;
                    cardObjects[i].trans.position = Vector3.MoveTowards(cardObjects[i].trans.position, targetDown, step); // moves card down
                }
                cardObjects[i].cardCurrent = cards[i];
                cardObjects[i].updateSprite(); //updates sprite of card
            }
        } else  
        {
            for (int i = 0; i < 21; i++)
            {
                if (cardHover == cardObjects[i])
                { //sends card up if selected
                    var step = speed * Time.deltaTime;
                    targetUp.x = cardObjects[i].trans.position.x;
                    targetUp.z = cardObjects[i].trans.position.z;
                    cardObjects[i].trans.position = Vector3.MoveTowards(cardObjects[i].trans.position, targetUp, step); //moves card up
                    cardGlowTR.position = cardObjects[i].trans.position; //moves glow to selected card
                    cardGlowSR.enabled = true; //enables glowing card sprite
                } else
                { //sends cardObjects not selected down
                    var step = speed * Time.deltaTime;
                    targetDown.x = cardObjects[i].trans.position.x;
                    targetDown.z = cardObjects[i].trans.position.z;
                    cardObjects[i].trans.position = Vector3.MoveTowards(cardObjects[i].trans.position, targetDown, step); // moves card down
                }
                cardObjects[i].cardCurrent = cards[i];
                cardObjects[i].updateSprite(); //updates sprite of card
            }
        }
        #endregion

        #region joker playing
        ums = FindObjectsOfType<UserManager>();
        if (jSelected)
        {
            foreach (UserManager UM in ums)
            { //goes through each Usermanager
                if (!UM.pv.IsMine)
                {
                    UM.cardGlow.enabled = true; //turns on the glow while a joker is selected
                    Vector2 POS;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(UM.canvasRT, Input.mousePosition, null, out POS);
                    if (Input.GetKeyDown(KeyCode.Mouse0) && POS.x > UM.rt.anchoredPosition.x - 24 && POS.x < UM.rt.anchoredPosition.x + 24 && POS.y > UM.rt.anchoredPosition.y - 16 && POS.y < UM.rt.anchoredPosition.y + 16)
                    { //checks if user clicked on a UserManager
                        userClicked(UM.playerNumber); //if so, calls userClicked to play joker
                    }
                }
            }
        } else
        { //disables glow on each UserManager
            foreach (UserManager UM in ums)
            {
                if (!UM.pv.IsMine && !Game.IsPaused)
                {
                    UM.cardGlow.enabled = false;
                }
            }
        }
        #endregion
    }

    public void userClicked(int Player)
    { //plays jokers after user clicks on another user
        if (jSelected)
        {
            tcs = FindObjectsOfType<TableCard>();
            foreach (TableCard TC in tcs)
            {
                TC.playJoker(Player, jCard);
            }
        }
    }

    public void drawCard()
    {
        if (countHand() < 21)
        { //if space is available in hand
            um = Object.FindObjectOfType<UserManager>();
            dmPV.RPC("requestDrawCard", RpcTarget.All, Game.selfPlayerNumber); //requests a card from the DM to add to hand
        }
    }

    public void addCardToHand(string Card)
    { //adds Card to first available spot in hand
        bool cont = false;
        for (int i = 0; i < 21 && !cont; i++)
            {
                if (cards[i] == "none")
                {
                    cards[i] = Card;
                    cont = true;
                }
            }
    }

    public int countHand()
    { //checks number of cards in hand which != "none"
        cardCount = 0;
        for (int i = 0; i < 21; i++)
        {
            if (cards[i] != "none")
            {
                cardCount++;
            }
        }
        return cardCount;
    }

    public void removeCard(int Card)
    {
        if (cardSelected && cardHover == cardObjects[Card])
        {
            cardSelected = !cardSelected;
        }
        if (countHand() == 1)
        {
            cards[0] = "none"; //if only one card is in hand, sets it to none

        } else if (countHand() == 2)
        {
            if (Card > 0)
            {
                cards[Card] = "none"; //if the discarded card is 1 or 2, sets it to none
            } else
            {
                if (cards[1] != "none") //if the discarded card is 0, sets the other card to none after replacing 0
                {
                    cards[0] = cards[1];
                    cards[1] = "none";
                } else
                {
                    cards[0] = cards[2];
                    cards[2] = "none";
                }
            }
        } else if (Card == 0)
        {
            if (countHand() % 2 == 0)  //if card to remove is 0, determines which side to remove from
            {
                //removes from left, brings cards right
                for (int i = 0; i < 21; i += 2)
                {
                    if (i + 2 < cards.Length) //checks to make sure the next card is in the array
                    {
                        cards[i] = cards[i + 2]; //sets card to next card out
                    } else
                    {
                        cards[i] = "none"; //sets card to none if the value would be out of the array
                    }
                }
            } else
            {
                //removes from right, brings cards left
                cards[0] = cards[1];
                for (int i = 1; i < 21; i += 2)
                {
                    if (i + 2 < cards.Length) //checks to make sure the next card is in the array
                    {
                        cards[i] = cards[i + 2]; //sets card to next card out
                    } else
                    {
                        cards[i] = "none"; //sets card to none if the value would be out of the array
                    }
                }
            }
        } else
        {
            if (Card % 2 == 0)
            {
                //card is left of 0
                if (countHand() % 2 == 0)
                {
                    //removes from left, brings cards right
                    for (int i = Card; i < 21; i += 2)
                    {
                        if (i + 2 < cards.Length) //checks to make sure the next card is in the array
                        {
                            cards[i] = cards[i + 2]; //sets card to next card out
                        } else
                        {
                            cards[i] = "none"; //sets card to none if the value would be out of the array
                        }
                    }
                } else
                {
                    //removes from right, brings cards left
                    for (int i = Card; i != 0; i -= 2) //goes as normal until it hits 0
                    {
                        cards[i] = cards[i - 2];
                    }
                    cards[0] = cards[1]; //sets 0 to 1
                    for (int i = 1; i < 21; i += 2) //continues as normal from 1
                    {
                        if (i + 2 < cards.Length) //checks to make sure the next card is in the array
                        {
                            cards[i] = cards[i + 2]; //sets card to next card out
                        } else
                        {
                            cards[i] = "none"; //sets card to none if the value would be out of the array
                        }
                    }
                }
            } else
            {
                //card is right of 0
                if (countHand() % 2 == 0)
                {
                    //removes from left, brings cards right
                    for (int i = Card; i != 1; i -= 2) //goes as normal until it hits 1
                    {
                        cards[i] = cards[i - 2];
                    }
                    cards[1] = cards[0]; //sets 1 to 0
                    for (int i = 0; i < 21; i += 2) //continues as normal from 2
                    {
                        if (i + 2 < cards.Length) //checks to make sure the next card is in the array
                        {
                            cards[i] = cards[i + 2]; //sets card to next card out
                        } else
                        {
                            cards[i] = "none"; //sets card to none if the value would be out of the array
                        }
                    }
                } else
                {
                    //removes from right, brings cards left
                    for (int i = Card; i < 21; i += 2)
                    {
                        if (i + 2 < cards.Length) //checks to make sure the next card is in the array
                        {
                            cards[i] = cards[i + 2]; //sets card to next card out
                        } else
                        {
                            cards[i] = "none"; //sets card to none if the value would be out of the array
                        }
                    }
                }
            }
        }
    }
}
