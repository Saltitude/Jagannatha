using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SC_BulletLaserGun : NetworkBehaviour
{

    public Vector3Int sensitivity;
    MeshRenderer mr;
    [SerializeField]
    Material Mat;
    float timer = 0;
    public float frequency;
    [SerializeField]
    float f_Scale_P = 0.2f;
    [SerializeField]
    float f_Scale_OP = 2f;

    void Start()
    {
        mr = this.GetComponent<MeshRenderer>();
    }

    public void DisplayLaser(GameObject helper_startPos, GameObject Target, bool Visible, Color32 targetColor)
    {

        if(Mat.color != targetColor)
            Mat.color = targetColor;

        //Positionne le laser a la base de l'arme (GunPos) et l'oriente dans la direction du point visée par le joueur
        transform.position = Vector3.Lerp(helper_startPos.transform.position, Target.transform.position, .5f);
        transform.LookAt(Target.transform.position);

        if (!mr.enabled && Visible)
            mr.enabled = true;
        else if (mr.enabled && !Visible)
            mr.enabled = false;

        //Scale en Z le laser pour l'agrandir jusqu'a ce qu'il touche le point visée par le joueur (C STYLE TAHU)
        transform.localScale = new Vector3(f_Scale_P, f_Scale_P, Vector3.Distance(helper_startPos.transform.position, Target.transform.position));

        Vector3 ScaleOP = new Vector3(f_Scale_OP, f_Scale_OP, Vector3.Distance(helper_startPos.transform.position, Target.transform.position));

        if (isServer)
            RpcDisplayLaserOP(gameObject, transform.position, transform.rotation, ScaleOP, targetColor);
        
    }

    public void ResetPos()
    {
        transform.position = new Vector3(1000, 1000, 1000);
        transform.localScale = new Vector3(f_Scale_P, f_Scale_P, f_Scale_P);
        mr.enabled = false;
        RpcDisplayLaserOP(gameObject, transform.position, transform.rotation, transform.localScale, Mat.color);
    }

    [ClientRpc]
    public void RpcDisplayLaserOP(GameObject target, Vector3 position, Quaternion rotation, Vector3 scale, Color32 targetColor)
    {
        if (!isServer)
        {

            if (!mr.enabled)
                mr.enabled = false;

            if (Mat.color != targetColor)
                Mat.color = targetColor;

            target.transform.position = position;
            target.transform.rotation = rotation;
            target.transform.localScale = scale;

        }
    }

}
