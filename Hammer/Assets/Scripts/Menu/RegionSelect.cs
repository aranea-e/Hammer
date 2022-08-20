using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class RegionSelect : MonoBehaviourPunCallbacks
{
    public TMP_Dropdown dd;
    public List<string> options;
    public PingList pl;
    public TextMeshProUGUI tmp;

    void Start()
    {
        pl = Object.FindObjectOfType<PingList>();
        options.Clear();
        int j = -1;
        for (int i = 0; i < pl.servers.Length; i++)
        {
            options.Add(pl.servers[i] + ", " + pl.pings[i] + "ms");
            if (pl.servers[i] == PhotonNetwork.CloudRegion)
            {
                j = i;
            }
        }
        dd.ClearOptions();
        dd.AddOptions(options);
        dd.value = j;
    }

    public void OnChange()
    {
        string regionName = "";
        bool cont = false;
        for (int i = 0; !cont; i++)
        {
            if (tmp.text.Substring(i, 1) == ",")
            {
                cont = true;
            } else
            {
                regionName += tmp.text.Substring(i, 1);
            }
        }
        PhotonNetwork.Disconnect();
        PhotonNetwork.ConnectToRegion(regionName);
    }
}
