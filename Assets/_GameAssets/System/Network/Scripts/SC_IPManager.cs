using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using System.Net.NetworkInformation;
using System.Net.Sockets;

public class SC_IPManager : NetworkBehaviour
{
    [SerializeField]
    NetworkManager _NetworkManager;

    [SerializeField]
    bool isLocalHostBuild;
    [SerializeField]
    bool isLocalHostEditor;

    [SerializeField]
    bool hamachiUsed = false;
    [SerializeField]
    string EthernetIpv4;
    [SerializeField]
    string HamachiIpv4;

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
        var host2 = System.Net.Dns.GetHostByName("TLS01029.etpa.local");

        foreach (var ip in host2.AddressList)
        {           

            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }

        throw new System.Exception("No network adapters with an IPv4 address in the system!");

    }

    public string GetIP()
    {
        foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
        {
            foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
            {             
                if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {

                    //Debug.Log("Find Ipv4 : " + item.Name + " | " + ip.Address.ToString());

                    if (item.Name == "Hamachi")
                    {
                        hamachiUsed = true;
                        HamachiIpv4 = ip.Address.ToString();
                    }      
                    
                    else if (item.Name == "Ethernet")
                    {
                        EthernetIpv4 = ip.Address.ToString();
                    }

                }               
            }
        }

        string IpToUsed;

        if (hamachiUsed)
            IpToUsed = HamachiIpv4;
        else
            IpToUsed = EthernetIpv4;

        return IpToUsed;

    }
}

