using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SC_SyncVar_BreakdownDisplay : NetworkBehaviour, IF_SyncVar_Sliders
{
    /// <summary>
    /// //////////////////////////////////List struct des Sliders
    /// </summary>
    /// 


    #region Singleton

    private static SC_SyncVar_BreakdownDisplay _instance;
    public static SC_SyncVar_BreakdownDisplay Instance { get { return _instance; } }

    #endregion



    //nombre de sliders pour l'init
    public int slidersNb = 6;

    public struct Slider
    {
        public float value;
        public float valueWanted;
        public bool isEnPanne;


        public Slider(float v1, float v2, bool v3) : this()
        {
            this.value = v1;
            this.valueWanted = v2;
            this.isEnPanne = v3;
        }

        public void setValue(float newValue)
        {
            this.value = newValue;
        }

        public void setValueWanted(float newValue)
        {
            this.valueWanted = newValue;
        }

        public void setIsEnPanne(bool newValue)
        {
            this.isEnPanne = newValue;
        }

    }

    public class SyncListSliders : SyncListStruct<Slider>
    {

    }

    public SyncListSliders SL_sliders = new SyncListSliders();



    /// ////////////////////////////////////////////////////////////////////// /// //////////////////////////////////////////////////////////////////////
    /// ////////////////////////////////////////////////////////////////////// /// //////////////////////////////////////////////////////////////////////


    /// <summary>
    /// //////////////////////////////////List struct des switches
    /// </summary>

    //nombre de switches pour l'init
    public int switchesNb = 1;

    public struct Switch
    {
        public bool value;
        public bool valueWanted;
        public bool isEnPanne;


        public Switch(bool v1, bool v2, bool v3) : this()
        {
            this.value = v1;
            this.valueWanted = v2;
            this.isEnPanne = v3;
        }

        public void setValue(bool newValue)
        {
            this.value = newValue;
        }

        public void setValueWanted(bool newValue)
        {
            this.valueWanted = newValue;
        }

        public void setIsEnPanne(bool newValue)
        {
            this.isEnPanne = newValue;
        }

    }

    public class SyncListSwitches : SyncListStruct<Switch>
    {

    }

    public SyncListSwitches SL_switches = new SyncListSwitches();



    /// ////////////////////////////////////////////////////////////////////// /// //////////////////////////////////////////////////////////////////////
    /// ////////////////////////////////////////////////////////////////////// /// //////////////////////////////////////////////////////////////////////



    /*
[SyncVar]
public float potar1value = 0;
[SyncVar]
public float potar1valueWanted = 0;
[SyncVar]
public bool potar1isEnPanne = false;

[SyncVar]
public float potar2value = 0;
[SyncVar]
public float potar2valueWanted = 0;
[SyncVar]
public bool potar2isEnPanne = false;

[SyncVar]
public float potar3value = 0;
[SyncVar]
public float potar3valueWanted = 0;
[SyncVar]
public bool potar3isEnPanne = false;
*/

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

    void Start()
    {

        
        if (isServer)
        {
            //init des sliders

            for (int i = 0; i < slidersNb; i++)
            {
                SL_sliders.Add(new Slider(0f, 0f, false));
            }


            //init des Switches

            for (int i = 0; i < switchesNb; i++)
            {
                SL_switches.Add(new Switch(false, false, false));
            }
        }

        



    }

    // Update is called once per frame
    void Update()
    {

    }

/// FONCTIONS DE SET dans la list de struc des slider, can we make it more compact?
    public void SliderChangeValue(int index, float newValue)
    {
        Slider slider = SL_sliders[index];
        slider.setValue(newValue);
        SL_sliders.RemoveAt(index);
        SL_sliders.Insert(index, slider);

    }
    public void SliderChangeValueWanted(int index, float newValue)
    {
        Slider slider = SL_sliders[index];
        slider.setValueWanted(newValue);
        SL_sliders.RemoveAt(index);
        SL_sliders.Insert(index, slider);

    }
    public void SliderChangeIsPanne(int index, bool newValue)
    {
        Slider slider = SL_sliders[index];
        slider.setIsEnPanne(newValue);
        SL_sliders.RemoveAt(index);
        SL_sliders.Insert(index, slider);

    }


    /// FONCTIONS DE SET dans la list de struc des switches, can we make it more compact?
    public void SwitchChangeValue(int index, bool newValue)
    {
        Switch aSwitch = SL_switches[index];
        aSwitch.setValue(newValue);
        SL_switches.RemoveAt(index);
        SL_switches.Insert(index, aSwitch);

    }
    public void SwitchChangeValueWanted(int index, bool newValue)
    {
        Switch aSwitch = SL_switches[index];
        aSwitch.setValueWanted(newValue);
        SL_switches.RemoveAt(index);
        SL_switches.Insert(index, aSwitch);

    }
    public void SwitchChangeIsPanne(int index, bool newValue)
    {
        Switch aSwitch = SL_switches[index];
        aSwitch.setIsEnPanne(newValue);
        SL_switches.RemoveAt(index);
        SL_switches.Insert(index, aSwitch);

    }




}
