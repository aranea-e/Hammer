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
        regionHandler.PingMinimumOfRegions(PingCompleteCallback, ""); //take a ping sample for each region
    }

    void PingCompleteCallback(RegionHandler regionHandler)
    {
        string regionName;
        List<Photon.Realtime.Region> regions = regionHandler.EnabledRegions;
        bool cont;
        int j = 0;
        foreach (var region in regions)
        { //goes through list of all regions
            
            regionName = "";
            cont = false;
            for (int i = 0; !cont; i++)
            { //finds ID for selected region
                if (region.ToString().Substring(i, 1) == "[")
                { //finishes if character i == [
                    cont = true;
                } else
                { //adds character i to string
                    regionName += region.ToString().Substring(i, 1);
                }
            }
            pl.servers[j] = regionName; //adds region ID to list
            pl.pings[j] = region.Ping; //adds region ping to list
            
            if (bestRegion == null || region.Ping < bestRegion.Ping)
            { //if this is the lowest ping, sets it to best region
                bestRegion = region;
            }
            j++;
        }
        cont = false;
        for (int i = 0; !cont; i++)
        { //finds ID for best region
            if (bestRegion.ToString().Substring(i, 1) == "[")
            {
                cont = true;
            } else
            {
                bestRegionID += bestRegion.ToString().Substring(i, 1);
            }
        }
        PhotonNetwork.ConnectToRegion(bestRegionID); //connects to best region
    }
}
