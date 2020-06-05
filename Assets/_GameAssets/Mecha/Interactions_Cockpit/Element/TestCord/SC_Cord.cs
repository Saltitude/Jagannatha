using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SC_Cord : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    GameObject Base;
    [SerializeField]
    MeshRenderer Renderer;
    [SerializeField]
    Material[] tab_Materials;

    [Header("General Parameters")]
    [SerializeField]
    int n_Index = 0;

    [Header("Physical Parameters")]
    [SerializeField, Range(0, 2)]
    float ConstraintRange = 0.7f;
    [SerializeField, Range(0, 0.5f)]
    float DeadZone = 0.15f;
    [SerializeField, Range(0, 0.5f)]
    float AddMaxRange = 0.3f;
    [SerializeField]
    float JointBeakFroce = 10000f;

    [Header("Graph Parameters")]
    [SerializeField, Range(1, 10000f)]
    float f_ColorFactor;

    [Header("Infos")]
    [SerializeField]
    bool b_InEditor = false;
    [SerializeField]
    float f_CurDistance;
    [SerializeField]
    bool b_InRange;
    [SerializeField]
    bool b_Enable = false;
    [SerializeField]
    bool b_Grabbing = false;

    //controller
    private ViveGrip_ControllerHandler controller;

    //Vibrations
    [SerializeField]
    private int _vibrationMilliSec = 10;
    [SerializeField]
    private float _vibrationStrength = 10f;


    //Non Public Refs
    Rigidbody Rb;
    FixedJoint CurJoint;
    ViveGrip_GripPoint RightHandGripPoint;
    ViveGrip_ControllerHandler RightHandController;

    // Start is called before the first frame update
    void Start()
    {

        Rb = this.GetComponent<Rigidbody>();
        SetMaterial(false);

        /*
        #if UNITY_EDITOR
            b_InEditor = true;
        #endif
        */

        SyncParam();

    }

    // Update is called once per frame
    void Update()
    {

        CalculateDistance();

        ObjectStatus();

        RangeEffect();

        SyncCord();

    }


    //Mise à jour controller

    void ViveGripGrabStart(ViveGrip_GripPoint gripPoint)
    {
        controller = gripPoint.controller;
    }

    //appelé quand lache l'objet
    void ViveGripGrabStop(ViveGrip_GripPoint gripPoint)
    {
        controller = null;
    }



    void CalculateDistance()
    {
        Vector3 Distance = Base.transform.position - this.transform.position;
        f_CurDistance = Distance.magnitude;
    }

    void ObjectStatus()
    {

        //a commenté possiblement si build fonctionne pas.
        /*
        if (b_ineditor)
        {

            if (unityeditor.selection.activeobject == this.gameobject && !rb.iskinematic)
                rb.iskinematic = true;

            else if (unityeditor.selection.activeobject != this.gameobject && rb.iskinematic)
                rb.iskinematic = false;

        }
        */

        /*
        if (Rb.isKinematic && !b_InEditor)
            Rb.isKinematic = false;
        */

    }

    void RangeEffect()
    {

        if (f_CurDistance < ConstraintRange && !b_InRange)
        {
            b_InRange = true;
            SetMaterial(false);

            //vibration constante en focntion de la distance
            
        }
        if (f_CurDistance > ConstraintRange)
        {
            if (controller != null)
                controller.Vibrate(_vibrationMilliSec, _vibrationStrength * Mathf.Clamp((f_CurDistance - ConstraintRange)/12, 0f, 1));
        }

        if (f_CurDistance > ConstraintRange + DeadZone && b_InRange)
        {
            b_Enable = !b_Enable;
            b_InRange = false;
            SetMaterial(true);
            SC_MovementBreakdown.Instance.AddToPilotSeq(n_Index);

            //cran de vibration
            if (controller != null)
                controller.Vibrate(_vibrationMilliSec*10, _vibrationStrength *10);
        }

        if (f_CurDistance > ConstraintRange + AddMaxRange)
        {
            if (controller != null)
                controller.Vibrate(_vibrationMilliSec * 5, _vibrationStrength * 100);

            ReleaseObject();

        }

    }

    void ReleaseObject()
    {
        /*
        #if UNITY_EDITOR
            UnityEditor.Selection.SetActiveObjectWithContext(null, null);
        #endif
        */
    }
    
    void SetMaterial(bool State)
    {

        //ici j'ai un peu bidouillé pour que ca change l'emissive à la place de changer le mat, du coup ton tableau de mat sert plus à rien ;}

        if (!State)
            Renderer.material.SetColor("_EmissionColor", (Color.grey));

        if (State)
            Renderer.material.SetColor("_EmissionColor", (Color.white * f_ColorFactor));   

    }

    public void HandKinematic(bool state)
    {

        //Debug.Log("HandKinematic - " + state);

        Rb.isKinematic = state;
        b_Grabbing = state;

    }

    public void CreateFixedJoint()
    {

        //Debug.Log("CreateFixedJoint");

        SC_SyncVar_MovementSystem.Instance.CurSelectedCord = n_Index;

        GameObject RightHand = SC_GetRightController.Instance.getGameObject();

        CurJoint = AddFixedJoint(RightHand);
        CurJoint.connectedBody = Rb;

    }

    public void DeleteFixedJoint()
    {
        if (CurJoint != null)
        {

            //Debug.Log("DeleteFixedJoint");

            SC_SyncVar_MovementSystem.Instance.CurSelectedCord = 0;

            CurJoint.connectedBody = null;
            Destroy(CurJoint);

        }
    }

    private FixedJoint AddFixedJoint(GameObject Target)
    {

        //Debug.Log("AddFixedJoint");

        FixedJoint fx = Target.AddComponent<FixedJoint>();
        //fx.breakForce = JointBeakFroce;
        //fx.breakTorque = JointBeakFroce;
        return fx;

    }

    void SyncParam()
    {
        SC_SyncVar_MovementSystem.Instance.ConstraintRange = ConstraintRange;
        SC_SyncVar_MovementSystem.Instance.DeadZone = DeadZone;
        SC_SyncVar_MovementSystem.Instance.AddMaxRange = AddMaxRange;
    }

    void SyncCord()
    {

        switch (n_Index)
        {

            case 1:
                SC_SyncVar_MovementSystem.Instance.CordLenght01 = f_CurDistance;
                break;

            case 2:
                SC_SyncVar_MovementSystem.Instance.CordLenght02 = f_CurDistance;
                break;

            case 3:
                SC_SyncVar_MovementSystem.Instance.CordLenght03 = f_CurDistance;
                break;

        }

    }

}

