using UnityEngine;
using System.Collections;
using TMPro;

public class SC_slider_calibr : MonoBehaviour
{

    [Header("Slider Parameters")]
    /// <summary>
    /// Index du slider pour sa structList
    /// </summary>
    [SerializeField]
    int index;

    [Header("References")]
    private ViveGrip_ControllerHandler controller;
    private GameObject Mng_SyncVar;
    private Rigidbody sliderRigidbody;
    private SC_SyncVar_calibr sc_syncvar_calibr;
    [SerializeField]
    Collider LeftLimit;
    [SerializeField]
    Collider RightLimit;

    [Header("Vibration Parameters")]
    [SerializeField, Range(0,500)]
    int VIBRATION_DURATION_IN_MILLISECONDS = 50;
    [SerializeField, Range(0, 1)]
    float MAX_VIBRATION_STRENGTH = 0.2f;
    [SerializeField, Range(0, 0.1f)]
    float MAX_VIBRATION_DISTANCE = 0.03f;

    #region Slide Variables

    public bool _freezeAlongX = true;
    public bool _freezeAlongY = false;
    public bool _freezeAlongZ = true;

    private float _localX = 0;
    private float _localY = 0;
    private float _localZ = 0;

    private Vector3 SliderPos;
    [SerializeField]
    private float SliderLenght;
    [SerializeField]
    private float SliderCurPos;
    [SerializeField]
    private float SliderNewRatio;
    [SerializeField]
    private float SliderOldRatio;


    //storage des valeurs traduites de 1 à 6 pour les vibrations de crans
    private int oldIntValue;
    private int curIntValue;


    //Texte d'affichage de la valeur actuelle
    public TMP_Text text_value_display;

    #endregion

    void Start ()
    {

        InitSlider();

        GetReferences();

    }

    void GetReferences()
    {       
        if (Mng_SyncVar == null)
            Mng_SyncVar = GameObject.FindGameObjectWithTag("Mng_SyncVar");
        if (Mng_SyncVar != null && sc_syncvar_calibr == null)
            sc_syncvar_calibr = Mng_SyncVar.GetComponent<SC_SyncVar_calibr>();
    }

    #region ViveGrip

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

    #endregion

    void Update ()
    {
        Slide();
    }

    void InitSlider()
    {
        sliderRigidbody = gameObject.GetComponent<Rigidbody>();
        Vector3 SliderLenghtVector = RightLimit.transform.position - LeftLimit.transform.position;
        SliderLenght = SliderLenghtVector.magnitude - LeftLimit.transform.localScale.x;
        SliderPos = this.transform.position - LeftLimit.transform.position;
        SliderCurPos = SliderPos.magnitude;
        SliderOldRatio = Ratio(SliderCurPos, SliderLenght, 0.45f, 0.0f, -0.45f);
    }

    void Slide()
    {

        //on traduit la position en position locale pour la freeze
        _localX = transform.localPosition.x;
        _localY = transform.localPosition.y;
        _localZ = transform.localPosition.z;

        //Freeze Axes
        if (_freezeAlongX) _localX = LeftLimit.transform.localPosition.x;
        if (_freezeAlongY) _localY = LeftLimit.transform.localPosition.y;
        if (_freezeAlongZ) _localZ = LeftLimit.transform.localPosition.z;
        gameObject.transform.localPosition = new Vector3(_localX, _localY, _localZ);

        SliderPos = this.transform.position - LeftLimit.transform.position;
        SliderCurPos = SliderPos.magnitude;


        //Limite ultime pour que ca sorte pas


        if (this.transform.localPosition.y > RightLimit.transform.localPosition.y+0.01f)
        {
            this.transform.localPosition = new Vector3(_localX, RightLimit.transform.localPosition.y-0.01f, _localZ);
        }
        else if (this.transform.localPosition.y < LeftLimit.transform.localPosition.y + 0.01f)
        {
            this.transform.localPosition = new Vector3(_localX, LeftLimit.transform.localPosition.y + 0.01f, _localZ);
        }


        SliderNewRatio = Ratio(SliderCurPos, SliderLenght, 0.45f, 0.0f, -0.45f);


        //Envoi à la syncVar, stockage de la valeur traduite en int et vibration si changement de cran
        if (SliderNewRatio != SliderOldRatio)
        {
            SendToSynchVar(SliderNewRatio);
            SyncWeapSystemCalib(Ratio(SliderCurPos, SliderLenght, 1.0f, 0.0f, 0.0f));

            curIntValue = (int)((SliderNewRatio + 0.4f) * 6.25f);

            text_value_display.text = (curIntValue + 1).ToString();

            if (oldIntValue != curIntValue)
            {
                if (controller != null)
                    controller.Vibrate(50, 0.3f);
            }

            oldIntValue = curIntValue;

        }                

        //Vibration
        if (controller != null)
        {
            float distance = Mathf.Min(Mathf.Abs(SliderNewRatio - SliderOldRatio), MAX_VIBRATION_DISTANCE);
            float vibrationStrength = (distance / MAX_VIBRATION_DISTANCE) * MAX_VIBRATION_STRENGTH;
            controller.Vibrate(VIBRATION_DURATION_IN_MILLISECONDS, vibrationStrength);
        }

        SliderOldRatio = SliderNewRatio;

    }

    //Met aussi à jour l'affichage de la valeur sur le slider
    void SendToSynchVar (float value)
    {    

        if (sc_syncvar_calibr == null)
        {
            GetReferences();
        }
        else
        {

            value = (value + 0.4f) * 6.25f;

            int intvalue = (int)value;

            sc_syncvar_calibr.CalibrInts[index] = intvalue;

            int curWeaponIndex = SC_WeaponManager.Instance.n_CurWeapon;
  
            GameObject curWeapon = SC_WeaponManager.Instance.tab_Weapons[curWeaponIndex];

            curWeapon.GetComponent<IF_Weapon>().SetSensitivity(index, intvalue);
       
        }

    }

    void SyncWeapSystemCalib(float Value)
    {

        if (index == 0)
            SC_SyncVar_WeaponSystem.Instance.f_AmplitudeCalib = Value;

        if (index == 1)
            SC_SyncVar_WeaponSystem.Instance.f_FrequenceCalib = Value;

        if (index == 2)
            SC_SyncVar_WeaponSystem.Instance.f_PhaseCalib = Value;

    }

    /// <summary>
    /// * Return the input value according a given range translated to an other range | 
    /// * @param float inputValue |
    /// * @param float inputMax | 
    /// * @param float outputMax | 
    /// * @param float inputMin | 
    /// * @param float outputMin
    /// </summary>
    float Ratio(float inputValue, float inputMax, float outputMax, float inputMin = 0.0f, float outputMin = 0.0f)
    {
        float product = (inputValue - inputMin) / (inputMax - inputMin);
        float output = ((outputMax - outputMin) * product) + outputMin;
        return output;
    }

}
