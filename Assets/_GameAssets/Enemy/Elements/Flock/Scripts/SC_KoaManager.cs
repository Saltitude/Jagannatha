using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Le Koa Manager gère tout les Boids et le Koa, exécute les ordres du Flock Manager 
///  | Sur le prefab Flock
///  | Auteur : Zainix
/// </summary>
public class SC_KoaManager : MonoBehaviour
{ 

    [Header("References")]
    [SerializeField]
    GameObject _koaPrefab; //Prefab du Koa

    [Header("Get References")]
    [SerializeField]
    GameObject NetPlayerP;
    [SerializeField]
    SC_NetPSpawnKoa_P NetPSpawnKoa;
    [SerializeField]
    SC_MoveKoaSync syncVarKoa;

    [Header("Parameters")]
    [SerializeField]
    int maxLife = 10;
    [SerializeField]
    int KoaLife = 10;
    [SerializeField]
    float recoveryDuration = 1.5f;

    [Header("Infos")]
    [SerializeField]
    Vector3Int sensitivity;

    string koaCharID;
    int koaNumID;
    string koaID;
    int type;

    bool regeneration = false;   
    float curRecoveryTimer;

    GameObject _koa; //Koa du 

    /// <summary>
    /// Current BoidSettings
    /// </summary>
    BoidSettings curBoidSettings; //Paramètres dans le scriptableObject Settings
    FlockSettings curFlockSettings; //Paramètres dans le scriptableObject Settings

    //public ComputeShader compute; //Shader
    SC_FlockManager flockManager;

    /// <summary>
    /// Contient toute la liste des guides actuels 
    /// </summary>
    List<Transform> _guideList;

    /// <summary>
    /// Guide actuel du Koa
    /// </summary>
    Transform _curKoaGuide; //Target

    /// <summary>
    /// List de tout les boids contenus dans le Flock
    /// </summary>
    Boid[] _boidsTab; //Tableau contenant les boids

    float respawnTimer;
    int spawnCount;
    public bool isActive;

    ParticleSystem vfx_Hit;
    GameObject SFX_Explosion;

    /// <summary>
    /// Avant le start, instanciation
    /// </summary>
    public void Initialize(Transform newGuide, int newSpawnCount, BoidSettings newSettings, FlockSettings flockSettings, Vector3Int newSensitivity)
    {
        GetReferences();
        regeneration = true;
        curRecoveryTimer = 0;
        recoveryDuration = 1.5f;
        flockManager = newGuide.GetComponent<SC_FlockManager>();
        curFlockSettings = flockSettings;
        spawnCount = newSpawnCount;

     
        switch (flockSettings.attackType)
        {
            case FlockSettings.AttackType.none:

                koaCharID = "Neutral";
                type = 0;
                break;

            case FlockSettings.AttackType.Bullet:

                koaCharID = "Bullet";
                type = 1;
                break;

            case FlockSettings.AttackType.Laser:

                koaCharID = "Laser";
                type = 2;
                break;

            case FlockSettings.AttackType.Kamikaze:

                koaCharID = "Kamikaze";
                type = 3;
                break;
        }

        koaNumID = SC_BoidPool.Instance.GetFlockID();
        koaID = koaCharID + " #" + koaNumID;

        //Instanciation des list de Boid et de Guide
        _boidsTab = SC_BoidPool.Instance.GetBoid(curFlockSettings.maxBoid);

        _guideList = new List<Transform>();

        //Récupération du comportement initial
        curBoidSettings = newSettings;
        if (SC_EnemyManager.Instance.curPhaseIndex != 0) sensitivity = newSensitivity;
        else sensitivity = new Vector3Int(3, 5, 4);
        //Ajout du premier guide a la liste
        _guideList.Add(newGuide);

        respawnTimer = 0;
        if (_koaPrefab != null)
        {
            _koa = NetPSpawnKoa.SpawnKoa();
            _koa.transform.position = transform.position;
            _koa.GetComponent<SC_KoaCollider>().Initialize(this);
            vfx_Hit = _koa.GetComponent<ParticleSystem>();

            syncVarKoa = _koa.GetComponent<SC_MoveKoaSync>();
            syncVarKoa.InitOPKoaSettings(sensitivity, flockSettings.spawnTimer, koaID, KoaLife, maxLife, type, newGuide);
            syncVarKoa.curboidNumber = spawnCount;
            syncVarKoa.curboidNumber = flockSettings.maxBoid;
        }

    }


