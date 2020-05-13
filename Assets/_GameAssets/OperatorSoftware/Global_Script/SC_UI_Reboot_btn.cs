using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SC_UI_Reboot_btn : MonoBehaviour
{
    SC_SyncVar_Main_Breakdown sc_syncvar;
    GameObject Mng_SyncVar = null;

    Color curColor = new Color(255, 159, 0, 255);
    Color panneColor = new Color(255, 0, 0, 255);

    [SerializeField]
    Image stateInterrupteur;

    [SerializeField]
    Material curMat;
    [SerializeField]
    Material breakdownMat;
    [SerializeField]
    GameObject warning;
    [SerializeField]
    GameObject sparkle;
    [SerializeField]
    TextMeshProUGUI SReboot;
    bool bBlink = false;
    bool CorouIsRunning = false;

    // Start is called before the first frame update
    void Start()
    {
        Mng_SyncVar = GameObject.FindGameObjectWithTag("Mng_SyncVar");
        GetReferences();
    }

    // Update is called once per frame
    void Update()
    {
        if (sc_syncvar == null || Mng_SyncVar == null)
            GetReferences();

        if (sc_syncvar != null)
        {
            if(sc_syncvar.mustReboot)
            {
                //Debug.Log("OP : Must Reboot");
                stateInterrupteur.material = breakdownMat;
                warning.SetActive(true);
                sparkle.SetActive(false);
                bBlink = true;
                if (!CorouIsRunning)
                    StartCoroutine(BlinkSystem());
               
                
            }
            else
            {
                //Debug.Log(" OP : No need reboot");
                stateInterrupteur.material = curMat;
                warning.SetActive(false);
                sparkle.SetActive(true);
                bBlink = false;
                CorouIsRunning = false;
            }

        }
    }

    void GetReferences()
    {
        if (Mng_SyncVar == null)
            Mng_SyncVar = GameObject.FindGameObjectWithTag("Mng_SyncVar");
        if (Mng_SyncVar != null && sc_syncvar == null)
            sc_syncvar = Mng_SyncVar.GetComponent<SC_SyncVar_Main_Breakdown>();
    }

    IEnumerator BlinkSystem()
    {
        CorouIsRunning = true;
        while (bBlink == true)
        {
            SReboot.SetText("SYSTEM");
            yield return new WaitForSeconds(0.5f);
            SReboot.SetText(" ");
            yield return new WaitForSeconds(0.5f);
            SReboot.SetText("REBOOT");
            yield return new WaitForSeconds(0.5f);
            SReboot.SetText(" ");
            yield return new WaitForSeconds(0.5f);
        }
        SReboot.SetText("SYSTEM");
        StopAllCoroutines();
    }
}
