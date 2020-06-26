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
    float maxLife = 10;
    [SerializeField]
    public float KoaLife = 10;
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
    Animator koaEmissiveAnimator;

    [SerializeField]
    GameObject PS_KoaExplosion;
    [SerializeField]
    GameObject PS_BossExplosion;
 
    GameObject curExplosion;
    [SerializeField]
    ParticleSystem.MainModule curExplosionPS;
    Color32 AmenoColor;

    public GameObject Explosion;
    //bool changeSensitivity = false;
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

    bool deathAnimation = false;

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
            case FlockSettings.FlockType.none:

                koaCharID = "Neutral";
                type = 0;
                break;

            case FlockSettings.FlockType.Bullet:

                koaCharID = "Bullet";
                type = 1;
                break;

            case FlockSettings.FlockType.Laser:

                koaCharID = "Laser";
                type = 2;
                break;

            case FlockSettings.FlockType.Kamikaze:

                koaCharID = "Kamikaze";
                type = 3;
                break;

            case FlockSettings.FlockType.Boss:

                koaCharID = "Boss";
                type = 4;
                break;
        }


        if (type == 4)
        {
            koaID = koaCharID;
            KoaLife = flockSettings.startingLife;
        }

        else
        {
            koaNumID = SC_BoidPool.Instance.GetFlockID();
            koaID = koaCharID + " #" + koaNumID;

        }

        //Instanciation des list de Boid et de Guide
        _boidsTab = SC_BoidPool.Instance.GetBoid(curFlockSettings.maxBoid);

        _guideList = new List<Transform>();

        //Récupération du comportement initial
        curBoidSettings = newSettings;
        
        if (SC_EnemyManager.Instance.curPhaseIndex != 0 && flockSettings.attackType != FlockSettings.FlockType.Boss) sensitivity = newSensitivity;
        else sensitivity = SC_WaveManager.Instance.GenerateSensitivityP();
        //Ajout du premier guide a la liste
        _guideList.Add(newGuide);

        respawnTimer = 0;
        if (_koaPrefab != null)
        {
            if (flockSettings.attackType == FlockSettings.FlockType.Boss)
            {
                _koa = NetPSpawnKoa.SpawnKoa(true);
                flockManager.BossAux = _koa.transform.GetChild(3).gameObject;
            }
            else _koa = NetPSpawnKoa.SpawnKoa(false);


            _koa.transform.position = transform.position;
            _koa.GetComponent<SC_KoaCollider>().Initialize(this);

            flockManager.KoaMainAnimator = _koa.transform.GetChild(0).GetComponent<Animator>();
            koaEmissiveAnimator = _koa.transform.GetChild(0).GetChild(0).GetComponent<Animator>();
            
            flockManager.KoaEmissiveAnimator = koaEmissiveAnimator;

            vfx_Hit = _koa.GetComponent<ParticleSystem>();

            syncVarKoa = _koa.GetComponent<SC_MoveKoaSync>();
            flockManager.moveKoaSync = syncVarKoa;

            bool spawnScale = true;
            if (type == 4) spawnScale = flockSettings.spanwScale;

            syncVarKoa.InitOPKoaSettings(sensitivity, flockSettings.spawnTimer, koaID, KoaLife, maxLife, type, newGuide, spawnScale);
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
                    GenerateNewBoid();
                }

            }

            if (!regeneration)
            {

                curRecoveryTimer += Time.deltaTime;

                if (curRecoveryTimer >= recoveryDuration)
                    regeneration = true;

            }
        }

        else if (!deathAnimation)
        {
            _koa.transform.position = gameObject.transform.position;
        }

    }

    void KoaBehavior()
    {
        switch (curBoidSettings.koaBehavior)
        {
            case (BoidSettings.KoaBehavior.Boid):

                int boidIndex = 1;

                while (!_boidsTab[boidIndex].isActive)
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

                        if (Vector3.Distance(_boidsTab[i].transform.position, flockManager.transform.position) < 200)
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

                if (_koa != null && nbActive != 0)
                    _koa.transform.position = Vector3.Lerp(_koa.transform.position, new Vector3(x, y, z), 5 * Time.deltaTime);

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
        if (all > 0 && div > 0)
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
        if (SC_FixedData.Instance.GetBoidIndex(newSettings) != 14 && syncVarKoa != null)
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
            //Debug.Log(powerPerCent);
            Sc_LaserFeedBack.Instance.SetLaserSize(Mathf.FloorToInt(powerPerCent));
            if (SC_Debug_Mng.Instance.b_weapon_Cheatcode)
            {
                powerPerCent = SC_Debug_Mng.Instance.powerPerCent;
            }

            if (powerPerCent > 0)
            {

                //Debug.Log("damage done = " + ((powerPerCent * maxLife) / 100) / 3 * curFlockSettings.damageMultiplicator);

                KoaLife -= (powerPerCent * maxLife) / 100 / 3 * curFlockSettings.damageMultiplicator;
                if (KoaLife <= 0)
                    KoaLife = 0;
                syncVarKoa.SetCurLife(KoaLife);
                if (KoaLife <= 0)
                {
                    AnimDestroy();
                }
                SC_HitMarker.Instance.HitMark(SC_HitMarker.HitType.Koa);

                if (!vfx_Hit.isPlaying)
                {
                    vfx_Hit.Stop();
                    vfx_Hit.Play();

                }

                //if (KoaLife <= 5 && changeSensitivity)
                //{
                //    changeSensitivity = false;
                //    sensitivity = SC_WaveManager.Instance.GenerateSensitivityP();
                //    syncVarKoa.SetNewSensitivity(sensitivity);
                //}
            }

            if (powerPerCent >= curFlockSettings.flightReactionMinSensibility)
            {
                flockManager.ReactionFlock(SC_FlockManager.PathType.Flight);
            }
            if (powerPerCent < curFlockSettings.hitReactionMaxSensibility)
            {
                flockManager.ReactionFlock(SC_FlockManager.PathType.ReactionHit);
            }

            if(curFlockSettings.attackType == FlockSettings.FlockType.Boss)
            {
                if(KoaLife <= curFlockSettings.fleeingLife)
                {
                    KoaLife = curFlockSettings.fleeingLife;
                    SFX_Explosion = CustomSoundManager.Instance.PlaySound(_koa.gameObject, "SFX_Boss_Flee", false, 0.4f, false);
                    flockManager.ReactionFlock(SC_FlockManager.PathType.bossFlight);
                }
            }

            ///DEBUG
            if (gunSensitivity.x == 100)
            {
                KoaLife = 0;
                syncVarKoa.SetCurLife(KoaLife);
                if (KoaLife <= 0)
                {
                    AnimDestroy();
                }
                //if (KoaLife <= 5 && changeSensitivity)
                //{
                //    changeSensitivity = false;
                //    sensitivity = SC_WaveManager.Instance.GenerateSensitivityP();
                //    syncVarKoa.SetNewSensitivity(sensitivity);
                //}
            }
        }


    }

    public void AnimDestroy()
    {
        float timeBeforeDestroyFlock = 3f;

        if (type != 4)
        {
            SFX_Explosion = CustomSoundManager.Instance.PlaySound(_koa.gameObject, "SFX_Explosion_Flock", false, 1f, false);

        }

        if (type == 4 && curFlockSettings.bossPhase == 3)
        {
            SFX_Explosion = CustomSoundManager.Instance.PlaySound(_koa.gameObject, "SFX_BossDeath", false, 1f, false);
            flockManager.BossAuxAnimator.SetBool("Pos", false);
            flockManager.BossAuxAnimator.SetBool("FullShield", false);
            flockManager.BossAuxAnimator.SetBool("Spike", false);
            flockManager.BossAuxAnimator.SetBool("Cube", false);
            flockManager.BossAuxAnimator.SetBool("Death", false);
            flockManager.BossAuxAnimator.SetBool("Death", true);
            timeBeforeDestroyFlock = 4f;
        }
        else if(type == 4 && curFlockSettings.bossPhase != 3)
        {
            flockManager.BossAuxAnimator.SetBool("Pos", false);
            flockManager.BossAuxAnimator.SetBool("FullShield", false);
            flockManager.BossAuxAnimator.SetBool("Spike", false);
            flockManager.BossAuxAnimator.SetBool("Cube", false);
            flockManager.BossAuxAnimator.SetBool("Death", false);
            flockManager.BossAuxAnimator.SetBool("Death", true);
            timeBeforeDestroyFlock = 0f;
        }


        //SetBehavior(DeathSettings);
        foreach (Boid b in _boidsTab) b.DestroyBoid(Boid.DestructionType.Massive);
        isActive = false;
        deathAnimation = true;
        //Destroy(_koa.gameObject);
        if(type != 4)
        {
            flockManager.AnimDestroy();
            curExplosion = Instantiate(PS_KoaExplosion, _koa.transform);
            SetVFXColor();

            syncVarKoa.HideOPMesh();
            Invoke("HideTheKoa", 1.3f);
            Invoke("DestroyFlock", timeBeforeDestroyFlock);

        }
        else
        {
            flockManager.StopAttack();

            syncVarKoa.HideOPMesh();
            Invoke("DestroyBoss", timeBeforeDestroyFlock);
        }


    }

    public void SetVFXColor()
    {
        AmenoColor = SC_UI_Cockpit_FrequenceLine.Instance.Color1;
        Gradient gradiend = new Gradient();
        GradientColorKey[] colorKeys = new GradientColorKey[3];
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];

        alphaKeys[0].time = 0;
        alphaKeys[0].alpha = 1;

        alphaKeys[1].time = 1;
        alphaKeys[1].alpha = 1;

        colorKeys[0].color = AmenoColor;
        colorKeys[1].color = AmenoColor;
        colorKeys[2].color = AmenoColor;

        gradiend.SetKeys(colorKeys, alphaKeys);
        gradiend.SetKeys(colorKeys, alphaKeys);

        Gradient gradiendSpe = new Gradient();
        GradientColorKey[] colorKeysSpe = new GradientColorKey[3];
        GradientAlphaKey[] alphaKeysSpe = new GradientAlphaKey[2];

        alphaKeysSpe[0].time = 0;
        alphaKeysSpe[0].alpha = 1;

        alphaKeysSpe[1].time = 1;
        alphaKeysSpe[1].alpha = 1;

        colorKeysSpe[0].color = Sc_LaserFeedBack.Instance.CurColor;
        colorKeysSpe[1].color = Sc_LaserFeedBack.Instance.CurColor;
        colorKeysSpe[2].color = Sc_LaserFeedBack.Instance.CurColor;

        gradiendSpe.SetKeys(colorKeysSpe, alphaKeysSpe);
        gradiendSpe.SetKeys(colorKeysSpe, alphaKeysSpe);

        for (int i = 0; i < curExplosion.transform.childCount; i++)
        {
            curExplosionPS = curExplosion.transform.GetChild(i).GetComponent<ParticleSystem>().main;
            curExplosionPS.startColor = gradiend;
            for (int j = 0; j < curExplosion.transform.GetChild(i).transform.childCount; j++)
            {
                if (i == 1 && (j == 3 || j == 1))
                {
                    //Debug.Log(i +"  "+ j);
                    curExplosionPS = curExplosion.transform.GetChild(i).transform.GetChild(j).GetComponent<ParticleSystem>().main;
                    curExplosionPS.startColor = gradiendSpe;
                }
                else
                {
                    curExplosionPS = curExplosion.transform.GetChild(i).transform.GetChild(j).GetComponent<ParticleSystem>().main;
                    curExplosionPS.startColor = gradiend;
                }
            }
        }

    }


    void HideTheKoa()
    {
        var i = 0;
        while (i <= 4)
        {
            _koa.transform.GetChild(0).GetChild(i).GetComponent<MeshRenderer>().enabled = false;
            i++;
        }
    }

    void DestroyFlock()
    {
 

        deathAnimation = false;

        Destroy(_koa.gameObject);
        flockManager.DestroyFlock();
        Destroy(this.gameObject);
    }

    void DestroyBoss()
    {
        if(curFlockSettings.bossPhase == 3)
        {
            curExplosion = Instantiate(PS_BossExplosion, _koa.transform);
            SetVFXColor();
            Invoke("DestroyFlock", 3f);
            SFX_Explosion = CustomSoundManager.Instance.PlaySound(_koa.gameObject, "SFX_Explosion_Flock", false, 1f, false);
            flockManager.AnimDestroy();
            Invoke("HideTheKoa", 1.3f);


        }
        else
        {
            HideTheKoa();
            flockManager.AnimDestroy();

            DestroyFlock();
        }

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
        //Debug.Log(powerPerCent);
        Sc_LaserFeedBack.Instance.SetLaserSize(Mathf.FloorToInt(powerPerCent));
        if (SC_Debug_Mng.Instance.b_weapon_Cheatcode)
        {
            powerPerCent = SC_Debug_Mng.Instance.powerPerCent;
        }

        if (powerPerCent >= curFlockSettings.flightReactionMinSensibility)
        {
            flockManager.ReactionFlock(SC_FlockManager.PathType.Flight);
        }
        if (powerPerCent < curFlockSettings.hitReactionMaxSensibility)
        {
            flockManager.ReactionFlock(SC_FlockManager.PathType.ReactionHit);
        }
        else
        {
            koaEmissiveAnimator.SetBool("Hit", true);
            StartCoroutine(ResetBool("Hit", false));
        }
    }

    IEnumerator ResetBool(string boolReset, bool statut)
    {
        yield return new WaitForEndOfFrame();
        koaEmissiveAnimator.SetBool(boolReset, statut);
        StopAllCoroutines();
    }

    public void ChangeKoaState(int state)
    {
        if (syncVarKoa != null)
            syncVarKoa.SetCurState(state);
    }
}