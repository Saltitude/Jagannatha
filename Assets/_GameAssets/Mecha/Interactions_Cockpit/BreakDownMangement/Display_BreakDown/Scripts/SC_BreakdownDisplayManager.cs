using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_BreakdownDisplayManager : MonoBehaviour, IF_BreakdownManager
{

    #region Singleton

    private static SC_BreakdownDisplayManager _instance;
    public static SC_BreakdownDisplayManager Instance { get { return _instance; } }

    #endregion

    #region Variables

    [Header("References")]
    public GameObject screenController;
    private IF_BreakdownManager Mng_BreakdownMain;
    private SC_breakdown_displays_screens sc_screens_controller;

    [Header("BreakDown Var")]
    [SerializeField]
    public int n_MaxBreakInterB4MaxBD = 5;
    public bool b_MaxBreakdown = false;
    public int CurNbOfBreakdown = 0;
    

    [Header("Interactibles"), SerializeField]
    public GameObject[] interactible;

    [Header("FX_panne_Pannel"), SerializeField]
    public ParticleSystem[] FX_pannel;

    //etat actuel des FX de panne => 0 -> pas de fx, 1 -> fx en play ====> autres rangs pour echelonner les fx
    private int FX_pannel_lvl = 0;


    #endregion Variables

    #region Init

    void Awake()
    {

        InitSingleton();       

        GetInteractibles();     

        Invoke("Demarage", 0.5f);

    }

    void Start()
    {
        Mng_BreakdownMain = this.transform.parent.gameObject.GetComponent<IF_BreakdownManager>();
        //get du script qui gere l'affichage des ecrans de panne
        sc_screens_controller = screenController.GetComponent<SC_breakdown_displays_screens>();
    }

    void InitSingleton()
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

    void GetInteractibles()
    {
        //LES ITNERACTIBLES d'ARME NECESSITENT CE TAG
        interactible = GameObject.FindGameObjectsWithTag("Interactible");
    }

    void Demarage()
    {
        ViveGripExample_Slider[] slidersTab = new ViveGripExample_Slider[4];
        slidersTab = FindObjectsOfType<ViveGripExample_Slider>();

        for (int i = 0; i < slidersTab.Length; i++)
        {
            if (slidersTab[i].isFLUX)
            {
                slidersTab[i].GetComponent<IInteractible>().ChangeDesired();
                CurNbOfBreakdown++;
                //on met en panne un écran
                sc_screens_controller.PutOneEnPanne();
                sc_screens_controller.CurNbOfScreenBreak++;

            }
        }
       
        /*
        bool oneSliderBreakdownee = false;
        int counterDebug = 0;
        while (!oneSliderBreakdownee)
        {
            int rand = Random.Range(0, 4);

            if (!slidersTab[rand].isFLUX)
            {
                slidersTab[rand].ChangeDesired();
                //on itere le nombre de pannes total
                CurNbOfBreakdown++;
                //on met en panne un écran
                sc_screens_controller.PutOneEnPanne();
                sc_screens_controller.CurNbOfScreenBreak++;
                oneSliderBreakdownee = true;
            }
        }
        */
        StartNewBreakdown(interactible.Length);
        sc_screens_controller.PanneAll();
    }

    #endregion Init

    #region MainFunctions

    public void StartNewBreakdown(int nbBreakdown)
    {

        int curBreakdown = nbBreakdown;
        bool newBreakdown = true;    

        for (int i=0;i< nbBreakdown;i++)
        {

            if (newBreakdown && !b_MaxBreakdown && !SC_MainBreakDownManager.Instance.b_BreakEngine)
            {

                int noBreakdown = 0;

                for (int j = 0; j < interactible.Length; j++)
                {
                    
                    if (!interactible[j].GetComponent<IInteractible>().isBreakdown())
                        noBreakdown++;                  
                }

                if (noBreakdown == 0)
                {
                    newBreakdown = false;
                    break;
                }             

                int rnd = Random.Range(0, interactible.Length);

                if (interactible[rnd].GetComponent<IInteractible>().isBreakdown() || !interactible[rnd].GetComponent<IInteractible>().testAgainstOdds())
                {
                    i--;
                }

                else
                {

                    interactible[rnd].GetComponent<IInteractible>().ChangeDesired();
                    //on itere le nombre de pannes total
                    CurNbOfBreakdown++;
                    //on met en panne un écran
                    sc_screens_controller.PutOneEnPanne();
                    sc_screens_controller.CurNbOfScreenBreak++;

                    curBreakdown++;

                    if (curBreakdown == nbBreakdown)
                        newBreakdown = false;

                }

            }

        }
        
    }

    public void CheckBreakdown()
    {

        //on update le nombre de pannes
        CurNbOfBreakdown = NumberOfBreakdown();
        SC_SyncVar_DisplaySystem.Instance.f_CurNbOfBd = CurNbOfBreakdown;

        //Gestion temporaire du play/stop des fx de panne
        if (CurNbOfBreakdown > 0)
            Play_Stop_All_Pannel_FX(true);
        else
            Play_Stop_All_Pannel_FX(false);

        //MaxBreakDown
        if (CurNbOfBreakdown >= n_MaxBreakInterB4MaxBD && !b_MaxBreakdown)
        {
            b_MaxBreakdown = true;
            SC_SyncVar_DisplaySystem.Instance.b_MaxBreakdown = true;
        }


        //IN DEBUG
        
        if(CurNbOfBreakdown > SC_breakdown_displays_screens.Instance.curNbPanne)
        {
            int nb_ScreensToBreak = CurNbOfBreakdown - SC_breakdown_displays_screens.Instance.curNbPanne;
            for(int i = 0; i < nb_ScreensToBreak; i++)
            {
                SC_breakdown_displays_screens.Instance.PutOneEnPanne();
            }
        }
        

        //OLD
        /*
        else if (CurNbOfBreakdown == 0 && b_MaxBreakdown)
            EndBreakdown();

        //Permet de régler les demi-pannes d'écrans
        else if (CurNbOfBreakdown == 0 && !b_MaxBreakdown && SC_main_breakdown_validation.Instance.isValidated)
            EndBreakdown();
        */

        //Resolution
        if (CurNbOfBreakdown == 0 && !SC_MainBreakDownManager.Instance.b_BreakEngine)
            EndBreakdown();

        else
        {
            SyncSystemState();
            SC_MainBreakDownManager.Instance.CheckBreakdown();
        }

    }

    public void EndBreakdown()
    {

        b_MaxBreakdown = false;
        SC_SyncVar_DisplaySystem.Instance.b_MaxBreakdown = false;
        sc_screens_controller.RepairAll();

        SyncSystemState();
        SC_MainBreakDownManager.Instance.CheckBreakdown();

    }

    #endregion MainFunctions

    int NumberOfBreakdown()
    {

        int n_BreakdownValue = 0;

        for (int j = 0; j < interactible.Length; j++)
        {
            if (interactible[j].GetComponent<IInteractible>().isBreakdown())
            {
                n_BreakdownValue++;
            }
        }

        return n_BreakdownValue;

    }

    void SyncSystemState()
    {
        if (CurNbOfBreakdown > 0)
            SC_SyncVar_Main_Breakdown.Instance.onPanneDisplayChange(true);
        else
            SC_SyncVar_Main_Breakdown.Instance.onPanneDisplayChange(false);
    }
    
    #region FX
    //Fonction jouant toutes les FX indiquant une panne sur le système,à échelonner plus tard)
    private void Play_Stop_All_Pannel_FX(bool startStop)
    {
        foreach(ParticleSystem FX in FX_pannel)
        {
            if (startStop)
                FX.Play();
            else
                FX.Stop();

        }

    }



    #endregion FX

    #region DebugMethod

    /// <summary>
    /// Focntion permettant de réparer tous les boutons automatiquement
    /// </summary>
    public void RepairBreakdownDebug()
    {
        for (int j = 0; j < interactible.Length; j++)
        {
            interactible[j].GetComponent<IInteractible>().Repair();
        }
    }

    public void RepairSingleBreakdownDebug()
    {

        List<GameObject> list = new List<GameObject>();
        for(int i =0; i< interactible.Length;i++)
        {

            if(interactible[i].GetComponent<IInteractible>().isBreakdown())
            {
                list.Add(interactible[i]);
            }

        }

        int rnd = Random.Range(0, list.Count);
        list[rnd].GetComponent<IInteractible>().Repair();

    }

    #endregion DebugMethod

    public void ForceUpdate()
    {
        SC_SyncVar_DisplaySystem.Instance.b_MaxBreakdown = b_MaxBreakdown;
        SC_SyncVar_DisplaySystem.Instance.f_CurNbOfBd = CurNbOfBreakdown;
        SyncSystemState();
    }

}