    void InitBoids()
    {
 

        //Initialisation de tout les boids
        for (int i = 0; i < spawnCount; i++)
        {
            Boid boid = _boidsTab[i];

            //Transform
            boid.transform.position = transform.position; //Déplacement à la position
            boid.transform.forward = Random.insideUnitSphere; //Rotation random

           
            //Lance l'initialisation de celui-ci avec le comportement initial et le premier guide
            boid.Initialize(curBoidSettings, _guideList[0], sensitivity, this, type);
       
        }

        //Instantie le Koa
        if (_koa != null)
            _koa.GetComponent<SC_MoveKoaSync>().SetPilotMeshActive();
        _boidsTab[1].GetComponent<BoxCollider>().enabled = false;
        _boidsTab[1].GetComponentInChildren<MeshRenderer>().enabled = false;

    }

    void Update()
    {
        if (isActive)
        {
            if (_koa != null)
            {
                KoaBehavior();
            }
            if (curFlockSettings.regenerationRate != 0 && regeneration)
            {
                respawnTimer += Time.deltaTime;
                if (respawnTimer > (60f / curFlockSettings.regenerationRate))
                {
                    respawnTimer = 0;
                    //GenerateNewBoid();
                }
            }
            if (!regeneration)
            {
                curRecoveryTimer += Time.deltaTime;
                if (curRecoveryTimer >= recoveryDuration)
                {
                    regeneration = true;
                }
            }
        }

    }

    void KoaBehavior()
    {
        switch (curBoidSettings.koaBehavior)
        {
            case (BoidSettings.KoaBehavior.Boid):

                int boidIndex = 1;
                while(!_boidsTab[boidIndex].isActive)
                {
                    boidIndex++;
                }
                _koa.transform.position = Vector3.Lerp(_koa.transform.position, _boidsTab[boidIndex].transform.position, 5 * Time.deltaTime);
                

                break;


            case (BoidSettings.KoaBehavior.Center):

                _koa.transform.position = Vector3.Lerp(_koa.transform.position, flockManager.transform.position, 5 * Time.deltaTime);

                break;

            case (BoidSettings.KoaBehavior.Average):

                float x = 0;
                float y = 0;
                float z = 0;

                int nbActive = 0;
                for (int i = 0; i < _boidsTab.Length; i++)
                {
                    if (_boidsTab[i].isActive)
                    {
                        

                        if (Vector3.Distance(_boidsTab[i].transform.position, flockManager.transform.position)<150)
                        {
                            nbActive++;
                            x += _boidsTab[i].transform.position.x;
                            y += _boidsTab[i].transform.position.y;
                            z += _boidsTab[i].transform.position.z;
                        }
                
                        else
                        {
                            _boidsTab[i].DestroyBoid(Boid.DestructionType.Solo);
                        }
                      
                    
                    }
                }


                x /= nbActive;
                y /= nbActive;
                z /= nbActive;

                _koa.transform.position = Vector3.Lerp(_koa.transform.position, new Vector3(x, y, z), 5* Time.deltaTime);
          
         

                break;

            case (BoidSettings.KoaBehavior.Cover):


                break;

        }
    }
    void GetReferences()
    {

        if (NetPlayerP == null)
            NetPlayerP = SC_CheckList.Instance.NetworkPlayerPilot;

        if (NetPlayerP != null && NetPSpawnKoa == null)
            NetPSpawnKoa = NetPlayerP.GetComponent<SC_NetPSpawnKoa_P>();

    }

    /// <summary>
    /// Lance le split de la nuée en fonction des guides envoyé par le Flock Manager | Param : List<Transform> nouveau guides (la division dépends du nombre de guide)
    /// </summary>
    /// <param name="newGuides"></param>
    public void Split(List<Transform> newGuides)
    {

        int splitNumber = newGuides.Count;//Nombre de division en fonciton du nombre de guides envoyé
        _guideList.Clear(); //Vide la guide liste de tout les guides actuel

        //Ajoute tout les novueaux guide a la list de guides
        foreach (Transform t in newGuides)
        {
            _guideList.Add(t);
        }


        //---------------------- Répartition des boids sur les guides de facon proportionnel
        int nbActiveBoid = 0;
        for (int i = 0; i < _boidsTab.Length; i++)
        {
            if (_boidsTab[i].isActive)
            {
                nbActiveBoid++;
            }
        }

        int all = nbActiveBoid; //Total des boids
        int div = splitNumber; //Total de guides
        float val = all / div; //Nombre de boids par Guides

        //Affectation des guides
        for (int i = 0; i < div; i++)
        {
            for (int j = Mathf.CeilToInt(val * i); j < Mathf.CeilToInt(val * (i + 1)); j++)
            {
                _boidsTab[j].GetComponent<Boid>().target = _guideList[i];
            }
        }
        //Si impaire, réparti le dernier boid sur une target
        _boidsTab[all - 1].GetComponent<Boid>().target = _guideList[div - 1];

    }


