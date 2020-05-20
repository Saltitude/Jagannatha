using Boo.Lang;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script sur la camera de cockpit pour tirer un ray depuis le casque | 
/// Script by Cycy Mod by Leni | 
/// </summary>

public class SC_raycast : MonoBehaviour
{

    [Header("Get References")]
    [SerializeField]
    GameObject Target;

    [Header("Infos")]
    [SerializeField]
    private RaycastHit hit;
    [SerializeField]
    Vector3 VT3_CockpitAimRaycast;
    [SerializeField]
    Vector3 VT3_HitPos;

    private int layerMask;


    private void Start()
    {
        layerMask = 1 << 0;
    }

    void Update()
    {

        if (Target == null)
            GetTarget();

        if (Target != null)
            CastRayInCockpit();

    }

    void GetTarget()
    {
        Target = SC_CheckList_ViewAiming .Instance.Target;
    }

    void CastRayInCockpit()
    {

        //Cast un ray à partir du casque
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 100f, layerMask))
        {

            if (hit.collider == Target.GetComponent<Collider>())
            {
                VT3_CockpitAimRaycast = hit.point - Target.transform.position;
                VT3_HitPos = hit.point;
            }

        }

    }

    /// <summary>
    /// Renvois le hit du raycast cockpit
    /// </summary>
    /// <returns></returns>
    public RaycastHit getRay()
    {
        return hit;
    }

    /// <summary>
    /// Renvois le Vt3 du raycast cockpit
    /// </summary>
    /// <returns></returns>
    public Vector3 getRayVector3()
    {
        return VT3_CockpitAimRaycast;
    }

}
