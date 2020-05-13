using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_UI_Display_Flock : MonoBehaviour
{
    #region Singleton

    private static SC_UI_Display_Flock _instance;
    public static SC_UI_Display_Flock Instance { get { return _instance; } }
    #endregion

    //------------------------Compute Shader et Pool
    [SerializeField]
    int nbBoid;

    const int threadGroupSize = 3;
    ComputeBuffer boidBuffer;
    BoidData[] boidData;

    [SerializeField]
    ComputeShader compute;

    Boid[] _boidsTab; //Tableau contenant les boids

    [SerializeField]
    BoidSettings baseBoidSettings;

    [SerializeField]
    Boid _boidPrefab;
    [SerializeField]
    GameObject _boidContainer;

    Coroutine m_boidCor = null;

    //--------------------------FLOCK 
    [SerializeField]
    GameObject _KoaPrefab;
    [SerializeField]
    GameObject _GuidePrefab;
    BoidSettings _curBoidSetting; //Contient le settings actuel
    Transform _mainGuide; //Guide général que suit toujours la nuée (correspond au flock (this) mais pour des pb de lisibilité le Transform est stocké dans une varible Main Guide


    Quaternion flockInitialRot;
    //---------------------------------------------      MultiGuide Variables  (Split)   ----------------------------------------------------------//

    [HideInInspector]
    public bool _splited = false; //Booléen d'état : nuée split 
    List<Transform> _GuideList; //Permet de stocké la liste des guides lors du split
    List<Vector3> _curCurveDistanceList; //Permet de stocké la distance sur les courbes pour chaque guide lors du split

    //--------------------------------------------------------------------------------------------------------------------------------------------//
    GameObject _KoaManager;
    SC_UI_Display_KoaManager _SCKoaManager;

    [SerializeField]
    BezierSolution.BezierSpline splineLine;
    BezierSolution.BezierWalkerWithSpeed bezierWalkerSpeed;

    bool windowActive = false;


    bool isActive = false;

    Animator mainAnim;
    Animator emissiveAnim;

    //Animation
    bool deploy;
    bool flight;
    bool bullet;
    bool laser;
    float speedFactor;



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
        _curBoidSetting = baseBoidSettings;
        InitializePoolAndComputeShader();
        InitializeFlock();

    }

    // Update is called once per frame
    void Update()
    {

        FlockUpdate();

    }

    void InitializePoolAndComputeShader()
    {

        _boidsTab = new Boid[nbBoid];
        GameObject boidContainer = Instantiate(_boidContainer);
        for (int i = 0; i<_boidsTab.Length;i++)
        {
            Boid curBoid = Instantiate(_boidPrefab);
            curBoid.transform.position = new Vector3(0, -2000, 0);
            curBoid.transform.SetParent(boidContainer.transform);
            _boidsTab[i] = curBoid;
        }


        boidData = new BoidData[_boidsTab.Length];
        boidBuffer = new ComputeBuffer(_boidsTab.Length, BoidData.Size);
        m_boidCor = StartCoroutine(corBoid());

    }

    void InitializeFlock()
    {

        flockInitialRot = transform.rotation;
        _mainGuide = gameObject.transform;
        _GuideList = new List<Transform>();//Instanciation de la guide list
        _curCurveDistanceList = new List<Vector3>(); // Instanciation de la list de distance sur les courbes pour chaque guide


       
        _KoaManager = Instantiate(_KoaPrefab, transform);//Instantiate Koa
        _SCKoaManager = _KoaManager.GetComponent<SC_UI_Display_KoaManager>(); //Récupère le Koa manager du koa instancié


        _SCKoaManager.Initialize(_mainGuide,nbBoid, baseBoidSettings);//Initialise le Koa | paramètre : Guide a suivre <> Nombre de Boids a spawn <> Comportement des boids voulu
        mainAnim = _KoaManager.GetComponent<SC_UI_Display_KoaManager>()._koa.transform.GetChild(0).GetComponent<Animator>();


        bezierWalkerSpeed = GetComponent<BezierSolution.BezierWalkerWithSpeed>();
        bezierWalkerSpeed.SetNewSpline(splineLine);

        ActivateFlock();

    }

    void ActivateFlock()
    {
        isActive = true;
        _SCKoaManager.ActivateKoa();

    }

    public Boid[] GetBoid()
    {
        return _boidsTab;
    }

    public void activateRender()
    {
        windowActive = true;
        for (int i = 0; i < _boidsTab.Length; i++)
        {
            _boidsTab[i].transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
        }

    }

    public void desactivateRender()
    {
        windowActive = false;
        if(_boidsTab != null)
        for (int i = 0; i < _boidsTab.Length; i++)
        {
            _boidsTab[i].transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
        }
    }


    #region Flock
    void SplitDivision(int splitNumber)
    {
        //Register old guides to destroy after creating new ones
        List<Transform> _oldGuideList = new List<Transform>();
        foreach (Transform guide in _GuideList)
        {
            if (!GameObject.Equals(guide, transform))
            {
                _oldGuideList.Add(guide);
            }
        }

        //Set splited to true
        _splited = true;

        //Clear current guide list
        _GuideList.Clear();

        //Clear Curve distance value List for multi guides
        _curCurveDistanceList.Clear();

        //Set new values link to split number
        for (int i = 0; i < splitNumber; i++)
        {
            //Create new guides 
            Transform curGuide = Instantiate<GameObject>(_GuidePrefab).transform;
            //Add them to guide list
            _GuideList.Add(curGuide);
            curGuide.SetParent(this.transform);

            //Calculate Offset for Curve distance
            float offset = (1f / splitNumber) * (i);
            //Register curve offset distance to Curve distance list
            _curCurveDistanceList.Add(new Vector3(offset, offset, offset));


        }

        _SCKoaManager.Split(_GuideList);

        //Destroy old useless guides
        foreach (Transform guide in _oldGuideList)
        {
            Destroy(guide.gameObject);
        }

    }

    void Reassemble()
    {
        foreach (Transform guide in _GuideList)
        {
            if (!GameObject.Equals(guide, transform))
                Destroy(guide.gameObject);
        }
        //Set splited to false
        _splited = false;

        //Clear current guide list
        _GuideList.Clear();

        //Clear Curve distance value List for multi guides
        _curCurveDistanceList.Clear();

        //Add main guide to guide list
        _GuideList.Add(_mainGuide);

        _SCKoaManager.Split(_GuideList);
    }
    void MultiGuideMovement()
    {

        //Set every Guide position
        for (int i = 0; i < _GuideList.Count; i++)
        {
            //Init new translate value
            float valueX = 0;
            float valueY = 0;
            float valueZ = 0;
            //Curve offset by guide
            float fguideOffset = (1 / _GuideList.Count) * (i + 1);
            Vector3 v3guideOffset = new Vector3(fguideOffset, fguideOffset, fguideOffset);

            //Add new dt to curv distance (x oscilliation speed)
            _curCurveDistanceList[i] += Time.deltaTime * _curBoidSetting.frequence;
            _curCurveDistanceList[i] += v3guideOffset;

            //curv distance in relation of guide number



            //Keep current curve distance values between 0 - 1
            if (_curCurveDistanceList[i].x > 1)
            {
                _curCurveDistanceList[i] = new Vector3(_curCurveDistanceList[i].x - 1, _curCurveDistanceList[i].y, _curCurveDistanceList[i].z);
            }
            if (_curCurveDistanceList[i].y > 1)
            {
                _curCurveDistanceList[i] = new Vector3(_curCurveDistanceList[i].x, _curCurveDistanceList[i].y - 1, _curCurveDistanceList[i].z);
            }
            if (_curCurveDistanceList[i].z > 1)
            {
                _curCurveDistanceList[i] = new Vector3(_curCurveDistanceList[i].x, _curCurveDistanceList[i].y, _curCurveDistanceList[i].z - 1);
            }


            //Get Z and Y translate value on the curve
            valueX = _curBoidSetting.curveX.Evaluate(_curCurveDistanceList[i].x) * _curBoidSetting.amplitude.x;
            valueY = _curBoidSetting.curveY.Evaluate(_curCurveDistanceList[i].y) * _curBoidSetting.amplitude.y;
            valueZ = _curBoidSetting.curveZ.Evaluate(_curCurveDistanceList[i].z) * _curBoidSetting.amplitude.z;


            if (_curBoidSetting.invert && i % 2 == 1)
            {
                valueX -= (valueX * 2) * _curBoidSetting.invertAxis.x;
                valueY -= (valueY * 2) * _curBoidSetting.invertAxis.y;
                valueZ -= (valueZ * 2) * _curBoidSetting.invertAxis.z;
             
            }

            //Add the offset value
            _GuideList[i].transform.localPosition = new Vector3(valueX, valueY, valueZ);
        }


    }

    public void SetAnimationBool(bool deploy, bool flight, bool bullet, bool laser, float speedFactor)
    {
 
        this.deploy = deploy;this.flight = flight;this.bullet = bullet;this.laser = laser;this.speedFactor = speedFactor;
    }

    void FlockUpdate()
    {
        if (isActive && _curBoidSetting != null)
        {
            if (_splited)
                MultiGuideMovement();

            bezierWalkerSpeed.Execute(Time.deltaTime);

            transform.Rotate(new Vector3(_curBoidSetting.axisRotationSpeed.x, _curBoidSetting.axisRotationSpeed.y, _curBoidSetting.axisRotationSpeed.z));
          
            mainAnim.SetBool("Deploy", deploy);
            mainAnim.SetBool("Flight", flight);
            mainAnim.SetBool("Bullet", bullet);
            mainAnim.SetBool("Laser", laser);
            mainAnim.SetFloat("SpeedFactor", speedFactor);
            
        }
    }
    public void StartNewBehavior(BoidSettings newSettings)
    {

        transform.rotation = flockInitialRot;
        _curBoidSetting = newSettings;
        Reassemble();

        if (_curBoidSetting.split)
        {
            SplitDivision(_curBoidSetting.splitNumber);
        }

        _SCKoaManager.SetBehavior(_curBoidSetting);
        bezierWalkerSpeed.speed = _curBoidSetting.speedOnSpline;
    }

    #endregion

    #region Compute Shader

    IEnumerator corBoid()
    {

        //Sécurité
        if (_boidsTab != null)
        {
            while (true)
            {

                //Ajoute le Koa a la nuée avant les calcules de Flock 
                //_boidsList.Add(_koa);

                Array.Clear(boidData, 0, boidData.Length);
                //BURST ?
                //Création d'un variable (Type BoidData) contenant un tableau avec le nombre d'éléments actuels


                for (int i = 0; i < _boidsTab.Length; i++)
                {
                    boidData[i].active = 0;
                    if (_boidsTab[i].isActive)
                    {
                        boidData[i].active = 1;
                        boidData[i].position = _boidsTab[i].position; //Chaque élément déjà positionné est stocké
                        boidData[i].direction = _boidsTab[i].forward; //Stockage direction
    

                    }
                }

                boidBuffer.SetData(boidData);//Dans boidBuffer (type ComputeBuffer), stocakge data du tableau actuel

                //ComputeShader compute est stockage du Shader

                //Buffer Configuration | 
                //para1 : kernelIndex (ordre de la fonction dans ComputeShader)
                //para2 : name ID (cf ComputeShader ligne 14)
                //para3 : buffer 
                compute.SetBuffer(0, "boids", boidBuffer);

                //Configuration des variables du ComputeShader
                compute.SetInt("numBoids", _boidsTab.Length);
                compute.SetFloat("viewRadius", _curBoidSetting.perceptionRadius);
                compute.SetFloat("avoidRadius", _curBoidSetting.avoidanceRadius);


                int threadGroups = Mathf.CeilToInt(_boidsTab.Length / (float)threadGroupSize); //Nombre de groupe = nombre éléments / nombre tkt
                compute.Dispatch(0, threadGroups, 1, 1); //Execute le Shader

                yield return new WaitForEndOfFrame();

                boidBuffer.GetData(boidData); //Récupère le résultat du Shader
        
                //Cf script Boid
                for (int i = 0; i < _boidsTab.Length; i++)
                {
                    if (_boidsTab[i].isActive)
                    {
                        _boidsTab[i].avgFlockHeading = boidData[i].flockHeading; //Stockage pour chaque boid : moyenne devant lui
                        _boidsTab[i].centreOfFlockmates = boidData[i].flockCentre; //Stockage pour chaque boid : moyenne à côté
                        _boidsTab[i].avgAvoidanceHeading = boidData[i].avoidanceHeading; //Stockage pour chaque boid : moyenne des éléments à éviter
                        _boidsTab[i].numPerceivedFlockmates = boidData[i].numFlockmates; //Stockage pour chaque boid : nombre de mate autour

                        if(windowActive)
                        _boidsTab[i].UpdateBoid(); //Update les boidss
                    }

                }

                yield return 0;

            }
        }
    }

    /// <summary>
    /// Structure envoyée dans le ComputeShader 
    /// </summary>
    public struct BoidData
    {
        public int active;

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
                return sizeof(float) * 3 * 5 + sizeof(int) * 2;
            }
        }
    }
    #endregion
}
