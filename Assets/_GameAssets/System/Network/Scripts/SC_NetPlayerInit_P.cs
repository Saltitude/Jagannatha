using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SC_NetPlayerInit_P : NetworkBehaviour
{

    GameObject Mng_CheckList = null;

    public GameObject Mng_GameState;
    public GameObject Mng_SyncVariablesManager;

    // Start is called before the first frame update
    void Start()
    {
        IsCheck();

        if (isServer && isLocalPlayer)
        {
            SpawnInit();
        }

    }

    void IsCheck()
    {

        Mng_CheckList = GameObject.FindGameObjectWithTag("Mng_CheckList");

        if (isServer && isLocalPlayer)
            Mng_CheckList.GetComponent<SC_CheckList>().NetworkPlayerPilot = this.gameObject;

        if (!isServer && !isLocalPlayer)
            Mng_CheckList.GetComponent<SC_CheckList>().NetworkPlayerOperator = this.gameObject;

    }

    void SpawnInit()
    {

        //SyncVariables Manager
        GameObject GO_SyncVariablesManager_Temp = (GameObject)Instantiate(Mng_SyncVariablesManager, Mng_SyncVariablesManager.transform.position, Mng_SyncVariablesManager.transform.rotation);
        NetworkServer.Spawn(GO_SyncVariablesManager_Temp);

        //SyncVariables Manager
        GameObject GO_Mng_GameState_Temp = (GameObject)Instantiate(Mng_GameState, Mng_GameState.transform.position, Mng_GameState.transform.rotation);
        NetworkServer.Spawn(GO_Mng_GameState_Temp);
    }

}
