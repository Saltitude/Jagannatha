using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_MainBreakDownManager : MonoBehaviour, IF_BreakdownManager
{

    #region Singleton

    private static SC_MainBreakDownManager _instance;
    public static SC_MainBreakDownManager Instance { get { return _instance; } }

    #endregion

    #region Variables

    [Header("References")]
    public GameObject Mng_Checklist;
    public GameObject Mng_BreakDownAlert;
    public GameObject screenController;
    GameObject BreakDownAudioSource;
    private SC_breakdown_displays_screens sc_screens_controller;

    [Header("References breakDown SC")]
    [SerializeField]
    public SC_BreakdownDisplayManager DisplayBreakdownSC;
    GameObject RenderSystem;
    [SerializeField]
    public SC_WeaponBreakdown WeaponBreakdownSC;
    GameObject WeaponSystem;
    [SerializeField]
    public SC_MovementBreakdown MovementBreakdownSC;
    GameObject MoveSystem;

    [Header("Breakdown Infos")]
    public int nbOfBreakDownBeforeTotalBreak;
    public bool b_BreakEngine = false;
    int SoundSourceNumb = 0;


    [Header("Display System Infos")]
    [SerializeField, Range(0,10)]
    int Displaylife = 10;
    [SerializeField]
    int n_MaxBreakInterB4MaxBD = 0;
    [SerializeField]
    bool ScreensMaxBreak = false;
    [SerializeField]
    int NbOfBreakDisplay = 0;

    [Header("Weapon System Infos")]
    [SerializeField, Range(0, 10)]
    int WeaponLife = 10;
    [SerializeField]
    bool WeaponMaxBreak = false;
    [SerializeField]
    int NbOfBreakWeapon = 0;


    [Header("Movement System Infos")]
    [SerializeField, Range(0, 10)]
    int MovementLife = 10;
    [SerializeField]
    bool MoveMaxBreak = false;
    [SerializeField]
    int MoveBreakLvl = 0;    

    /*
    public bool b_BreakMove = false;
    public bool b_BreakScreen = false;
    public bool b_BreakWeapons = false;
    */

    /*
    [Header("Old Var (Normally no Used)")]
    public bool b_BreakMiniGun = false;
    GameObject MiniGunSystem;
    public bool b_BreakShrapnel = false;
    GameObject ShrapnelSystem;
    public bool b_BreakFlameThrower = false;
    GameObject FlameSystem;
    */

    #endregion Variables

    #region Init

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

    void Start()
    {       
        UpdateSystemInfos();
        //ForceUpdate();
    }

    #endregion Init

    void Update()
    {
        DebugInput();
    }

    void DebugInput()
    {

        if (Input.GetKeyDown(KeyCode.Y))
        {
            SC_BreakdownDisplayManager.Instance.CheckBreakdown();
            Debug.Log("display : " +SC_BreakdownDisplayManager.Instance.CurNbOfBreakdown);
            SC_WeaponBreakdown.Instance.CheckBreakdown();
            Debug.Log("weapon : " +SC_WeaponBreakdown.Instance.CurNbOfBreakdown);
            SC_MovementBreakdown.Instance.CheckBreakdown();
            Debug.Log("movement" +SC_MovementBreakdown.Instance.n_BreakDownLvl);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SC_BreakdownDisplayManager.Instance.RepairBreakdownDebug();
            SC_WeaponBreakdown.Instance.RepairBreakdownDebug();
            SC_MovementBreakdown.Instance.RepairBreakdownDebug();
            CheckBreakdown();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            CauseDamageOnSystem(FlockSettings.AttackFocus.Display, 1);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            CauseDamageOnSystem(FlockSettings.AttackFocus.Movement, 1);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            CauseDamageOnSystem(FlockSettings.AttackFocus.Weapon, 1);
        }

    }

    public void CheckBreakdown()
    {      

        UpdateSystemInfos();
        //ForceUpdate();

        SyncSystemStates();

        //Si on est pas encore en panne Generale
        //Ici on additionne toutes les pannes des sytemes pour savoir si on déclanche la Generale
        if (!b_BreakEngine && SC_BreakdownDisplayManager.Instance.CurNbOfBreakdown + SC_WeaponBreakdown.Instance.CurNbOfBreakdown + SC_MovementBreakdown.Instance.n_BreakDownLvl >= nbOfBreakDownBeforeTotalBreak)
        {
            
            //Debug.Log("CheckBd - ToBd");

            //On s'assure que chaque system ai au moins une panne
            //important doit etre avant le changement de booleen puisque checké dans le lancement de panne locale

            if (SC_WeaponBreakdown.Instance.CurNbOfBreakdown == 0)
                SC_WeaponBreakdown.Instance.StartNewBreakdown(1);

            if (SC_MovementBreakdown.Instance.n_BreakDownLvl == 0)
                SC_MovementBreakdown.Instance.StartNewBreakdown(1);

            if (SC_BreakdownDisplayManager.Instance.CurNbOfBreakdown == 0)
                SC_BreakdownDisplayManager.Instance.StartNewBreakdown(1);

            //

            //b_BreakEngine = true;
            SetBreakEngine(true);

            //On met en pause les Wave


            //FB Alert
            if (SC_GameStates.Instance.CurState == SC_GameStates.GameState.Game)
            {
                SC_WaveManager.Instance.b_nextWave = false;
                SC_BreakdownOnBreakdownAlert.Instance.LaunchGlobalAlert();
                SC_AmbiancePilot.Instance.StopAmbiance();

                SC_FogBreakDown.Instance.BreakDownDensity();
                SC_LightAlarm.Instance.BreakDownLight();
                if (SoundSourceNumb == 0)
                {
                    BreakDownAudioSource = CustomSoundManager.Instance.PlaySound(gameObject, "SFX_p_breackdown_alarm", true, 0.1f);
                    SoundSourceNumb += 1;
                }
            }

            //on fout tous les systemes en panne à balle
            SC_breakdown_displays_screens.Instance.PanneAll();


            //descendre le bouton de validation
            SC_main_breakdown_validation.Instance.isValidated = false;
            SC_main_breakdown_validation.Instance.textStopBlink();
            SC_main_breakdown_validation.Instance.bringDown();

        }

        //Si on est en panne Generale
        else if (b_BreakEngine)
        {

            //Les System ont été reparé
            if(SC_BreakdownDisplayManager.Instance.CurNbOfBreakdown == 0 && SC_WeaponBreakdown.Instance.CurNbOfBreakdown == 0 && SC_MovementBreakdown.Instance.b_SeqIsCorrect)
            {

                //En Attente de Reboot (Validation)
                if (!SC_main_breakdown_validation.Instance.isValidated)
                {

                    //Debug.Log("CheckBd - WaitValid");

                    //Fait clignoter le Text du bouton
                    SC_main_breakdown_validation.Instance.textBlink();

                    //Désactive le timer
                    SC_BreakdownOnBreakdownAlert.Instance.StopAllCoroutines();

                    /*
                    if (SC_GameStates.Instance.CurTutoState == SC_GameStates.TutorialState.Tutorial1_8)
                        SC_GameStates.Instance.ChangeTutoGameState(SC_GameStates.TutorialState.Tutorial1_9);*/

                    SC_FogBreakDown.Instance.ClearDensity();
                    SC_LightAlarm.Instance.ClearLight();

                }

                //Reboot (Validation)
                else if (SC_main_breakdown_validation.Instance.isValidated)
                {

                    //Debug.Log("CheckBd - IsValid");
                    //SC_main_breakdown_validation.Instance.isValidated = false;

                    if (SC_GameStates.Instance.CurState == SC_GameStates.GameState.Game)
                    {
                        
                        if (BreakDownAudioSource != null)
                        {

                            if (BreakDownAudioSource.GetComponent<AudioSource>() != null)
                            {
                                if (BreakDownAudioSource.GetComponent<AudioSource>().isPlaying)
                                {
                                    //Debug.Log(BreakDownAudioSource.GetComponent<AudioClip>().name);
                                    BreakDownAudioSource.GetComponent<AudioSource>().Stop();
                                    SoundSourceNumb = 0;
                                }
                   
                            }
                        }
                        else
                        {
                            Debug.LogError("BreakDownAudioSource est null ;)");

                        }

                        SC_BreakdownOnBreakdownAlert.Instance.StopGlobalAlert();

                        //protection pour les kamikazes en fin de wave
                        if (SC_WaveManager.Instance.b_nextWave == false)
                        {

                            SC_WaveManager.Instance.b_nextWave = true;

                            if (SC_WaveManager.Instance.waveEnded == true)
                                SC_PhaseManager.Instance.EndWave();

                        }

                    }

                    //Sortie de la Panne Globale
                    //b_BreakEngine = false;
                    SetBreakEngine(false);

                    //remonter le bouton de validation
                    SC_main_breakdown_validation.Instance.bringUp();

                    //on répare touuuus les systemes
                    SC_BreakdownDisplayManager.Instance.EndBreakdown();
                    SC_WeaponBreakdown.Instance.EndBreakdown();
                    SC_MovementBreakdown.Instance.EndBreakdown();

                    ////changement de state du tuto (Effectué dans l'action reboot)
                    //if (SC_GameStates.Instance.CurState == SC_GameStates.GameState.Tutorial)
                    //    SC_GameStates.Instance.ChangeGameState(SC_GameStates.GameState.Tutorial2);

                }

            }

        }

        if(SC_GameStates.Instance.CurState == SC_GameStates.GameState.Tutorial)
        SetTutoState();

        SyncSystemsLifes();

    }

    /// <summary>
    /// Fonction de prise de dommage | 
    /// </summary>
    /// <param name="attackFocus">System Visé</param>
    /// <param name="DmgValue">Quantitée de Degats</param>
    public void CauseDamageOnSystem(FlockSettings.AttackFocus attackFocus, int DmgValue)
    {

        SC_BreakdownDisplayManager.Instance.CheckBreakdown();
        SC_WeaponBreakdown.Instance.CheckBreakdown();
        SC_MovementBreakdown.Instance.CheckBreakdown();
        
        if ((SC_BreakdownDisplayManager.Instance.CurNbOfBreakdown + SC_WeaponBreakdown.Instance.CurNbOfBreakdown + SC_MovementBreakdown.Instance.n_BreakDownLvl) < nbOfBreakDownBeforeTotalBreak && !b_BreakEngine)
        {

            switch (attackFocus)
            {

                ////////////////////////////////////////////////////////////////////////////////////////////DISPLAY
                case FlockSettings.AttackFocus.Display:

                    if (!SC_BreakdownDisplayManager.Instance.b_MaxBreakdown)
                        Displaylife -= DmgValue;

                    else
                    {

                        int rnd;
                        rnd = Random.Range(0, 2);

                        if (rnd == 0 && !SC_MovementBreakdown.Instance.b_MaxBreakdown)
                            CauseDamageOnSystem(FlockSettings.AttackFocus.Movement, DmgValue);
                        else if (rnd != 0 || !SC_WeaponBreakdown.Instance.b_MaxBreakdown)
                            CauseDamageOnSystem(FlockSettings.AttackFocus.Weapon, DmgValue);

                    }

                    if (Displaylife <= 0)
                    {
                        SC_BreakdownDisplayManager.Instance.StartNewBreakdown(2);
                        Displaylife = 10;
                    }

                    break;

                ////////////////////////////////////////////////////////////////////////////////////////////MOVEMENT
                case FlockSettings.AttackFocus.Movement:

                    if (!SC_MovementBreakdown.Instance.b_MaxBreakdown)
                        MovementLife -= DmgValue;

                    else
                    {
                        if (!SC_BreakdownDisplayManager.Instance.b_MaxBreakdown)
                            CauseDamageOnSystem(FlockSettings.AttackFocus.Display, DmgValue);
                        else if (!SC_WeaponBreakdown.Instance.b_MaxBreakdown)
                            CauseDamageOnSystem(FlockSettings.AttackFocus.Weapon, DmgValue);
                    }
                        
                    if (MovementLife <= 0)
                    {
                        SC_MovementBreakdown.Instance.StartNewBreakdown(1);
                        StartCoroutine(resetLifeMovement());
                    }

                    break;

                ////////////////////////////////////////////////////////////////////////////////////////////WEAPON
                case FlockSettings.AttackFocus.Weapon:

                    if (!SC_WeaponBreakdown.Instance.b_MaxBreakdown)
                        WeaponLife -= DmgValue;

                    else
                    {
                        if (!SC_BreakdownDisplayManager.Instance.b_MaxBreakdown)
                            CauseDamageOnSystem(FlockSettings.AttackFocus.Display, DmgValue);
                        else if (!SC_MovementBreakdown.Instance.b_MaxBreakdown)
                            CauseDamageOnSystem(FlockSettings.AttackFocus.Movement, DmgValue);
                    }                       

                    if (WeaponLife <= 0)
                    {
                        SC_WeaponBreakdown.Instance.StartNewBreakdown(1);
                        StartCoroutine(resetLifeWeapon());
                    }


                    break;

            }

        }

        CheckBreakdown();

    }

    IEnumerator resetLifeWeapon ()
    {
        yield return 0;
        WeaponLife = 10;
        SyncSystemsLifes();
    }
    IEnumerator resetLifeMovement()
    {
        yield return 0;
        MovementLife = 10;
        SyncSystemsLifes();
    }

    /// <summary>
    /// Syncronis l'etat des Sytems | 
    /// En Panne ou Non |
    /// FONCTION A UTILITE DOUTEUSE | Marche mais peut etre qu'on peut la delete, a voir avec au moment d'une potentielle seconde croisade sur les breakdown
    /// </summary>
    void SyncSystemStates()
    {
        
        //Display
        if (SC_BreakdownDisplayManager.Instance.CurNbOfBreakdown > 0)
            SC_SyncVar_Main_Breakdown.Instance.onPanneDisplayChange(true);

        else
            SC_SyncVar_Main_Breakdown.Instance.onPanneDisplayChange(false);

        //Weapon
        if (SC_WeaponBreakdown.Instance.CurNbOfBreakdown > 0)
            SC_SyncVar_Main_Breakdown.Instance.onPanneWeaponChange(true);

        else
            SC_SyncVar_Main_Breakdown.Instance.onPanneWeaponChange(false);

        //Movement
        if (!SC_MovementBreakdown.Instance.b_SeqIsCorrect)
            SC_SyncVar_Main_Breakdown.Instance.onPanneMovementChange(true);

        else
            SC_SyncVar_Main_Breakdown.Instance.onPanneMovementChange(false);

  

    }

    void SetBreakEngine(bool Target)
    {
        b_BreakEngine = Target;
        SC_JoystickMove.Instance.b_BreakEngine = b_BreakEngine;
        SC_WeaponManager.Instance.b_BreakEngine = b_BreakEngine;
    }

    void SyncSystemsLifes()
    {
        SC_SyncVar_DisplaySystem.Instance.f_Displaylife = Ratio(Displaylife, 10, 1);
        SC_SyncVar_DisplaySystem.Instance.b_BreakEngine = b_BreakEngine;
        SC_SyncVar_MovementSystem.Instance.f_MovementLife = Ratio(MovementLife, 10, 1);
        SC_SyncVar_MovementSystem.Instance.b_BreakEngine = b_BreakEngine;
        SC_SyncVar_WeaponSystem.Instance.f_WeaponLife = Ratio(WeaponLife, 10, 1);
        SC_SyncVar_WeaponSystem.Instance.b_BreakEngine = b_BreakEngine;
    }

    public void UpdateSystemInfos()
    {

        n_MaxBreakInterB4MaxBD = SC_BreakdownDisplayManager.Instance.n_MaxBreakInterB4MaxBD;

        NbOfBreakDisplay = SC_BreakdownDisplayManager.Instance.CurNbOfBreakdown;
        NbOfBreakWeapon = SC_WeaponBreakdown.Instance.CurNbOfBreakdown;
        MoveBreakLvl = SC_MovementBreakdown.Instance.n_BreakDownLvl;

        ScreensMaxBreak = SC_BreakdownDisplayManager.Instance.b_MaxBreakdown;
        WeaponMaxBreak = SC_WeaponBreakdown.Instance.b_MaxBreakdown;
        MoveMaxBreak = SC_MovementBreakdown.Instance.b_MaxBreakdown;

    }

    void SetTutoState()
    {
        if (SC_GameStates.Instance.CurTutoState == SC_GameStates.TutorialState.StartRepairDisplay && SC_BreakdownDisplayManager.Instance.CurNbOfBreakdown == 0)
        {
            SC_GameStates.Instance.ChangeTutoGameState(SC_GameStates.TutorialState.EndRepairDisplay);
            
            SC_breakdown_displays_screens.Instance.TutoDisplayRepair();
        }

        else if (SC_GameStates.Instance.CurTutoState == SC_GameStates.TutorialState.StartRepairWeapon && SC_WeaponBreakdown.Instance.CurNbOfBreakdown == 0)
        {
            SC_GameStates.Instance.ChangeTutoGameState(SC_GameStates.TutorialState.EndRepairWeapon);
        }
        else if (SC_GameStates.Instance.CurTutoState == SC_GameStates.TutorialState.StartRepairMotion && SC_MovementBreakdown.Instance.n_BreakDownLvl == 0)
        {
            SC_GameStates.Instance.ChangeTutoGameState(SC_GameStates.TutorialState.EndRepairMotion);
        }
    
    }

    float Ratio(float inputValue, float inputMax, float outputMax, float inputMin = 0.0f, float outputMin = 0.0f)
    {
        float product = (inputValue - inputMin) / (inputMax - inputMin);
        float output = ((outputMax - outputMin) * product) + outputMin;
        return output;
    }

    public void ForceUpdate()
    {
        UpdateSystemInfos();
        SyncSystemsLifes();
        SC_BreakdownDisplayManager.Instance.ForceUpdate();
        SC_MovementBreakdown.Instance.ForceUpdate();
        SC_WeaponBreakdown.Instance.ForceUpdate();
    }

}
