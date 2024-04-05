using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util {
    public static Sprite[] cardSprites;

    public static void Start() {
        loadCardSprites();
    }
    
    static void loadCardSprites() {
        cardSprites = Resources.LoadAll<Sprite>("Sprites/Cards");
    }
}
