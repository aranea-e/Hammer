using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour {
    [SerializeField] private CardSO[] hand;
    [SerializeField] private HandCard[] cardObjects;
    [SerializeField] private int selected = -1;
    [SerializeField] private bool selectLock;

    void Awake() {
        resetHand();
    }

    void Start() {
        randomizeHand();
    }

    void Update() {
        hand = balanceCardArray(sortCardArray(hand));
        updateHandSprites();
        selectCard();
        updateCardSelectionStatus();
    }

    void updateHandSprites() {
        for (int i = 0; i < hand.Length; i++) {
            cardObjects[i].setCard(hand[i]);
        }
    }

    #region get/set functions
    public void setCardAtIndex(CardSO c, int index) {
        hand[index] = c;
    }

    public CardSO getCardAtIndex(int index) {
        return hand[index];
    }

    public bool getLock() {
        return selectLock;
    }
    #endregion
    
    #region modify hand
    #region menu functions
    [ContextMenu("reset hand")]
    public void resetHand() {
        hand = new CardSO[21];
    }

    [ContextMenu("randomize hand")]
    void randomizeHand() {
        for (int i = 0; i < hand.Length; i++) {
            if (Random.Range(0, 5) == 0) {
                hand[i] = null;
            } else {
                hand[i] = Util.deck[Random.Range(0, 54)];
            }
        }
    }

    [ContextMenu("randomize full hand")]
    void randomizeFullHand() {
        for (int i = 0; i < hand.Length; i++) {
            hand[i] = Util.deck[Random.Range(0, 54)];
        }
    }
    #endregion

    #region sort functions
    /**
    *   Returns true if card a is of a higher number than b. Ties are broken by suit.
    *   Spades > Hearts > Diamonds > Clubs
    *   Red Joker > Black Joker
    *   Considers null to be of highest value
    *   Returns false as a failsafe
    */
    bool compareCards(CardSO a, CardSO b) {
        if (b == null) {
            return false;
        }
        if (a == null) {
            return true;
        }

        if (a.value > b.value) {
            return true;
        }
        if (a.value < b.value) {
            return false;
        }

        if (a.joker) {
            return a.id != "J1";
        }

        if (a.suit > b.suit) {
            return true;
        }
        return false;
    }

    /**
    *   Sorts input array by rules in compareCards
    */
    CardSO[] sortCardArray(CardSO[] input) {
        for (int i = 0; i < input.Length; i++) {
            for (int j = i + 1; j < input.Length; j++) {
                if (compareCards(input[i], input[j])) {
                    CardSO temp = input[i];
                    input[i] = input[j];
                    input[j] = temp;
                }
            }
        }
        return input;
    }

    /**
    *   Counts number of null indexes in input
    */
    int countNull(CardSO[] input) {
        int count = 0;
        for (int i = 0; i < input.Length; i++) {
            if (input[i] == null) {
                count++;
            }
        }
        return count;
    }

    /**
    *   Takes input of sorted array and balances an equal number of null 
    *   indexes at the beginning and end. If number of null indexes is 
    *   odd, the extra null index will be placed at the end of the array.
    */
    CardSO[] balanceCardArray(CardSO[] input) {
        CardSO[] output = new CardSO[input.Length];
        int mod = countNull(input) / 2;
        for (int i = 0; i < input.Length && input[i] != null; i++) {
            output[i + mod] = input[i];
        }
        return output;
    }
    #endregion
    #endregion

    #region card selection
    /**
    *   First checks if the cursor is not on a card, in which selection will be
    *   set to -1 if no card is locked in. Regardless of whether a card is 
    *   locked in, the function will return at this point if no card is
    *   detected on the cursor.
    *
    *   The function will then check if a card is locked in. If so, it checks
    *   for a left click. If a left click is detected, selectLock is set to
    *   false. Regardless of a click, the function will return at this point if
    *   a card was locked in.
    *
    *   Finally, since a card is on the cursor and no card is locked in, the
    *   function will set selected to cardOnCursor(). The function will finish
    *   by checking for a click, in which case the selected card will be locked
    *   in.
    */
    void selectCard() {
        if (cardOnCursor() == -1) {
            if (!selectLock) {
                selected = -1;
            } 
            return;
        }

        if (selectLock) {
            if (Input.GetMouseButtonDown(0)) {
                selectLock = false;
            } 
            return;
        }

        selected = cardOnCursor();
        selectLock = Input.GetMouseButtonDown(0);
    }

    void updateCardSelectionStatus() {
        for (int i = 0; i < cardObjects.Length; i++) {
            cardObjects[i].setSelected(i == selected);
        }
    }

    /**
    *   Detects which hand card the cursor is currently over
    *
    *   In case of overlap, prioritizes the card to the right (which should
    *   be rendered on top)
    *
    *   Returns the index of the card, defaults to -1 if no card is found
    */
    int cardOnCursor() {
        int output = -1;
        for (int i = 0; i < cardObjects.Length; i++) {
            if (Util.mouseInBoundsNoBottom(cardObjects[i].getSR()) && hand[i] != null) {
                output = i;
            }
        }
        return output;
    }
    #endregion
}
