﻿using System.Collections;
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
    public enum TutorialState { StartTutorial1, StartRepairDisplay, EndRepairDisplay, StartRepairWeapon,EndRepairWeapon, StartRepairMotion, EndRepairMotion, Reboot, StartTutorial2, TutorialEnd}
    public GameState CurState;
    public TutorialState CurTutoState;
    public bool Disp = false;
    public bool Mov = false;
    public bool Weap = false;

    public bool skipTutoSafety = false;

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
            skipTutoSafety = false;
            StartCoroutine(SkipTuto());
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

    IEnumerator SkipTuto()
    {
        if(isServer)
        {
            ChangeGameState(GameState.Tutorial2);
            while(!skipTutoSafety)
            {
                yield return new WaitForEndOfFrame();
            }
           
            SC_MainBreakDownManager.Instance.DisplayBreakdownSC.RepairBreakdownDebug();
            SC_MainBreakDownManager.Instance.WeaponBreakdownSC.RepairBreakdownDebug();
            SC_MainBreakDownManager.Instance.MovementBreakdownSC.RepairBreakdownDebug();
            SC_main_breakdown_validation.Instance.Validate();
        }

        if (!isServer)
        {
            SC_TutorialUIManager.Instance.ActivateSystem(SC_TutorialUIManager.System.Display, true);
            SC_TutorialUIManager.Instance.ActivateSystem(SC_TutorialUIManager.System.Weapon, true);
            SC_TutorialUIManager.Instance.ActivateSystem(SC_TutorialUIManager.System.Motion, true);



            SC_Display_MechState.Instance.UpdateVar();
            SC_Movement_MechState.Instance.UpdateVar();
            SC_Weapon_MechState.Instance.UpdateVar();


            SC_passwordLock.Instance.cheatCode = true;


            SC_TutorialUIManager.Instance.ActivateBlink(SC_TutorialUIManager.System.Display, false);
            SC_TutorialUIManager.Instance.ActivateBlink(SC_TutorialUIManager.System.Weapon, false);
            SC_TutorialUIManager.Instance.ActivateBlink(SC_TutorialUIManager.System.Motion, false);
            SC_instruct_op_manager.Instance.ChangeUIOnMat();

        }

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
                ChangeTutoGameState(TutorialState.StartTutorial1);

                break;

            case GameState.Tutorial2:

                ChangeTutoGameState(TutorialState.StartTutorial2);

                if (isServer)
                {
                    skipTutoSafety = true;
                }

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
            case TutorialState.StartTutorial1:
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


                    SC_instruct_op_manager.Instance.DeactivateImage(0);
                    SC_instruct_op_manager.Instance.DeactivateImage(1);
                    SC_instruct_op_manager.Instance.DeactivateImage(2);
                    SC_instruct_op_manager.Instance.DeactivateImage(3);
                    SC_instruct_op_manager.Instance.Deactivate(17);
                    SC_instruct_op_manager.Instance.Activate(18);
                }
                SC_AmbiancePilot.Instance.PlayAmbiance(SC_AmbiancePilot.Ambiance.TutoRepa);

                ChangeTutoGameState(TutorialState.StartRepairDisplay);

                break;

            //----------------------------DISPLAY---------------------------------------//
            case TutorialState.StartRepairDisplay:
                if(!isServer)
                {
                    //Debut Display
                    SC_Display_MechState.Instance.UpdateVar();
                }
                break;

            case TutorialState.EndRepairDisplay:
           
                if (!isServer)
                {
                    //Fin Display
                    SC_TutorialUIManager.Instance.ActivateBlink(SC_TutorialUIManager.System.Display, false);
                    SC_TutorialUIManager.Instance.CTA(SC_TutorialUIManager.System.Display, true,true);
                    SC_instruct_op_manager.Instance.ChangeMaterial(SC_instruct_op_manager.ChangeMat.ReturnDisplay);

                }
                ChangeTutoGameState(TutorialState.StartRepairWeapon);
                
                break;
               
            //----------------------------WEAPON---------------------------------------//
            case TutorialState.StartRepairWeapon:
                if(!isServer)
                {

                    //Debut Weapon
                }
                break;

            case TutorialState.EndRepairWeapon:
                if (!isServer)
                {
                    SC_TutorialUIManager.Instance.CTA(SC_TutorialUIManager.System.Display, false, true);


                    SC_TutorialUIManager.Instance.ActivateBlink(SC_TutorialUIManager.System.Weapon, false);
                    SC_TutorialUIManager.Instance.CTA(SC_TutorialUIManager.System.Weapon, true, true);
                    SC_instruct_op_manager.Instance.ChangeMaterial(SC_instruct_op_manager.ChangeMat.ReturnWeapon);

                }
                else
                    CustomSoundManager.Instance.PlaySound(gameObject, "SFX_p_weaponRepar", false, 0.3f);
                ChangeTutoGameState(TutorialState.StartRepairMotion);

                break;
            
            //----------------------------MOTION---------------------------------------//
            case TutorialState.StartRepairMotion:
                if(!isServer)
                {

                }
                break;

            case TutorialState.EndRepairMotion:
                if (!isServer)
                {
                    SC_TutorialUIManager.Instance.CTA(SC_TutorialUIManager.System.Weapon, false, true);


                    SC_TutorialUIManager.Instance.ActivateBlink(SC_TutorialUIManager.System.Motion, false);
                    SC_TutorialUIManager.Instance.CTA(SC_TutorialUIManager.System.Motion, true, true);
                    SC_instruct_op_manager.Instance.ChangeMaterial(SC_instruct_op_manager.ChangeMat.ReturnMotion);

                }
                ChangeTutoGameState(TutorialState.Reboot);

                break;


            case TutorialState.Reboot:
                if(!isServer)
                {
                    SC_TutorialUIManager.Instance.ActivateBlink(SC_TutorialUIManager.System.Reboot, true);

                }
                break;

           
            //------------------------------------- PART 2 ---------------------------------------//

            case TutorialState.StartTutorial2:
                if(isServer)
                {
                    SC_EnemyManager.Instance.Initialize();

                }
                if (!isServer)
                {
                    SC_TutorialUIManager.Instance.CTA(SC_TutorialUIManager.System.Motion, false, true);

                    SC_instruct_op_manager.Instance.ChangeUIOnMat();

                    SC_instruct_op_manager.Instance.ActivateImage(0);
                    SC_instruct_op_manager.Instance.ActivateImage(1);
                    SC_instruct_op_manager.Instance.ActivateImage(2);
                    SC_instruct_op_manager.Instance.ActivateImage(3);

                    SC_instruct_op_manager.Instance.Activate(17);
                    SC_instruct_op_manager.Instance.Activate(19);
                    SC_instruct_op_manager.Instance.Deactivate(18);

                    SC_TutorialUIManager.Instance.ActivateBlink(SC_TutorialUIManager.System.Reboot, false);
                }

                SC_AmbiancePilot.Instance.PlayAmbiance(SC_AmbiancePilot.Ambiance.TutoFlock);

                break;


            case TutorialState.TutorialEnd:

                if(isServer)
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
