using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script gerant l'enchainement des Phases
///  | Sur le prefab EnemyManager(à instantié une fois)
///  | Auteur : Zainix
/// </summary>
public class SC_EnemyManager : MonoBehaviour
{

    #region Singleton

    private static SC_EnemyManager _instance;
    public static SC_EnemyManager Instance { get { return _instance; } }

    #endregion

    public PhaseSettings[] phases;
    public Slider Progress;
    public  int curPhaseIndex;
    GameObject Mng_SyncVar = null;
    SC_SyncVar_DisplaySystem sc_syncvar;

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

 
    public void Initialize()
    {
        InitNewPhase(0);
        Progress = GameObject.FindGameObjectWithTag("ProgressBar").GetComponent<Slider>();
        Progress.value = 0;
        sendToSynchVar(Progress.value);
    }

    public void InitNewPhase(int phaseIndex)
    {
        SC_PhaseManager.Instance.Initialize(phases[phaseIndex]);

        if (phaseIndex == 1)
        {
            SC_GameStates.Instance.ChangeGameState(SC_GameStates.GameState.Game);
        }
    }


    public void EndPhase()
    {
        curPhaseIndex++;
        if(curPhaseIndex >= phases.Length)
        {
            SC_GameStates.Instance.ChangeGameState(SC_GameStates.GameState.GameEnd);
            Progress.value = 100f;
            sendToSynchVar(Progress.value);
        }
        else
         InitNewPhase(curPhaseIndex);


    }

    void GetReferences()
    {
        if (Mng_SyncVar == null)
            Mng_SyncVar = GameObject.FindGameObjectWithTag("Mng_SyncVar");
        if (Mng_SyncVar != null && sc_syncvar == null)
            sc_syncvar = Mng_SyncVar.GetComponent<SC_SyncVar_DisplaySystem>();

    }

    public void sendToSynchVar(float value)
    {

        if (sc_syncvar != null)
        {
            sc_syncvar.OnDownload(value);
        }
        else
            GetReferences();

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F12))
        {
            SC_breakdown_displays_screens.Instance.EndScreenDisplay();
        }
    }
}
