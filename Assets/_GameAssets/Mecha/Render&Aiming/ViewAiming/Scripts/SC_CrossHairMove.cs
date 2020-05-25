using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_CrossHairMove : MonoBehaviour
{

    [Header("References")]
    [SerializeField]
    bool b_RefsGet = false;   
    [SerializeField]
    GameObject AimIndicator;
    [SerializeField]
    GameObject ViewIndicator;

    Camera Cam_Mech = null;
    Camera Cam_Cockpit = null;
    bool b_IsVR = false;
    bool b_IsFPS = false;

    Coroutine curSnapCoro;

    [Header("CrossHair Parameters")]
    public float f_CrossHairDist = 5f;

    [Header("Snap Infos")]
    [SerializeField]
    bool b_Snapping = true;
    [SerializeField]
    public bool b_TargetKoa = false;
    [SerializeField]
    bool b_OnKoa = false;

    [Header("Snap Coroutine Infos")]
    [Range(0, 2)]
    public float f_Duration = 1.0f;
    [SerializeField]
    bool b_GoToKoaRun = false; 
    [SerializeField]
    bool b_GoToViewRun = false; 
    [SerializeField]
    AnimationCurve SnapCurve;

    public enum Target { None, View, Koa }
    public Target SnapTarget = Target.None;
    public Target CrossHairTarget = Target.None;
    public Target CoroTarget = Target.None;

    // Start is called before the first frame update
    void Start()
    {
        GetReferences();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (!b_RefsGet)
            GetReferences();

        if (Cam_Cockpit != null && Cam_Mech != null)
            UpdatePos();

    }

    #region GetReferences

    void GetReferences()
    {

        if (!b_IsVR && !b_IsFPS)
            GetDeviceState();
        if (Cam_Cockpit == null && (b_IsFPS || b_IsVR))
            GetCockpitCam();
        if (Cam_Mech == null)
            GetMechCam();
        if (Cam_Cockpit != null && Cam_Mech != null)
            b_RefsGet = true;
    }

    void GetDeviceState()
    {

        if (SC_CheckList.Instance.GetComponent<SC_CheckList>().Mng_Device.GetComponent<SC_DeviceManager>().b_IsVR != null)
            b_IsVR = SC_CheckList.Instance.Mng_Device.GetComponent<SC_DeviceManager>().b_IsVR;

        if (SC_CheckList.Instance.GetComponent<SC_CheckList>().Mng_Device.GetComponent<SC_DeviceManager>().b_IsFPS != null)
            b_IsFPS = SC_CheckList.Instance.Mng_Device.GetComponent<SC_DeviceManager>().b_IsFPS;

    }

    void GetCockpitCam()
    {

        if (b_IsFPS && Cam_Cockpit == null)
            Cam_Cockpit = SC_CheckList.Instance.Cam_FPS;

        if (b_IsVR && Cam_Cockpit == null)
            Cam_Cockpit = SC_CheckList.Instance.Cam_VR;

    }

    void GetMechCam()
    {
        Cam_Mech = SC_CheckList.Instance.Cam_Mecha;
    }

    #endregion

    void UpdatePos()
    {

        Vector3 hitCockpit = Cam_Cockpit.GetComponent<SC_raycast>().getRayVector3();

        if (hitCockpit != null)
        {

            if (b_Snapping && CrossHairTarget == SnapTarget)
            {

                //Snap
                if (SnapTarget == Target.Koa)
                {
                    hitCockpit = AimIndicator.transform.position - Cam_Mech.transform.position;
                    transform.position = Cam_Mech.transform.position + hitCockpit.normalized * f_CrossHairDist;
                }
                //Manual
                else
                {                  
                    hitCockpit = Cam_Mech.transform.rotation * hitCockpit;
                    transform.position = Cam_Mech.transform.position + hitCockpit.normalized * f_CrossHairDist;
                }
            }
            else if (b_Snapping && CrossHairTarget != SnapTarget)
            {
                CheckTarget();
            }
            //DefaultCase = Manual
            else
            {  
                hitCockpit = Cam_Mech.transform.rotation * hitCockpit;
                transform.position = Cam_Mech.transform.position + hitCockpit.normalized * f_CrossHairDist;
            }

            transform.LookAt(Cam_Mech.transform);

        }

    }

    void CheckTarget()
    {
        if(CrossHairTarget != SnapTarget && CoroTarget != SnapTarget)
        {

            //StopAllCoroutines();
            //StartCoroutine(Snapping(SnapTarget));

            if (curSnapCoro != null)
                StopCoroutine(curSnapCoro);

            curSnapCoro = StartCoroutine(Snapping(SnapTarget));

        }
    }

    IEnumerator Snapping(Target SnapTarget)
    {

        CoroTarget = SnapTarget;

        float t = 0.0f;
        float rate = 1.0f / f_Duration;

        while (t < 1.0)
        {

            t += Time.deltaTime * rate;
            float Lerp = SnapCurve.Evaluate(t);

            Vector3 hitCockpit = new Vector3();

            if (SnapTarget == Target.Koa)
                hitCockpit = AimIndicator.transform.position - Cam_Mech.transform.position;
            else
                hitCockpit = ViewIndicator.transform.position - Cam_Mech.transform.position;

            transform.position = Vector3.Lerp(transform.position, Cam_Mech.transform.position + hitCockpit.normalized * f_CrossHairDist, Lerp);

            yield return 0;

        }
    
        CrossHairTarget = SnapTarget;
        CoroTarget = Target.None;

    }

}
