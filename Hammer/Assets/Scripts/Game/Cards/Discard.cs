using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Discard : MonoBehaviour
{
    public string discardedCard;
    public Sprite sprite;
    public Sprite[] sprites;
    
    public SpriteRenderer sr;

    void Start()
    {
        sprites = Resources.LoadAll<Sprite>("Sprites/Cards"); //loads spritesheet of cards for later use
    }

    void Update()
    {
        if (discardedCard == "none") //if no card was drawn, hides the sprite
        {
            sr.enabled = false;
        } else
        {
            for (int i = 0; i < sprites.Length; i++) //finds sprite that matches discardedCard
            {
                if (discardedCard == sprites[i].name)
                {
                    sprite = sprites[i];
                }
            }
            sr.sprite = sprite; //sets sprite
            sr.enabled = true; //enables the sprite in case it was previously none
        }
    }

    public void discardCard(string Card)
    {
        discardedCard = Card;
    }
}
