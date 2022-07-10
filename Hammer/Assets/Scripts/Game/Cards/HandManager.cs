using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    public float speed;
    public Vector3 targetUp;
    public Vector3 targetDown;
    public HandCard cardHover;
    public bool cardSelected;
    public int cardCount;

    public HandCard[] cardObjects;
    public string[] cards;
    public DeckManager dm;
    public SpriteRenderer cardGlowSR;
    public Transform cardGlowTR;

    public void Start()
    {
        dm = Object.FindObjectOfType<DeckManager>();
    }

    public void Update()
    {
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
        

        if (Input.GetKeyDown(KeyCode.Mouse0) && cardHover != null && cardHover.cardCurrent != "none")
        {

            cardSelected = !cardSelected;

        }
        if (Input.GetKeyDown("space"))
        {
            dm = Object.FindObjectOfType<DeckManager>();
            bool cont = false;
            for (int i = 0; i < 21 && !cont; i++)
            {
                if (cards[i] == "none")
                {
                    cards[i] = dm.drawCard();
                    cont = true;
                }
            }
        }
    }

    public int countHand()
    {
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
}
