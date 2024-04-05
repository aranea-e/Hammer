using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Card", order = 0)]
public class CardSO : ScriptableObject {
    public Sprite s;
    public string id;
    public int value;
    public bool joker;
    public bool special;

    public string holder;
}
