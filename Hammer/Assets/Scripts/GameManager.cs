using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks {
    void Awake() {
        Util.Start();
        FindObjectOfType<Deck>().ManualStart();
    }
}
