using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SC_SyncVar_DisplaySystem : NetworkBehaviour
{

    #region Singleton

    private static SC_SyncVar_DisplaySystem _instance;
    public static SC_SyncVar_DisplaySystem Instance { get { return _instance; } }

    #endregion

    #region Var SC_GameStates

    [Header("Var SC_GameStates")]
    [SyncVar(hook = "OnChangeGameState")]
    public SC_GameStates.GameState CurState = SC_GameStates.GameState.Lobby;

    void OnChangeGameState(SC_GameStates.GameState _CurState)
    {
        CurState = _CurState;
        UpdateOnClient();
    }

    #endregion Var SC_GameStates

    #region Var SC_EnemyManager

    [Header("Var Download")]
    [SyncVar(hook = "OnDownload")]
    public float Progress = 0;
    [SyncVar(hook = "OnUpdate")]
    public bool Updating = false;

    public void OnDownload(float newValue)
    {
        Progress = newValue;
    }
    public void OnUpdate(bool newBool)
    {
        Updating = newBool;
    }

    #endregion Var SC_EnemyManager

    #region Var SC_WaveManager

    [Header("Var Wave")]
    [SyncVar(hook = "OnChangeWave")]
    public int curWave = 0;


    public void OnChangeWave(int newValue)
    {
        curWave = newValue;
        if(!isServer)
        {
            PlayAmbiance(curWave);
        }
    }
    void PlayAmbiance(int waveIndex)
    {
        switch (waveIndex)
        {
            case 3:
                SC_AmbiancePilot.Instance.PlayAmbiance(SC_AmbiancePilot.Ambiance.Rising1);
                break;

            case 6:
                SC_AmbiancePilot.Instance.PlayAmbiance(SC_AmbiancePilot.Ambiance.Rising2);
                break;

            case 11:
                SC_AmbiancePilot.Instance.PlayAmbiance(SC_AmbiancePilot.Ambiance.Climax1);
                break;

            case 12:
                SC_AmbiancePilot.Instance.PlayAmbiance(SC_AmbiancePilot.Ambiance.Slow1);
                break;

            case 13:
                SC_AmbiancePilot.Instance.PlayAmbiance(SC_AmbiancePilot.Ambiance.Rising3);
                break;

            case 14:
                SC_AmbiancePilot.Instance.PlayAmbiance(SC_AmbiancePilot.Ambiance.ClimaxMaxMax);
                break;
        }
    }
    #endregion

    #region Var SC_main_breakdown_validation

    [Header("Var SC_main_breakdown_validation")]
    [SyncVar(hook = "OnLaunch")]
    public bool b_IsLaunch = false;

    void OnLaunch(bool TargetBool)
    {
        b_IsLaunch = TargetBool;
        UpdateOnClient();
    }

    #endregion Var SC_main_breakdown_validation

    #region Var SC_MainBreakDownManager

    [Header("Var SC_MainBreakDownManager")]
    [SyncVar(hook = "OnChangeSystemLife")]
    public float f_Displaylife = 0;
    [SyncVar(hook = "OnChangeBreakEngine")]
    public bool b_BreakEngine = false;

    void OnChangeSystemLife(float newLife)
    {
        f_Displaylife = newLife;
        UpdateOnClient();
    }

    void OnChangeBreakEngine(bool Breakdown)
    {
        if((int)SC_GameStates.Instance.CurState >= (int)SC_GameStates.GameState.Game && !isServer)
        SC_UI_BlinkBreakdownManager.Instance.SetBreakdown(Breakdown);
        b_BreakEngine = Breakdown;
        UpdateOnClient();
    }

    #endregion Var SC_MainBreakDownManager

    #region Var SC_BreakdownDisplayManager

    [Header("Var SC_BreakdownDisplayManager")]
    [SyncVar(hook = "OnChangeNbOfBd")]
    public float f_CurNbOfBd = 0;
    [SyncVar(hook = "OnChangeMaxBd")]
    public bool b_MaxBreakdown = false;

    void OnChangeNbOfBd(float Target)
    {
        f_CurNbOfBd = Target;
        UpdateOnClient();
    }

    void OnChangeMaxBd(bool Target)
    {
        b_MaxBreakdown = Target;
        UpdateOnClient();
    }

    #endregion Var SC_BreakdownDisplayManager

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

    void UpdateOnClient()
    {
        if (!isServer)
            SC_Display_MechState.Instance.UpdateVar();
    }

}
