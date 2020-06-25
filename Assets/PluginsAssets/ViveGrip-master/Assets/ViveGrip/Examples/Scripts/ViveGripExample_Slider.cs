using UnityEngine;
using TMPro;
using System.Collections;

public class ViveGripExample_Slider : MonoBehaviour, IInteractible {
  private ViveGrip_ControllerHandler controller;
  private float oldX;
  private int VIBRATION_DURATION_IN_MILLISECONDS = 50;
  private float MAX_VIBRATION_STRENGTH = 0.2f;
  private float MAX_VIBRATION_DISTANCE = 0.03f;



    private float _localX = 0;
    private float _localY = 0;
    private float _localZ = 0;
    public bool _freezeAlongX = true;
    public bool _freezeAlongY = false;
    public bool _freezeAlongZ = true;

    public float limit = 6;

    /// <summary>
    /// Index du slider pour sa structList
    /// </summary>
    /// 


    public int index;
    public int curValue;

    public float desiredValue = 0;
    public bool isEnPanne = false;
    public float precision = 0;

    private GameObject Mng_SyncVar;
    private Rigidbody sliderRigidbody;
    public GameObject LocalBreakdownMng;

    private SC_SyncVar_BreakdownDisplay sc_syncvar;

    [SerializeField]
    public bool isFLUX = false;

    //SoundDesign
    GameObject SFX_Flux;
    int SoundSourceNumb = 0;

    [Range(0, 1)]
    public float probability = 1;

    public float precisionPercent = 10;

    public TMP_Text text_value_display;
    /*
    [SerializeField]
    button bouton;

    
     enum button
     {
        slider1,
        slider2,
        slider3

     }
     */
     void Awake()
    {
        sc_syncvar = SC_SyncVar_BreakdownDisplay.Instance;

    }

    void Start ()
    {
        oldX = transform.position.x;
        sliderRigidbody = gameObject.GetComponent<Rigidbody>();
        GetReferences();
        precision = (limit *0.45f* 2 / 100) * precisionPercent;
    }

    void GetReferences()
    {
        if (LocalBreakdownMng == null)
            LocalBreakdownMng = this.transform.parent.parent.gameObject;
        if (Mng_SyncVar == null)
            Mng_SyncVar = GameObject.FindGameObjectWithTag("Mng_SyncVar");
        if (Mng_SyncVar != null && sc_syncvar == null)
            sc_syncvar = SC_SyncVar_BreakdownDisplay.Instance;
    }

    void ViveGripGrabStart(ViveGrip_GripPoint gripPoint)
    {
    controller = gripPoint.controller;
        sliderRigidbody.isKinematic = false;
    }

    void ViveGripGrabStop()
    {
        controller = null;
        sliderRigidbody.isKinematic = true;
    }

	void Update () {
        
        //on traduit la position en position locale pour la freeze
        _localX = transform.localPosition.x;
        _localY = transform.localPosition.y;
        _localZ = transform.localPosition.z;

        if (_freezeAlongX) _localX = 0;
        if (_freezeAlongY) _localY = 0;
        if (_freezeAlongZ) _localZ = 0;
        gameObject.transform.localPosition = new Vector3(_localX, _localY, _localZ);

        //sécurité juste en y

        if (transform.localPosition.x<-limit)
        {

            transform.localPosition = new Vector3(-limit, transform.localPosition.y, transform.localPosition.z);

        }
        else if (transform.localPosition.x > limit)
        {
            transform.localPosition = new Vector3(limit, transform.localPosition.y,  transform.localPosition.z);

        }

        float newX = gameObject.transform.localPosition.x;
        //if (newX != oldX && SoundSourceNumb == 0)
        //{
        //    SFX_Flux = CustomSoundManager.Instance.PlaySound(gameObject, "SFX_p_ClicFlux", false, 0.3f, true, 0.05f);
        //    SoundSourceNumb += 1;
        //}
        //Debug.Log(newX + " = " + oldX);
        //if (newX == oldX)
        //    SoundSourceNumb = 0;
        //on envoie la valeur à la syncvar si celle ci a changé
        if (newX != oldX) sendToSynchVar(-Mathf.Round(Ratio(gameObject.transform.localPosition.x,limit,0.45f,-limit,-0.45f)*100)/100);
        //if (newX != oldX) sendToSynchVar(-Mathf.FloorToInt(Ratio(gameObject.transform.localPosition.x, limit, 0.45f, -limit, -0.45f)));
        //Debug.Log(gameObject.transform.localPosition.x);
        if (text_value_display != null)
        {
            //curValue = Mathf.FloorToInt(Ratio(-Mathf.Round(Ratio(gameObject.transform.localPosition.x, limit, 0.45f, -limit, -0.45f) * 100) / 100, 0.4f, 10, -0.4f, 0.1f));
            curValue = Mathf.FloorToInt(Ratio(sc_syncvar.SL_sliders[index].value, 0.45f, 10, -0.45f, 1));
            
            text_value_display.text = curValue.ToString();

        }

        if (controller != null) {
          float distance = Mathf.Min(Mathf.Abs(newX - oldX), MAX_VIBRATION_DISTANCE);
          float vibrationStrength = (distance / MAX_VIBRATION_DISTANCE) * MAX_VIBRATION_STRENGTH;
          controller.Vibrate(VIBRATION_DURATION_IN_MILLISECONDS, vibrationStrength);
        }
        oldX = newX;

        

        IsValueOk();

    }


