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
    *   Takes 2 Vector2 inputs, the bottom left corner and top right corner of the area to be checked
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
    /**
    *   Calculates modifier for y position of hand card based on the function
    *   f(t) = -cos(3πt)/2 + 0.5
    */
    public static float calculateMod(float t) {
        return (-Mathf.Cos(10 * Mathf.PI * t) / 2) + 0.5f;
    }
    #endregion

}
