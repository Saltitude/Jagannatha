/*
using UnityEngine;


public class ViveGripExample_Dial : MonoBehaviour, IInteractible
{
    public Transform attachedLight;
    private HingeJoint joint;

    private GameObject Mng_SyncVar;
    private SC_SyncVar_BreakdownDisplay sc_syncvar;
    public GameObject LocalBreakdownMng;

    [SerializeField]
    button bouton;

    private float oldAngle = 0;

    private float oldAngleForSound = 0;

    public float desiredValue = 0;

    public bool isEnPanne = false;
    public float precision = 1f;


    private CustomSoundManager sc_audio_mng;

    private ViveGrip_ControllerHandler controller;

    enum button
    {
        potar1,
        potar2,
        potar3

    }

    void ViveGripGrabStart(ViveGrip_GripPoint gripPoint)
    {
        controller = gripPoint.controller;
    }

    void ViveGripGrabStop()
    {
        controller = null;
    }

    void Start()
    {
        joint = GetComponent<HingeJoint>();


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

    void Update()
    {

       // Debug.Log(joint.angle);

        if (joint.angle != oldAngle)
        {
            sendToSynchVar(joint.angle);
        }

        oldAngle = joint.angle;

        /*float g = (joint.angle + 90) / 180;
        float r = 1 - g;
        Color newColor = new Color(r, g, 0);
            if(attachedLight != null)
            {
                attachedLight.gameObject.GetComponent<Renderer>().material.color = newColor;
                attachedLight.GetChild(0).GetComponent<Light>().color = newColor;

            }

      }
      


        playSound();
        IsValueOk();
    }


    void sendToSynchVar(float value)
    {
        if (sc_syncvar == null)
        {
            GetReferences();
        }
        else
        {
            switch (bouton)
            {
                case button.potar1:
                    sc_syncvar.potar1value = Mathf.Round(value*100f)/100f ;
                    break;
                case button.potar2:
                    sc_syncvar.potar2value = Mathf.Round(value * 100f) / 100f;
                    break;
                case button.potar3:
                    sc_syncvar.potar3value = Mathf.Round(value * 100f) / 100f;
                    break;
                default:
                    break;
            }
        }
    }


    public void ChangeDesired()
    {
        desiredValue = Random.Range(-70f, 70f);
        //Debug.Log("DESIRED VALUE : " + desiredValue);
        while (gameObject.transform.localPosition.y >= desiredValue - precision && gameObject.transform.localPosition.y <= desiredValue + precision)
        {
            desiredValue = Random.Range(-70f,70f);
            
        }
        SetIsEnPanne(true);

        switch (bouton)
        {
            case button.potar1:
                sc_syncvar.potar1valueWanted = desiredValue;
                sc_syncvar.potar1isEnPanne = true;
                break;
            case button.potar2:
                sc_syncvar.potar2valueWanted = desiredValue;
                sc_syncvar.potar2isEnPanne = true;
                break;
            case button.potar3:
                sc_syncvar.potar3valueWanted = desiredValue;
                sc_syncvar.potar3isEnPanne = true;
                break;
            default:
                break;

        }
        

    }

    public bool isBreakdown()
    {
        return isEnPanne;
    }

    public void IsValueOk()
    {

        if (joint.angle >= desiredValue - precision && joint.angle <= desiredValue + precision)
        {
            if (isEnPanne)
            {
                

                if (sc_syncvar != null)
                {

                    SetIsEnPanne(false);
                    switch (bouton)
                    {
                        case button.potar1:
                            sc_syncvar.potar1isEnPanne = false;
                            break;
                        case button.potar2:
                            sc_syncvar.potar2isEnPanne = false;
                            break;
                        case button.potar3:
                            sc_syncvar.potar3isEnPanne = false;
                            break;
                        default:
                            break;
                    }
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
                    switch (bouton)
                    {
                        case button.potar1:
                            sc_syncvar.potar1isEnPanne = true;
                            break;
                        case button.potar2:
                            sc_syncvar.potar2isEnPanne = true;
                            break;
                        case button.potar3:
                            sc_syncvar.potar3isEnPanne = true;
                            break;
                        default:
                            break;
                    }
                }
                else
                    GetReferences();
            }
            

        }

    }

    void SetIsEnPanne(bool value)
    {
        if(LocalBreakdownMng == null)
            GetReferences();
        isEnPanne = value;
        LocalBreakdownMng.GetComponent<IF_BreakdownManager>().CheckBreakdown();
    }

    void playSound()
    {
        


        switch (bouton)
        {
            case button.potar1:
                if (oldAngleForSound > joint.angle && Mathf.Abs(oldAngleForSound - joint.angle) >= 10)
                {

                    CustomSoundManager.Instance.PlaySound(gameObject, "SFX_p_potentiometer_1", false, 1, 0.1f, 0f);
                    oldAngleForSound = joint.angle;

                    if (controller != null)
                        controller.Vibrate(50, 0.2f);

                }
                else if (oldAngleForSound < joint.angle && Mathf.Abs(joint.angle - oldAngleForSound) >= 10)
                {

                    CustomSoundManager.Instance.PlaySound(gameObject, "SFX_p_potentiometer_2", false, 1, 0.1f, 0.4f);
                    oldAngleForSound = joint.angle;

                    if (controller != null)
                        controller.Vibrate(50, 0.2f);
                }
                break;
            
            default:

                if (oldAngleForSound > joint.angle && Mathf.Abs(oldAngleForSound - joint.angle) >= 3)
                {

                    CustomSoundManager.Instance.PlaySound(gameObject, "SFX_p_potentiometer_1", false, 0.3f, 0.1f, 0.8f);
                    oldAngleForSound = joint.angle;

                    if (controller != null)
                        controller.Vibrate(30, 0.1f);
                }
                else if (oldAngleForSound < joint.angle && Mathf.Abs(joint.angle - oldAngleForSound) >= 1)
                {

                    CustomSoundManager.Instance.PlaySound(gameObject, "SFX_p_potentiometer_2", false, 0.3f, 0.1f, 0.6f);
                    oldAngleForSound = joint.angle;

                    if (controller != null)
                        controller.Vibrate(30, 0.1f);
                }

                break;

        }


        



    }

    public void ForceSync()
    {

    }

}
*/
