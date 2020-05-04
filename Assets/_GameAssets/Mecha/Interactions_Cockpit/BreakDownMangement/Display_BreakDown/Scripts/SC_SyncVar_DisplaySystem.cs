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


    [Header("Var Download")]
    [SyncVar(hook = "OnDownload")]
    public float Progress = 0;

    public void OnDownload(float newValue)
    {
        Progress = newValue;
    }

    #endregion Var SC_GameStates

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
