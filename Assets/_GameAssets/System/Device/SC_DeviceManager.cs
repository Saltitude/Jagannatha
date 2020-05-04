using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

/// <summary>
/// Sur Mng_Device | 
/// Verifie la presence du casque VR.
/// </summary>
public class SC_DeviceManager : MonoBehaviour
{

    #region Singleton

    private static SC_DeviceManager _instance;
    public static SC_DeviceManager Instance { get { return _instance; } }

    #endregion

    GameObject Mng_CheckList = null;

    public GameObject VR_Assets;

    public bool b_IsVR = false;
    public bool b_IsFPS = false;

    [SerializeField]
    string[] tab_Device;

    public int n_JoyNumToUse;

    public bool[] tab_TorqueAxesToUse;

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

        IsCheck();

        CheckDevice();

        GetJoyStickToUse();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
            GetJoyStickToUse();
    }

    void IsCheck()
    {
        Mng_CheckList = GameObject.FindGameObjectWithTag("Mng_CheckList");
        Mng_CheckList.GetComponent<SC_CheckList>().Mng_Device = this.gameObject;
    }

    void CheckDevice()
    {
        if (XRSettings.isDeviceActive)
        {
            b_IsVR = true;
        }
        else
        {
            b_IsFPS = true;
        }
    }

    //Notes pour corriger le bug du Torque
    //Recuper l'index du JS dans le tableau et utilisé un axes avec un joynum correspondant
    //preparer les axes
    void GetJoyStickToUse()
    {

        tab_Device = Input.GetJoystickNames();

        tab_TorqueAxesToUse = new bool[tab_Device.Length];

        for (int i = 0; i < tab_TorqueAxesToUse.Length; i++)
            tab_TorqueAxesToUse[i] = false;

        for (int i = 0; i < tab_Device.Length; i++)
        {
            if (!tab_Device[i].Contains("OpenVR") && !tab_Device[i].Contains("UMDF Virtual hidmini device Product string"))
            {
                //Debug.Log("Use Device " + i + " Joynum = " + i + 1);
                n_JoyNumToUse = i+1;
                tab_TorqueAxesToUse[i] = true;
            }
                
        }

    }

}
