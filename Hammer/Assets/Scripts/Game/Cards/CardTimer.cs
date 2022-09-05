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
            timerTime += Time.deltaTime;
            if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x <= trans.position.x + 0.5f && Camera.main.ScreenToWorldPoint(Input.mousePosition).x >= trans.position.x - 0.5f && Camera.main.ScreenToWorldPoint(Input.mousePosition).y <= trans.position.y + 0.75f && Camera.main.ScreenToWorldPoint(Input.mousePosition).y >= trans.position.y - 0.75f && Input.GetKeyDown(KeyCode.Mouse0))
            {
                timerTime = 0f;
                hm.drawCard();
                hm.drawCard();
            }
            if (timerTime >= timeToDraw)
            {
                timerTime -= timeToDraw;
                hm.drawCard();
                hm.drawCard();
            }
            sr.sprite = sprites[(int)Math.Floor((timerTime / timeToDraw) * 9)];
        }
    }
}
