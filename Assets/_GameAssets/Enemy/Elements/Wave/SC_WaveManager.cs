using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Script gerant la wave d'enemy actuel selon le Setting donné
///  | Sur le prefab EnemyManager(à instantié une fois)
///  | Auteur : Zainix
/// </summary>
public class SC_WaveManager : MonoBehaviour
{

    #region Singleton

    private static SC_WaveManager _instance;
    public static SC_WaveManager Instance { get { return _instance; } }

    #endregion
    //---------------------------------------------------------------------//
    //---------------------------- VARIABLES ------------------------------//
    //---------------------------------------------------------------------//
    #region Variables
    WaveSettings _curWaveSettings;
    public bool waveStarted;
    public bool waveEnded;
    public bool b_nextWave;
    //Récupère les prefabs
    [SerializeField]
    GameObject _FlockPrefab; //Prefab du flock gérant la totalité de la nuée (guide compris)
    [SerializeField]
    GameObject _MultiFlockManagerPrefab; //Préfab du mutli flock manager, instantié lors d'un rassemblment de plusieurs flock

    BezierSolution.BezierSpline[] spawnSplines;


    List<GameObject> _FlockList; //Contient la totalité des flocks présents dans le jeu


    float curBackupTimer = 0;
    bool backupSend;
    


    Vector3Int sensitivityA;
    Vector3Int sensitivityB;
    Vector3Int sensitivityC;
    Vector3Int sensitivityD;

    #endregion
    //---------------------------------------------------------------------//

    #region Start/Update
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


        _FlockList = new List<GameObject>(); //Instantiation de la list de flock
        
