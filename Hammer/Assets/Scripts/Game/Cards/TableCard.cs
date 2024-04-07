using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableCard : MonoBehaviour {
    [SerializeField] private CardSO[] cards;
    [SerializeField] private int id;
    [SerializeField] private SpriteRenderer s;
    [SerializeField] private SpriteRenderer specialSR;
    [SerializeField] private SpriteRenderer glow;
    [SerializeField] private SpriteRenderer specialGlow;

    void Awake() {
        reset();
    }

    void Start() {
        cards[0] = FindObjectOfType<Deck>().drawCard();
    }

    void Update() {
        updateSprite();
        checkForPlayCard();
    }


    [ContextMenu("Reset Card")]
    void reset() {
        cards = new CardSO[5];
    }

    /**
    *   If there is no cards on a given stack, hides that stack as well as its
    *   special card
    *
    *   Else if the top card is an Ace or Joker and there are multiple cards in
    *   the stack, displays it as the special card (off to the side) and shows 
    *   the card before it in the normal position.
    *
    *   In all other cases, displays the final card in the stack
    *
    *   Then, updates the card glow. Turns on the glow if the currently 
    *   selected card can be played and the attatched SpriteRenderer is 
    *   enabled.
    */
    void updateSprite() {
        if (lastCard() == -1) {
            s.enabled = false;
            specialSR.enabled = false;
        } else if (cards[lastCard()].special && lastCard() != 0) {
            s.sprite = cards[lastCard() - 1].s;
            s.enabled = true;
            specialSR.sprite = cards[lastCard()].s;
            specialSR.enabled = true;
        } else {
            s.sprite = cards[lastCard()].s;
            s.enabled = true;
            specialSR.enabled = false;
        }

        glow.enabled = Util.canPlay(FindObjectOfType<Hand>().getSelectedCardSO(), cards)
                       && s.enabled;
        specialGlow.enabled = Util.canPlay(FindObjectOfType<Hand>().getSelectedCardSO(), cards)
                       && specialSR.enabled;
    }

    /**
    *   Checks for the last index in the cards array to not return null
    *   If all are null, returns -1
    *   If non are null, returns final index
    *   Assumes cards array does not contain null between cards
    */
    int lastCard() {
        for (int i = 0; i < cards.Length; i++) {
            if (cards[i] == null) {
                return i - 1;
            }
        }
        return cards.Length - 1;
    }

    /**
    *   If the mouse is clicked, the cursor is on the card, and the selected
    *   card can be played on this card, calls playCard() with the selected 
    *   card.
    */
    void checkForPlayCard() {
        if (Input.GetMouseButtonDown(0) 
            && (Util.mouseInBounds(s) || (Util.mouseInBounds(specialSR) && specialSR.enabled))
            && Util.canPlay(FindObjectOfType<Hand>().getSelectedCardSO(), cards)) {
            playCard(FindObjectOfType<Hand>().getSelectedCardSO());
        }
    }

    void playCard(CardSO c) {
        FindObjectOfType<Hand>().playCard(c);
        cards[lastCard() + 1] = c;
    }
}
