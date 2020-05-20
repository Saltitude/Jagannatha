using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class SC_IPManager : NetworkBehaviour
{
    [SerializeField]
    NetworkManager _NetworkManager;

    [SerializeField]
    bool isLocalHostBuild;
    [SerializeField]
    bool isLocalHostEditor;

    public string localComputerIP = "0.0.0.0";
    // Start is called before the first frame update
    void Start()
    {
        //        if(isLocalHostBuild)
        //            _NetworkManager.networkAddress = "localhost";

        //        else
        //            _NetworkManager.networkAddress = "192.168.151.116";

        //#if UNITY_EDITOR
        //        if (isLocalHostEditor)
        //        {
        //            _NetworkManager.networkAddress = "localhost";
        //        }
        //        else
        //        {
        //            _NetworkManager.networkAddress = "192.168.151.116";
        //        }



        //#endif
        //localComputerIP = GetLocalIPAddress();
        //Debug.Log("Current IP is : " + localComputerIP);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public string GetLocalIPAddress()
    {
        var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }

        throw new System.Exception("No network adapters with an IPv4 address in the system!");
    }
}
