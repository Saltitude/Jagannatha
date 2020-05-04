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
    [SerializeField]
    int curNbPanne = 0;
    
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
        tab_screens_renderers = new Renderer[gameObject.transform.childCount];

        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            
            tab_screens_renderers[i] = gameObject.transform.GetChild(i).GetComponent<Renderer>();
            tab_screens_renderers[i].material = mat[0];
            tab_screens_renderers[i].enabled = false;
            tab_screens_renderers[i].GetComponent<SC_playvideo>().StopVideo();
            tab_screens_renderers[i].GetComponent<SC_playvideo>().PlayVideo();
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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
            PutOneEnPanne();
    }

    public void FirstPanneFinish()
    {
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            tab_screens_renderers[i].material = mat[1];
            tab_screens_renderers[i].GetComponent<SC_playvideo>().StopVideo();
            tab_screens_renderers[i].GetComponent<SC_playvideo>().PlayVideo();
        }
        demarage = false;
        SC_EnemyManager.Instance.Initialize();
    }

    public void EndScreenDisplay()
    {
        gameEnded = true;
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            tab_screens_renderers[i].material = mat[2];
            tab_screens_renderers[i].enabled = true;
            tab_screens_renderers[i].GetComponent<SC_playvideo>().StopVideo();
            tab_screens_renderers[i].GetComponent<SC_playvideo>().PlayVideo();
        }
    }

    public void PutOneEnPanne()
    {

        if(!gameEnded)
        {

            for (int i = 0; i < 1; i++)
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

        for (int i = 0; i < tab_screens_renderers.Length; i++)
        {
            SetScreenState(i,true);            
        }

    }

    public void RepairAll()
    {

        if(demarage)
        {
            FirstPanneFinish();
            CustomSoundManager.Instance.PlaySound(gameObject, "SFX_p_ScreenActivated", false, 0.1f);
        }

        for (int i = 0; i < tab_screens_renderers.Length; i++)
        {
            SetScreenState(i, false);
        }

    }

    //fonction qui change state l'ecran demandé des deux cotes true == panne false == repare
    private void SetScreenState(int index, bool state)
    {
    
        if (state == true && tab_screens_renderers[index].enabled != state)
            curNbPanne++;

        else if (tab_screens_renderers[index].enabled != state)
            curNbPanne--;

        tab_screens_renderers[index].enabled = state;
        if (state == true) tab_screens_renderers[index].GetComponent<SC_playvideo>().PlayVideo();
        if (state == false) tab_screens_renderers[index].GetComponent<SC_playvideo>().StopVideo();

        if (Mng_SyncVar == null)
            GetReferences();

        //cote operateur
        sc_syncvar_display.displayAll[index] = state;       
      
    }
}
