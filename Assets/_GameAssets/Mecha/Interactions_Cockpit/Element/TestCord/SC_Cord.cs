using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_Cord : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    GameObject Base;
    [SerializeField]
    MeshRenderer Renderer;
    [SerializeField]
    Material[] tab_Materials;

    [Header("Parameters")]
    [SerializeField]
    int n_Index = 0;
    [SerializeField, Range(0, 1)]
    float ConstraintRange = 0.7f;
    [SerializeField, Range(0, 0.5f)]
    float DeadZone = 0.15f;
    [SerializeField, Range(0, 0.5f)]
    float AddMaxRange = 0.3f;
    [SerializeField]
    float JointBeakFroce;

    [Header("Infos")]
    [SerializeField]
    float f_CurDistance;
    [SerializeField]
    bool b_InRange;
    [SerializeField]
    bool b_Enable = false;
    [SerializeField]
    bool b_Grabbing = false;

    //Non Public Refs
    Rigidbody Rb;
    FixedJoint CurJoint;
    ViveGrip_GripPoint RightHandGripPoint;
    ViveGrip_ControllerHandler RightHandController;

    // Start is called before the first frame update
    void Start()
    {
        Rb = this.GetComponent<Rigidbody>();
        SetMaterial();
    }

    // Update is called once per frame
    void Update()
    {

        CalculateDistance();

        ObjectStatus();

        RangeEffect();

    }

    void CalculateDistance()
    {
        Vector3 Distance = Base.transform.position - this.transform.position;
        f_CurDistance = Distance.magnitude;
    }

    void ObjectStatus()
    {

        #if UNITY_EDITOR

        if (UnityEditor.Selection.activeObject == this.gameObject && !Rb.isKinematic)
            Rb.isKinematic = true;

        else if (UnityEditor.Selection.activeObject != this.gameObject && Rb.isKinematic)
            Rb.isKinematic = false;

        #endif

    }

    void RangeEffect()
    {

        if (f_CurDistance < ConstraintRange && !b_InRange)
        {
            b_InRange = true;
            SetMaterial(false);
        }        

        if (f_CurDistance > ConstraintRange + DeadZone && b_InRange)
        {
            b_Enable = !b_Enable;
            b_InRange = false;
            SetMaterial(true);
            SC_MovementBreakdown.Instance.AddToPilotSeq(n_Index);
        }

        if (f_CurDistance > ConstraintRange + AddMaxRange)
            ReleaseObject();

    }

    void ReleaseObject()
    {

        #if UNITY_EDITOR

        UnityEditor.Selection.SetActiveObjectWithContext(null, null);

        #endif

    }
    
    void SetMaterial(bool State)
    {
        if (!State)
            Renderer.material = tab_Materials[0];
        if (State)
            Renderer.material = tab_Materials[1];
    }

    public void HandKinematic(bool state)
    {
        Debug.Log("HandKinematic - " + state);
        Rb.isKinematic = state;
    }

    public void CreateFixedJoint()
    {

        GameObject RightHand = SC_GetRightController.Instance.getGameObject();

        CurJoint = AddFixedJoint(RightHand);
        CurJoint.connectedBody = Rb;

    }

    public void DeleteFixedJoint()
    {
        if (CurJoint != null)
        {
            CurJoint.connectedBody = null;
            Destroy(CurJoint);
        }
    }

    private FixedJoint AddFixedJoint(GameObject Target)
    {
        FixedJoint fx = Target.AddComponent<FixedJoint>();
        fx.breakForce = JointBeakFroce;
        fx.breakTorque = JointBeakFroce;
        return fx;
    }

}

