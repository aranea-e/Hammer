using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TableCard : MonoBehaviourPunCallbacks, IPunObservable
{
    #region vars
    public string cardCurrent;
    public string[] cardStack;
    public int[] cardPlayers;
    public int cardNumber;
    public int secondCard;
    public Vector2 cardLocation;
    public Sprite sprite;
    public Sprite[] sprites;
    public Sprite[] playerMarkerSprites;
    public Sprite[] timerSprites;
    public Sprite[] symbolSprites;
    public bool isPlayable;
    public int lastPlayer;
    public bool timerActive;
    public float timerTime;
    public int timerStage;
    public float timerCooldown;
    #endregion

    #region references
    public DeckManager dm;
    public HandManager hm;
    public Spawn Spawn;
    public Discard discard;
    public UserManager um;
    public Transform trans;
    public SpriteRenderer sr;
    public SpriteRenderer JAsr;
    public SpriteRenderer playerMarker;
    public SpriteRenderer JApm;
    public SpriteRenderer cardGlow;
    public SpriteRenderer JAcg;
    public SpriteRenderer timer;
    public SpriteRenderer symbolSR;
    public PhotonView pv;
    public GameManager Game;
    #endregion

    void Start()
    {
        dm = Object.FindObjectOfType<DeckManager>(); //sets references
        hm = Object.FindObjectOfType<HandManager>();
        Game = Object.FindObjectOfType<GameManager>();
        Spawn = Object.FindObjectOfType<Spawn>();
        discard = Object.FindObjectOfType<Discard>();
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
        timerSprites = Resources.LoadAll<Sprite>("Sprites/Clock");
        symbolSprites = Resources.LoadAll<Sprite>("Sprites/Card Interactions");
        
    }

    public void Update()
    {
        um = Object.FindObjectOfType<UserManager>(); //finds any user manager to get own player number
        if (hm.cardHover != null)
        {
            if (lastPlayer != -1 && (cardCurrent.Substring(0, 1) == "J" || cardCurrent.Remove (0, 1) == "1"))
            { //checks whether the top card is a J/A
                cardGlow.enabled = false;
                JAcg.enabled = canPlayCard(hm.cardHover.cardCurrent, um.selfPlayerNumber); ////glows if a card can be played, does not glow if a card cannot be played
            } else
            {
                cardGlow.enabled = canPlayCard(hm.cardHover.cardCurrent, um.selfPlayerNumber); //glows if a card can be played, does not glow if a card cannot be played
                JAcg.enabled = false;
            }
        } else
        {
            cardGlow.enabled = false;
            JAcg.enabled = false;
        }
        
        if (hm.cardHover != null && canPlayCard(hm.cardHover.cardCurrent, um.selfPlayerNumber) && Input.GetKeyDown(KeyCode.Mouse0) && isMouseOnCard())
        { //checks if a card can be played and the player clicked on this card
            pv.RPC("playCard", RpcTarget.All, hm.cardHover.cardCurrent, hm.cardHover.cardNumber, um.selfPlayerNumber); //calls playCard rpc
        }
        if (timerActive && !Game.IsPaused) //checks if this card has an active timer
        {
            timerTime += Time.deltaTime;
            if (timerTime >= 1f) //sees if 1 second has passed since the last time stage
            {
                timerTime = timerTime % 1f;
                advanceTimer(); //advances the timer
            }
        }
        if (timerCooldown <= 1f) //a cooldown to prevent clients from seeing cards drawn twice
        {
            timerCooldown += Time.deltaTime;
        }
        updateSprite();
    }

    #region functions

    public int countStack()
    {
        int cardCount = 0;
        for (int i = 0; i < 8; i++)
        {
            if (cardStack[i] != "none")
            {
                cardCount++;
            }
        }
        return cardCount;
    }

    public void advanceTimer()
    {
        timerStage++;
        if (timerStage > 8)
        {
            timerStage = 0;
            timerActive = false;
            onTimerExpire();
        }
    }

    public void onTimerExpire()
    {
        if (symbolSR.sprite == symbolSprites[2] && pv.IsMine)
        {
            bool cont = false;
            for (int i = 0; i < 8 && !cont; i++)
            {
                if (cardStack[i] == "none") //finds the last card, sets the previous one to secondCard
                {
                    secondCard = i - 2;
                    cont = true;
                } else if (i == 7) //catch if the last card is card 7
                {
                    secondCard = 6;
                }
            }
            pv.RPC("drawPenaltyCards", RpcTarget.All, cardPlayers[secondCard], 3);
        }
        discard.discardCard(cardCurrent);
        if (timerCooldown > 1f)
        {
           drawCard(); 
           timerCooldown = 0f;
        }
        updateSprite();
    }

    public bool canPlayCard(string CardToPlay, int Number)
    {
        if (Number == lastPlayer || cardCurrent == "none" || Game.IsPaused || (lastPlayer == -1 && (cardCurrent.Substring(0, 1) == "J" || cardCurrent.Remove (0, 1) == "1")))
        {
            isPlayable = false;
        } else if (lastPlayer == -1 && (CardToPlay.Substring(0, 1) == "J" || CardToPlay.Remove (0, 1) == "1"))
        { //determines if selected card is a joker or ace and this card was not played by a player
            isPlayable = true; //allows card to be played on this card
        } else if (lastPlayer != -1 && (cardCurrent.Substring(0, 1) == "J" || cardCurrent.Remove (0, 1) == "1"))
        { //determines if this card is a joker or ace played by a player
            bool cont = false;
            for (int i = 0; i < 8 && !cont; i++)
            {
                if (cardStack[i] == "none") //finds the last card, sets the previous one to secondCard
                {
                    secondCard = i - 2;
                    cont = true;
                } else if (i == 7) //catch if the last card is card 7
                {
                    secondCard = 6;
                }
            }
            isPlayable = (cardStack[secondCard].Remove(0, 1) == CardToPlay.Remove(0, 1));
            //sets isPlayable to true if the second top card is equivalent to the selected card
        } else if (cardCurrent.Remove(0, 1) == CardToPlay.Remove(0, 1))
        { //determines if selected card is the same value as cardCurrent
            isPlayable = true; //allows card to be played on this card
        } else
        {
            isPlayable = false; //disallows card to be played on this card
        }
        return isPlayable;
    }

    [PunRPC]
    public void playCard(string Card, int CardObjectNumber, int Number)
    {
        if (pv.IsMine)
        {
            bool cardPlayedSuccessfully = false;
            if (canPlayCard(Card, Number))
            {
                bool cont = false;
                for (int i = 0; i < 8 && !cont; i++)
                {
                    if (cardStack[i] == "none") //adds played card to array of cards played at this location
                    {
                        cardStack[i] = Card;
                        cardPlayers[i] = Number;
                        cont = true;
                    }
                }
                updateCurrentCard();
                timerActive = true;
                timerTime = 0f;
                timerStage = 0;
                cardPlayedSuccessfully = true;
            } 
            pv.RPC("returnCardResult", RpcTarget.All, cardPlayedSuccessfully, CardObjectNumber, Number);
        }
    }

    [PunRPC]
    public void drawPenaltyCards(int PlayerNumber, int CardNumber)
    {
        if (um.selfPlayerNumber == PlayerNumber)
        {
            for (int i = 0; i < CardNumber; i++)
            {
                hm.drawCard();
            }
        }
    }

    [PunRPC]
    public void returnCardResult(bool Success, int CardObjectNumber, int Number)
    {
        if (um.selfPlayerNumber == Number && Success)
        {
            hm.removeCard(CardObjectNumber);
        }
    }

    public void updateCurrentCard()
    {
        bool cont = false;
        if (cardStack[0] != "none")
        {
            for (int i = 0; i < 8 && !cont; i++)
            {
                if (cardStack[i] == "none") //finds the last card, sets it to cardCurrent
                {
                    cardCurrent = cardStack[i - 1];
                    lastPlayer = cardPlayers[i - 1];
                    cont = true;
                } else if (i == 7) //catch if the last card is card 7
                {
                    cardCurrent = cardStack[i];
                    lastPlayer = cardPlayers[i];
                }
            }
        } else
        {
            cardCurrent = "none";
            lastPlayer = -1;
        }
        
    }

    public void drawCard() //resets cardStack array and draws 1
    {
        for (int i = 0; i < 8; i++)
        {
            cardStack[i] = "none";
            cardPlayers[i] = -1;
        }
        cardStack[0] = dm.drawCard();
    }

    public bool isMouseOnCard()
    {
        return (Camera.main.ScreenToWorldPoint(Input.mousePosition).x >= trans.position.x - .5 && Camera.main.ScreenToWorldPoint(Input.mousePosition).x <= trans.position.x + .5 && Camera.main.ScreenToWorldPoint(Input.mousePosition).y >= trans.position.y - .75 && Camera.main.ScreenToWorldPoint(Input.mousePosition).y <= trans.position.y + .75);
        //returns true if mouse is on this card
    }

    public void updateSprite()
    {
        updateCurrentCard();
        if (cardCurrent == "none") //if no card was drawn, hides the sprite
        {
            sr.enabled = false;
            playerMarker.enabled = false;
            JAsr.enabled = false;
            JApm.enabled = false;
            symbolSR.enabled = false;
        } else
        {
            if ((cardCurrent.Substring(0, 1) == "J" || cardCurrent.Remove (0, 1) == "1") && lastPlayer != -1) //if a card is a played joker or ace, find the next card and put the J/A on top
            {
                bool cont = false;
                for (int i = 0; i < 8 && !cont; i++)
                {
                    if (cardStack[i] == "none") //finds the last card, sets the previous one to secondCard
                    {
                        secondCard = i - 2;
                        cont = true;
                    } else if (i == 7) //catch if the last card is card 7
                    {
                        secondCard = 6;
                    }
                }
                for (int i = 0; i < sprites.Length; i++) //finds sprite that matches cardStack[secondCard]
                {
                    if (cardStack[secondCard] == sprites[i].name)
                    {
                        sprite = sprites[i];
                    }
                }
                if (cardPlayers[secondCard] != -1) //checks if a player played this card
                {
                    playerMarker.sprite = playerMarkerSprites[cardPlayers[secondCard]];
                    playerMarker.enabled = true; //enables player marker
                } else
                {
                    playerMarker.enabled = false; //disables player marker
                }
                sr.sprite = sprite; //sets sprite
                for (int i = 0; i < sprites.Length; i++) //runs above code again for the J/A
                {
                    if (cardCurrent == sprites[i].name)
                    {
                        sprite = sprites[i];
                    }
                }
                if (lastPlayer != -1)
                {
                    JApm.sprite = playerMarkerSprites[lastPlayer];
                    JApm.enabled = true; //enables J/A player marker
                } else
                {
                    JApm.enabled = false; //disables J/A player marker
                }
                JAsr.enabled = true;
                JAsr.sprite = sprite; //sets sprite
            } else //if a card is not a joker or ace, displays it and hides the second card
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
                    playerMarker.sprite = playerMarkerSprites[lastPlayer];
                    playerMarker.enabled = true; //enables player marker
                } else
                {
                    playerMarker.enabled = false; //disables player marker
                }
                sr.sprite = sprite; //sets sprite
                sr.enabled = true; //enables the sprite in case it was previously none
                JApm.enabled = false;
                JAsr.enabled = false;
            }
            if (cardCurrent.Substring(0, 1) == "J" && countStack() > 1)
            {
                symbolSR.enabled = true;
                symbolSR.sprite = symbolSprites[1];
            }
            if (countStack() < 3 && !(cardCurrent.Substring(0, 1) == "J" && countStack() > 1))
            {
                symbolSR.enabled = false;
            } else if (countStack() == 3)
            {
                symbolSR.enabled = true;
                symbolSR.sprite = symbolSprites[2];
            } else if (countStack() > 3)
            {
                symbolSR.enabled = true;
                symbolSR.sprite = symbolSprites[0];
            }
        }
        timer.enabled = timerActive;
        if (timerActive)
        {
            timer.sprite = timerSprites[timerStage];
        }
    }
    #endregion

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) //if client is owner of cards, sends current card
        {
            for (int i = 0; i < 8; i++)
            {
                stream.SendNext(cardStack[i]);
                stream.SendNext(cardPlayers[i]);
            }
            stream.SendNext(timerStage);
            stream.SendNext(timerActive);
            stream.SendNext(timerTime);
        } else //if client is not owner of cards, recieves cards from server and updates the sprite
        {
            for (int i = 0; i < 8; i++)
            {
                cardStack[i] = (string)stream.ReceiveNext();
                cardPlayers[i] = (int)stream.ReceiveNext();
            }
            timerStage = (int)stream.ReceiveNext();
            timerActive = (bool)stream.ReceiveNext();
            timerTime = (float)stream.ReceiveNext();
            updateSprite();
        }
    }
}