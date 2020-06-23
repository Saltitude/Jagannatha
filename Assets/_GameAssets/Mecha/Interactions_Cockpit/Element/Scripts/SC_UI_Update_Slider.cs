using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SC_UI_Update_Slider : MonoBehaviour
{

    SC_SyncVar_BreakdownDisplay sc_syncvar;
    GameObject Mng_SyncVar = null;
   

    //index du slider
    public int index;

    [SerializeField]
    Image barrettePanne;
    [SerializeField]
    Image barretteValueWanted;
    [SerializeField]
    Image barretteValueWanted2;

    [SerializeField]
    Image flecheHaut;
    [SerializeField]
    Image flecheBas;

    [SerializeField]
    GameObject warning;
    [SerializeField]
    GameObject sparkle;

    //va te te faire
    float offSetMultiplier = 254;

    bool isBreakdown;

    [SerializeField]
    SC_UI_WireBlink wireBlinkMaster;

    [SerializeField]
    int[] wireIndex;

    // Start is called before the first frame update
    void Start()
    {
        Mng_SyncVar = GameObject.FindGameObjectWithTag("Mng_SyncVar");
        GetReferences();
        flecheBas.enabled = false;
        flecheHaut.enabled = false;
    }

    void GetReferences()
    {
        if (Mng_SyncVar == null)
            Mng_SyncVar = GameObject.FindGameObjectWithTag("Mng_SyncVar");
        if (Mng_SyncVar != null && sc_syncvar == null)
            sc_syncvar = Mng_SyncVar.GetComponent<SC_SyncVar_BreakdownDisplay>();
    }

    // Update is called once per frame
    void Update()
    {
        updateRenderSlider();


        if (sc_syncvar == null || Mng_SyncVar == null)
            GetReferences();

        if( sc_syncvar != null)
        {

            if (Input.GetKeyDown(KeyCode.T))
            {
                //Debug.Log(sc_syncvar.SL_sliders[0].isEnPanne);
            }

            if (sc_syncvar.SL_sliders[index].isEnPanne && !isBreakdown)
            {
                ChangeState(true);
            }
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
            barrettePanne.enabled = true;
            float posY = sc_syncvar.SL_sliders[index].valueWanted * offSetMultiplier;

            warning.SetActive(true);
            sparkle.SetActive(false);
            barrettePanne.gameObject.transform.localPosition = new Vector3(barrettePanne.gameObject.transform.localPosition.x, posY, barrettePanne.gameObject.transform.localPosition.z);
            barretteValueWanted.color = new Color32(255, 0, 0,255);
            barretteValueWanted2.color = new Color32(255, 0, 0,255);

            if (sc_syncvar.SL_sliders[index].value > sc_syncvar.SL_sliders[index].valueWanted)
            {
                flecheBas.enabled = true;
                flecheHaut.enabled = false;
            }
            else
            {
                flecheHaut.enabled = true;
                flecheBas.enabled = false;
            }
                

        }
        else
        {
            barrettePanne.enabled = false;
            warning.SetActive(false);
            sparkle.SetActive(true);
            barretteValueWanted.color = new Color32(255, 255, 255, 255);
            barretteValueWanted2.color = new Color32(255, 255, 255, 255);


            flecheBas.enabled = false;
            flecheHaut.enabled = false;
        }

        for(int i = 0; i < wireIndex.Length;i++)
        {
            wireBlinkMaster.SetBreakDown(wireIndex[i],breakdown);
        }
        isBreakdown = breakdown;
    }

    void updateRenderSlider()
    {

        float posY1 = sc_syncvar.SL_sliders[index].value * offSetMultiplier;
        this.gameObject.transform.localPosition = new Vector3(this.gameObject.transform.localPosition.x, posY1, this.gameObject.transform.localPosition.z);

    }

}
