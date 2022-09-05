using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TableCard : MonoBehaviourPunCallbacks, IPunObservable
{
    #region vars
    public string cardCurrent;
    public string[] cardStack;
    public List<string> hammerCards;
    public List<int> hammerPlayers;
    public int[] cardPlayers;
    public int cardNumber;
    public int secondCard;
    public int jTarget;
    public Vector2 cardLocation;
    public Sprite sprite;
    public Sprite hammerSprite;
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
    public SpriteRenderer targetMarker;
    public SpriteRenderer plus2;
    public PhotonView pv;
    public GameManager Game;
    public CardTimer ct;
    #endregion

    public void Start()
    {
        dm = Object.FindObjectOfType<DeckManager>(); //sets references
        hm = Object.FindObjectOfType<HandManager>();
        Game = Object.FindObjectOfType<GameManager>();
        Spawn = Object.FindObjectOfType<Spawn>();
        discard = Object.FindObjectOfType<Discard>();
        ct = Object.FindObjectOfType<CardTimer>();
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
        if (cardNumber == 11)
        {
            dm.drawTable(); //if this is the final card, tells the DM to draw a table full of cards
        }
    }

    public void Update()
    {
        um = Object.FindObjectOfType<UserManager>(); //finds any user manager to get own player number
        #region card glow
        if (hm.cardHover != null  && hm.cardHover.cardCurrent != "none" && !hm.jSelected)
        { //checks if a card is selected 
            if (lastPlayer != -1 && (cardCurrent.Substring(0, 1) == "J" || cardCurrent.Remove (0, 1) == "1"))
            { //checks whether the top card is a J/A
                cardGlow.enabled = false;
                JAcg.enabled = canPlayCard(hm.cardHover.cardCurrent, Game.selfPlayerNumber); //glows if a card can be played, does not glow if a card cannot be played
            } else
            {
                cardGlow.enabled = (!Game.IsPaused && (canPlayCard(hm.cardHover.cardCurrent, Game.selfPlayerNumber) || (countStack() > 3 && lastPlayer != Game.selfPlayerNumber && cardPlayers[countStack() - 2] != Game.selfPlayerNumber && !hammerPlayers.Contains(Game.selfPlayerNumber)))); //glows if a card can be played, does not glow if a card cannot be played
                JAcg.enabled = false;
            }
        } else
        {
            cardGlow.enabled = false;
            JAcg.enabled = false;
        }
        #endregion
        
        #region play or take cards
        if (hm.cardHover != null && canPlayCard(hm.cardHover.cardCurrent, Game.selfPlayerNumber) && Input.GetKeyDown(KeyCode.Mouse0) && isMouseOnCard())
        { //checks if a card can be played and the player clicked on this card
            if (hm.cardHover.cardCurrent.Substring(0, 1) == "J") //checks if a joker was played
            {
                hm.jSelected = true;
                hm.jCard = cardNumber;
            } else
            {
                pv.RPC("playCard", RpcTarget.All, hm.cardHover.cardCurrent, hm.cardHover.cardNumber, Game.selfPlayerNumber, -1); //calls playCard rpc
            }
        }
        if (!Game.IsPaused && cardStack[1] == "none" && (cardCurrent.Substring(0, 1) == "J" || cardCurrent.Remove (0, 1) == "1") && Input.GetKeyDown(KeyCode.Mouse0) && isMouseOnCard())
        { //checks if player clicked on a table J/A
            pv.RPC("takeJA", RpcTarget.All, Game.selfPlayerNumber);
        }
        if (countStack() > 3 && Input.GetKeyDown(KeyCode.Mouse0) && isMouseOnCard() && !Game.IsPaused)
        { //checks if player clicked on a hammer
            if (cardPlayers[countStack() - 2] == Game.selfPlayerNumber )
            { //checks if it was targeting the person who clicked
                pv.RPC("takeHammer", RpcTarget.All, Game.selfPlayerNumber);
            } else if (lastPlayer != Game.selfPlayerNumber && hm.cardSelected)
            { //checks if it was not played by this player and a card is selected
                pv.RPC("playOnHammer", RpcTarget.All, hm.cardHover.cardCurrent, hm.cardHover.cardNumber, Game.selfPlayerNumber);
            }
            
        }
        #endregion

        #region timer
        if (timerActive && !Game.IsPaused) //checks if this card has an active timer
        {
            timerTime += Time.deltaTime;
            if (timerTime >= 1f) //sees if 1 second has passed since the last time stage
            {
                timerTime -= 1f;
                advanceTimer(); //advances the timer
            }
        }
        if (timerCooldown <= 1f) //a cooldown to prevent clients from seeing cards drawn twice
        {
            timerCooldown += Time.deltaTime;
        }
        #endregion
        updateSprite();
    }

    #region functions

    public void playJoker(int Player, int CardNumber)
    {
        if (CardNumber == cardNumber)
        {
            um = Object.FindObjectOfType<UserManager>();
            pv.RPC("playCard", RpcTarget.All, hm.cardHover.cardCurrent, hm.cardHover.cardNumber, Game.selfPlayerNumber, Player);
        }
    }

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

    #region timer

    public void advanceTimer()
    {
        timerStage++; //advances timer 1 stage
        if (timerStage > 8)
        { //if timer expires
            timerStage = 0; //resets timer
            timerActive = false;
            onTimerExpire(); //calls method for timer expiration
        }
    }

    public void onTimerExpire()
    {
        updateSprite();
        if (symbolSR.sprite == symbolSprites[2] && pv.IsMine)
        { //checks if the card was a "hit"
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
            if (cardStack[1].Substring(0, 1) == "J")
            { //gives 5 penalty cards if the "hit" was on a "tap"
                pv.RPC("drawPenaltyCards", RpcTarget.All, cardPlayers[secondCard], 5);
            } else
            { //gives 3 penalty cards otherwise
                pv.RPC("drawPenaltyCards", RpcTarget.All, cardPlayers[secondCard], 3);
            }
        }
        if (symbolSR.sprite == symbolSprites[1] && pv.IsMine)
        { //gives penalty cards to joker target
            pv.RPC("drawPenaltyCards", RpcTarget.All, jTarget, 2);
        }
        if ((symbolSR.sprite == symbolSprites[0] || symbolSR.sprite == hammerSprite) && pv.IsMine)
        { //gives penalty cards to hammer target
            string[] Cards = new string[8 + hammerCards.Count];
            for (int i = 0; i < 8; i++)
            { //adds cards from stack to Cards
                Cards[i] = cardStack[i];
            }
            for (int i = 0; i < hammerCards.Count; i++)
            { //adds extra cards from hammer to Cards
                Cards[i + 8] = hammerCards[i];
            }
            pv.RPC("hammerExpire", RpcTarget.All, cardPlayers[countStack() - 2], Cards); //calls hammerExpire RPC to give penalty cards
        } else
        {
            discard.discardCard(cardCurrent); //discards card
        }
        if (timerCooldown > 1f)
        {
           drawCard(); //replaces card if not on cooldown to prevent clients from drawing too many
           timerCooldown = 0f;
        }
        updateSprite();
    }

    #endregion

    #region playing and taking cards

    public bool canPlayCard(string CardToPlay, int Number)
    {
        if (hammerCards.Count != 0 || Number == lastPlayer || cardCurrent == "none" || Game.IsPaused || (lastPlayer == -1 && (cardCurrent.Substring(0, 1) == "J" || cardCurrent.Remove (0, 1) == "1")))
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
    public void playCard(string Card, int CardObjectNumber, int Number, int JTarget)
    {
        if (pv.IsMine)
        {
            bool cardPlayedSuccessfully = false;
            if (canPlayCard(Card, Number))
            { //checks if user can play card
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
                updateCurrentCard(); //updates card
                jTarget = JTarget; //updates and resets relevant variables
                timerActive = true;
                timerTime = 0f;
                timerStage = 0;
                cardPlayedSuccessfully = true;
            } 
            pv.RPC("returnCardResult", RpcTarget.All, cardPlayedSuccessfully, CardObjectNumber, Number); //returns result
        }
    }

    [PunRPC]
    public void takeJA(int Number)
    {
        if (pv.IsMine)
        {
            string JANumber = "";
            bool cardTakenSuccessfully = false;
            if (cardStack[1] == "none" && (cardCurrent.Substring(0, 1) == "J" || cardCurrent.Remove (0, 1) == "1"))
            { //checks if card is a natural J/A
                drawCard(); //replaces card
                cardTakenSuccessfully = true;
                JANumber = cardCurrent;
            }
            pv.RPC("returnJAResult", RpcTarget.All, cardTakenSuccessfully, Number, JANumber); //returns result
        }
    }

    [PunRPC]
    public void takeHammer(int Number)
    {
        if (pv.IsMine)
        {
            bool cardTakenSuccessfully = false;
            string[] Cards = new string[8 + hammerCards.Count];
            if (countStack() > 3 && cardPlayers[countStack() - 2] == Number)
            {
                for (int i = 0; i < 8; i++)
                { //adds stack to Cards
                    Cards[i] = cardStack[i];
                }
                for (int i = 0; i < hammerCards.Count; i++)
                { //adds extra cards from hammer to Cards
                    Cards[i + 8] = hammerCards[i];
                }
                drawCard(); //draws a new card
                cardTakenSuccessfully = true;
            }
            pv.RPC("returnHammerResult", RpcTarget.All, cardTakenSuccessfully, Number, Cards); //returns result of taking hammer
        }
    }

    [PunRPC]
    public void playOnHammer(string Card, int CardObjectNumber, int Number)
    {
        if (pv.IsMine)
        { //checks if user could play on hammer
            bool cardPlayedSuccessfully = false;
            if (countStack() > 3 && Number != lastPlayer && Number != cardPlayers[countStack() - 2] && !hammerPlayers.Contains(Number))
            {
                hammerCards.Add(Card); //adds played card to list
                hammerPlayers.Add(Number);
                updateCurrentCard(); //updates hammer sprite if needed
                cardPlayedSuccessfully = true;
            } 
            pv.RPC("returnCardResult", RpcTarget.All, cardPlayedSuccessfully, CardObjectNumber, Number); //returns result
        }
    }

    #endregion

    #region results and penalties

    [PunRPC]
    public void drawPenaltyCards(int PlayerNumber, int CardNumber)
    {
        if (Game.selfPlayerNumber == PlayerNumber)
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
        if (Game.selfPlayerNumber == Number && Success)
        { //checks if this player successfully played a card
            hm.removeCard(CardObjectNumber); //removes card from hand
            hm.jSelected = false; //resets relevant variables
            ct.timerTime = 0f;
        }
    }

    [PunRPC]
    public void hammerExpire(int Number, string[] Cards)
    {
        if (Game.selfPlayerNumber == Number)
        { //checks if this was the target of the hammer
            for (int i = 0; i < Cards.Length; i++)
            {
                if (Cards[i] != "none")
                { //gives user cards from hammer
                    hm.addCardToHand(Cards[i]);
                }
            }
        }
    }

    [PunRPC]
    public void returnJAResult(bool Success, int Number, string JANumber)
    {
        if (Game.selfPlayerNumber == Number && Success)
        { //checks if this player took a J/A successfully
            hm.addCardToHand(JANumber); //adds cards to hand
            hm.cardsToDiscard++; //allows user to discard 1 card
        }
    }

    [PunRPC]
    public void returnHammerResult(bool Success, int Number, string[] Cards)
    { //returns if a hammer pickup was successful
        if (Game.selfPlayerNumber == Number && Success)
        { //checks if this player took the hammer successfully
            for (int i = 0; i < Cards.Length; i++)
            {
                if (Cards[i] != "none")
                { //adds cards to hand
                    hm.addCardToHand(Cards[i]);
                }
            }
        }
    }
    #endregion

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

    public void drawCard() //resets relevant variables and draws 1
    {
        for (int i = 0; i < 8; i++)
        {
            cardStack[i] = "none";
            cardPlayers[i] = -1;
        }
        jTarget = -1;
        cardStack[0] = dm.drawCard();
        hammerCards.Clear();
        timerActive = false;
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
            targetMarker.enabled = false;
            plus2.enabled = false;
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
            plus2.enabled = false;
            if (cardCurrent.Substring(0, 1) == "J" && countStack() > 1)
            { //checks if there is a "tap" and displays symbol accordingly
                symbolSR.enabled = true;
                symbolSR.sprite = symbolSprites[1];
                targetMarker.enabled = true;
                targetMarker.sprite = playerMarkerSprites[jTarget];
            } else
            {
                targetMarker.enabled = false;
            }
            if (countStack() < 3 && !(cardCurrent.Substring(0, 1) == "J" && countStack() > 1))
            { //disables symbol if it is not needed
                symbolSR.enabled = false;
            } else if (countStack() == 3)
            { //checks if there is a "hit" and displays symbol accordingly
                symbolSR.enabled = true;
                symbolSR.sprite = symbolSprites[2];
                targetMarker.enabled = true;
                targetMarker.sprite = playerMarkerSprites[cardPlayers[1]];
                if (cardStack[1].Substring(0, 1) == "J")
                { //displays a +2 if a "tap" was hit
                    plus2.enabled = true;
                }
            } else if (countStack() > 3)
            { //displays hammer
                symbolSR.enabled = true;
                if (hammerCards.Count == 0)
                { //if no cards were played, shows hollow sprite
                    symbolSR.sprite = symbolSprites[0];
                } else
                { //if cards were played, shows filled sprite
                    symbolSR.sprite = hammerSprite;
                }
                targetMarker.enabled = true;
                targetMarker.sprite = playerMarkerSprites[cardPlayers[countStack() - 2]];
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
        if (stream.IsWriting) //if client is owner of cards, sends data
        {
            for (int i = 0; i < 8; i++)
            {
                stream.SendNext(cardStack[i]);
                stream.SendNext(cardPlayers[i]);
            }
            stream.SendNext(hammerCards.Count);
            for (int i = 0; i < hammerCards.Count; i++)
            {
                stream.SendNext(hammerCards[i]);
                stream.SendNext(hammerPlayers[i]);
            }
            stream.SendNext(timerStage);
            stream.SendNext(timerActive);
            stream.SendNext(timerTime);
            stream.SendNext(jTarget);
        } else //if client is not owner of cards, recieves data
        {
            hammerCards.Clear();
            hammerPlayers.Clear();
            for (int i = 0; i < 8; i++)
            {
                cardStack[i] = (string)stream.ReceiveNext();
                cardPlayers[i] = (int)stream.ReceiveNext();
                
            }
            int HammerLength = (int)stream.ReceiveNext();
            for (int i = 0; i < HammerLength; i++)
            {
                hammerCards.Add((string)stream.ReceiveNext());
                hammerPlayers.Add((int)stream.ReceiveNext());
            }
            timerStage = (int)stream.ReceiveNext();
            timerActive = (bool)stream.ReceiveNext();
            timerTime = (float)stream.ReceiveNext();
            jTarget = (int)stream.ReceiveNext();
            updateSprite();
        }
    }
}