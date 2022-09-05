using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Trash : MonoBehaviour
{
    public HandManager hm;
    public TextMeshProUGUI textmesh;
    public SpriteRenderer glow;
    public Transform trans;
    public GameManager Game;

    void Update()
    {
        textmesh.text = hm.cardsToDiscard.ToString();
        glow.enabled = (!Game.IsPaused && hm.cardHover != null && hm.cardHover.cardCurrent != "none" && hm.cardsToDiscard > 0);
    }
}
