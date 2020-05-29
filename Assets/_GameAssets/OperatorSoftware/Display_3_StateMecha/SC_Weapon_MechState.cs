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
        SystemState newState;
        if ((SC_GameStates.Instance.CurState == SC_GameStates.GameState.Tutorial && (int)SC_GameStates.Instance.CurTutoState >= (int)SC_GameStates.TutorialState.StartRepairWeapon) || (SC_GameStates.Instance.CurState != SC_GameStates.GameState.Tutorial && !SC_SyncVar_WeaponSystem.Instance.b_BreakEngine) )
        {

            newState = SystemState.Connected;

            if ((SC_GameStates.Instance.CurState == SC_GameStates.GameState.Tutorial && SC_SyncVar_WeaponSystem.Instance.f_CurNbOfBd == 0) || (SC_GameStates.Instance.CurState != SC_GameStates.GameState.Tutorial && !SC_SyncVar_WeaponSystem.Instance.b_MaxBreakdown))
            {

                newState = SystemState.Initialize;

                if (SC_SyncVar_WeaponSystem.Instance.b_IsLaunch)
                {
                    newState = SystemState.Launched;
                }
            }
        }

        else
        {
            newState = SystemState.Disconnected;
        }


        if(newState != CurState)
        {
            CurState = newState;
            StopAllCoroutines();
            StartCoroutine(ApplyState());
        }
        
    }

    IEnumerator ApplyState()
    {


        switch (CurState)
        {

            case SystemState.Disconnected:
                SC_TutorialUIManager.Instance.ActivateSystem(SC_TutorialUIManager.System.Weapon, false);
                DisconnectedState.SetActive(true);

                break;

            case SystemState.Connected:

                SC_TutorialUIManager.Instance.ActivateSystem(SC_TutorialUIManager.System.Weapon, false);

                ConnectedOffState.SetActive(false);
                InitializeOffState.SetActive(true);
                LaunchedOffState.SetActive(true);
                GeneralOffState.SetActive(true);
                InitializedState.SetActive(false);

                yield return new WaitForSeconds(0.75f);
                SC_TutorialUIManager.Instance.ActivateSystem(SC_TutorialUIManager.System.Weapon, true);
                DisconnectedState.SetActive(false);

                yield return new WaitForSeconds(0.1f);
                SC_TutorialUIManager.Instance.ActivateSystem(SC_TutorialUIManager.System.Weapon, false);

                DisconnectedState.SetActive(true);

                yield return new WaitForSeconds(0.1f);
                SC_TutorialUIManager.Instance.ActivateSystem(SC_TutorialUIManager.System.Weapon, true);

                DisconnectedState.SetActive(false);

                yield return new WaitForSeconds(0.1f);
                SC_TutorialUIManager.Instance.ActivateSystem(SC_TutorialUIManager.System.Weapon, false);

                DisconnectedState.SetActive(true);

                yield return new WaitForSeconds(0.1f);
                SC_TutorialUIManager.Instance.ActivateSystem(SC_TutorialUIManager.System.Weapon, true);
                SC_TutorialUIManager.Instance.ActivateBlink(SC_TutorialUIManager.System.Weapon, true);
                DisconnectedState.SetActive(false);


                break;

            case SystemState.Initialize:
                SC_TutorialUIManager.Instance.ActivateSystem(SC_TutorialUIManager.System.Weapon, true);


                DisconnectedState.SetActive(false);

                InitializeOffState.SetActive(false);

                yield return new WaitForSeconds(0.75f);

                GeneralOffState.SetActive(false);
                InitializedState.SetActive(true);


                break;

            case SystemState.Launched:
                SC_TutorialUIManager.Instance.ActivateSystem(SC_TutorialUIManager.System.Weapon, true);
                SC_TutorialUIManager.Instance.ActivateBlink(SC_TutorialUIManager.System.Weapon, false);

                DisconnectedState.SetActive(false);
                LaunchedOffState.SetActive(false);
                ConnectedOffState.SetActive(false);
                InitializeOffState.SetActive(false);
                GeneralOffState.SetActive(false);


                yield return new WaitForSeconds(0.75f);

                InitializedState.SetActive(false);

                break;

        }
    }
    #endregion States

}