    public bool isBreakdown()
    {
        return isEnPanne;
    }

    void sendToSynchVar (float value)
    {

        if (sc_syncvar == null)
        {
            GetReferences();
        }
        else
        {

            sc_syncvar.SliderChangeValue(index, value);

        }          

    }


    public void ChangeDesired()
    {

        desiredValue = Random.Range(-limit, limit);
        while (gameObject.transform.localPosition.x >= desiredValue - (precision+precision/3) && gameObject.transform.localPosition.x <= desiredValue + (precision + precision / 3))
        {
            desiredValue = Random.Range(-limit, limit);
        }

        SetIsEnPanne(true);

        sc_syncvar.SliderChangeValueWanted(index, -Mathf.Round(Ratio(desiredValue, limit, 0.45f, -limit, -0.45f) * 100) / 100);
        sc_syncvar.SliderChangeIsPanne(index, true);

        Debug.Log("Slider Index" + index);
        Debug.Log("Value : " + curValue);
        Debug.Log("ValueWanted : " + desiredValue);
        

    }

    public void Repair()
    {

        desiredValue = gameObject.transform.localPosition.x;

        if (isFLUX)
            desiredValue = sc_syncvar.SL_sliders[index].value;

        SetIsEnPanne(false);

        sc_syncvar.SliderChangeValueWanted(index, desiredValue);
        sc_syncvar.SliderChangeIsPanne(index, false);

    }


    public void IsValueOk()
    {
        if (isFLUX && Mathf.FloorToInt(Ratio(sc_syncvar.SL_sliders[index].value, 0.45f, 10, -0.45f, 1)) == Mathf.FloorToInt(Ratio(sc_syncvar.SL_sliders[index].valueWanted, 0.45f, 10, -0.45f, 1)))
        {
            if (isEnPanne)
            {
                SetIsEnPanne(false);



                if (sc_syncvar == null)
                {

                    GetReferences();
                }
                else
                {

                    sc_syncvar.SliderChangeIsPanne(index, false);

                }
            }



        }
        else if (!isFLUX && gameObject.transform.localPosition.x >= desiredValue - precision && gameObject.transform.localPosition.x <= desiredValue + precision)
        {

            if (isEnPanne)
            {
                SetIsEnPanne(false);



                if (sc_syncvar == null)
                {

                    GetReferences();
                }
                else
                {

                    sc_syncvar.SliderChangeIsPanne(index, false);

                }
            }

            
        }
        else
        {

            if (!isEnPanne)
            {
                SetIsEnPanne(true);

                if (sc_syncvar == null)
                {

                    GetReferences();
                }
                else
                {

                    sc_syncvar.SliderChangeIsPanne(index, true);

                }
            }

            
        }

    }

    void SetIsEnPanne(bool value)
    {
        isEnPanne = value;
        LocalBreakdownMng.GetComponent<IF_BreakdownManager>().CheckBreakdown();
    }

    public bool testAgainstOdds()
    {
        float rand = Random.Range(0f, 1f);

        if (rand < probability)
            return true;
        else
            return false;


    }

    float Ratio(float inputValue, float inputMax, float outputMax, float inputMin = 0.0f, float outputMin = 0.0f)
    {
        float product = (inputValue - inputMin) / (inputMax - inputMin);
        float output = ((outputMax - outputMin) * product) + outputMin;
        return output;
    }

    public void ForceSync()
    {
        sendToSynchVar(-Mathf.Round(Ratio(gameObject.transform.localPosition.x, limit, 0.45f, -limit, -0.45f) * 100) / 100);
    }



}
