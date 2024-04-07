using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Deck : MonoBehaviour {
    [SerializeField] private CardSO[] deck;
    [SerializeField] private TextMeshProUGUI text;

    public void ManualStart() {
        resetDeck();
        shuffle();
    }

    void Update() {
        text.text = (54 - countNull()).ToString();
    }

    public CardSO[] getDeck() {
        return deck;
    }

    [ContextMenu("reset deck")]
    public void resetDeck() {
        deck = new CardSO[Util.deck.Length];
        for (int i = 0; i < deck.Length; i++) {
            deck[i] = Util.deck[i];
        }
    }

    public CardSO topCard() {
        return deck[0];
    }

    [ContextMenu("move null to end")]
    void moveNullToEnd() {
        for (int i = 0; i < deck.Length; i++) {
            if (deck[i] == null) {
                for (int j = i + 1; j < deck.Length && deck[i] == null; j++) {
                    if (deck[j] != null) {
                        deck[i] = deck[j];
                        deck[j] = null;
                    }
                }
                if (deck[i] == null) { return; }
            }
        }
    }

    int countNull() {
        int count = 0;
        for (int i = 0; i < deck.Length; i++) {
            if (deck[i] == null) {
                count++;
            }
        }
        return count;
    }

    /**
    *   Moves all cards one index down, and returns the card which was
    *   previously at deck[0]
    */
    public CardSO drawCard() {
        CardSO output = deck[0];
        for (int i = 1; i < deck.Length; i++) {
            deck[i - 1] = deck[i];
        }
        deck[deck.Length - 1] = null;
        return output;
    }

    [ContextMenu("shuffle")]
    public void shuffle() {
        List<CardSO> deckList = new List<CardSO>();
        List<CardSO> shuffledDeckList = new List<CardSO>();
        for (int i = 0; i < deck.Length; i++) {
            deckList.Add(deck[i]);
        }

        while (deckList.Count > 0) {
            int i = Random.Range(0, deckList.Count);
            shuffledDeckList.Add(deckList[i]);
            deckList.RemoveAt(i);
        }

        for (int i = 0; i < deck.Length; i++) {
            deck[i] = shuffledDeckList[i];
        }
    }
}
