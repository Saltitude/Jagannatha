using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//SUR LA MAP
public class SC_KoaID_operator_display : MonoBehaviour
{

    SC_KoaSettingsOP sc_koa;

    TextMesh txt;

    // Start is called before the first frame update
    void Start()
    {
        sc_koa = transform.parent.GetComponent<SC_KoaSettingsOP>();
        txt = this.GetComponent<TextMesh>();
    }

    // Update is called once per frame


    public void SetTextActive(bool active = true)
    {
        if(sc_koa != null)
            koaID = sc_koa.GetKoaID();
        txt.enabled = active;
        if (!isScanned && active)
            StartCoroutine(ChangeTextFont());
        
    }

    IEnumerator ChangeTextFont()
    {

        txt.text = "scanningKoa";
        yield return new WaitForSeconds(delayLetter);
        txt.text = "gettingInfo";
        yield return new WaitForSeconds(delayLetter);
        txt.text = "leniismyboy";
        yield return new WaitForSeconds(delayLetter);
        txt.text = "tktfratelol";
        yield return new WaitForSeconds(delayLetter);
        txt.text = "bijourjesap";
        yield return new WaitForSeconds(delayLetter);
        txt.text = "scanningKoa";
        yield return new WaitForSeconds(delayLetter);
        txt.text = "gettingInfo";
        yield return new WaitForSeconds(delayLetter);
        txt.text = "leniismyboy";
        yield return new WaitForSeconds(delayLetter);
        txt.text = "tktfratelol";
        yield return new WaitForSeconds(delayLetter);
        txt.text = "bijourjesap";

        txt.text = koaID;
        txt.font = voiceFont;
        isScanned = true;
        StopAllCoroutines();
    }


}
