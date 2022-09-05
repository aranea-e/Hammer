using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCard : MonoBehaviour
{
    public Sprite[] sprites;
    public Sprite sprite;
    public string cardCurrent;
    public int cardNumber;
    
    public HandManager hm;
    public SpriteRenderer sr;
    public Transform trans;

    public void Start()
    {
        sprites = Resources.LoadAll<Sprite>("Sprites/Cards"); //loads spritesheet of cards for later use
    }

    public void updateSprite()
    {
        if (cardCurrent == "none") //if no card was drawn, hides the sprite
        {
            sr.enabled = false;
        } else
        {
            for (int i = 0; i < sprites.Length; i++) //finds sprite that matches cardCurrent
            {
                if (cardCurrent == sprites[i].name)
                {
                    sprite = sprites[i];
                }
            }
            sr.sprite = sprite; //sets sprite
            sr.enabled = true; //enables the sprite in case it was previously none
        }
    }
}
