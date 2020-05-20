using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_WeaponLaserGun : MonoBehaviour, IF_Weapon, IF_BreakdownSystem
{

    #region Singleton

    private static SC_WeaponLaserGun _instance;
    public static SC_WeaponLaserGun Instance { get { return _instance; } }

    #endregion

    #region Variables

    [Header("References")]   
    public GameObject prefab_bullet;
    public GameObject helper_startPos;
    public GameObject Target;
    GameObject Mng_CheckList;
    GameObject NetPlayerP;
    SC_AimHit SC_AimHit;
    public Sc_LaserFeedBack LaserFB;

    [Header("Laser Parameters")]
    [SerializeField]
    bool b_DebugLaser = false; 
    [SerializeField]
    GameObject _bulletContainer;
    GameObject Bullet;
    MeshRenderer mrBullet;
    SC_BulletLaserGun BulletSC;
    [SerializeField]
    public Color CurColor;

    [Header("Breakdown Infos")]
    [SerializeField]
    bool b_InBreakdown = false;

    [Header("Laser Infos")]
    public float frequency;
    float f_LaserTimer = 0;
    Vector3 LaserDir;

    [Header("Raycast Infos")]
    public RaycastHit LaserHit;
    int layerMask = 1 << 5 | 1 << 15 | 1 << 16 | 1 << 25 | 1 << 26;

    [Header("Bullet Infos (Normally no Used)")]
    public int n_BulletMagazine; //Nombre de balles totale dans le bullet pool (a initialisé dans l'éditeur)
    GameObject[] t_Bullet; //Tableau permettant de stocker toutes les balles initialisées (Bullet pool )
    MeshRenderer[] t_MrBullet;   
    int n_CurBullet; //Permet de stocker la prochaine balle a tirer dans le chargeur
    Vector3Int sensitivity;

    [SerializeField]
    float speedW = 0f;
    public bool trig = false;

    [SerializeField]
    Animator PlasmaBall;

    #endregion Variables

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
        GetReferences();
        CreateBullet();
    }

    void GetReferences()
    {

        if (Target == null)
            Target = SC_CheckList_Weapons.Instance.AimIndicator;
        if (Target != null && SC_AimHit == null)
            SC_AimHit = Target.GetComponent<SC_AimHit>();

        if (NetPlayerP == null)
            NetPlayerP = SC_CheckList.Instance.NetworkPlayerPilot;

    }

    // Update is called once per frame
    void Update()
    {

        if (Target == null)
            GetReferences();

        if (Input.GetKeyDown(KeyCode.L))
            b_DebugLaser = !b_DebugLaser;
        if (!SC_WeaponBreakdown.Instance.CanFire() || trig ==false)
        {
            if (!SC_WeaponBreakdown.Instance.CanFire())
            {
                StartCoroutine(weaponBreak());

            }
            speedW = Mathf.Lerp(speedW, 0f, 0.1f);
            GetComponent<Animator>().SetFloat("rotateSpeed", speedW);
            PlasmaBall.SetBool("isShooting", false);
        }
    }

    #region BulletCreation

    //Plus Utilisé
    void CreateBulletPull()
    {

        GameObject bulletContainer = Instantiate(_bulletContainer);

        //Initialise le tableau de la longueur du chargeur voulu
        t_Bullet = new GameObject[n_BulletMagazine];
        t_MrBullet = new MeshRenderer[n_BulletMagazine];

        for (int i = 0; i < n_BulletMagazine; i++)
        {
            //Initialisation du Prefab BallePilote par le Serveur pour la scene pilote et la scene opérateur
            GameObject curBullet = Instantiate(prefab_bullet, new Vector3(1000, 1000, 1000), Quaternion.identity);
            curBullet.transform.SetParent(bulletContainer.transform);
            t_Bullet[i] = curBullet;
            t_MrBullet[i] = curBullet.GetComponentInChildren<MeshRenderer>();
        }

        //Je sais plus pourquoi mais c'est utile tkt
        n_CurBullet = 0;
        

    }

    void CreateBullet()
    {

        GameObject bulletContainer = Instantiate(_bulletContainer);

        Bullet = NetPlayerP.GetComponent<SC_NetPlayerWeaponsMng>().SpawnLaser(prefab_bullet);

        Bullet.transform.SetParent(bulletContainer.transform);

        mrBullet = Bullet.GetComponentInChildren<MeshRenderer>();
        BulletSC = Bullet.GetComponent<SC_BulletLaserGun>();

        BulletSC.frequency = frequency;

    }

    #endregion

    #region FireFunctions

    public void Trigger()
    {
        SC_SyncVar_WeaponSystem.Instance.f_curEnergyLevel = SC_WeaponBreakdown.Instance.f_EnergyValue;

        if (SC_WeaponBreakdown.Instance.CanFire())
        {
            Fire();
            speedW = Mathf.Lerp(speedW, 1f, 0.1f);
            trig = true;
            GetComponent<Animator>().SetFloat("rotateSpeed", speedW);
            PlasmaBall.SetBool("isShooting", true);
        }

        else
        {

            LaserFB.DiseableLaser();
        }

    }

    public void ReleaseTrigger()
    {
        SC_SyncVar_WeaponSystem.Instance.f_curEnergyLevel = 0;
        Bullet.GetComponent<SC_BulletLaserGun>().ResetPos();
        SC_AimHit.b_OnFire = false;
        LaserFB.DiseableLaser();
        trig = false;
    }

    void Fire()
    {

        BulletSC.DisplayLaser(helper_startPos, Target, b_DebugLaser, CurColor);
        LaserDir = Target.transform.position - helper_startPos.transform.position;
        if (Physics.Raycast(helper_startPos.transform.position, LaserDir.normalized, out LaserHit, 2000f, layerMask))
        {

            if(LaserHit.collider != null)
            {
                Hit();
                LaserFB.EnableLaser(LaserHit);
            }
                
        }

    }

    IEnumerator weaponBreak()
    {

        speedW = 0.5f;
        GetComponent<Animator>().SetFloat("rotateSpeed", speedW);
        yield return new WaitForSeconds(0.1f);
        speedW = 0f;
        GetComponent<Animator>().SetFloat("rotateSpeed", speedW);
        yield return new WaitForSeconds(0.1f);
        speedW = -0.5f;
        GetComponent<Animator>().SetFloat("rotateSpeed", speedW);
        yield return new WaitForSeconds(0.1f);
        speedW = 0f;
        GetComponent<Animator>().SetFloat("rotateSpeed", speedW);
    }

    void Hit()
    {
        
        if (LaserHit.collider.gameObject.layer == 26)
        {

            if (f_LaserTimer > (1 / frequency))
            {
                f_LaserTimer = 0;
                LaserHit.collider.GetComponent<Boid>().HitBoid(sensitivity);
            }

            f_LaserTimer += Time.deltaTime;

        }

        else if (LaserHit.collider.gameObject.layer == 25)
        {

            if (f_LaserTimer > (1 / frequency))
            {
                f_LaserTimer = 0;
                LaserHit.collider.GetComponentInParent<SC_KoaCollider>().GetHit(sensitivity);           
            }

            f_LaserTimer += Time.deltaTime;

        }

        else
        {
            SC_HitMarker.Instance.HitMark(SC_HitMarker.HitType.none);
        }

    }

    #endregion

    #region Breakdown

    public void SetBreakdownState(bool State)
    {
        b_InBreakdown = State;
    }

    public void SetEngineBreakdownState(bool State) { }

    #endregion

    #region Sensitivity

    public Vector3Int GetWeaponSensitivity() { return sensitivity; }

    public void SetSensitivity(int index, int value)
    {      
        switch (index)
        {
            case 0:
                sensitivity.x = value;
                break;
            case 1:
                sensitivity.y = value;
                break;
            case 2:
                sensitivity.z = value;
                break;

            default:

                break;
        }
    }

    #endregion

    #region Debug&Color

    void DebugLaser()
    {

        //Positionne le laser a la base de l'arme (GunPos) et l'oriente dans la direction du point visée par le joueur
        Bullet.transform.position = Vector3.Lerp(helper_startPos.transform.position, Target.transform.position, .5f);
        Bullet.transform.LookAt(Target.transform.position);

        if (mrBullet.enabled == false)
            mrBullet.enabled = true;

        //Scale en Z le laser pour l'agrandir jusqu'a ce qu'il touche le point visée par le joueur (C STYLE TAHU)
        Bullet.transform.localScale = new Vector3(Bullet.transform.localScale.x,
                                                Bullet.transform.localScale.y,
                                                Vector3.Distance(helper_startPos.transform.position, Target.transform.position));

    }

    public void AlignColor(Color32 targetColor)
    {
        CurColor = targetColor;
    }

    #endregion

}
