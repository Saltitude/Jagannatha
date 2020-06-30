using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_UI_Display_KoaManager : MonoBehaviour
{
    [SerializeField]
    GameObject _koaPrefab; 
    public GameObject _koa;

    /// <summary>
    /// Current BoidSettings
    /// </summary>
    BoidSettings curBoidSettings; //Paramètres dans le scriptableObject Settings
    SC_UI_Display_Flock flockManager;
    /// <summary>
    /// Contient toute la liste des guides actuels 
    /// </summary>
    List<Transform> _guideList;
    /// <summary>
    /// Guide actuel du Koa
    /// </summary>

    /// <summary>
    /// List de tout les boids contenus dans le Flock
    /// </summary>
    Boid[] _boidsTab; //Tableau contenant les boids

    int spawnCount;
    public bool isActive;

    [SerializeField]
    int regenerationRate;

    
    
    float respawnTimer;

    /// <summary>
    /// Avant le start, instanciation
    /// </summary>
    public void Initialize(Transform newGuide, int spawnCount,BoidSettings newSettings)
    {
        this.spawnCount = spawnCount;
        flockManager = SC_UI_Display_Flock.Instance;

        //Instanciation des list de Boid et de Guide
        _boidsTab = flockManager.GetBoid();

        _guideList = new List<Transform>();

        //Récupération du comportement initial
        curBoidSettings = newSettings;

        //Ajout du premier guide a la liste
        _guideList.Add(newGuide);

        _koa = Instantiate(_koaPrefab);
        _koa.transform.position = transform.position;


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
            boid.Initialize(curBoidSettings, _guideList[0], new Vector3Int(100,100,100), s, 0);
        }

    }

    void Update()
    {
        if (isActive)
        {
            if (_koa != null)
            {
                KoaBehavior();
            }

            respawnTimer += Time.deltaTime;

            if (respawnTimer > (60f / regenerationRate))
            {
                respawnTimer = 0;
                GenerateNewBoid();
            }


        }

    }

    void GenerateNewBoid()
    {

        for (int i = 0; i < spawnCount; i++)
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

                SC_KoaManager s = null;
                _boidsTab[i].Initialize(curBoidSettings, _guideList[0], new Vector3Int(100, 100, 100), s, 0);
                return;
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


                        if (Vector3.Distance(_boidsTab[i].transform.position, flockManager.transform.position) < 150)
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

                if (nbActive != 0)
                {
                    x /= nbActive;
                    y /= nbActive;
                    z /= nbActive;
                    _koa.transform.position = Vector3.Lerp(_koa.transform.position, new Vector3(x, y, z), 5 * Time.deltaTime);

                }





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
                if(_boidsTab[j] != null && _guideList[i] != null)
                    _boidsTab[j].GetComponent<Boid>().target = _guideList[i];
            }
        }
        //Si impaire, réparti le dernier boid sur une target
        if (_boidsTab[all-1] != null && _guideList[div - 1] != null)
            _boidsTab[all - 1].GetComponent<Boid>().target = _guideList[div - 1];

    }


    /// <summary>
    /// Changement de comportement des boids | Param : BoidSettings Nouveau comportement <> bool poids vers la target supèrieur pour le Koa
    /// </summary>
    /// <param name="newSettings"></param>
    /// <param name="KoaTargetWeight"></param>
    public void SetBehavior(BoidSettings newSettings)
    {
        for (int i = 0; i < _boidsTab.Length; i++)
        {
            _boidsTab[i].SetNewSettings(newSettings);
            curBoidSettings = newSettings; 
        }



    }


    public void ActivateKoa()
    {
        InitBoids();
        isActive = true;
    }

}
