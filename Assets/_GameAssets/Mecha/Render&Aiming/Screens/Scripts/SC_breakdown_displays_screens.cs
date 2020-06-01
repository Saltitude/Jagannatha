using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// script sur le parent des ecrans de panne
/// </summary>

public class SC_breakdown_displays_screens : MonoBehaviour
{

    #region Singleton

    private static SC_breakdown_displays_screens _instance;
    public static SC_breakdown_displays_screens Instance { get { return _instance; } }

    #endregion

    [Header("References")]
    public Renderer[] tab_screens_renderers;
    public Material[] mat;
    GameObject Mng_SyncVar = null;
    SC_SyncVar_StateMecha_Display sc_syncvar_display;

    [Header("States")]
    [SerializeField]
    bool demarage = true;
    [SerializeField]
    bool gameEnded = false;

    [Header("BreakDown Infos")]
    public int curNbPanne = 0;
    public int CurNbOfScreenBreak = 0;

    [SerializeField]
    GameObject sphereReturnCacheMisere;

    enum ScreenState
    {
        Tuto,
        TutoDisplayRepared,
        PartialBreakdown,
        TotalBreakdown,
        End
    }
    ScreenState curScreenState;

    private int nbOfChildrenAtInit = 13;

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
        nbOfChildrenAtInit = gameObject.transform.childCount;

        tab_screens_renderers = new Renderer[nbOfChildrenAtInit];

        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            tab_screens_renderers[i] = gameObject.transform.GetChild(i).GetComponent<Renderer>();
            tab_screens_renderers[i].material = mat[(int)ScreenState.Tuto];
            tab_screens_renderers[i].enabled = false;
        }

    }

    void changeScreenMat(ScreenState screenState, int index, bool state = true)
    {
        
        //tab_screens_renderers[index].enabled = state;
        AlignMshRenderer(index, state);

        tab_screens_renderers[index].material = mat[(int)screenState];
        curScreenState = screenState;

        if (screenState != ScreenState.Tuto && screenState != ScreenState.TutoDisplayRepared)
        {
            tab_screens_renderers[index].GetComponent<SC_playvideo>().StopVideo();
            tab_screens_renderers[index].GetComponent<SC_playvideo>().PlayVideo();
        }
    }

    void GetReferences()
    {
        if (Mng_SyncVar == null)
        {
            Mng_SyncVar = GameObject.FindGameObjectWithTag("Mng_SyncVar");
            sc_syncvar_display = Mng_SyncVar.GetComponent<SC_SyncVar_StateMecha_Display>();
        }
    }


    public void TutoDisplayRepair()
    {
        StartCoroutine(TutoDisplaySequence());

    }

    IEnumerator TutoDisplaySequence()
    {
        float rnd;

        for (int i = 0; i < nbOfChildrenAtInit; i++)
        {
            StartCoroutine(BlinkScreen(i));
            rnd = Random.Range(0.1f, 0.5f);
            yield return new WaitForSeconds(rnd);
        }
        yield return new WaitForSeconds(1);
        StopAllCoroutines();
    }
    IEnumerator BlinkScreen(int index)
    {
        changeScreenMat(ScreenState.TutoDisplayRepared, index);
        yield return new WaitForSeconds(0.2f);
        changeScreenMat(ScreenState.Tuto, index);
        yield return new WaitForSeconds(0.2f);
        changeScreenMat(ScreenState.TutoDisplayRepared, index);
        yield return new WaitForSeconds(0.1f);
        changeScreenMat(ScreenState.Tuto, index);
        yield return new WaitForSeconds(0.1f);
        changeScreenMat(ScreenState.TutoDisplayRepared, index);


    }


    public void PannePartielleDisplay()
    {
        for (int i = 0; i < nbOfChildrenAtInit; i++)
        {
            changeScreenMat(ScreenState.PartialBreakdown, i);
        }

    }


    //pour foutre le mat video de panne totale
    public void FullPanneDisplay()
    {
        for (int i = 0; i < nbOfChildrenAtInit; i++)
        {
            changeScreenMat(ScreenState.TotalBreakdown, i);
        }
    }

    public void EndScreenDisplay()
    {
        gameEnded = true;
        for (int i = 0; i < nbOfChildrenAtInit; i++)
        {
            changeScreenMat(ScreenState.End, i);
        }
    }



    public void PutOneEnPanne()
    {

        if (!gameEnded && !SC_MainBreakDownManager.Instance.b_BreakEngine)
        {

            //int machin = 0;

            //&& !SC_MainBreakDownManager.Instance.b_BreakEngine
            for (int i = 0; i < 1; i++)
            {

                //Debug.Log(machin);

                //if (SC_MainBreakDownManager.Instance.b_BreakEngine)
                    //break;

                //if (machin > 80)
                    //break;

                if (curNbPanne < tab_screens_renderers.Length)
                {

                    int rand = Random.Range(0, tab_screens_renderers.Length);
                    if (tab_screens_renderers[rand].enabled)
                    {
                        i--;
                        //machin++;
                    }

                    else
                    {
                        SetScreenState(rand, true);
                    }

                }

            }

        }

    }

    /* Unuse For Now
    void PutXenPanne(int x)
    {

        for (int i = 0; i < x; i++)
        {

            if (curNbPanne < tab_screens_renderers.Length)
            {

                int rand = Random.Range(0, tab_screens_renderers.Length);
                if (tab_screens_renderers[rand].enabled)
                {
                    i--;
                }

                else
                {
                    tab_screens_renderers[rand].enabled = true;
                    curNbPanne++;
                }

            }

        }

    }
    */

    public void PanneAll()
    {

        CurNbOfScreenBreak = SC_BreakdownDisplayManager.Instance.interactible.Length * 2;

        for (int i = 0; i < tab_screens_renderers.Length; i++)
        {
            SetScreenState(i, true);
        }

        if (SC_GameStates.Instance.CurState != SC_GameStates.GameState.Lobby)
            FullPanneDisplay();

    }

    public void RepairAll()
    {

        CurNbOfScreenBreak = 0;

        if (demarage)
        {
            demarage = false;
            sphereReturnCacheMisere.SetActive(false);
            CustomSoundManager.Instance.PlaySound(gameObject, "SFX_p_ScreenActivated", false, 0.1f);
        }

        else
            PannePartielleDisplay();

        for (int i = 0; i < tab_screens_renderers.Length; i++)
        {
            SetScreenState(i, false);
        }

    }

    //fonction qui change state l'ecran demandé des deux cotes true == panne false == repare
    private void SetScreenState(int index, bool state)
    {

        AlignMshRenderer(index, state);

        if (curScreenState != ScreenState.Tuto && curScreenState != ScreenState.TutoDisplayRepared)
        {
            if (state == true) tab_screens_renderers[index].GetComponent<SC_playvideo>().PlayVideo();
            if (state == false) tab_screens_renderers[index].GetComponent<SC_playvideo>().StopVideo();
        }

        if (Mng_SyncVar == null)
            GetReferences();

        //cote operateur
        sc_syncvar_display.displayAll[index] = state;

    }

    void AlignMshRenderer(int index, bool state)
    {
        if (state == true && tab_screens_renderers[index].enabled != state)
        {
            Debug.Log("TINTIN : " + tab_screens_renderers[index].enabled + " | " + state);
            curNbPanne++;
        }


        else if (state == false && tab_screens_renderers[index].enabled != state)
        {
            Debug.Log("ENCORE VOUS : " + tab_screens_renderers[index].enabled + " | " + state);
            curNbPanne--;
        }


        tab_screens_renderers[index].enabled = state;
    }

}
