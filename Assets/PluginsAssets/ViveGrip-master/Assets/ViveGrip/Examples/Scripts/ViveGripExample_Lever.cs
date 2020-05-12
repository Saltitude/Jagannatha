using UnityEngine;
using System.Collections;

public class ViveGripExample_Lever : MonoBehaviour, IInteractible
{
    private ViveGrip_ControllerHandler controller;
    private float oldXRotation;
    private int VIBRATION_DURATION_IN_MILLISECONDS = 50;
    private float MAX_VIBRATION_STRENGTH = 0.7f;
    private float MAX_VIBRATION_ANGLE = 35f;

    private Rigidbody leverRigidBody;

    public GameObject TopCollider;
    public GameObject BotCollider;
    private BoxCollider TopBox;
    private BoxCollider BotBox;


    public bool isEnPanne = false;

    private bool curState = false;

    private bool desiredValue = false;


    private GameObject Mng_SyncVar;
    private SC_SyncVar_BreakdownDisplay sc_syncvar;
    public GameObject LocalBreakdownMng;

    public int index;


    void Start()
    {
        oldXRotation = transform.eulerAngles.x;

        leverRigidBody = gameObject.GetComponent<Rigidbody>();
        TopBox = TopCollider.GetComponent<BoxCollider>();
        BotBox = BotCollider.GetComponent<BoxCollider>();

        GetReferences();
    }

    void GetReferences()
    {
        if (LocalBreakdownMng == null)
            LocalBreakdownMng = this.transform.parent.parent.gameObject;
        if (Mng_SyncVar == null)
            Mng_SyncVar = GameObject.FindGameObjectWithTag("Mng_SyncVar");
        if (Mng_SyncVar != null && sc_syncvar == null)
            sc_syncvar = Mng_SyncVar.GetComponent<SC_SyncVar_BreakdownDisplay>();


    }

    void ViveGripGrabStart(ViveGrip_GripPoint gripPoint)
    {
        controller = gripPoint.controller;
        leverRigidBody.isKinematic = false;
    }

    void ViveGripGrabStop()
    {
        controller = null;
        leverRigidBody.isKinematic = true;
    }


    void Update()
    {
        float newXRotation = transform.eulerAngles.x;
        if (controller != null)
        {
            float distance = Mathf.Min(Mathf.Abs(newXRotation - oldXRotation), MAX_VIBRATION_ANGLE);
            float vibrationStrength = (distance / MAX_VIBRATION_ANGLE) * MAX_VIBRATION_STRENGTH;
            controller.Vibrate(VIBRATION_DURATION_IN_MILLISECONDS, vibrationStrength);
        }
        oldXRotation = newXRotation;

        IsValueOk();

    }

    public bool isBreakdown()
    {
        return isEnPanne;
    }


    public void ChangeDesired()
    {

        desiredValue = !curState;

        SetIsEnPanne(true);


        sc_syncvar.SwitchChangeValueWanted(index, desiredValue);
        sc_syncvar.SwitchChangeIsPanne(index, true);


    }

    public void Repair()
    {

        desiredValue = curState;

        SetIsEnPanne(false);


        sc_syncvar.SwitchChangeValueWanted(index, desiredValue);
        sc_syncvar.SwitchChangeIsPanne(index, false);


    }

    void sendToSynchVar(bool value)
    {

        if (sc_syncvar != null)
        {
            sc_syncvar.SwitchChangeValue(index, value);
        }
        else
            GetReferences();

    }


    public void IsValueOk()
    {
        if (desiredValue == curState)
        {
            if (isEnPanne)
            {
                if (sc_syncvar != null)
                {
                    SetIsEnPanne(false);
                    sc_syncvar.SwitchChangeIsPanne(index, false);
                }
                else
                    GetReferences();
            }
        }
        else
        {
            if (!isEnPanne)
            {
                if (sc_syncvar != null)
                {
                    SetIsEnPanne(true);
                    sc_syncvar.SwitchChangeIsPanne(index, true);
                }
                else
                    GetReferences();
            }
        }
    }

    void SetIsEnPanne(bool value)
    {

        isEnPanne = value;
        LocalBreakdownMng.GetComponent<IF_BreakdownManager>().CheckBreakdown();

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == TopBox)
        {
            curState = false;
            sendToSynchVar(curState);

        }
        if (other == BotBox)
        {
            curState = true;
            sendToSynchVar(curState);

        }
    }

    public bool testAgainstOdds()
    {
        return true;
    }

    public void ForceSync()
    {
        sendToSynchVar(curState);
    }

}
