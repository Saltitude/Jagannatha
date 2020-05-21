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
    GameObject InitializedState;
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

        _SystmShield.simpleValue = SC_SyncVar_MovementSystem.Instance.f_MovementLife;

        updateDirection();
        updateBrokenDirection();
        CheckState();

    }

    #region States

    void CheckState()
    {
        SystemState newState;
        if ((SC_GameStates.Instance.CurState == SC_GameStates.GameState.Tutorial && (int)SC_GameStates.Instance.CurTutoState >= (int)SC_GameStates.TutorialState.StartRepairDisplay) || (SC_GameStates.Instance.CurState != SC_GameStates.GameState.Tutorial && !SC_SyncVar_MovementSystem.Instance.b_BreakEngine) )
        {
            newState = SystemState.Connected;

            if ((SC_GameStates.Instance.CurState == SC_GameStates.GameState.Tutorial && SC_SyncVar_MovementSystem.Instance.b_SeqIsCorrect) || (SC_GameStates.Instance.CurState != SC_GameStates.GameState.Tutorial && !SC_SyncVar_MovementSystem.Instance.b_MaxBreakdown))
            {
                newState = SystemState.Initialize;

                if (SC_SyncVar_MovementSystem.Instance.b_IsLaunch)
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
                SC_TutorialUIManager.Instance.ActivateSystem(SC_TutorialUIManager.System.Motion, false);

                DisconnectedState.SetActive(true);

                break;

            case SystemState.Connected:


                SC_TutorialUIManager.Instance.ActivateSystem(SC_TutorialUIManager.System.Motion, false);

                ConnectedOffState.SetActive(false);
                InitializeOffState.SetActive(true);
                LaunchedOffState.SetActive(true);
                GeneralOffState.SetActive(true);
                InitializedState.SetActive(false);

                yield return new WaitForSeconds(0.75f);
                SC_TutorialUIManager.Instance.ActivateSystem(SC_TutorialUIManager.System.Motion, true);
                DisconnectedState.SetActive(false);

                yield return new WaitForSeconds(0.1f);
                SC_TutorialUIManager.Instance.ActivateSystem(SC_TutorialUIManager.System.Motion, false);

                DisconnectedState.SetActive(true);

                yield return new WaitForSeconds(0.1f);
                SC_TutorialUIManager.Instance.ActivateSystem(SC_TutorialUIManager.System.Motion, true);

                DisconnectedState.SetActive(false);

                yield return new WaitForSeconds(0.1f);
                SC_TutorialUIManager.Instance.ActivateSystem(SC_TutorialUIManager.System.Motion, false);

                DisconnectedState.SetActive(true);

                yield return new WaitForSeconds(0.1f);
                SC_TutorialUIManager.Instance.ActivateSystem(SC_TutorialUIManager.System.Motion, true);
                SC_TutorialUIManager.Instance.ActivateBlink(SC_TutorialUIManager.System.Motion, true);

                DisconnectedState.SetActive(false);


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
        int nbBreakdown = SC_SyncVar_MovementSystem.Instance.n_BreakDownLvl;

        if (nbBreakdown != 0)
        {
            if(SC_SyncVar_MovementSystem.Instance.CurBrokenDir == SC_JoystickMove.Dir.Left)
            {
                for (int i = 0; i < nbBreakdown; i++)
                {
                    arcL[i].color = breakdownColor;
                }
                for (int i = 0; i < arcR.Length; i++)
                {
                    arcR[i].color = validColor;
                }
            } 
            if(SC_SyncVar_MovementSystem.Instance.CurBrokenDir == SC_JoystickMove.Dir.Right)
            {
                for (int i = 0; i < nbBreakdown; i++)
                {
                    arcR[i].color = breakdownColor;
                }
                for (int i = 0; i < arcL.Length; i++)
                {
                    arcL[i].color = validColor;
                }
            }

            if (SC_SyncVar_MovementSystem.Instance.b_BreakEngine)
            {
                for (int i = 0; i < arcR.Length; i++)
                {
                    arcR[i].color = breakdownColor;
                }
                for (int i = 0; i < arcL.Length; i++)
                {
                    arcL[i].color = breakdownColor;
                }
            }

        }
        else
        {
            for (int i = 0; i < arcR.Length; i++)
            {
                arcR[i].color = validColor;
            }

            for (int i = 0; i < arcL.Length; i++)
            {
                arcL[i].color = validColor;
            }
        }


        //if (SC_SyncVar_MovementSystem.Instance.CurBrokenDir == SC_JoystickMove.Dir.Left)
        //{

        //    dirLeft.b_IsBreak = true;
        //    dirRight.b_IsBreak = false;

        //    int nbBreakdown = SC_SyncVar_MovementSystem.Instance.n_BreakDownLvl;

        //    if(nbBreakdown !=0)
        //    {
        //        for (int i = 0; i < nbBreakdown; i++)
        //        {
        //            arcL[i].color = breakdownColor;
        //        }
        //    }

        //    else
        //    {
        //        for (int i = 0; i < arcL.Length; i++)
        //        {
        //            arcL[i].color = validColor;
        //        }
        //    }

        //}

        //if (SC_SyncVar_MovementSystem.Instance.CurBrokenDir == SC_JoystickMove.Dir.Right)
        //{

        //    dirLeft.b_IsBreak = false;
        //    dirRight.b_IsBreak = true;

        //    int nbBreakdown = SC_SyncVar_MovementSystem.Instance.n_BreakDownLvl;

        //    if (nbBreakdown != 0)
        //    {
        //        for (int i = 0; i < nbBreakdown; i++)
        //        {
        //            arcR[i].color = breakdownColor;
        //        }
        //    }

        //    else
        //    {
                
        //        for (int i = 0; i < arcL.Length; i++)
        //        {
        //            arcR[i].color = validColor;
        //        }
        //    }


        //}
 
    }

    #endregion Directions

}
