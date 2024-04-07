using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableCard : MonoBehaviour {
    [SerializeField] private CardSO c;
    [SerializeField] private int id;

    void Update() {
        updateSprite();
    }

    void updateSprite() {
        if (c != null) {
            GetComponent<SpriteRenderer>().sprite = c.s;
        } else {
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
