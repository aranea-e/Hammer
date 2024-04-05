using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCard : MonoBehaviour {
    [SerializeField] private CardSO c;

    void Update() {
        updateSprite();
    }

    void updateSprite() {
        if (c != null) {
            GetComponent<SpriteRenderer>().enabled = true;
            GetComponent<SpriteRenderer>().sprite = c.s;
        } else {
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<SpriteRenderer>().sprite = Util.cardSprites[0];
        }
    }
    
    public void setCard(CardSO c) {
        this.c = c;
    }

    public CardSO getCard() {
        return c;
    }

    [ContextMenu("Reset Card")]
    void setNull() {
        setCard(null);
    }
}
