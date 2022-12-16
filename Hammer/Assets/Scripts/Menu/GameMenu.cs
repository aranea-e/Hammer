using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public static class GameMenu
{
    public static bool isOpen = false;
    public static bool leavingRoom;
    public static byte leaveReason;

    public static void openMenu()
    {
        SceneManager.LoadScene(4, LoadSceneMode.Additive);
        isOpen = true;
    }

    public static void closeMenu()
    {
        SceneManager.UnloadSceneAsync(4);
        isOpen = false;
    }

    public static void exit(byte reason = 0)
    {
        if (PhotonNetwork.CurrentRoom != null && !leavingRoom)
        {
            PhotonNetwork.LeaveRoom();
            leavingRoom = true;
            leaveReason = reason;
        }
    }
}
