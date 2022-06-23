using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Card : MonoBehaviourPunCallbacks, IPunObservable
{
    #region vars
    public string cardCurrent;
    public int cardNumber;
    public Vector2 cardLocation;
    public Sprite sprite;
    public Sprite[] sprites;
    #endregion

    #region references
    public DeckManager dm;
    public Spawn Spawn;
    public Transform trans;
    public SpriteRenderer sr;
    public PhotonView pv;
    #endregion

    void Start()
    {
        dm = Object.FindObjectOfType<DeckManager>();
        Spawn = Object.FindObjectOfType<Spawn>();
        bool cont = false;
        cardLocation.x = trans.position.x;
        cardLocation.y = trans.position.y;
        for (int i = 0; !cont; i++)
        {
            if (Spawn.cardLocations[i] == cardLocation)
            {
                cardNumber = i;
                dm.cardObjects[i] = this;
                cont = true;
            }
        }
        sprites = Resources.LoadAll<Sprite>("Sprites/Cards");
    }

    #region functions
    public void drawCard()
    {
        cardCurrent = dm.drawCard(dm.deck);
        updateSprite(cardCurrent);
    }

    public void updateSprite(string cardToDisplay)
    {
        if (cardToDisplay == "none")
        {
            sr.enabled = false;
        } else
        {
            for (int i = 0; i < sprites.Length; i++)
            {
                if (cardToDisplay == sprites[i].name)
                {
                    sprite = sprites[i];
                }
            }
            sr.sprite = sprite;
            sr.enabled = true;
        }
    }
    #endregion

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(cardCurrent);
        } else
        {
            cardCurrent = (string)stream.ReceiveNext();
            updateSprite(cardCurrent);
        }
    }
}