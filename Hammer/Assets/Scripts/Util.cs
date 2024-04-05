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
    
    static void loadCardSprites() {
        cardSprites = Resources.LoadAll<Sprite>("Sprites/Cards");
    }
    
    static void loadDeckSO() {
        deck = Resources.LoadAll<CardSO>("ScriptableObjects/Cards");
    }
}
