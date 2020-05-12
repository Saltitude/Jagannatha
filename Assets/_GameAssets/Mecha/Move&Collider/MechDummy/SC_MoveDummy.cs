using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SC_MoveDummy : NetworkBehaviour
{

    private GameObject Target;
    private MeshRenderer mr;
    [SerializeField]
    GameObject Mesh_OP;
    [SerializeField]
    GameObject CannonImg;
    [SerializeField]
    GameObject Cannon;
    GameObject CannonTarget = null;

    // Start is called before the first frame update
    void Start()
    {

        if (!isServer)
            GetReferences();

        SetMesh();

    }

    // Update is called once per frame
    void Update()
    {
        if (isServer)
        {
            SyncPos();
            SyncCannon();
            SendRpc();
        }
    }

    void SetMesh()
    {
        if (isServer)
        {
            Mesh_OP.SetActive(false);
            CannonImg.SetActive(false);
        }
    }

    void GetReferences()
    {
        Target = GameObject.FindGameObjectWithTag("Player");
        if (Target == null)
            Debug.LogWarning("Can't Find Player Tagged Object");

        mr = this.GetComponentInChildren<MeshRenderer>();
        if (mr != null)
            mr.enabled = false;
        else
            Debug.LogWarning("Can't Find MeshRenderer");
    }

    void SyncCannon()
    {

        if (CannonTarget == null)
            CannonTarget = SC_CheckList_Weapons.Instance.AimIndicator;

        else if (CannonTarget != null)
        {
            Cannon.transform.LookAt(CannonTarget.transform);
        }

    }

    void SyncPos()
    {
        if (Target != null)
        {
            transform.position = Target.transform.position;
            //transform.rotation = Target.transform.rotation;
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, Target.transform.eulerAngles.y, transform.eulerAngles.z);
        }
        else
            GetReferences();
    }

    void SendRpc()
    {
        RpcSendVt3Position(gameObject, transform.position);
        RpcSendQtnRotation(gameObject, transform.rotation);
        RpcSendCannonRotation(Cannon.transform.rotation);
    }

    /// <summary>
    ///Vector3 Transform => change la position d'un objet dans un espace 3D
    /// </summary>
    [ClientRpc]
    public void RpcSendVt3Position(GameObject Target, Vector3 vt3_Position)
    {
        if (!isServer)
            Target.transform.position = vt3_Position;
    }

    /// <summary>
    ///Quaternion => change la rotation d'un objet à partir d'un quaternion
    /// </summary>
    [ClientRpc]
    public void RpcSendQtnRotation(GameObject Target, Quaternion qtn_Rotation)
    {
        if (!isServer)
            Target.transform.rotation = qtn_Rotation;
    }

    [ClientRpc]
    public void RpcSendCannonRotation(Quaternion qtn_Rotation)
    {
        if (!isServer)
            Cannon.transform.rotation = qtn_Rotation;
    }

}
