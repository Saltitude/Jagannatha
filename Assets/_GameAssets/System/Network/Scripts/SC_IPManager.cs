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
    bool EthernetFind = false;
    [SerializeField]
    bool noNameFind = false;
    [SerializeField]
    string EthernetIpv4;
    [SerializeField]
    string HamachiIpv4;
    [SerializeField]
    string OtherIpv4;

    [SerializeField]
    List<string> list_AllIpv4;

    public string localComputerIP = "0.0.0.0";

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

        int i = 0;
        bool b_otherIpIsSet = false;

        foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
        {
            foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
            {             
                if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {

                    list_AllIpv4.Add(ip.Address.ToString());

                    //Debug.Log("Find Ipv4 : " + item.Name + " | " + ip.Address.ToString());

                    if (item.Name == "Hamachi")
                    {
                        hamachiUsed = true;
                        HamachiIpv4 = ip.Address.ToString();
                    }      
                    
                    //else if (item.Name == "Ethernet")
                    else if (item.Name.Contains("Ethernet"))
                    {
                        EthernetFind = true;
                        EthernetIpv4 = ip.Address.ToString();
                    }

                    else if(!b_otherIpIsSet)
                    {
                        b_otherIpIsSet = true;
                        noNameFind = true;
                        OtherIpv4 = ip.Address.ToString();
                    }

                }               
            }
        }

        string IpToUsed;

        if (hamachiUsed)
            IpToUsed = HamachiIpv4;
        else if (EthernetFind)
            IpToUsed = EthernetIpv4;
        else
            IpToUsed = OtherIpv4;

        return IpToUsed;

    }
}

