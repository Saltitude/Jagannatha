using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SC_Weapon_MechState : MonoBehaviour
{
    #region Singleton

    private static SC_Weapon_MechState _instance;
    public static SC_Weapon_MechState Instance { get { return _instance; } }

    #endregion

    [SerializeField]
    SC_WeaponLineState SC_WeaponLineState;

    [SerializeField]
    SC_UI_SystmShield _SystmShield;
    [SerializeField]
    SC_UI_SystmShield _WeaponEnergyLevel;

    [SerializeField]
    Image _Amplitude;
    [SerializeField]
    Image _Frequence;
    [SerializeField]
    Image _Phase;

    [SerializeField]
    Text _curTarget;

    [SerializeField]
    GameObject GeneralOffState;
    [SerializeField]
    GameObject InitializedState;
    [SerializeField]
    GameObject ConnectedOffState;
    [SerializeField]
    GameObject InitializeOffState;
    [SerializeField]
    GameObject LaunchedOffState;
    [SerializeField]
    GameObject DisconnectedState;

    public enum SystemState { Disconnected, Connected, Initialize, Launched }
    public SystemState CurState;



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

    public void UpdateVar()
    {

        _SystmShield.simpleValue = SC_SyncVar_WeaponSystem.Instance.f_WeaponLife;

        _WeaponEnergyLevel.simpleValue = SC_SyncVar_WeaponSystem.Instance.f_curEnergyLevel;

        _curTarget.text = SC_SyncVar_WeaponSystem.Instance.s_KoaID;

        _Amplitude.fillAmount = SC_SyncVar_WeaponSystem.Instance.f_AmplitudeCalib;
        _Frequence.fillAmount = SC_SyncVar_WeaponSystem.Instance.f_FrequenceCalib;
        _Phase.fillAmount = SC_SyncVar_WeaponSystem.Instance.f_PhaseCalib;

        CheckState();

        SC_WeaponLineState.UpdateLine();

    }

    #region States

    void CheckState()
    {

        if ((SC_GameStates.Instance.CurState == SC_GameStates.GameState.Tutorial && (int)SC_GameStates.Instance.CurTutoState >= (int)SC_GameStates.TutorialState.RepairWeapon) || (SC_GameStates.Instance.CurState != SC_GameStates.GameState.Tutorial && !SC_SyncVar_WeaponSystem.Instance.b_BreakEngine) )
        {

            CurState = SystemState.Connected;

            if ((SC_GameStates.Instance.CurState == SC_GameStates.GameState.Tutorial && SC_SyncVar_WeaponSystem.Instance.f_CurNbOfBd == 0) || (SC_GameStates.Instance.CurState != SC_GameStates.GameState.Tutorial && !SC_SyncVar_WeaponSystem.Instance.b_MaxBreakdown))
            {

                CurState = SystemState.Initialize;

                if (SC_SyncVar_WeaponSystem.Instance.b_IsLaunch)
                {
                    CurState = SystemState.Launched;
                }

            }

        }

        else
        {
            CurState = SystemState.Disconnected;
        }

        ApplyState();

    }

    void ApplyState()
    {

        switch (CurState)
        {

            case SystemState.Disconnected:

                DisconnectedState.SetActive(true);

                break;

            case SystemState.Connected:

                DisconnectedState.SetActive(false);

                InitializeOffState.SetActive(true);
                LaunchedOffState.SetActive(true);
                GeneralOffState.SetActive(true);

                InitializedState.SetActive(false);

                ConnectedOffState.SetActive(false);


                break;

            case SystemState.Initialize:

                DisconnectedState.SetActive(false);

                ConnectedOffState.SetActive(false);
                InitializeOffState.SetActive(false);
                LaunchedOffState.SetActive(true);
                GeneralOffState.SetActive(false);

                InitializedState.SetActive(true);

                ConnectedOffState.SetActive(false);

                break;

            case SystemState.Launched:
                DisconnectedState.SetActive(false);

                ConnectedOffState.SetActive(false);
                InitializeOffState.SetActive(false);
                LaunchedOffState.SetActive(false);
                GeneralOffState.SetActive(false);

                InitializedState.SetActive(false);

                break;

        }

    }

    #endregion States

}
