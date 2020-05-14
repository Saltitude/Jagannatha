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


    [SerializeField]
    GameObject guideC;
    Vector3 guideCannon = Vector3.zero;
    [SerializeField]
    GameObject guideL;
    Vector3 guideLaser = Vector3.zero;
    [SerializeField]
    GameObject meshLaser;
    Color32 newColor;
    private MeshRenderer mshRend;
    private Material mat_Laser;
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
            guideC.SetActive(false);
        }
    }

    void GetReferences()
    {
        Target = GameObject.FindGameObjectWithTag("Player");
        if (Target == null)
            Debug.LogWarning("Can't Find Player Tagged Object");

        mshRend = meshLaser.GetComponent<MeshRenderer>();
        mat_Laser = meshLaser.GetComponent<Material>();

        mr = this.GetComponentInChildren<MeshRenderer>();
        if (mr != null)
        {
            //mr.enabled = false;
        }

        else
            Debug.LogWarning("Can't Find MeshRenderer");
    }

    void SyncCannon()
    {

        if (CannonTarget == null)
            CannonTarget = SC_CheckList_Weapons.Instance.AimIndicator;

        else if (CannonTarget != null)
        {
            //Cannon.transform.LookAt(CannonTarget.transform);
            guideCannon = CannonTarget.transform.position;
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
        RpcSendCannonGuide(guideCannon, SC_WeaponLaserGun.Instance.trig, SC_WeaponBreakdown.Instance.bCanFire, SC_WeaponLaserGun.Instance.CurColor);
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

    [ClientRpc]
    public void RpcSendCannonGuide(Vector3 vt3_Position, bool isTrig, bool bCanFire, Color32 targetColor)
    {
        if (!isServer)
        {
            Vector3 curTempPosC = new Vector3(vt3_Position.x, guideC.transform.position.y, vt3_Position.z);
            guideC.transform.position = curTempPosC;
            Vector3 curTempPosL = new Vector3(vt3_Position.x, guideL.transform.position.y, vt3_Position.z);
            guideL.transform.position = curTempPosL;

            if (isTrig && bCanFire)
            {
                mshRend.enabled = true;
                meshLaser.transform.localScale = new Vector3(meshLaser.transform.localScale.x, meshLaser.transform.localScale.y, curTempPosL.magnitude);
                meshLaser.transform.position = new Vector3(vt3_Position.x/2, meshLaser.transform.position.y, vt3_Position.z/2);
                mat_Laser.color = targetColor;
            }
            else
            {
                mshRend.enabled = false;
            }

        }
            
    }

}
