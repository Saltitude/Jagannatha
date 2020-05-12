using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_Display_MechState : MonoBehaviour
{
    #region Singleton

    private static SC_Display_MechState _instance;
    public static SC_Display_MechState Instance { get { return _instance; } }

    #endregion

    [SerializeField]
    SC_UI_SystmShield _SystmShield;

    [SerializeField]
    GameObject GeneralOffState;
    [SerializeField]
    GameObject ConnectedOffState;
    [SerializeField]
    GameObject InitializeOffState;
    [SerializeField]
    GameObject LaunchedOffState;

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

        _SystmShield.simpleValue = SC_SyncVar_DisplaySystem.Instance.f_Displaylife;

        CheckState();

    }

    #region States

    void CheckState()
    {

        if ( (SC_GameStates.Instance.CurState == SC_GameStates.GameState.Tutorial && SC_passwordLock.Instance.b_IsConnected) || (SC_GameStates.Instance.CurState != SC_GameStates.GameState.Tutorial && !SC_SyncVar_DisplaySystem.Instance.b_BreakEngine) )
        {

            CurState = SystemState.Connected;

            if((SC_GameStates.Instance.CurState == SC_GameStates.GameState.Tutorial && SC_SyncVar_DisplaySystem.Instance.f_CurNbOfBd == 0) || (SC_GameStates.Instance.CurState != SC_GameStates.GameState.Tutorial && !SC_SyncVar_DisplaySystem.Instance.b_MaxBreakdown))
            {

                CurState = SystemState.Initialize;

                if (SC_SyncVar_DisplaySystem.Instance.b_IsLaunch)
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
                ConnectedOffState.SetActive(true);
                InitializeOffState.SetActive(true);
                LaunchedOffState.SetActive(true);
                GeneralOffState.SetActive(true);
                break;

            case SystemState.Connected:               
                ConnectedOffState.SetActive(false);
                InitializeOffState.SetActive(true);
                LaunchedOffState.SetActive(true);
                GeneralOffState.SetActive(true);
                break;

            case SystemState.Initialize:               
                ConnectedOffState.SetActive(false);
                InitializeOffState.SetActive(false);
                LaunchedOffState.SetActive(true);
                GeneralOffState.SetActive(true);
                break;

            case SystemState.Launched:               
                ConnectedOffState.SetActive(false);
                InitializeOffState.SetActive(false);
                LaunchedOffState.SetActive(false);
                GeneralOffState.SetActive(false);
                break;

        }

    }

    #endregion States

}
