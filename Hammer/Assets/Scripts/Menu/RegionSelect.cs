using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class RegionSelect : MonoBehaviourPunCallbacks {
    public TMP_Dropdown dd;
    public List<string> options;
    public PingList pl;
    public TextMeshProUGUI tmp;
    public bool performOnChange;

    void Start() {
        pl = Object.FindObjectOfType<PingList>();
        updateList();
    }

    void updateList() {
        performOnChange = false;
        options.Clear();
        int j = -1;
        for (int i = 0; i < pl.servers.Length; i++) { 
            //adds all servers from PingList, and sets j to current server
            options.Add(pl.servers[i] + ", " + pl.pings[i] + "ms");
            if (pl.servers[i] == PhotonNetwork.CloudRegion) {
                j = i;
            }
        }
        dd.ClearOptions();
        dd.AddOptions(options); //adds options to dropdown menu
        dd.value = j; //sets selected option to current region
        performOnChange = true;
    }

    public void OnChange() {
        if (performOnChange)
        {
            string regionName = "";
            bool cont = false;
            for (int i = 0; !cont; i++) { //finds ID for selected region
                if (tmp.text.Substring(i, 1) == ",") {
                    cont = true;
                    regionName = tmp.text.Substring(0, i);
                }
            }
            PhotonNetwork.Disconnect(); //leaves current region
            pl.overrideRegion = regionName; //sets override region for load process
            SceneManager.LoadScene(1);
        }
    }
}
