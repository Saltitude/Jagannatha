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

    //Tkt ca marche
    const int threadGroupSize = 3;

    int coroutineCount = 0;


    public Boid _boidPrefab; //Prefab du boid
    public Boid _koaPrefab; //Prefab du Koa

    ComputeBuffer boidBuffer;
    BoidData[] boidData;

    Boid _koa; //Koa du 

    /// <summary>
    /// Current BoidSettings
    /// </summary>
    BoidSettings _curSettings; //Paramètres dans le scriptableObject Settings

    [SerializeField]
    ComputeShader compute;
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

    bool testCoroutine;

    /// <summary>
    /// Avant le start, instanciation
    /// </summary>
    public void Initialize(Transform newGuide, int newSpawnCount, BoidSettings newSettings)
    {

        flockManager = newGuide.GetComponent<SC_FlockManager>();
        //Instanciation des list de Boid et de Guide
        _boidsTab = GameObject.FindGameObjectWithTag("Mng_Enemy").GetComponent<BoidPool>().GetBoid(newSpawnCount);
        _guideList = new List<Transform>();

        //Récupération du comportement initial
        _curSettings = newSettings;

        //Ajout du premier guide a la liste
        _guideList.Add(newGuide);

        //Set le guide du koa au premier guide 
        _curKoaGuide = newGuide;

        //Initialisation de tout les boids
        for (int i = 0; i < newSpawnCount; i++)
        {
            Boid boid = _boidsTab[i];

            //Transform
            boid.transform.position = transform.position; //Déplacement à la position
            boid.transform.forward = Random.insideUnitSphere; //Rotation random

            //Add le boid a la Boid List
            _boidsTab[i] = boid;

            //Lance l'initialisation de celui-ci avec le comportement initial et le premier guide
            boid.Initialize(_curSettings, _guideList[0]);
        }

        //Instantie le Koa
        _koa = Instantiate(_koaPrefab);

        //Transform(ballec en vrai)
        _koa.transform.position = transform.position;

        //Lance l'initialisation de celui-ci avec le comportement initial et le premier guide
        _koa.Initialize(_curSettings, _curKoaGuide, true);


        boidBuffer = new ComputeBuffer(newSpawnCount, BoidData.Size);
        // boidData = new BoidData[newSpawnCount]; //Création d'un variable (Type BoidData) contenant un tableau avec le nombre d'éléments actuels

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
        int all = _boidsTab.Length; //Total des boids
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

        //Affection du guide du Koa
        _curKoaGuide = _guideList[Random.Range(0, _guideList.Count)];
        _koa.GetComponent<Boid>().target = _curKoaGuide;
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
            _koa.SetNewSettings(newSettings, KoaTargetWeight);
            _curSettings = newSettings;
        }
    }



    public void GetHit()
    {

        //SetBehavior(DeathSettings);
        foreach (Boid b in _boidsTab)
        {
            b.DestroyBoid();

        }
        Destroy(_koa.gameObject);
        flockManager.DestroyFlock();
        Destroy(this.gameObject);

    }


    /// <summary>
    /// Structure envoyée dans le ComputeShader 
    /// </summary>
    public struct BoidData
    {
        public Vector3 position;
        public Vector3 direction;

        public Vector3 flockHeading;
        public Vector3 flockCentre;
        public Vector3 avoidanceHeading;

        public int numFlockmates;

        //sizeof => retourne la mémoire en bit, pour un type de variable
        //Ici float* 3(Vector3) * 5(nombre de valeurs) + int * 1
        public static int Size
        {
            get
            {
                return sizeof(float) * 3 * 5 + sizeof(int);
            }
        }
    }
}