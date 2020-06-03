using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SC_UI_Update_SliderSolo : MonoBehaviour
{

    [SerializeField]
    Text textValue;

    [SerializeField]
    Text textWanted;


    GameObject Mng_SyncVar = null;
    SC_SyncVar_BreakdownDisplay sc_syncvar;

    [SerializeField]
    GameObject Bar;

    public int index;

    bool isBreakdown;

    SC_UI_WireBlink wireBlink;

    [SerializeField]
    int[] wireIndex;

    public Material Red;
    public Material Orange;
    //private Color _orange = new Color(255, 159, 0);
    //private Color _red = new Color(255, 0, 0);

    //pour forcer la première update
    bool firstUpdate = false;

    // Start is called before the first frame update
    void Start()
    {

        wireBlink = GetComponentInParent<SC_UI_WireBlink>();
        Mng_SyncVar = GameObject.FindGameObjectWithTag("Mng_SyncVar");
        textWanted.enabled = true;
    }

    // Update is called once per frame²²
    void Update()
    {

        //textWanted.text = Mathf.RoundToInt(ratio(sc_syncvar.SL_sliders[index].valueWanted, 0.45f, 10, -0.45f, 0)).ToString();


        if (sc_syncvar == null || Mng_SyncVar == null)
        {
            if (Mng_SyncVar == null)
                Mng_SyncVar = GameObject.FindGameObjectWithTag("Mng_SyncVar");

            if (Mng_SyncVar != null)
                sc_syncvar = Mng_SyncVar.GetComponent<SC_SyncVar_BreakdownDisplay>();

        }
        if (sc_syncvar != null)
        {
            updateSliderSolo();
            
            //première update
            if (firstUpdate == false)
            {
                textWanted.text = Mathf.FloorToInt(ratio(sc_syncvar.SL_sliders[index].valueWanted, 0.45f, 10, -0.45f, 1)).ToString();

                if(isBreakdown)
                    textWanted.material = Red;
                else
                    textWanted.material = Orange;

            }


            //PANNE
            if (sc_syncvar.SL_sliders[index].isEnPanne && !isBreakdown)
            {
                ChangeState(true);

            }
            //NO PANNE
            else if(!sc_syncvar.SL_sliders[index].isEnPanne && isBreakdown)
            {
                ChangeState(false);
            }
            
        }
    }


    void ChangeState(bool breakdown)
    {
        if (breakdown)
        {
            //textWanted.enabled = true;


            textWanted.text = Mathf.FloorToInt(ratio(sc_syncvar.SL_sliders[index].valueWanted, 0.45f, 10, -0.45f, 1)).ToString();
            textWanted.material = Red;
           // Debug.Log("u are red bitch");
        }
        else
        {
            //textWanted.enabled = false;

            textWanted.text = Mathf.FloorToInt(ratio(sc_syncvar.SL_sliders[index].valueWanted, 0.45f, 10, -0.45f, 1)).ToString();
            textWanted.material = Orange;
            Debug.Log("u are orange bitch");
        }

        for (int i = 0; i < wireIndex.Length; i++)
        {
            wireBlink.SetBreakDown(wireIndex[i], breakdown);
        }
        isBreakdown = breakdown;
    }

    void updateSliderSolo()
    {

        textValue.text = Mathf.FloorToInt(ratio(sc_syncvar.SL_sliders[index].value, 0.45f, 10, -0.45f, 1)).ToString();
        //Bar.GetComponent<SC_UI_SystmShield>().simpleValue = ratio(sc_syncvar.SL_sliders[index].value, 0.45f, 1, -0.45f, 0.1f);
        Bar.GetComponent<SC_Display_flux_bar>().simpleValue = Mathf.FloorToInt(ratio(sc_syncvar.SL_sliders[index].value, 0.45f, 10, -0.45f, 1));
    }

    float ratio(float inputValue, float inputMax, float outputMax, float inputMin = 0.0f, float outputMin = 0.0f)
    {
        float product = (inputValue - inputMin) / (inputMax - inputMin);
        float output = ((outputMax - outputMin) * product) + outputMin;
        return output;
    }
}
