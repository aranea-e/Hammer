using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCard : MonoBehaviour {
    [SerializeField] private CardSO c;
    [SerializeField] private SpriteRenderer s;
    [SerializeField] private SpriteRenderer glow;
    [SerializeField] private float animationTimer = 0;
    [SerializeField] private bool selected;

    void Update() {
        updateSprite();
        updateTimer();
        updateCardPos();
        enabledGlow();
    }

    #region get/set fuctions
    public SpriteRenderer getSR() {
        return s;
    }
    
    public void setCard(CardSO c) {
        this.c = c;
    }

    public CardSO getCard() {
        return c;
    }
    
    public void setSelected(bool selected) {
        this.selected = selected;
    }

    [ContextMenu("Reset Card")]
    void setNull() {
        setCard(null);
    }
    #endregion

    #region sprite and animations
    void updateSprite() {
        if (c != null) {
            s.enabled = true;
            s.sprite = c.s;
        } else {
            s.enabled = false;
            s.sprite = Util.cardSprites[0];
        }
    }

    void updateTimer() {
        if (s.enabled == false) {
            animationTimer = 0.0f;
        } else if (selected) {
            animationTimer += Time.deltaTime;
        } else {
            animationTimer -= Time.deltaTime;
        }

        if (animationTimer > 0.1f) {
            animationTimer = 0.1f;
        }
        if (animationTimer < 0.0f) {
            animationTimer = 0.0f;
        }
    }

    void updateCardPos() {
        transform.position = new Vector2(transform.position.x, -5 + Util.calculateMod(animationTimer));
    }

    void enabledGlow() {
        glow.enabled = (FindObjectOfType<Hand>().getLock() && selected && s.enabled);
    }
    #endregion
}
