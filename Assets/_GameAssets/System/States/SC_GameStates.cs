using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SC_GameStates : NetworkBehaviour
{

    #region Singleton

    private static SC_GameStates _instance;
    public static SC_GameStates Instance { get { return _instance; } }

    #endregion

    public enum GameState {Lobby, Tutorial, Tutorial2, Game, GameEnd }
    public enum TutorialState { StartTutorial, StartRepairDisplay, EndRepairDisplay, StartRepairWeapon,EndRepairWeapon, StartRepairMotion, EndRepairMotion, Reboot, Tutorial2_1, Tutorial2_2, Tutorial2_3, Tutorial2_4, Tutorial2_5, TutorialEnd}
    public GameState CurState;
    public TutorialState CurTutoState;
    public bool Disp = false;
    public bool Mov = false;
    public bool Weap = false;

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

    // Start is called before the first frame update
    void Start()
    {
        if(isServer)
            RpcSetState(GameState.Lobby);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.W))
        {
            SkipTuto();
        }
    }

    public void ChangeGameState (GameState TargetState)
    {
        if (isServer)
        {
            RpcSetState(TargetState);
            SyncSystemState(TargetState);
        }

    }
    public void ChangeTutoGameState (TutorialState TargetTutoState)
    {
        if (isServer)
        {
            RpcSetTutoState(TargetTutoState);
            SyncSystemTutoState(TargetTutoState);
        }

    }

    public void SkipTuto()
    {
        ChangeTutoGameState(TutorialState.Tutorial2_4);
    }

    [ClientRpc]
    public void RpcSetState(GameState TargetState)
    {

        CurState = TargetState;  
        
        switch (TargetState)
        {
            case GameState.Lobby:

                break;

            case GameState.Tutorial:
                //Descendre le Bouton Reboot au tuto
                ChangeTutoGameState(TutorialState.StartTutorial);

                break;

            case GameState.Tutorial2:

                ChangeTutoGameState(TutorialState.Tutorial2_1);

                break;     

            case GameState.Game:

                if (!isServer)
                {
                    SC_instruct_op_manager.Instance.Deactivate(2);
                    SC_instruct_op_manager.Instance.Deactivate(3);
                }
                    
                break;

            case GameState.GameEnd:

                if (!isServer)
                    SC_EndGameOP.Instance.EndGameDisplay();

                if(isServer)
                    SC_breakdown_displays_screens.Instance.EndScreenDisplay();

                break;

        }

    }

    [ClientRpc]
    public void RpcSetTutoState(TutorialState TargetTutoState)
    {

        CurTutoState = TargetTutoState;  
        
        switch (TargetTutoState)
        {
            case TutorialState.StartTutorial:
                if (isServer)
                {
                    SC_main_breakdown_validation.Instance.isValidated = false;
                    SC_main_breakdown_validation.Instance.textStopBlink();
                    SC_main_breakdown_validation.Instance.bringDown();
                }
                if(!isServer)
                {
                    SC_TutorialUIManager.Instance.ActivateSystem(SC_TutorialUIManager.System.Display, false);
                    SC_TutorialUIManager.Instance.ActivateSystem(SC_TutorialUIManager.System.Weapon, false);
                    SC_TutorialUIManager.Instance.ActivateSystem(SC_TutorialUIManager.System.Motion, false);
                }
                StartCoroutine(Swichtuto(1f, TutorialState.StartRepairDisplay));

                break;

            //----------------------------DISPLAY---------------------------------------//
            case TutorialState.StartRepairDisplay:
                if(!isServer)
                {
                    //Debut Display
                    SC_Display_MechState.Instance.UpdateVar();
                    SC_TutorialUIManager.Instance.ActivateSystem(SC_TutorialUIManager.System.Display,true);
                    SC_TutorialUIManager.Instance.ActivateBlink(SC_TutorialUIManager.System.Display,true);
                }
                break;

            case TutorialState.EndRepairDisplay:
                if (!isServer)
                {
                    //Fin Display
                    SC_TutorialUIManager.Instance.ActivateBlink(SC_TutorialUIManager.System.Display, false);
                }
                StartCoroutine(Swichtuto(1f, TutorialState.StartRepairWeapon));

                break;
               
            //----------------------------WEAPON---------------------------------------//
            case TutorialState.StartRepairWeapon:
                if(!isServer)
                {                    
                    //Debut Weapon
                    SC_Weapon_MechState.Instance.UpdateVar();
                    SC_TutorialUIManager.Instance.ActivateSystem(SC_TutorialUIManager.System.Weapon, true);
                    SC_TutorialUIManager.Instance.ActivateBlink(SC_TutorialUIManager.System.Weapon, true);

                }
                break;

            case TutorialState.EndRepairWeapon:
                if (!isServer)
                {
                    SC_TutorialUIManager.Instance.ActivateBlink(SC_TutorialUIManager.System.Weapon, false);
                }
                StartCoroutine(Swichtuto(1f, TutorialState.StartRepairMotion));

                break;
            
            //----------------------------MOTION---------------------------------------//
            case TutorialState.StartRepairMotion:
                if(!isServer)
                {
                    //Debut Motion
                    SC_Movement_MechState.Instance.UpdateVar();
                    SC_TutorialUIManager.Instance.ActivateSystem(SC_TutorialUIManager.System.Motion, true);
                    SC_TutorialUIManager.Instance.ActivateBlink(SC_TutorialUIManager.System.Motion, true);
                }
                break;

            case TutorialState.EndRepairMotion:
                if (!isServer)
                {
                    SC_TutorialUIManager.Instance.ActivateBlink(SC_TutorialUIManager.System.Motion, false);

                }
                StartCoroutine(Swichtuto(1f, TutorialState.Reboot));

                break;

            case TutorialState.Reboot:
                if(!isServer)
                {

                }
                break;     

           


            case TutorialState.Tutorial2_1:

                if (!isServer)
                {
                    SC_TutorialUIManager.Instance.ActivateBlink(SC_TutorialUIManager.System.Display, false);
                    SC_TutorialUIManager.Instance.ActivateBlink(SC_TutorialUIManager.System.Weapon, false);
                    SC_TutorialUIManager.Instance.ActivateBlink(SC_TutorialUIManager.System.Motion, false);

                    SC_instruct_op_manager.Instance.Deactivate(1);
                    SC_instruct_op_manager.Instance.Deactivate(13);
                    SC_instruct_op_manager.Instance.Deactivate(14);
                    SC_instruct_op_manager.Instance.Activate(0);
                    SC_instruct_op_manager.Instance.Deactivate(6);

                }

                break;

            case TutorialState.Tutorial2_2: 

                if(!isServer)
                {
                    SC_instruct_op_manager.Instance.Activate(2);
                    SC_instruct_op_manager.Instance.Deactivate(0);
                }

                break;
                
            case TutorialState.Tutorial2_3:


                break;

            case TutorialState.Tutorial2_4:
                if (isServer)
                {
                    SC_MainBreakDownManager.Instance.DisplayBreakdownSC.RepairBreakdownDebug();
                    SC_MainBreakDownManager.Instance.WeaponBreakdownSC.RepairBreakdownDebug();
                    SC_MainBreakDownManager.Instance.MovementBreakdownSC.RepairBreakdownDebug();
                    SC_main_breakdown_validation.Instance.Validate();

                }
                if (!isServer)
                {
                    SC_passwordLock.Instance.cheatCode = true;
                }
                    StartCoroutine(Swichtuto(0.5f, TutorialState.Tutorial2_5));
                break;

            case TutorialState.Tutorial2_5:
                if (!isServer)
                {
                   
                    SC_instruct_op_manager.Instance.Deactivate(0);
                    SC_instruct_op_manager.Instance.Deactivate(1);
                    SC_instruct_op_manager.Instance.Deactivate(2);
                    SC_instruct_op_manager.Instance.Deactivate(3);
                    SC_instruct_op_manager.Instance.Deactivate(4);
                    SC_instruct_op_manager.Instance.Deactivate(5);
                    SC_instruct_op_manager.Instance.Deactivate(6);
                    SC_instruct_op_manager.Instance.Deactivate(7);
                    SC_instruct_op_manager.Instance.Deactivate(8);
                    SC_instruct_op_manager.Instance.Deactivate(9);
                    SC_instruct_op_manager.Instance.Deactivate(10);
                    SC_instruct_op_manager.Instance.Deactivate(11);
                    SC_instruct_op_manager.Instance.Deactivate(12);
                    SC_instruct_op_manager.Instance.Deactivate(13);
                    SC_instruct_op_manager.Instance.Deactivate(14);

                }
                StartCoroutine(Swichtuto(0.5f, TutorialState.TutorialEnd));
                break;

            case TutorialState.TutorialEnd:
                SC_instruct_op_manager.Instance.Deactivate(0);
                SC_instruct_op_manager.Instance.Deactivate(1);
                SC_instruct_op_manager.Instance.Deactivate(2);
                SC_instruct_op_manager.Instance.Deactivate(3);
                SC_instruct_op_manager.Instance.Deactivate(4);
                SC_instruct_op_manager.Instance.Deactivate(5);
                SC_instruct_op_manager.Instance.Deactivate(6);
                SC_instruct_op_manager.Instance.Deactivate(7);
                SC_instruct_op_manager.Instance.Deactivate(8);
                SC_instruct_op_manager.Instance.Deactivate(9);
                SC_instruct_op_manager.Instance.Deactivate(10);
                SC_instruct_op_manager.Instance.Deactivate(11);
                SC_instruct_op_manager.Instance.Deactivate(12);
                SC_instruct_op_manager.Instance.Deactivate(13);
                SC_instruct_op_manager.Instance.Deactivate(14);
                ChangeGameState(GameState.Game);

                break;

        }

    }
   

    void SyncSystemState(GameState TargetState)
    {
        SC_SyncVar_DisplaySystem.Instance.CurState = TargetState;
        SC_SyncVar_MovementSystem.Instance.CurState = TargetState;
        SC_SyncVar_WeaponSystem.Instance.CurState = TargetState;
    }
    void SyncSystemTutoState(TutorialState TargetTutoState)
    {

    }
    IEnumerator Swichtuto(float duration, TutorialState TargetState)
    {
        yield return new WaitForSeconds(duration);
        ChangeTutoGameState(TargetState);
        StopAllCoroutines();
    }
}
