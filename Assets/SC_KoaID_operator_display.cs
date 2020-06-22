using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

 
public class SC_KoaID_operator_display : MonoBehaviour
{

    SC_KoaSettingsOP sc_koa;

    TextMeshPro txt;

    [SerializeField]
    float delayLetter;

    [SerializeField]
    TMPro.TMP_FontAsset sanskritFont;
    [SerializeField]
    TMPro.TMP_FontAsset voiceFont;

    bool isScanned = false;

    
    string koaID;
    // Start is called before the first frame update
    void Start()
    {
        sc_koa = transform.parent.GetComponent<SC_KoaSettingsOP>();
        txt = this.GetComponent<TextMeshPro>();
        txt.font = sanskritFont;
        SetTextActive(false);
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
