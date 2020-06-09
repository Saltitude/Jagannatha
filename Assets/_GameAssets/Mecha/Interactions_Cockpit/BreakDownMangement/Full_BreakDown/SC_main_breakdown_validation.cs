using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_main_breakdown_validation : MonoBehaviour
{
    #region Singleton

    private static SC_main_breakdown_validation _instance;
    public static SC_main_breakdown_validation Instance { get { return _instance; } }

    #endregion

    public bool isValidated = false;
    GameObject SFX_Validate;
    GameObject SFX_ValidateSound;

    private void Awake()
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

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.V))
            Validate();

    }

    public void bringDown()
    {
        this.GetComponent<Animator>().SetBool("BOUGE", true);
    }

    public void bringUp()
    {
        this.GetComponent<Animator>().SetBool("BOUGE", false);
        this.GetComponent<Animator>().SetBool("BOUGE", false);
    }

    public void textBlink()
    {
        this.GetComponent<Animator>().SetBool("B_Blink", true);
        SC_SyncVar_Main_Breakdown.Instance.checkReboot(true);
    }
    public void textStopBlink()
    {
        this.GetComponent<Animator>().SetBool("B_Blink", false);
        SC_SyncVar_Main_Breakdown.Instance.checkReboot(false);
    }

    public void Validate()
    {

        //Debug.Log("Valid - Display - " + SC_BreakdownDisplayManager.Instance.CurNbOfBreakdown + " - Weap - " + SC_WeaponBreakdown.Instance.CurNbOfBreakdown + " - Move - " + SC_MovementBreakdown.Instance.b_SeqIsCorrect);

        //On check pour savoir si tous les systemes sont déjà réparés en additionant leurs compteurs
        if(SC_BreakdownDisplayManager.Instance.CurNbOfBreakdown + SC_WeaponBreakdown.Instance.CurNbOfBreakdown == 0 && SC_MovementBreakdown.Instance.b_SeqIsCorrect)
        {
            //Debug.Log("Validation");
            isValidated = true;
            SyncVariables();
            SFX_Validate = CustomSoundManager.Instance.PlaySound(gameObject, "SFX_p_voice_rebooting_system", false, 1f);
            SFX_ValidateSound = CustomSoundManager.Instance.PlaySound(gameObject, "SFX_p_reboot_button_validate", false, 0.1f);
            if(SC_GameStates.Instance.CurTutoState == SC_GameStates.TutorialState.Reboot)
            {
                SC_GameStates.Instance.RpcSetState(SC_GameStates.GameState.Tutorial2);
            }
            else
            {
                SC_AmbiancePilot.Instance.RestartAmbiance();
            }
        }

        else
        {
            SFX_Validate = CustomSoundManager.Instance.PlaySound(gameObject, "SFX_p_voice_system_failure", false, 0.5f);
            SFX_ValidateSound = CustomSoundManager.Instance.PlaySound(gameObject, "SFX_p_reboot_button_fail", false, 0.1f);
        }

        SC_MainBreakDownManager.Instance.CheckBreakdown();
        textStopBlink();

        //Désactive le timer
        SC_BreakdownOnBreakdownAlert.Instance.StopAllCoroutines();

    }

    void SyncVariables()
    {
        SC_SyncVar_DisplaySystem.Instance.b_IsLaunch = isValidated;
        SC_SyncVar_MovementSystem.Instance.b_IsLaunch = isValidated;
        SC_SyncVar_WeaponSystem.Instance.b_IsLaunch = isValidated;
    }

}
