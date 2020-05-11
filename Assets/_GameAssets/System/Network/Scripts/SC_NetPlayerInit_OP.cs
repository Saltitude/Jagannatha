using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SC_NetPlayerInit_OP : NetworkBehaviour
{

    #region Singleton

    private static SC_NetPlayerInit_OP _instance;
    public static SC_NetPlayerInit_OP Instance { get { return _instance; } }

    #endregion

    GameObject Mng_CheckList = null;

    void Awake()
    {

        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        IsCheck();
    }

    void IsCheck()
    {

        Mng_CheckList = GameObject.FindGameObjectWithTag("Mng_CheckList");

        if (!isServer && isLocalPlayer)
            Mng_CheckList.GetComponent<SC_CheckList>().NetworkPlayerOperator = this.gameObject;

        if (!isServer && !isLocalPlayer)
            Mng_CheckList.GetComponent<SC_CheckList>().NetworkPlayerPilot = this.gameObject;

    }

    // Update is called once per frame
    void Update()
    {
        if (!isServer && isLocalPlayer)
        {



        }
    }

    [Command]
    public void CmdSendForceUpdate()
    {
        if(isServer)
            SC_MainBreakDownManager.Instance.ForceUpdate();
    }

}
