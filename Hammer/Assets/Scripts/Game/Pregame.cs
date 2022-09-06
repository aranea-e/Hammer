using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class Pregame : MonoBehaviour
{
    public Canvas canvas;
    public Canvas postgameCanvas;
    public TextMeshProUGUI playerCount;
    public TextMeshProUGUI buttonText;
    public TextMeshProUGUI winnerText;
    public GameManager Game;

    void Update()
    {
        if (Game.IsPregame)
        { 
            if (!Game.isWinscreen)
            {//checks if the game is in pregame and not displaying winscreen, and displays information if it is
                canvas.enabled = true;
                postgameCanvas.enabled = false;
                playerCount.text = "Players: " + PhotonNetwork.PlayerList.Length;
                if (PhotonNetwork.PlayerList.Length > 1)
                { //checks if there are enough players to start
                    if (Game.pv.IsMine)
                    { //checks if user is host
                        buttonText.text = "<smallcaps>Start!</smallcaps>"; //shows user can press button to start
                        buttonText.fontStyle = FontStyles.Bold;
                    } else
                    {
                        buttonText.text = "Only the host may start the game.";
                        buttonText.fontStyle = FontStyles.Italic;
                    }
                } else
                {
                    buttonText.text = "2 or more players are required to start.";
                    buttonText.fontStyle = FontStyles.Italic;
                } 
            } else
            {
                canvas.enabled = false;
                postgameCanvas.enabled = true;
                winnerText.text = "Winner:\n" + Game.winner;
            }
        } else
        { //disables canvas during game
            canvas.enabled = false;
            postgameCanvas.enabled = false;
        }
    }

    public void onButtonPress()
    {
        if (PhotonNetwork.PlayerList.Length > 1 && Game.pv.IsMine)
        { //if player is host and enough players are present to start, tells GM to start
            Game.startGame();
        }
    }
}
