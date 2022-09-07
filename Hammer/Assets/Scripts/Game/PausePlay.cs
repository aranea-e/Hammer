using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausePlay : MonoBehaviour
{
    public GameManager Game;
    public SpriteRenderer sr;
    public Sprite quake;
    public Sprite pause;

    void Update()
    {
        sr.enabled = Game.IsPaused; //displays if game is paused
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
        }
    }
}
