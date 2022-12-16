using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausePlay : MonoBehaviour
{
    public GameManager Game;
    public SpriteRenderer sr;
    public Sprite quake;
    public Sprite pause;
    float fade = 1.0f;
    [SerializeField] Color color;

    void Update()
    {
        
        if (Game.IsPaused)
        {
            fade = 1.0f; //sets float to fade play button away after pause
        } else
        {
            if (fade > 0)
            {
                fade -= Time.deltaTime;
            }
        }
        if (Game.pauseReason == 3)
        {
            sr.sprite = quake;
        } else
        {
            sr.sprite = pause;
        }
        if (Game.IsPregame)
        {
            sr.enabled = false; //hides if the game is in pregame
        } else
        {
            sr.enabled = true;
        }
        color.a = fade;
        sr.color = color;
    }
}
