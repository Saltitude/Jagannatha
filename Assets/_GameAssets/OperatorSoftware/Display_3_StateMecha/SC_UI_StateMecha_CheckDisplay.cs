using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SC_UI_StateMecha_CheckDisplay : MonoBehaviour
{
    SC_SyncVar_StateMecha_Display sc_syncvar;
    GameObject Mng_SyncVar = null;

    Image curImage;

    Color32 curColor;
    [SerializeField]
    Color32 breakdownColor;

    public int index;
    // Start is called before the first frame update
    void Start()
    {
        Mng_SyncVar = GameObject.FindGameObjectWithTag("Mng_SyncVar");
        GetReferences();
        curImage = this.GetComponent<Image>();
        curColor = this.GetComponent<Image>().color;
    }

    // Update is called once per frame
    void Update()
    {
        if (sc_syncvar == null || Mng_SyncVar == null)
            GetReferences();

        if (sc_syncvar != null)
        {
            
        }
    }

    void GetReferences()
    {
        if (Mng_SyncVar == null)
            Mng_SyncVar = GameObject.FindGameObjectWithTag("Mng_SyncVar");
        if (Mng_SyncVar != null && sc_syncvar == null)
            sc_syncvar = Mng_SyncVar.GetComponent<SC_SyncVar_StateMecha_Display>();
    }

    public void changeColorOnDisplayBreakdown()
    {
            curImage.color = breakdownColor;
        
    }
    public void changeColorOnDisplayNeutral()
    {
            curImage.color = curColor;

    }
}
