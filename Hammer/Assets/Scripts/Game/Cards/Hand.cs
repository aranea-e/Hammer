using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour {
    [SerializeField] private CardSO[] hand;
    [SerializeField] private HandCard[] cardObjects;

    void Awake() {
        resetHand();
    }

    void Update() {
        updateHandSprites();
        sortHand();
    }

    public void updateHandSprites() {
        for (int i = 0; i < hand.Length; i++) {
            cardObjects[i].setCard(hand[i]);
        }
    }

    public void resetHand() {
        hand = new CardSO[21];
    }
    
    public void setCardAtIndex(CardSO c, int index) {
        hand[index] = c;
    }

    public CardSO getCardAtIndex(int index) {
        return hand[index];
    }

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

    [ContextMenu("sort hand")]
    void sortHand() {
        hand = sortCardArray(hand);
    }

    [ContextMenu("randomize hand")]
    void randomizeHand() {
        for (int i = 0; i < hand.Length; i++) {
            if (Random.Range(0, 10) == 0) {
                hand[i] = null;
            } else {
                hand[i] = Util.deck[Random.Range(0, 54)];
            }
        }
        sortHand();
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
}
