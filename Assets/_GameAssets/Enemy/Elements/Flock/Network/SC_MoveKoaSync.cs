using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SC_MoveKoaSync : NetworkBehaviour
{

    public GameObject mr_P;
    public GameObject mr_OP;
    Transform guide;
    [SyncVar]
    public int curboidNumber = 0;
    [SyncVar]
    public int MaxboidNumber = 0;

    public string KoaID;

    // Start is called before the first frame update
    void Start()
    {
        if (isServer)
        {
            mr_OP.GetComponent<SphereCollider>().enabled = false;
            mr_OP.GetComponent<MeshRenderer>().enabled = false;
            mr_OP.SetActive(false);
            mr_P.GetComponent<SphereCollider>().enabled = false;

            for (int i =0; i< mr_P.transform.childCount; i++)
            {
                mr_P.transform.GetChild(i).GetComponent<MeshRenderer>().enabled = false;
            }
        }
        else if (!isServer)
        {
            for (int i = 0; i < mr_P.transform.childCount; i++)
            {
                mr_P.transform.GetChild(i).GetComponent<MeshRenderer>().enabled = false;
            }
            
            mr_OP.SetActive(true);
        }         
    }

    public void SetPilotMeshActive()
    {
        if(isServer)
        {
            mr_P.GetComponent<SphereCollider>().enabled = true;
            for (int i = 0; i < mr_P.transform.childCount; i++)
            {
                mr_P.transform.GetChild(i).GetComponent<MeshRenderer>().enabled = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isServer)
            RpcSendVt3Position(gameObject, guide.position);
    }

    /// <summary>
    ///Vector3 Transform => change la position d'un objet dans un espace 3D
    /// </summary>
    [ClientRpc]
    public void RpcSendVt3Position(GameObject Target, Vector3 vt3_Position)
    {
        if (!isServer)
            Target.transform.position = new Vector3(vt3_Position.x, 50, vt3_Position.z);
    }

    [ClientRpc]
    public void RpcSendIntCurLife(GameObject Target, int curLife)
    {
        if (!isServer) 
            Target.transform.GetChild(1).GetComponent<SC_KoaSettingsOP>().SetKoaLife(curLife);
    }
    
    [ClientRpc]
    public void RpcSendIntBehaviorIndex(GameObject Target, int boidSettingsIndex)
    {
        if (!isServer) 
            Target.transform.GetChild(1).GetComponent<SC_KoaSettingsOP>().SetBoidSettings(boidSettingsIndex);
    }

    [ClientRpc]
    public void RpcSendIntCurState(GameObject Target, int curState)
    {
        if (!isServer)
            Target.transform.GetChild(1).GetComponent<SC_KoaSettingsOP>().SetKoaState(curState);
    }

    [ClientRpc]
    public void RpcSendStartInfo(GameObject Target, Vector3 vt3_Sensibility, int timeBeforeSpawn,string KoaID,int curLife, int maxLife,int type)
    {
        this.KoaID = KoaID;
        if (!isServer)
        {
            SC_KoaSettingsOP sc_KoaSettings = Target.transform.GetChild(1).GetComponent<SC_KoaSettingsOP>();
            sc_KoaSettings.SetSensibility(vt3_Sensibility);
            sc_KoaSettings.SetTimeBeforeSpawn(timeBeforeSpawn);
            sc_KoaSettings.SetKoaID(KoaID);
            sc_KoaSettings.SetKoaLife(curLife);
            sc_KoaSettings.SetKoamaxLife(maxLife);
            sc_KoaSettings.SetKoaType(type);
            

        }
    }

    [ClientRpc]
    public void RpcSendAnimationBool(GameObject Target,bool deploy, bool flight, bool bullet, bool laser, float speedFactor, bool chargeLaser)
    {
        SC_KoaSettingsOP sc_KoaSettings = Target.transform.GetChild(1).GetComponent<SC_KoaSettingsOP>();
        sc_KoaSettings.SetBoolAnimation(deploy, flight, bullet, laser, speedFactor, chargeLaser);
    }

    public void InitOPKoaSettings(Vector3 sensibility, int timeBeforeSpawn, string KoaID,int curLife, int maxLife, int type, Transform guide)
    {
        if (isServer)
        {
            RpcSendStartInfo(gameObject, sensibility, timeBeforeSpawn, KoaID, curLife, maxLife, type);
            this.guide = guide;
        }
    }

    public void SetCurLife(int curLife)
    {
        RpcSendIntCurLife(gameObject, curLife);
    }

    public void SetCurState(int curState)
    {
        RpcSendIntCurState(gameObject, curState);
    }

    public void SetNewBehavior(int boidSettingsIndex)
    {
        RpcSendIntBehaviorIndex(gameObject, boidSettingsIndex);
    }


    public void SetAnimationBool(bool deploy,bool flight, bool bullet,bool laser,float speedFactor, bool chargeLaser )
    {

        RpcSendAnimationBool(gameObject ,deploy, flight, bullet, laser, speedFactor, chargeLaser);
    }
}