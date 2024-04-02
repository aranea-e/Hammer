using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class ConnectToServer : MonoBehaviourPunCallbacks {
    PingList pl = null;
    void Awake() {
        pl = FindObjectOfType<PingList>();
    }

    void Start() { //automatic process with no user input 
        PhotonNetwork.ConnectUsingSettings(); //connects to servers, allowing user to later join a room
    }

    public override void OnConnectedToMaster() { //activates when connected to server 
        PhotonNetwork.JoinLobby(); //joins the lobby, allowing users to now join rooms
    }

    public override void OnJoinedLobby() { //activates when user joins the lobby
        SceneManager.LoadScene("Lobby"); //loads lobby scene with room selection
    }

    public override void OnRegionListReceived(RegionHandler regionHandler) {
        regionHandler.PingMinimumOfRegions(PingCompleteCallback, ""); //take a ping sample for each region
    }

    void PingCompleteCallback(RegionHandler regionHandler) {
        pl.updateRegionHandler(regionHandler);
        Photon.Realtime.Region bestRegion = pl.updateList();
        if (pl.overrideRegion != null) { //overrides to selected user region
            PhotonNetwork.ConnectToRegion(pl.overrideRegion);
            return;
        }

        bool cont = false;
        string bestRegionID = null;
        for (int i = 0; !cont; i++) { //finds ID for best region
            if (bestRegion.ToString().Substring(i, 1) == "[") {
                bestRegionID = bestRegion.ToString().Substring(0, i);
                cont = true;
            }
        }
        PhotonNetwork.ConnectToRegion(bestRegionID); //connects to best region
    }
}
