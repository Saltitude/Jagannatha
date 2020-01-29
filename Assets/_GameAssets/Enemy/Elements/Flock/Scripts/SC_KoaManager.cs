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
    List<Boid> _boidsList; //Tableau contenant les boids

    bool testCoroutine;

    /// <summary>
    /// Avant le start, instanciation
    /// </summary>
    public void Initialize(Transform newGuide, int newSpawnCount, BoidSettings newSettings)
    {

        flockManager = newGuide.GetComponent<SC_FlockManager>();
        //Instanciation des list de Boid et de Guide
        _boidsList = new List<Boid>();
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
            //Instantiation du boid grace au prefab
            Boid boid = Instantiate(_boidPrefab);

            //Transform
            boid.transform.position = transform.position; //Déplacement à la position
            boid.transform.forward = Random.insideUnitSphere; //Rotation random

            //Add le boid a la Boid List
            _boidsList.Add(boid);

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

        m_boidCor = StartCoroutine(BoidTest());
        //GameObject.FindGameObjectWithTag("Mng_Enemy").GetComponent<SC_BoidCompute>().AddNewBoids(_boidsList, _curSettings);
    }

    Coroutine m_boidCor = null;



    IEnumerator BoidTest()
    {



        //Sécurité
        if (_boidsList != null)
        {

            while (true)
            {

                //Ajoute le Koa a la nuée avant les calcules de Flock 
                //_boidsList.Add(_koa);


                int numBoids = _boidsList.Count; //Conversion en int du nombres selon le nombre de boid
                //BURST ?
                boidData = new BoidData[numBoids]; //Création d'un variable (Type BoidData) contenant un tableau avec le nombre d'éléments actuels


                for (int i = 0; i < _boidsList.Count; i++)
                {
                    boidData[i].position = _boidsList[i].position; //Chaque élément déjà positionné est stocké
                    boidData[i].direction = _boidsList[i].forward; //Stockage direction
                }

                //Création du buffer | paramètre 1 count : nombre d'éléments | paramètre 2 stride (type de data en bit) : typage du data de BoidData
                //boidBuffer = new ComputeBuffer(numBoids, BoidData.Size);
                boidBuffer.SetData(boidData);//Dans boidBuffer (type ComputeBuffer), stocakge data du tableau actuel

                //ComputeShader compute est stockage du Shader

                //Buffer Configuration | 
                //para1 : kernelIndex (ordre de la fonction dans ComputeShader)
                //para2 : name ID (cf ComputeShader ligne 14)
                //para3 : buffer 
                compute.SetBuffer(0, "boids", boidBuffer);

                //Configuration des variables du ComputeShader
                compute.SetInt("numBoids", _boidsList.Count);
                compute.SetFloat("viewRadius", _curSettings.perceptionRadius);
                compute.SetFloat("avoidRadius", _curSettings.avoidanceRadius);


                int threadGroups = Mathf.CeilToInt(numBoids / (float)threadGroupSize); //Nombre de groupe = nombre éléments / nombre tkt
                compute.Dispatch(0, threadGroups, 1, 1); //Execute le Shader

                yield return 0;



                boidBuffer.GetData(boidData); //Récupère le résultat du Shader

                //Cf script Boid
                for (int i = 0; i < _boidsList.Count; i++)
                {
                    _boidsList[i].avgFlockHeading = boidData[i].flockHeading; //Stockage pour chaque boid : moyenne devant lui
                    _boidsList[i].centreOfFlockmates = boidData[i].flockCentre; //Stockage pour chaque boid : moyenne à côté
                    _boidsList[i].avgAvoidanceHeading = boidData[i].avoidanceHeading; //Stockage pour chaque boid : moyenne des éléments à éviter
                    _boidsList[i].numPerceivedFlockmates = boidData[i].numFlockmates; //Stockage pour chaque boid : nombre de mate autour

                    _boidsList[i].UpdateBoid(); //Update les boidss
                }

                //boidBuffer.Release(); //Vidage du buffer

                // yield return new WaitForEndOfFrame();
                //StartCoroutine("BoidTest");
                yield return 0;

            }
        }
        //Retir le Koa de la nuée

        //_boidsList.RemoveAt(_boidsList.Count-1);
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
        int all = _boidsList.Count; //Total des boids
        int div = splitNumber; //Total de guides
        float val = all / div; //Nombre de boids par Guides

        //Affectation des guides
        for (int i = 0; i < div; i++)
        {
            for (int j = Mathf.CeilToInt(val * i); j < Mathf.CeilToInt(val * (i + 1)); j++)
            {
                _boidsList[j].GetComponent<Boid>().target = _guideList[i];
            }
        }
        //Si impaire, réparti le dernier boid sur une target
        _boidsList[all - 1].GetComponent<Boid>().target = _guideList[div - 1];

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
        for (int i = 0; i < _boidsList.Count; i++)
        {
            _boidsList[i].SetNewSettings(newSettings);
            _koa.SetNewSettings(newSettings, KoaTargetWeight);
            _curSettings = newSettings;
        }
    }



    public void GetHit()
    {

        //SetBehavior(DeathSettings);
        foreach (Boid b in _boidsList)
        {
            Destroy(b.gameObject);

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