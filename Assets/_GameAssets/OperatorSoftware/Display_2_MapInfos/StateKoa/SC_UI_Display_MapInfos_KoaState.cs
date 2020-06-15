using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SC_UI_Display_MapInfos_KoaState : MonoBehaviour
{
    #region Singleton

    private static SC_UI_Display_MapInfos_KoaState _instance;
    public static SC_UI_Display_MapInfos_KoaState Instance { get { return _instance; } }

    #endregion

    public SC_KoaSettingsOP curKoaScriptKoaSettings;

    GameObject Mng_SyncVar = null;
    SC_SyncVar_calibr sc_syncvar;
    SC_FixedData fixedData;

    BoidSettings boidSettings;

    public bool activated;

    [SerializeField]
    Text type;

    [SerializeField]
    Text koaLife;

    [SerializeField]
    SC_UI_SystmShield life;

    [SerializeField]
    SC_Triangle_Parameters _triangle;

    [SerializeField]
    Text koaStateTxt;
    //[SerializeField]
    //Text optiWeapon;

    [SerializeField]
    Font VoiceActivated;
    [SerializeField]
    Font sanskritFont;

    string[] StringState = { "Spawning", "Roaming", "Attacking", "Death", "Fleeing", "Absorbing" };
    public enum KoaState
    {
        Spawning = 0,
        Roam = 1,
        AttackPlayer = 2,
        Death = 3,
        Flight = 4,
        Absorbtion = 5
    }

    public KoaState curState;


    [SerializeField]
    Image[] barOpti = new Image[4];


    public float optiPercent;

    public float fKoaLife = 100;
    public float curfKoaLife = 100;
    public Vector3 koaSensibility;
    public Vector3 gunSensibility;

    bool secondaryBarChecker = false;
    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        fixedData = SC_FixedData.Instance;
        Mng_SyncVar = GameObject.FindGameObjectWithTag("Mng_SyncVar");
        GetReferences();
        activated = false;
    }

    public void SetNewKoaSettings(SC_KoaSettingsOP newSettings)
    {
        curKoaScriptKoaSettings = newSettings;
        boidSettings = fixedData.GetBoidSettings(curKoaScriptKoaSettings.GetBoidSettingsIndex());
        activated = true;
    }

    void GetReferences()
    {
        if (Mng_SyncVar == null)
            Mng_SyncVar = GameObject.FindGameObjectWithTag("Mng_SyncVar");
        if (Mng_SyncVar != null && sc_syncvar == null)
            sc_syncvar = Mng_SyncVar.GetComponent<SC_SyncVar_calibr>();
        if (sc_syncvar != null)
            gunSensibility = new Vector3(sc_syncvar.CalibrInts[0], sc_syncvar.CalibrInts[1], sc_syncvar.CalibrInts[2]);
    }


    // Update is called once per frame
    void Update()
    {

        if (sc_syncvar == null || Mng_SyncVar == null)
        {
            GetReferences();

        }
        if (sc_syncvar != null)
        {
            if (activated)
            {
                koaSensibility = new Vector3(curKoaScriptKoaSettings.GetSensibility().x, curKoaScriptKoaSettings.GetSensibility().y, curKoaScriptKoaSettings.GetSensibility().z);

                curfKoaLife = fKoaLife;

                //_triangle.b_Init = true;

                //sensi[0].text = (koaSensibility.x + 1).ToString();
                //sensi[1].text = (koaSensibility.y + 1).ToString();
                //sensi[2].text = (koaSensibility.z + 1).ToString();

                _triangle.amplitudeValue = (koaSensibility.y + 1);
                _triangle.frequenceValue = (koaSensibility.x + 1);
                _triangle.phaseValue = (koaSensibility.z + 1);
                

                _triangle.b_Init = false;

                //fKoaLife = (curKoaScriptKoaSettings.GetCurKoaLife() / curKoaScriptKoaSettings.GetMaxKoaLife()) * 100;
                fKoaLife = Mathf.Round(curKoaScriptKoaSettings.GetCurKoaLife() * 10);

                koaLife.text = fKoaLife.ToString();
                life.simpleValue = fKoaLife/100;

                gunSensibility = new Vector3(sc_syncvar.CalibrInts[0], sc_syncvar.CalibrInts[1], sc_syncvar.CalibrInts[2]);

                displayOptiBar();
                if(curKoaScriptKoaSettings.GetKoaType() == 4)
                {
                    type.font = sanskritFont;
                    type.text = "lenigrosfdplapute";

                    SC_TargetMap.Instance.SetFont(SC_TargetMap.FontList.Sanskri);
                    SC_TargetMap.Instance.SetText("lenigrosfdplapute");
                }
                else
                {
                    type.font = VoiceActivated;
                    type.text = "Type " + curKoaScriptKoaSettings.GetKoaID();

                    SC_TargetMap.Instance.SetFont(SC_TargetMap.FontList.VoiceActivated);
                    SC_TargetMap.Instance.SetText(type.text);
                }


                koaStateTxt.text = StringState[curKoaScriptKoaSettings.GetKoaState()].ToUpper();
                curState = (KoaState)curKoaScriptKoaSettings.GetKoaState();
                //koaStateTxt.text = curState.ToString().ToUpper();

                if (curfKoaLife != fKoaLife)
                {

                    SC_UI_Display_MapInfos_KOAShake.Instance.ShakeIt(5f,0.5f);
                    SC_UI_Display_MapInfos_StateManager.Instance.checkState();
                }
                else if (curfKoaLife <= 0)
                {
                    SC_UI_Display_MapInfos_StateManager.Instance.checkState();
                }

            
                BoidSettings newBoidsettings = fixedData.GetBoidSettings(curKoaScriptKoaSettings.GetBoidSettingsIndex());
                if (boidSettings != newBoidsettings)
                {
                    boidSettings = newBoidsettings;
                    SC_UI_Display_Flock.Instance.StartNewBehavior(boidSettings);
                }

                SC_UI_Display_Flock.Instance.SetAnimationBool(curKoaScriptKoaSettings.GetDeploy(), curKoaScriptKoaSettings.GetFlight(), curKoaScriptKoaSettings.GetBullet(), curKoaScriptKoaSettings.GetLaser(), curKoaScriptKoaSettings.GetSpeedFactor(), curKoaScriptKoaSettings.GetChargeLaser());
                
            }
            else
            {
                
                _triangle.b_Init = true;
            }

        }
        
    }
    

    int GetOptiPerCent()
    {

        float x = Mathf.Abs((int)gunSensibility.x - (int)koaSensibility.x);
        float y = Mathf.Abs((int)gunSensibility.y - (int)koaSensibility.y);
        float z = Mathf.Abs((int)gunSensibility.z - (int)koaSensibility.z);

        float ecart = x + y + z;


        float power = 6 - ecart;

        if (power < 0) power = 0;
        float powerPerCent = (power / 6) * 100;


        return (int)powerPerCent;
    }

    void displayOptiBar()
    {
        
        if (GetOptiPerCent() > 0)
        {
            barOpti[0].enabled = true;
        }
        else
        {
            barOpti[0].enabled = false;
        }
        if (GetOptiPerCent() >= 25)
        {
            barOpti[1].enabled = true;
        }
        else
        {
            barOpti[1].enabled = false;
        }
        if (GetOptiPerCent() >= 50)
        {
            barOpti[2].enabled = true;
        }
        else
        {
            barOpti[2].enabled = false;
        }
        if (GetOptiPerCent() >= 75)
        {
            barOpti[3].enabled = true;
        }
        else
        {
            barOpti[3].enabled = false;
        }
    }
}
