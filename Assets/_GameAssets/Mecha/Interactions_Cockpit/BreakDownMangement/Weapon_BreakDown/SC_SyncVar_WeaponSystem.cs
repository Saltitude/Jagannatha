using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SC_SyncVar_WeaponSystem : NetworkBehaviour
{

    #region Singleton

    private static SC_SyncVar_WeaponSystem _instance;
    public static SC_SyncVar_WeaponSystem Instance { get { return _instance; } }

    #endregion

    #region SC_GameStates

    [Header("Var SC_GameStates")]
    [SyncVar(hook = "OnChangeGameState")]
    public SC_GameStates.GameState CurState = SC_GameStates.GameState.Lobby;

    void OnChangeGameState(SC_GameStates.GameState _CurState)
    {
        CurState = _CurState;
        UpdateOnClient();
    }

    #endregion SC_GameStates

    #region SC_main_breakdown_validation

    [Header("Var SC_main_breakdown_validation")]
    [SyncVar(hook = "OnLaunch")]
    public bool b_IsLaunch = false;

    void OnLaunch(bool TargetBool)
    {
        b_IsLaunch = TargetBool;
        UpdateOnClient();
    }

    #endregion SC_main_breakdown_validation

    #region SC_MainBreakDownManager

    [Header("Var SC_MainBreakDownManager")]
    [SyncVar(hook = "OnChangeSystemLife")]
    public float f_WeaponLife = 0;
    [SyncVar(hook = "OnChangeBreakEngine")]
    public bool b_BreakEngine = false;

    void OnChangeSystemLife(float newLife)
    {
        f_WeaponLife = newLife;
        UpdateOnClient();

        if (newLife <= 0 && !isServer)
        {
            SC_Weapon_MechState.Instance.IncrementBuffer();
        }

    }

    void OnChangeBreakEngine(bool Breakdown)
    {
        b_BreakEngine = Breakdown;
        UpdateOnClient();
    }

    #endregion SC_MainBreakDownManager

    #region SC_BreakdownWeaponManager

    [Header("Var SC_BreakdownWeaponManager")]
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

    #endregion SC_BreakdownWeaponManager

    #region SC_slider_calibr

    //SC_slider_calibr
    [Header("Var SC_slider_calibr")]
    [SyncVar(hook = "OnChanheAmp")]
    public float f_AmplitudeCalib = 0;
    [SyncVar(hook = "OnChanheFre")]
    public float f_FrequenceCalib = 0;
    [SyncVar(hook = "OnChanhePha")]
    public float f_PhaseCalib = 0;

    void OnChanheAmp(float TargetValue)
    {
        f_AmplitudeCalib = TargetValue;
        UpdateOnClient();
    }

    void OnChanheFre(float TargetValue)
    {
        f_FrequenceCalib = TargetValue;
        UpdateOnClient();
    }

    void OnChanhePha(float TargetValue)
    {
        f_PhaseCalib = TargetValue;
        UpdateOnClient();
    }

    #endregion SC_slider_calibr

    //CuurentTarget
    [SyncVar(hook = "OnChangeID")]
    public string s_KoaID = "";

    void OnChangeID(string NewID)
    {
        s_KoaID = NewID;
        UpdateOnClient();
    }

    //Status
    //WeaNrjLevel
    [SyncVar(hook = "ChangeEnergyLvl")]
    public float f_curEnergyLevel;

    void ChangeEnergyLvl(float TargetValue)
    {
        f_curEnergyLevel = TargetValue;
        UpdateOnClient();
    }


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
        if (!isServer && SC_Weapon_MechState.Instance)
            SC_Weapon_MechState.Instance.UpdateVar();
    }

}
