using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CardTimer : MonoBehaviour
{
    public float timerTime;
    public float timeToDraw;

    public SpriteRenderer sr;
    public Sprite[] sprites;
    public GameManager Game;
    public HandManager hm;

    void Update()
    {
        if (!Game.IsPaused)
        {
            timerTime += Time.deltaTime;
            if (timerTime >= timeToDraw)
            {
                timerTime -= timeToDraw;
                hm.drawCard();
            }
            sr.sprite = sprites[(int)Math.Floor((timerTime / timeToDraw) * 9)];
        }
    }
}
