using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Le flock Manager contient les guides que suivent une nuée et éxecute les ordres de l'EnemyManager
///  | Sur le prefab Flock
///  | Auteur : Zainix
/// </summary>
public class SC_FlockDisplay : MonoBehaviour
{

    //---------------------------------------------------------------------//
    //---------------------------- VARIABLES ------------------------------//
    //---------------------------------------------------------------------//
    #region Variables

    //-----------PREFAB
    [SerializeField]
    GameObject _KoaPrefab;
    [SerializeField]
    GameObject _GuidePrefab;

    BezierSolution.BezierSpline[] _splineTab;
    BezierSolution.BezierSpline _curSpline;

    GameObject _Player;

    BoidSettings[][] _BoidSettings;

    BoidSettings[] spawnSettings;
    BoidSettings[] roamSettings;
    BoidSettings[] attackSettings;
    BoidSettings[] destructionSettings;
    BoidSettings[] reactionSettings;

    int curSettingsIndex;

    [SerializeField]
    FlockSettings flockDisplay;

   
    BoidSettings _curBoidSetting; //Contient le settings actuel

    FlockSettings flockSettings; //Flocksettings de la nuée (défini a la création par le waveSettings)
    GameObject _KoaManager; //Stock le Koa de la nuée
    public SC_KoaManagerDisplay _SCKoaManager; //Stock le script KoaManager du Koa
    Transform _mainGuide; //Guide général que suit toujours la nuée (correspond au flock (this) mais pour des pb de lisibilité le Transform est stocké dans une varible Main Guide

    [SerializeField]
    BezierSolution.BezierSpline splineLine;



    BezierSolution.BezierWalkerWithSpeed bezierWalkerSpeed;
    BezierSolution.BezierWalkerWithTime bezierWalkerTime;

    bool inAttack;
    bool isActive;


    Quaternion flockInitialRot;
    //---------------------------------------------      MultiGuide Variables  (Split)   ----------------------------------------------------------//

    [HideInInspector]
    public bool _splited = false; //Booléen d'état : nuée split 
    List<Transform> _GuideList; //Permet de stocké la liste des guides lors du split
    List<Vector3> _curCurveDistanceList; //Permet de stocké la distance sur les courbes pour chaque guide lors du split

    //--------------------------------------------------------------------------------------------------------------------------------------------//
    int curState = 0;

    [HideInInspector]
    public bool _merged = false;  //Booléen d'état : nuée fusionnée avec une/des autre(s)

    float startAttackTimer = 0;

    enum PathType
    {
        Roam,
        line,
        AttackPlayer,
    }

    PathType curtype;


    #endregion
    //---------------------------------------------------------------------//


    //---------------------------------------------------------------------//
    //------------------------------- INIT --------------------------------//
    //---------------------------------------------------------------------//
    #region Init
    void Awake()
    {
        isActive = false;
        bezierWalkerSpeed = GetComponent<BezierSolution.BezierWalkerWithSpeed>();
        bezierWalkerTime = GetComponent<BezierSolution.BezierWalkerWithTime>();

    }


    void Start()
    {
        Invoke("InitializeFlock", 2f);
       
    }

    /// <summary>
    /// Initialisation du Flock
    /// </summary>
    public void InitializeFlock()
    {
        flockSettings = flockDisplay;

        flockInitialRot = transform.rotation;
        inAttack = false;
    
        _mainGuide = gameObject.transform; //Main guide prends la valeur de this (CF : Variable _mainGuide)

        _GuideList = new List<Transform>();//Instanciation de la guide list
        _curCurveDistanceList = new List<Vector3>(); // Instanciation de la list de distance sur les courbes pour chaque guide

        _BoidSettings = new BoidSettings[5][];

        spawnSettings = flockSettings.spawnSettings;
        roamSettings = flockSettings.roamSettings;
        attackSettings = flockSettings.attackSettings;
        destructionSettings = flockSettings.destructionSettings;
        reactionSettings = flockSettings.getAwaySettings;

        _BoidSettings[0] = spawnSettings;
        _BoidSettings[1] = roamSettings;
        _BoidSettings[2] = attackSettings;
        _BoidSettings[3] = destructionSettings;
        _BoidSettings[4] = reactionSettings;


        _KoaManager = Instantiate(_KoaPrefab, transform);//Instantiate Koa
        _SCKoaManager = _KoaManager.GetComponent<SC_KoaManagerDisplay>(); //Récupère le Koa manager du koa instancié

        _SCKoaManager.Initialize(_mainGuide, flockSettings.boidSpawn, spawnSettings[0], flockSettings);//Initialise le Koa | paramètre : Guide a suivre <> Nombre de Boids a spawn <> Comportement des boids voulu
     

        _splineTab = new BezierSolution.BezierSpline[_BoidSettings.Length];
        for (int i = 0; i < flockSettings.splines.Length; i++)
        {

            if (flockSettings.splines[i] != null)
            {
                _splineTab[i] = Instantiate(flockSettings.splines[i]);

            }

        }
        _curSpline = splineLine;
        bezierWalkerSpeed.SetNewSpline(_curSpline);
        ActivateFlock();
        isActive = true;
    }
    #endregion
    //---------------------------------------------------------------------//


