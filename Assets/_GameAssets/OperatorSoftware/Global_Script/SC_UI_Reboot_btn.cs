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
    Material curLotusMat;
    [SerializeField]
    GameObject warning;
    [SerializeField]
    GameObject sparkle;
    [SerializeField]
    TextMeshProUGUI SReboot;
    bool bBlink = false;
    bool CorouIsRunning = false;

    [SerializeField]
    Image[] img_ToBreakDown;
    bool[] IndexToActivate;
    Material[] wireSafe;
    [SerializeField]
    Material wireBreakdown;

    [SerializeField]
    Image LotusUpImg;
    [SerializeField]
    Image LotusDownImg;

    // Start is called before the first frame update
    void Start()
    {
        Mng_SyncVar = GameObject.FindGameObjectWithTag("Mng_SyncVar");
        GetReferences();
        wireSafe = new Material[img_ToBreakDown.Length];
        IndexToActivate = new bool[img_ToBreakDown.Length];

        for (int i = 0; i < wireSafe.Length; i++)
        {
            wireSafe[i] = img_ToBreakDown[i].material;
        }
        //StartCoroutine(RedWireCoro());
    }

    // Update is called once per frame
    void Update()
    {
        if (sc_syncvar == null || Mng_SyncVar == null)
            GetReferences();

        if (sc_syncvar != null)
        {
            if (sc_syncvar.mustReboot)
            {
                //Debug.Log("OP : Must Reboot");
                stateInterrupteur.material = breakdownMat;
                //warning.SetActive(true);
                sparkle.SetActive(false);

                LotusUpImg.material = breakdownMat;
                LotusDownImg.material = breakdownMat;
                bBlink = true;
                if(SC_GameStates.Instance.CurState == SC_GameStates.GameState.Game)
                    SetBreakDown(0, true);
                if (!CorouIsRunning)
                    StartCoroutine(BlinkSystem());


            }
            else
            {
                //Debug.Log(" OP : No need reboot");
                stateInterrupteur.material = curMat;
                //warning.SetActive(false);
                sparkle.SetActive(true);
                bBlink = false;
                CorouIsRunning = false;
                SetBreakDown(0, false);
                LotusUpImg.material = curLotusMat;
                LotusDownImg.material = curLotusMat;

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
        StartCoroutine(RedWireCoro());
        CorouIsRunning = true;
        while (bBlink == true)
        {
            SReboot.SetText("PILOT");
            yield return new WaitForSeconds(0.5f);
            SReboot.SetText(" ");
            yield return new WaitForSeconds(0.5f);
            SReboot.SetText("REBOOT");
            yield return new WaitForSeconds(0.5f);
            SReboot.SetText(" ");
            yield return new WaitForSeconds(0.5f);
        }
        SReboot.SetText("SYSTEM");
        StopCoroutine(BlinkSystem());
        StopCoroutine(RedWireCoro());
    }

    IEnumerator RedWireCoro()
    {
        float animTime = 0.5f;
        float maxOpacity = 1;
        float minOpacity = 0f;
        float ratePerSec = ((maxOpacity - minOpacity) / animTime);
        float curOpacity;
        bool Add = true;
        float t = 0;

        Vector4 ColorTampon = Color.white;
        curOpacity = minOpacity;

        while (true)
        {
            if (t < animTime)
            {
                t += Time.deltaTime;
                if (Add)
                {

                    if (curOpacity < maxOpacity)
                        curOpacity = Mathf.Lerp(curOpacity, maxOpacity, ratePerSec * Time.deltaTime);
                }
                else
                {

                    if (curOpacity > minOpacity)
                        curOpacity = Mathf.Lerp(curOpacity, minOpacity, ratePerSec * Time.deltaTime);

                }

                for (int i = 0; i < img_ToBreakDown.Length; i++)
                {
                    if (IndexToActivate[i])
                        img_ToBreakDown[i].color = new Vector4(ColorTampon.x, ColorTampon.y, ColorTampon.z, curOpacity);
                }

            }
            else
            {
                Add = !Add;
                t = 0;
            }
            yield return 0;
        }

    }
    void EndCoroutine(int index)
    {
        img_ToBreakDown[index].material = wireSafe[index];
        img_ToBreakDown[index].color = Color.white;
    }
    void SetBreakDown(int index, bool activate)
    {

        if (activate)
        {

            if (!IndexToActivate[index])
            {
                img_ToBreakDown[index].material = wireBreakdown;
            }

        }
        else
        {
            if (IndexToActivate[index])
                EndCoroutine(index);

        }
        IndexToActivate[index] = activate;

    }
}