        resetVariables();

    }

    void Start()
    {
        spawnSplines = SC_SpawnInfo.Instance.GetBezierSplines();
        b_nextWave = true;
    }

    void Update()
    {
        BackupUpdate();

        //Debug
        if(Input.GetKeyDown(KeyCode.G))
        {
            _FlockList[0].GetComponent<SC_FlockManager>()._SCKoaManager.GetHit(new Vector3(100,0,0));
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            foreach(GameObject b in _FlockList)
            {
                b.GetComponent<SC_FlockManager>()._SCKoaManager.GetHit(new Vector3(100, 0, 0));
            }           
        }
    }
    #endregion

    //---------------------------------------------------------------------//
    //------------------------ INITIALIZE NEW WAVE ------------------------//
    //---------------------------------------------------------------------//
    #region Initialize New Wave
    public void InitializeWave(WaveSettings newWaveSettings)
    {
            resetVariables();
            _curWaveSettings = newWaveSettings;

        resetVariables();
        _curWaveSettings = newWaveSettings;

        if (!_curWaveSettings.backup)
            backupSend = true;
        StartCoroutine(SpawnInitialFlock());

        if( _curWaveSettings.backup)
        {
            for (int i = 0; i < _curWaveSettings.backupSpawnFlock.Length; i++)
            {
                StartCoroutine(SC_KoaSpawn.Instance.GoTargetPos(SC_PhaseManager.Instance.curWaveIndex, 1, i, _curWaveSettings.backupSpawnPosition[i], 350, 3.5f));
            }
        }
        else if (SC_PhaseManager.Instance.curWaveIndex + 1 < SC_PhaseManager.Instance.waves.Length )
        {
            WaveSettings nextWave = SC_PhaseManager.Instance.waves[SC_PhaseManager.Instance.curWaveIndex + 1];
            for (int i = 0; i < nextWave.initialSpawnFlock.Length; i++)
            {
                StartCoroutine(SC_KoaSpawn.Instance.GoTargetPos(SC_PhaseManager.Instance.curWaveIndex+1, 0, i, nextWave.initialSpawnPosition[i], 350, 3.5f));
            }
        }
    }
    IEnumerator SpawnInitialFlock()
    {
        _FlockList.Clear();

        for (int i = 0; i < _curWaveSettings.initialSpawnFlock.Length; i++)
        {
            SpawnNewFlock(_curWaveSettings.initialSpawnFlock[i], i);


            StartCoroutine(SC_KoaSpawn.Instance.SpawnCoro(SC_PhaseManager.Instance.curWaveIndex, 0, i, _curWaveSettings.initialSpawnPosition[i]));

            yield return new WaitForSeconds(_curWaveSettings.timeBetweenSpawnInitial);            
        }
        StopCoroutine(SpawnInitialFlock());
        waveStarted = true;
        curBackupTimer = 0;


    }
    #endregion
    //---------------------------------------------------------------------//

    //---------------------------------------------------------------------//
    //------------------------ BACKUP MANAGEMENT --------------------------//
    //---------------------------------------------------------------------//
    #region Backup Management

    IEnumerator SpawnBackupFlock()
    {
        for (int i = 0; i < _curWaveSettings.backupSpawnFlock.Length; i++)
        {
            SpawnNewFlock(_curWaveSettings.backupSpawnFlock[i], i, true);

            StartCoroutine(SC_KoaSpawn.Instance.SpawnCoro(SC_PhaseManager.Instance.curWaveIndex,1,i, _curWaveSettings.backupSpawnPosition[i]));

            yield return new WaitForSeconds(_curWaveSettings.timeBetweenSpawnBackup);
        }

        WaveSettings nextWave = null;
        if (SC_PhaseManager.Instance.curWaveIndex+1 <= SC_PhaseManager.Instance.waves.Length)
             nextWave = SC_PhaseManager.Instance.waves[SC_PhaseManager.Instance.curWaveIndex + 1];


        if (nextWave != null)
        for (int i = 0; i < nextWave.initialSpawnFlock.Length; i++)
        {
            StartCoroutine(SC_KoaSpawn.Instance.GoTargetPos(SC_PhaseManager.Instance.curWaveIndex + 1, 0, i, nextWave.initialSpawnPosition[i], 200, 3.5f));
        }
    }



    void BackupUpdate()
    {
       
        if(waveStarted)
        {
            
            if (_curWaveSettings.backup && !backupSend)
            {
                curBackupTimer += Time.deltaTime;

                if (_FlockList.Count <= _curWaveSettings.flockLeftBeforeBackup)
                {
                    StartCoroutine(SpawnBackupFlock());
                    backupSend = true;
                }
                else if (curBackupTimer >= _curWaveSettings.timeBeforeBackup && _curWaveSettings.timeBeforeBackup!=-1)
                {
                    StartCoroutine(SpawnBackupFlock());
                    backupSend = true;

                }
            }
        }

    }

    #endregion
    //---------------------------------------------------------------------//


    //---------------------------------------------------------------------//
    //---------------------------- END WAVE -------------------------------//
    //---------------------------------------------------------------------//
    #region End Wave
    public void FlockDestroyed(GameObject flock)
    {
        for (int i = 0; i < _FlockList.Count; i++)
        {
            if (_FlockList[i] == flock)
            {
                _FlockList.RemoveAt(i);
                if (SC_GameStates.Instance.CurState == SC_GameStates.GameState.Game)
                {

                    SC_EnemyManager.Instance.Progress.value += 100 * 1f / SC_KoaSpawn.Instance.nb_totalFlock;
                }

            }
        }


        if (_FlockList.Count == 0 && backupSend )
        {
            waveEnded = true;
            SC_PhaseManager.Instance.EndWave();
        }
    }

    #endregion
    //---------------------------------------------------------------------//

    //---------------------------------------------------------------------//
    //--------------------------- UTILITIES -------------------------------//
    //---------------------------------------------------------------------//
    #region Utilites
    /// <summary>
    /// Invoque un nouveau Flock
    /// </summary>
    void SpawnNewFlock(FlockSettings flockSettings,int index, bool backup = false)
    {
        Vector3Int newSensitivity = new Vector3Int(0, 0, 0);
        Vector3Int baseSensitivity = new Vector3Int(0, 0, 0);

        switch(flockSettings.attackType)
        {
            case FlockSettings.AttackType.none:

                baseSensitivity = sensitivityA;

                break;


            case FlockSettings.AttackType.Bullet:

                baseSensitivity = sensitivityB;

                break;


            case FlockSettings.AttackType.Laser:

                baseSensitivity = sensitivityC;

                break;     
            
            
            case FlockSettings.AttackType.Kamikaze:

                baseSensitivity = sensitivityD;

                break;
        }


        int[] tabValue = new int[3];

        tabValue[0] = baseSensitivity.x;
        tabValue[1] = baseSensitivity.y;
        tabValue[2] = baseSensitivity.z;

        int remainingOffset = 2;

        for (int i = 0; i<3;i++)
        {
            if(remainingOffset>0)
            {
                int newValue = GetVariationSensitivity(tabValue[i]);
                if (newValue != tabValue[i])
                {
                    tabValue[i] = newValue;
                    remainingOffset -= 1;
                }
                if(i == 2 && remainingOffset >0)
                {
                    i = 0;
                }

            }
        }
       
  
        newSensitivity = new Vector3Int(tabValue[0], tabValue[1], tabValue[2]);
        
        //Instantiate new flock
        GameObject curFlock = Instantiate(_FlockPrefab);

        //Add new flock to the flock list
        _FlockList.Add(curFlock);

        BezierSolution.BezierSpline spawnSpline;
        if (backup) spawnSpline = spawnSplines[_curWaveSettings.backupSpawnPosition[index]];
        else spawnSpline = spawnSplines[_curWaveSettings.initialSpawnPosition[index]];

        //Initialize flock
        curFlock.GetComponent<SC_FlockManager>().InitializeFlock(flockSettings, spawnSpline, newSensitivity);
    }


 

    /// <summary>
    /// Fusion de plusieur flock | Parametre : List de flock a fusionnés
    /// </summary>
    /// <param name="flockToMerge"></param>
    void StartMultiFlock(List<GameObject> flockToMerge)
    {
        GameObject newMultiFlock =  Instantiate(_MultiFlockManagerPrefab, flockToMerge[0].transform.position, transform.rotation); //Instantie un nouveau MultiFlock manager à la position du premier flock de la list
        newMultiFlock.GetComponent<SC_MultiFlockManager>().Initialize(flockToMerge); //Initialize la fusion des flock dans la liste
    }


    void resetVariables()
    {
        curBackupTimer = 0;

        backupSend = false;
        waveEnded = false;
        waveStarted = false;
        GenerateNewSensitivity();

    }

    //---------------------------SENSITIVITY-------------------------------//
    void GenerateNewSensitivity()
    {
        int x;
        int y;
        int z;

        x = Random.Range(0, 6);
        y = Random.Range(0, 6);
        z = Random.Range(0, 6);

        sensitivityA = new Vector3Int(x, y, z);

        x = Random.Range(0, 6);
        y = Random.Range(0, 6);
        z = Random.Range(0, 6);

        sensitivityB = new Vector3Int(x, y, z);

        sensitivityC = new Vector3Int(GetRangedValue(x), GetRangedValue(y), GetRangedValue(z));

        sensitivityD = new Vector3Int(5, 5, 5);
    }

    int GetRangedValue(int baseValue)
    {
        int newValue;

        if(baseValue >= 3)
        {
            newValue = baseValue - Random.Range(2, 5);
            if (newValue < 0)
            {
                newValue = 0;
            }
        }
        else
        {
            newValue = baseValue + Random.Range(2, 5);
            if (newValue > 5)
            {
                newValue = 5;
            }
        }

        return newValue;

    }

    int GetVariationSensitivity(int baseValue)
    {
        int rnd = Random.Range(-1, 2);
        int newValue = baseValue + rnd;
        if (newValue < 0) newValue = 0; if (newValue > 5) newValue = 5;
        return newValue;
        
    }

    //---------------------------------------------------------------------//


    #endregion
    //---------------------------------------------------------------------//

}
