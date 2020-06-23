using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Le Koa Manager gère tout les Boids et le Koa, exécute les ordres du Flock Manager 
///  | Sur le prefab Flock
///  | Auteur : Zainix
/// </summary>
public class SC_KoaManagerDisplay : MonoBehaviour
{
    Vector3Int sensitivity;

    [SerializeField]
    GameObject _koaPrefab; //Prefab du Koa

    int maxLife = 10;
    int KoaLife = 10;

    char koaCharID;
    int koaNumID;
    string koaID;

    bool regeneration = true;
    float recoveryDuration;
    float curRecoveryTimer;

    GameObject _koa; //Koa du 

    GameObject Player;
    /// <summary>
    /// Current BoidSettings
    /// </summary>
    BoidSettings curBoidSettings; //Paramètres dans le scriptableObject Settings
    FlockSettings curFlockSettings; //Paramètres dans le scriptableObject Settings

    //public ComputeShader compute; //Shader
    SC_FlockDisplay flockManager;
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
    public void Initialize(Transform newGuide, int newSpawnCount, BoidSettings newSettings, FlockSettings flockSettings)
    {
        regeneration = true;
        curRecoveryTimer = 0;
        recoveryDuration = 1.5f;
        flockManager = newGuide.GetComponent<SC_FlockDisplay>();
        curFlockSettings = flockSettings;
        spawnCount = newSpawnCount;

        Player = GameObject.FindGameObjectWithTag("Player");
        switch (flockSettings.attackType)
        {
            case FlockSettings.FlockType.none:

                koaCharID = 'A';

                break;

            case FlockSettings.FlockType.Bullet:

                koaCharID = 'B';

                break;  

            case FlockSettings.FlockType.Laser:

                koaCharID = 'C';

                break;
        }
        koaNumID = SC_BoidPool.Instance.GetFlockID();
        koaID = koaCharID + "#"+koaNumID;

        //Instanciation des list de Boid et de Guide
        _boidsTab = SC_BoidPool.Instance.GetBoid(curFlockSettings.maxBoid);
        
        _guideList = new List<Transform>();

        //Récupération du comportement initial
        curBoidSettings = newSettings;


        sensitivity = new Vector3Int(3, 5, 4);
        //Ajout du premier guide a la liste
        _guideList.Add(newGuide);

        respawnTimer = 0;
        _koa = Instantiate(_koaPrefab);
        _koa.transform.position = transform.position;

        Invoke("InitBoids",1f);



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

            SC_KoaManager s = null;
            //Lance l'initialisation de celui-ci avec le comportement initial et le premier guide
            boid.Initialize(curBoidSettings, _guideList[0], sensitivity, s, (int)curFlockSettings.attackType);
        }

    }

    void Update()
    {
        if(isActive)
        {
            if (_koa != null)
            {
                KoaBehavior();
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
                    {
                        regeneration = true;
                    }
                }
            }
        }

    }

    void KoaBehavior()
    {
        switch (curBoidSettings.koaBehavior)
        {
            case (BoidSettings.KoaBehavior.Boid):

                _koa.transform.position = _boidsTab[1].transform.position;

                break;


            case (BoidSettings.KoaBehavior.Center):

                _koa.transform.position = Vector3.Lerp(_koa.transform.position, flockManager.transform.position, curBoidSettings.maxSpeed * Time.deltaTime);

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
                        nbActive++;
                        x += _boidsTab[i].transform.position.x;
                        y += _boidsTab[i].transform.position.x;
                        z += _boidsTab[i].transform.position.x;
                    }
                }

                x /= nbActive;
                y /= nbActive;
                z /= nbActive;

                x += flockManager.transform.position.x;
                y += flockManager.transform.position.y;
                z += flockManager.transform.position.z;

                _koa.transform.position = new Vector3(x, y, z);

                break;

            case (BoidSettings.KoaBehavior.Cover):

                
                break;
            
        }
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
        for(int i = 0; i<_boidsTab.Length; i++)
        {
            if(_boidsTab[i].isActive)
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
    public void SetBehavior(BoidSettings newSettings, bool KoaTargetWeight = false)
    {
        for (int i = 0; i < _boidsTab.Length; i++)
        {
            _boidsTab[i].SetNewSettings(newSettings);
            curBoidSettings = newSettings;;
        }

    }

    public void GenerateNewBoid()
    {
        
        for(int i=0;i<curFlockSettings.maxBoid; i++)
        {
            if(!_boidsTab[i].isActive)
            { 
                _boidsTab[i].transform.position = _koa.transform.position; //Déplacement à la position
                _boidsTab[i].transform.forward = Random.insideUnitSphere; //Rotation random
                int rnd = 0;
                if(_guideList.Count>1)
                {
                    rnd = Random.Range(1, _guideList.Count);
                }
                SC_KoaManager s = null;
                _boidsTab[i].Initialize(curBoidSettings, _guideList[rnd],sensitivity,s,(int)curFlockSettings.attackType);
                return;
            }
        }
    }

    public void GetHit(Vector3 gunSensitivity)
    {
   
        float x = Mathf.Abs((int)gunSensitivity.x - (int)sensitivity.x);
        float y = Mathf.Abs((int)gunSensitivity.y - (int)sensitivity.y);
        float z = Mathf.Abs((int)gunSensitivity.z - (int)sensitivity.z);

        float power = 18 - (x + y + z);

        float powerPerCent = (power / 18 )* 100;

        if(powerPerCent>50)
        {
            KoaLife -= (int)((powerPerCent * maxLife) / 100) / 3;
          
            if (KoaLife <= 0) AnimDestroy();
            SC_HitMarker.Instance.HitMark(SC_HitMarker.HitType.Koa);
            vfx_Hit.Play();
        }

        ///DEBUG
        if (gunSensitivity.x == 100)
            AnimDestroy();
    }

    void AnimDestroy()
    {
        CustomSoundManager.Instance.PlaySound(_koa.gameObject, "SFX_Explosion_Flock", false, 0.1f,false);


        //SetBehavior(DeathSettings);
        foreach (Boid b in _boidsTab) b.DestroyBoid(Boid.DestructionType.Massive);
        isActive = false;
        Destroy(_koa.gameObject);
        Invoke("DestroyFlock",1);
    }

    void DestroyFlock()
    {        
        Destroy(_koa.gameObject);
        flockManager.DestroyFlock();
        Destroy(this.gameObject);
    }


    Vector3Int GetNewSensitivity()
    {
        int x = Random.Range(0, 6);
        int y = Random.Range(0, 6);
        int z = Random.Range(0, 6);

        return new Vector3Int(x, y, z);
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
   

}