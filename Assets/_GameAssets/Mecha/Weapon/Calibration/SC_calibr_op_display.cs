using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SC_calibr_op_display : MonoBehaviour
{
    SC_SyncVar_calibr sc_syncvar;
    Text text_component_cur;

    GameObject Mng_SyncVar = null;



    public int index;
    int numChild;

    // Start is called before the first frame update
    void Start()
    {
        text_component_cur = gameObject.GetComponent<Text>();
        numChild = this.transform.childCount;
        Mng_SyncVar = GameObject.FindGameObjectWithTag("Mng_SyncVar");
        GetReferences();
    }

    void GetReferences()
    {
        if (Mng_SyncVar == null)
            Mng_SyncVar = GameObject.FindGameObjectWithTag("Mng_SyncVar");
        if (Mng_SyncVar != null && sc_syncvar == null)
            sc_syncvar = Mng_SyncVar.GetComponent<SC_SyncVar_calibr>();

    }

    // Update is called once per frame
    void Update()
    {

        //text_component_cur.text = sc_syncvar.CalibrInts[index].ToString();
        
        for(int i = 0;i<numChild;i++)
        {
            if(sc_syncvar.CalibrInts[index] == i)
            {
                animMaison(this.transform.GetChild(i));
            }
            else
            {
                this.transform.GetChild(i).transform.localScale = Vector3.one;
            }
        }
    }

    void animMaison(Transform imageToModify)
    {
        imageToModify.gameObject.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
    }
}
