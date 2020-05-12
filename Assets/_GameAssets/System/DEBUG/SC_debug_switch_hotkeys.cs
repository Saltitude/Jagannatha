using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_debug_switch_hotkeys : MonoBehaviour
{


    public SC_SwitchChangeColor Button1;
    public SC_SwitchChangeColor Button2;
    public SC_SwitchChangeColor Button3;
    public SC_SwitchChangeColor Button4;
    public SC_SwitchChangeColor Button5;

    [SerializeField]
    ViveGripExample_Switch[] _Switch;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Button1.LightOnOFF();
           _Switch[0].Flip();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Button2.LightOnOFF();
           _Switch[1].Flip();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Button3.LightOnOFF();
            _Switch[2].Flip();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Button4.LightOnOFF();
           _Switch[3].Flip();
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Button5.LightOnOFF();
            _Switch[4].Flip();
        }

    }
}
