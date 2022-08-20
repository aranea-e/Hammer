using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class RegionIndicator : MonoBehaviour
{
    public TextMeshProUGUI tmp;
    public string region;

    void Update()
    {
        if (PhotonNetwork.CloudRegion != null)
        {
            region = "";
            bool cont = false;
            for (int i = 0; !cont && i < PhotonNetwork.CloudRegion.Length; i++)
            {
                if (PhotonNetwork.CloudRegion.Substring(i, 1) == "/")
                {
                    cont = true;
                } else
                {
                    region += PhotonNetwork.CloudRegion.Substring(i, 1);
                }
            }
            tmp.text = "Connected: " + region + ", " + PhotonNetwork.GetPing() + "ms";
        }
    }
}
