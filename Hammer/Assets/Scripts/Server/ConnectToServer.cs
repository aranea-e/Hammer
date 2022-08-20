using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    public Photon.Realtime.Region bestRegion;
    public string bestRegionID;
    public PingList pl;

    void Start() //automatic process with no user input
    {
        PhotonNetwork.ConnectUsingSettings(); //connects to servers, allowing user to later join a room
    }

    public override void OnConnectedToMaster() //activates when connected to server
    {
        PhotonNetwork.JoinLobby(); //joins the lobby, allowing users to now join rooms
    }

    public override void OnJoinedLobby() //activates when user joins the lobby
    {
        SceneManager.LoadScene("Lobby"); //loads lobby scene with room selection
    }

    public override void OnRegionListReceived(RegionHandler regionHandler)
    {
        regionHandler.PingMinimumOfRegions(PingCompleteCallback, "");
    }

    void PingCompleteCallback(RegionHandler regionHandler)
    {
        string regionName;
        List<Photon.Realtime.Region> regions = regionHandler.EnabledRegions;
        bool cont;
        int j = 0;
        foreach (var region in regions)
        {
            
            regionName = "";
            cont = false;
            for (int i = 0; !cont; i++)
            {
                if (region.ToString().Substring(i, 1) == "[")
                {
                    cont = true;
                } else
                {
                    regionName += region.ToString().Substring(i, 1);
                }
            }
            pl.servers[j] = regionName;
            pl.pings[j] = region.Ping;
            
            if (bestRegion == null || region.Ping < bestRegion.Ping)
            {
                bestRegion = region;
            }
            j++;
        }
        cont = false;
        for (int i = 0; !cont; i++)
        {
            if (bestRegion.ToString().Substring(i, 1) == "[")
            {
                cont = true;
            } else
            {
                bestRegionID += bestRegion.ToString().Substring(i, 1);
            }
        }
        PhotonNetwork.ConnectToRegion(bestRegionID);
    }
}
