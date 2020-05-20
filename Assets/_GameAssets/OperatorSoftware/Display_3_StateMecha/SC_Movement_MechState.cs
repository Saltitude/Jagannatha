using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SC_Movement_MechState : MonoBehaviour
{
    #region Singleton

    private static SC_Movement_MechState _instance;
    public static SC_Movement_MechState Instance { get { return _instance; } }

    #endregion

    [SerializeField]
    SC_UI_SystmShield _SystmShield;

    [SerializeField]
    SC_Movement_Direction dirRight;
    [SerializeField]
    SC_Movement_Direction dirLeft;

    [SerializeField]
    Image[] arcR;
    [SerializeField]
    Image[] arcL;
    [SerializeField]
    Color32 validColor;
    [SerializeField]
    Color32 breakdownColor;

    [SerializeField]
    GameObject GeneralOffState;
    [SerializeField]
    GameObject ConnectedOffState;
    [SerializeField]
    GameObject InitializeOffState;
    [SerializeField]
    GameObject LaunchedOffState;
    [SerializeField]
    GameObject LeftOffState;
    [SerializeField]
    GameObject RightOffState;
    [SerializeField]
    GameObject InitializedState;

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

        _SystmShield.simpleValue = SC_SyncVar_MovementSystem.Instance.f_MovementLife;

        updateDirection();
        updateBrokenDirection();
        CheckState();

    }

    #region States

    void CheckState()
    {

        if ( (SC_GameStates.Instance.CurState == SC_GameStates.GameState.Tutorial && SC_passwordLock.Instance.b_IsConnected) || (SC_GameStates.Instance.CurState != SC_GameStates.GameState.Tutorial && !SC_SyncVar_MovementSystem.Instance.b_BreakEngine) )
        {
            CurState = SystemState.Connected;

            if ((SC_GameStates.Instance.CurState == SC_GameStates.GameState.Tutorial && SC_SyncVar_MovementSystem.Instance.b_SeqIsCorrect) || (SC_GameStates.Instance.CurState != SC_GameStates.GameState.Tutorial && !SC_SyncVar_MovementSystem.Instance.b_MaxBreakdown))
            {
                CurState = SystemState.Initialize;

                if (SC_SyncVar_MovementSystem.Instance.b_IsLaunch)
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
                InitializedState.SetActive(false);

                break;

            case SystemState.Connected:
                ConnectedOffState.SetActive(false);
                InitializeOffState.SetActive(true);
                LaunchedOffState.SetActive(true);
                GeneralOffState.SetActive(true);
                InitializedState.SetActive(false);

                break;

            case SystemState.Initialize:
                ConnectedOffState.SetActive(false);
                InitializeOffState.SetActive(false);
                LaunchedOffState.SetActive(true);
                GeneralOffState.SetActive(false);
                InitializedState.SetActive(true);
                break;

            case SystemState.Launched:
                ConnectedOffState.SetActive(false);
                InitializeOffState.SetActive(false);
                LaunchedOffState.SetActive(false);
                GeneralOffState.SetActive(false);
                InitializedState.SetActive(false);

                break;
        }

    }

    #endregion States

    #region Directions

    void updateDirection()
    {
        if (SC_SyncVar_MovementSystem.Instance.CurDir != SC_JoystickMove.Dir.None)
        {
            if (SC_SyncVar_MovementSystem.Instance.CurDir == SC_JoystickMove.Dir.Right)
            {
                dirLeft.b_InUse = false;
                dirRight.b_InUse = true;
                dirRight.CurRotationSpeed = dirRight.speedRotateUsed;
                dirLeft.CurRotationSpeed = dirLeft.speedRotateInit;
            }
            else if (SC_SyncVar_MovementSystem.Instance.CurDir == SC_JoystickMove.Dir.Left)
            {
                dirLeft.b_InUse = true;
                dirRight.b_InUse = false;
                dirLeft.CurRotationSpeed = dirLeft.speedRotateUsed;
                dirRight.CurRotationSpeed = dirRight.speedRotateInit;
            }
        }
        else
        {
            dirLeft.b_InUse = false;
            dirRight.b_InUse = false;
            dirRight.CurRotationSpeed = dirRight.speedRotateInit;
            dirLeft.CurRotationSpeed = dirLeft.speedRotateInit;
        }
    }

    void updateBrokenDirection()
    {

        if(SC_SyncVar_MovementSystem.Instance.CurBrokenDir == SC_JoystickMove.Dir.Left)
        {

            dirLeft.b_IsBreak = true;
            dirRight.b_IsBreak = false;

            int nbBreakdown = SC_SyncVar_MovementSystem.Instance.n_BreakDownLvl;

            if(nbBreakdown !=0)
            {
                for (int i = 0; i < nbBreakdown; i++)
                {
                    arcL[i].color = breakdownColor;
                }
            }

            else
            {
                for (int i = 0; i < arcL.Length; i++)
                {
                    arcL[i].color = validColor;
                }
            }

        }

        else if (SC_SyncVar_MovementSystem.Instance.CurBrokenDir == SC_JoystickMove.Dir.Right)
        {

            dirLeft.b_IsBreak = false;
            dirRight.b_IsBreak = true;

            int nbBreakdown = SC_SyncVar_MovementSystem.Instance.n_BreakDownLvl;

            if (nbBreakdown != 0)
            {
                Debug.Log("Panne");
                for (int i = 0; i < nbBreakdown; i++)
                {
                    arcR[i].color = breakdownColor;
                }
            }

            else
            {
                
                for (int i = 0; i < arcL.Length; i++)
                {
                    arcR[i].color = validColor;
                }
            }

            dirLeft.b_IsBreak = true;
            dirRight.b_IsBreak = false;

        }

    }

    #endregion Directions

}
