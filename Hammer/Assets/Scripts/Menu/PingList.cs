using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PingList : MonoBehaviourPunCallbacks
{
    public string[] servers;
    public int[] pings;
    RegionHandler regionHandler;
    public string overrideRegion;

    void Awake()
    {
        if (FindObjectsOfType<PingList>().Length > 1) {
            Destroy(this.gameObject);
        }
        //prevents list from being destroyed, allowing ping data to be accessed at any time
        DontDestroyOnLoad(this.gameObject);
        overrideRegion = null;
    }

    public void updateRegionHandler(RegionHandler regionHandler) {
        this.regionHandler = regionHandler;
    }

    /**
    *   updates list of all regions and their pings. Returns region with lowest ping.
    */
    public Photon.Realtime.Region updateList() {
        string regionName;
        Photon.Realtime.Region bestRegion = null;
        List<Photon.Realtime.Region> regions = regionHandler.EnabledRegions;
        bool cont;
        int j = 0;
        servers = new string[regions.Count];
        pings = new int[regions.Count];
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
            servers[j] = regionName; //adds region ID to list
            pings[j] = region.Ping; //adds region ping to list
            
            if (bestRegion == null || region.Ping < bestRegion.Ping)
            { //if this is the lowest ping, sets it to best region
                bestRegion = region;
            }
            j++;
        }
        return bestRegion;
    }
}
