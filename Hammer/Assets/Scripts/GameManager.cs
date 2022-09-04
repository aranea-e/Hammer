using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool IsPaused;
    
    public Spawn spawner;
    public HandManager hm;
    public DeckManager dm;
    public TableCard[] tableCards;
    public HandCard[] handCards;

    void Awake()
    {
        spawner.SpawnStart();
    }
}
