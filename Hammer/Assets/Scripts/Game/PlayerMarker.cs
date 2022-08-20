using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMarker : MonoBehaviour
{
    public UserManager um;
    public Sprite[] sprites;
    public SpriteRenderer sr;

    void Start()
    {
        sprites = Resources.LoadAll<Sprite>("Sprites/PlayerMarkers");
    }

    void Update()
    {
        um = Object.FindObjectOfType<UserManager>();
        sr.sprite = sprites[um.selfPlayerNumber];
    }
}
