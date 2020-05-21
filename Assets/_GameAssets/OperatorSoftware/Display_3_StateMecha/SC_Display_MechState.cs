﻿using System.Collections;
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

        _SystmShield.simpleValue = SC_SyncVar_DisplaySystem.Instance.f_Displaylife;

        CheckState();

    }

    #region States

    void CheckState()
    {
        SystemState newState;
        if ((SC_GameStates.Instance.CurState == SC_GameStates.GameState.Tutorial && (int)SC_GameStates.Instance.CurTutoState >= (int)SC_GameStates.TutorialState.StartRepairDisplay) || (SC_GameStates.Instance.CurState != SC_GameStates.GameState.Tutorial && !SC_SyncVar_DisplaySystem.Instance.b_BreakEngine) )
        {

            newState = SystemState.Connected;

            if((SC_GameStates.Instance.CurState == SC_GameStates.GameState.Tutorial && SC_SyncVar_DisplaySystem.Instance.f_CurNbOfBd == 0) || (SC_GameStates.Instance.CurState != SC_GameStates.GameState.Tutorial && !SC_SyncVar_DisplaySystem.Instance.b_MaxBreakdown))
            {
                newState = SystemState.Initialize;

                if (SC_SyncVar_DisplaySystem.Instance.b_IsLaunch)
                {
                    newState = SystemState.Launched;
                }

            }

        }

        else
        {

            newState = SystemState.Disconnected;
        }


        if (newState != CurState)
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

                DisconnectedState.SetActive(true);

                break;

            case SystemState.Connected:


                ConnectedOffState.SetActive(false);

                yield return new WaitForSeconds(0.75f);

                DisconnectedState.SetActive(false);


                InitializeOffState.SetActive(true);
                LaunchedOffState.SetActive(true);
                GeneralOffState.SetActive(true);
                
                InitializedState.SetActive(false);




                break;

            case SystemState.Initialize:
                DisconnectedState.SetActive(false);

                InitializeOffState.SetActive(false);

                yield return new WaitForSeconds(0.75f);

                GeneralOffState.SetActive(false);
                InitializedState.SetActive(true);


                break;

            case SystemState.Launched:
                DisconnectedState.SetActive(false);


                LaunchedOffState.SetActive(false);

                yield return new WaitForSeconds(0.75f);

                InitializedState.SetActive(false);

                break;

        }

    }

    #endregion States

}
