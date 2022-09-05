using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(IsPaused); //sends game pause state to clients
        } else
        {
            IsPaused = (bool)stream.ReceiveNext(); //recieves game pause state
        }
    }
}
