using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausePlay : MonoBehaviour
{
    public GameManager Game;
    public SpriteRenderer sr;

    void Update()
    {
        sr.enabled = Game.IsPaused; //displays if game is paused
    }
}