    /// <summary>
    /// Changement de comportement des boids | Param : BoidSettings Nouveau comportement <> bool poids vers la target supèrieur pour le Koa
    /// </summary>
    /// <param name="newSettings"></param>
    /// <param name="KoaTargetWeight"></param>
    public void SetBehavior(BoidSettings newSettings)
    {

        curBoidSettings = newSettings; 
        for (int i = 0; i < _boidsTab.Length; i++)
        {
            _boidsTab[i].SetNewSettings(curBoidSettings);

        }
        if(SC_FixedData.Instance.GetBoidIndex(newSettings) != 14)
        syncVarKoa.SetNewBehavior(SC_FixedData.Instance.GetBoidIndex(newSettings));

    }

    public void GenerateNewBoid()
    {

        for (int i = 0; i < curFlockSettings.maxBoid; i++)
        {
            if (!_boidsTab[i].isActive)
            {
                _boidsTab[i].transform.position = _koa.transform.position; //Déplacement à la position
                _boidsTab[i].transform.forward = Random.insideUnitSphere; //Rotation random
                int rnd = 0;
                if (_guideList.Count > 1)
                {
                    rnd = Random.Range(1, _guideList.Count);
                }
                _boidsTab[i].Initialize(curBoidSettings, _guideList[rnd], sensitivity, this, type);
                return;
            }
        }
    }

    public void GetHit(Vector3 gunSensitivity)
    {
        if (KoaLife > 0)
        {
            float x = Mathf.Abs((int)gunSensitivity.x - (int)sensitivity.x);
            float y = Mathf.Abs((int)gunSensitivity.y - (int)sensitivity.y);
            float z = Mathf.Abs((int)gunSensitivity.z - (int)sensitivity.z);

            float ecart = x + y + z;


            float power = 6 - ecart;

            if (power < 0) power = 0;
            float powerPerCent = (power / 6) * 100;
            

            if(SC_Debug_Mng.Instance.b_weapon_Cheatcode)
            {
                powerPerCent = SC_Debug_Mng.Instance.powerPerCent;
            }

            if (powerPerCent > 0)
            {
                KoaLife -= (int)((powerPerCent * maxLife) / 100) / 3;
                if (KoaLife <= 0)
                    KoaLife = 0;
                syncVarKoa.SetCurLife(KoaLife);
                if (KoaLife <= 0)
                {
                    AnimDestroy();
                }
                SC_HitMarker.Instance.HitMark(SC_HitMarker.HitType.Koa);

                vfx_Hit.Play();
            }

            if (powerPerCent >= curFlockSettings.flightReactionMinSensibility)
            {
                flockManager.ReactionFlock(SC_FlockManager.PathType.Flight);
            }
            if (powerPerCent < curFlockSettings.hitReactionMaxSensibility)
            {
                flockManager.ReactionFlock(SC_FlockManager.PathType.ReactionHit);
            }

            ///DEBUG
            if (gunSensitivity.x == 100)
            {
                KoaLife = 0;
                syncVarKoa.SetCurLife(0);
                AnimDestroy();
            }
        }


    }

    void AnimDestroy()
    {
        CustomSoundManager.Instance.PlaySound(_koa.gameObject, "SFX_Explosion_Flock", false, 0.1f, false);
        flockManager.AnimDestroy();

        //SetBehavior(DeathSettings);
        foreach (Boid b in _boidsTab) b.DestroyBoid(Boid.DestructionType.Massive);
        isActive = false;
        Destroy(_koa.gameObject);
        Invoke("DestroyFlock", 1);
    }

    void DestroyFlock()
    {
        Destroy(_koa.gameObject);
        flockManager.DestroyFlock();
        Destroy(this.gameObject);
    }


    public void ActivateKoa()
    {
        InitBoids();
        isActive = true;
    }

    public void StopRegeneration()
    {
        regeneration = false;
        curRecoveryTimer = 0;

    }

    public void BoidHit(Vector3 gunSensitivity)
    {
        float x = Mathf.Abs((int)gunSensitivity.x - (int)sensitivity.x);
        float y = Mathf.Abs((int)gunSensitivity.y - (int)sensitivity.y);
        float z = Mathf.Abs((int)gunSensitivity.z - (int)sensitivity.z);

        float ecart = x + y + z;
        float power = 6 - ecart;

        if (power < 0) power = 0;
        float powerPerCent = (power / 6) * 100;
        if (SC_Debug_Mng.Instance.b_weapon_Cheatcode)
        {
            powerPerCent = SC_Debug_Mng.Instance.powerPerCent;
        }

        if (powerPerCent >= curFlockSettings.flightReactionMinSensibility)
        {
            flockManager.ReactionFlock(SC_FlockManager.PathType.Flight);
        }
        if(powerPerCent < curFlockSettings.hitReactionMaxSensibility)
        {
            flockManager.ReactionFlock(SC_FlockManager.PathType.ReactionHit);
        }
    }

    public void ChangeKoaState(int state)
    {
        syncVarKoa.SetCurState(state);
    }
}