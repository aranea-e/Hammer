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
    public Transform trans;

    void Update()
    {
        if (!Game.IsPaused)
        {
            timerTime += Time.deltaTime; //adds time since last frame
            if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x <= trans.position.x + 0.5f && Camera.main.ScreenToWorldPoint(Input.mousePosition).x >= trans.position.x - 0.5f && Camera.main.ScreenToWorldPoint(Input.mousePosition).y <= trans.position.y + 0.75f && Camera.main.ScreenToWorldPoint(Input.mousePosition).y >= trans.position.y - 0.75f && Input.GetKeyDown(KeyCode.Mouse0))
            { //if this was clicked on
                timerTime = 0f; //resets timer
                hm.drawCard(); //draws twice
                hm.drawCard();
            }
            if (timerTime >= timeToDraw)
            { //if time is greater than set value
                timerTime -= timeToDraw; //reduces time by set value
                hm.drawCard(); //draws twice
                hm.drawCard();
            }
            sr.sprite = sprites[(int)Math.Floor((timerTime / timeToDraw) * 9)]; //displays most accurate clock sprite
        }
    }
}
