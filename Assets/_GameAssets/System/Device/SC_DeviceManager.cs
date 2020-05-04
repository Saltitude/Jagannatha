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

    GameObject Mng_CheckList = null;

    public GameObject VR_Assets;

    public bool b_IsVR = false;
    public bool b_IsFPS = false;

    [SerializeField]
    string[] tab_Device;

    // Start is called before the first frame update
    void Start()
    {

        IsCheck();

        CheckDevice();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
            GetJoyStickName();
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
    void GetJoyStickName()
    {

        tab_Device = Input.GetJoystickNames();

        for(int i = 0; i < tab_Device.Length; i++)
        {
            if (!tab_Device[i].Contains("OpenVR") && !tab_Device[i].Contains("UMDF Virtual hidmini device Product string"))
                Debug.Log("Use Device " + i + " Joynum = " + i + 1);
        }

    }

}
