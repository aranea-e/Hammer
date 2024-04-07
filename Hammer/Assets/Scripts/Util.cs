using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util {
    public static Sprite[] cardSprites;
    public static CardSO[] deck;

    public static void Start() {
        loadCardSprites();
        loadDeckSO();
    }
    
    #region mouse position
    /**
    *   Returns current position of the cursor in world space as a Vector2 containing floats
    */
    public static Vector2 mousePosition() {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    /**
    *   Takes 2 Vector2 inputs, the bottom left corner and top right corner of
    *   the area to be checked
    *   Returns true if the cursor is within the specified bounds, inclusive
    *   Returns false if the cursor is outside of the specified bounds
    */
    public static bool mouseInBounds(Vector2 low, Vector2 high) {
        return low.x <= mousePosition().x &&
               low.y <= mousePosition().y &&
               high.x >= mousePosition().x &&
               high.y >= mousePosition().y;
    }

    /**
    *   Takes 1 Bounds input
    *   Runs mouseInBounds(Vector2, Vector2) using the min and max of Bounds
    */
    public static bool mouseInBounds(Bounds b) {
        return mouseInBounds(b.min, b.max);
    }

    /**
    *   Takes 1 SpriteRenderer input
    *   Runs mouseInBounds(Vector2, Vector2) using the min and max of SpriteRenderer.Bounds
    */
    public static bool mouseInBounds(SpriteRenderer s) {
        return mouseInBounds(s.bounds.min, s.bounds.max);
    }

    /**
    *   Takes 2 Vector2 inputs, the left bound and top right corner of the area
    *   to be checked, ignoring the bottom bound
    *   Returns true if the cursor is within the specified bounds, inclusive
    *   Returns false if the cursor is outside of the specified bounds
    */
    public static bool mouseInBoundsNoBottom(Vector2 low, Vector2 high) {
        return low.x <= mousePosition().x &&
               high.x >= mousePosition().x &&
               high.y >= mousePosition().y;
    }

    /**
    *   Takes 1 SpriteRenderer input
    *   Runs mouseInBoundsNoBottom(Vector2, Vector2) using the min and max of SpriteRenderer.Bounds
    */
    public static bool mouseInBoundsNoBottom(SpriteRenderer s) {
        return mouseInBoundsNoBottom(s.bounds.min, s.bounds.max);
    }
    #endregion

    #region load resources
    static void loadCardSprites() {
        cardSprites = Resources.LoadAll<Sprite>("Sprites/Cards");
    }
    
    static void loadDeckSO() {
        deck = Resources.LoadAll<CardSO>("ScriptableObjects/Cards");
    }
    #endregion

    #region misc
    public static CardSO randomCard() {
        return deck[Random.Range(0, deck.Length)];
    }

    /**
    *   Calculates modifier for y position of hand card based on the function
    *   f(t) = -cos(10πt)/2 + 0.5
    */
    public static float calculateMod(float t) {
        return (-Mathf.Cos(10 * Mathf.PI * t) / 2) + 0.5f;
    }

    /**
    *   Takes two inputs
    *   a is the card to be played
    *   b is the stack on which it is intended to be played
    *   b is assumed to have no null cards between non-null cards
    *
    *   If b is empty or a is null, returns false
    *   If b is a single card and special, returns false
    *   If a has the same value as the first card of b, returns true
    *   If a is special and b only has one card, returns true
    *   All other cases return false
    */
    public static bool canPlay(CardSO a, CardSO[] b) {
        if (b[0] == null || a == null || (b[1] == null && b[0].special) ) {
            return false;
        }

        if (a.value == b[0].value || (b[1] == null && a.special) ) {
            return true;
        }

        return false;
    }
    #endregion

}
