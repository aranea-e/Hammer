using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMarker : MonoBehaviour
{
    public GameManager Game;
    public Sprite[] sprites;
    public SpriteRenderer sr;

    void Start()
    {
        sprites = Resources.LoadAll<Sprite>("Sprites/PlayerMarkers");
    }

    void Update()
    {
        sr.sprite = sprites[Game.selfPlayerNumber]; //sets sprite in corner to own player marker
        sr.enabled = Game.isUserPlaying;
    }
}