    //---------------------------------------------------------------------//
    //----------------------------- UPDATE  -------------------------------//
    //---------------------------------------------------------------------//
    #region Update
    void Update()
    {
        if (_splited)
            MultiGuideMovement();

        //Si le flock n'est pas fusionné, déplace le main guide selon la spline actuel       
        //bezierWalkerSpeed.Execute(Time.deltaTime);

        if(isActive && _curBoidSetting != null)
        transform.Rotate(new Vector3(_curBoidSetting.axisRotationSpeed.x, _curBoidSetting.axisRotationSpeed.y, _curBoidSetting.axisRotationSpeed.z));

        ChangeStateManuel();
    }


    void ChangeStateManuel()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            curState++;
            if (curState == _BoidSettings.Length)
            {
                curState = 0;
            }
            StartNewBehavior(curState);
            Debug.Log(curState+1);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            curState--;
            if (curState == -1)
            {
                curState = _BoidSettings.Length-1;
            }
            StartNewBehavior(curState);
            Debug.Log(curState+1);
        }
    }

    public void StartNewBehavior(int behaviorIndex)
    {
        transform.rotation = flockInitialRot;

        StopAllCoroutines();
        curSettingsIndex = 0;

        StartCoroutine(SwitchSettings(_BoidSettings[behaviorIndex]));

    }

    IEnumerator SwitchSettings(BoidSettings[] settings)
    {
        while (true)
        {
            _curBoidSetting = settings[curSettingsIndex];

            int rnd = Random.Range(0, 2);
            if (rnd == 0)
                bezierWalkerSpeed.speed = _curBoidSetting.speedOnSpline;
            else
                bezierWalkerSpeed.speed = -_curBoidSetting.speedOnSpline;

            Reassemble();
            if (_curBoidSetting.split)
            {
                SplitDivision(_curBoidSetting.splitNumber);
            }
            _SCKoaManager.SetBehavior(_curBoidSetting);
            //https://www.youtube.com/watch?v=bOZT-UpRA2Y

            if (settings.Length == 1)
            {
                StopAllCoroutines();
                break;
            }

            yield return new WaitForSeconds(_curBoidSetting.settingDuration);

            curSettingsIndex++;
            if (curSettingsIndex >= settings.Length)
                curSettingsIndex = 0;

        }

    }


    /// <summary>
    /// Guides movement when the flock is Split
    /// </summary>
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


    #endregion
    //---------------------------------------------------------------------//



    //---------------------------------------------------------------------//
    //---------------------------- UTILITIES ------------------------------//
    //---------------------------------------------------------------------//
    #region Utilities


    void ActivateFlock()
    {
        isActive = true;
        _SCKoaManager.ActivateKoa();

    }




    /// <summary>
    /// Start the Flock split [Param : (int)Split Number]
    /// </summary>
    /// <param name="splitNumber"></param>
    /// 
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



    public void DestroyFlock()
    {
        GetComponent<SC_FlockWeaponManager>().DestroyFx();
        SC_WaveManager.Instance.FlockDestroyed(this.gameObject);
        Destroy(this.gameObject);
    }

    public void EndAttack()
    {
        inAttack = false;
    }


    Vector3 GetRandomSpawnPosition()
    {
        var radius = 210;
        float x = Random.Range(0f, 1f);
        float y = 1 - x;

        int rndNeg1 = Random.Range(0, 2);
        int rndNeg2 = Random.Range(0, 2);

        if (rndNeg1 == 1) x = -x;
        if (rndNeg2 == 1) y = -y;


        return new Vector3(x * radius, 80, y * radius);
    }
    #endregion
    //---------------------------------------------------------------------//

}
