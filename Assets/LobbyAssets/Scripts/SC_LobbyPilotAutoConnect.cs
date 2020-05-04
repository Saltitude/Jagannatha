using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class SC_LobbyPilotAutoConnect : MonoBehaviour
{
    // Start is called before the first frame update
    public NetworkManager manager;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!NetworkClient.active && !NetworkServer.active)
        {
            manager.StartHost();
        }

    }
}
