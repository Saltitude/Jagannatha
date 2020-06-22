using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

 
public class SC_KoaID_operator_display : MonoBehaviour
{

    SC_KoaSettingsOP sc_koa;

    TextMeshPro txt;

    // Start is called before the first frame update
    void Start()
    {
        sc_koa = transform.parent.GetComponent<SC_KoaSettingsOP>();
        txt = this.GetComponent<TextMeshPro>();
        SetTextActive(false);
    }

    // Update is called once per frame

    public void SetTextActive(bool active = true)
    {
        if(sc_koa != null)
            txt.text = sc_koa.GetKoaID();
        txt.enabled = active;
    }


}
