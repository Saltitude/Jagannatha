using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_FlockWeaponManager : MonoBehaviour
{

    ////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////
    ///
    /////////////////////-- NETWORK --//////////////////////

    GameObject NetPlayerP;
    SC_NetPlayer_Flock_P NetPFloackSC;

    ///////////////////////-- BOTH --//////////////////////
    FlockSettings flockSettings;
    Transform target;

    float timer;
    bool isFiring;

    ///////////////////////-- LASER --//////////////////////
    [Header("Laser Refrences")]
    [SerializeField]                                      
    GameObject laserPrefab;
    [SerializeField]
    GameObject laserFxPrefab;

    GameObject laserFx;
    GameObject laser;
    SC_LaserFlock laserSC;
    bool laserFire;
    float laserTimer;
    bool startLaser;

    ///////////////////////-- BULLET --//////////////////////
    [Header("Bullet Refrences")]
    [SerializeField]
    GameObject bulletPrefab;
    [SerializeField]
    GameObject bulletContainer;
    GameObject[] bulletPool;
    int n_CurBullet;
    int nbBulletFire;

    Animator animator;
    ////////////////////////////////////////////////////////


    void Awake()
    {
        GetReferences();
    }

    void Start()
    {
        
        Reset();
        
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }
    public void Initialize(FlockSettings curFlockSettings,Animator animator)
    {
        flockSettings = curFlockSettings;
        switch (flockSettings.attackType)
        {
            case FlockSettings.AttackType.Bullet: //Bullet
                InitBulletPool();
                break;

            case FlockSettings.AttackType.Laser: //Laser
                InitLaser();
                break;
        }
        this.animator = animator;
    }

    void GetReferences()
    {
        if (NetPlayerP == null)
            NetPlayerP = SC_CheckList.Instance.NetworkPlayerPilot;
        if (NetPlayerP != null && NetPFloackSC == null)
            NetPFloackSC = NetPlayerP.GetComponent<SC_NetPlayer_Flock_P>();
    }

    public void StartFire()
    {
        isFiring = true;
        startLaser = true;
    }

    // Update is called once per frame
    void Update()
    {

        if (NetPlayerP == null || NetPFloackSC == null)
            GetReferences();

        FireUpdate();
    
    }

    void FireUpdate()
    {
        if(isFiring)
        {

            timer += Time.deltaTime;
            switch (flockSettings.attackType)
            {
                case FlockSettings.AttackType.Bullet: //Bullet

                    animator.SetBool("Bullet", true);
                    if (timer >= 1/flockSettings.fireRate )
                    {
                        FireBullet(false);
                        timer = 0;
                        if(nbBulletFire >= flockSettings.nbBulletToShoot)
                        {
                            EndOfAttack();
                        }
                    }
                    break;

                case FlockSettings.AttackType.Laser: //Laser

                    if(!laserFire)
                    {
                        laserFx.transform.position = animator.transform.position;
                        float scale = (Time.deltaTime / flockSettings.chargingAttackTime)*5;
                        laserFx.transform.localScale += new Vector3 (scale,scale,scale);
                    }
                    if(timer >= flockSettings.chargingAttackTime -1f)
                    animator.SetBool("Laser", true);

                    if (timer >= flockSettings.chargingAttackTime)
                    {
                        FireLaser();
                    }

                    break;

                case FlockSettings.AttackType.Kamikaze:

                    transform.position = Vector3.Lerp(transform.position, target.position, flockSettings.speedToTarget*Time.deltaTime);
                    if(Vector3.Distance(transform.position,target.position) < 20)
                    {
                        isFiring = false;
                        this.GetComponent<SC_FlockManager>()._SCKoaManager.GetHit(new Vector3(100,100,100));
                        Sc_ScreenShake.Instance.ShakeIt(0.025f, flockSettings.activeDuration);
                        SC_CockpitShake.Instance.ShakeIt(0.025f, flockSettings.activeDuration);
                        SC_MainBreakDownManager.Instance.CauseDamageOnSystem(flockSettings.attackFocus, flockSettings.damageOnSystem);
                    
                    }
                    break;

            }
        }
    }

    #region Bullet
    void InitBulletPool()
    {

        GameObject _bulletContainer = Instantiate(bulletContainer);

        bulletPool = new GameObject[20];
        for (int i = 0; i < 20; i++)
        {
            
            //GameObject curBullet = Instantiate(bulletPrefab);
            GameObject curBullet = NetPFloackSC.SpawnBulletF();
            bulletPool[i] = curBullet;
            curBullet.transform.SetParent(_bulletContainer.transform); 
        }
    }

    void FireBullet(bool superBullet)
    {
        Rigidbody rb = bulletPool[n_CurBullet].GetComponent<Rigidbody>();

        bulletPool[n_CurBullet].transform.position = transform.position;
        bulletPool[n_CurBullet].transform.rotation = transform.rotation;

        rb.isKinematic = true;
        rb.isKinematic = false;

        //noise
        Vector3 dir = new Vector3(transform.forward.x , transform.forward.y , transform.forward.z);

        bulletPool[n_CurBullet].GetComponent<SC_BulletFlock>().b_IsFire = true;
        bulletPool[n_CurBullet].GetComponent<SC_BulletFlock>().b_ReactionFire = superBullet;
        bulletPool[n_CurBullet].GetComponent<SC_BulletFlock>().flockSettings = flockSettings;

        rb.AddForce(dir * 24000);

        n_CurBullet++;

        if (n_CurBullet >= bulletPool.Length)
            n_CurBullet = 0;

        nbBulletFire++;
    }

    public void FireSuperBullet()
    {
        switch (flockSettings.attackType)
        {
            case FlockSettings.AttackType.Bullet: //Bullet

                FireBullet(true);

                break;
        }
    }

    #endregion

    #region Laser
    void InitLaser()
    {
        //laser = Instantiate(laserPrefab);
        laser = NetPFloackSC.SpawnLaserF();
        laserFx = Instantiate(laserFxPrefab);
        laserSC = laser.GetComponent<SC_LaserFlock>();
    }


    void FireLaser()
    {
        laserFire = true;
        if(startLaser)
        {
            Sc_ScreenShake.Instance.ShakeIt(0.025f, flockSettings.activeDuration);
            SC_CockpitShake.Instance.ShakeIt(0.025f, flockSettings.activeDuration);
            SC_HitDisplay.Instance.Hit(transform.position);
            SC_MainBreakDownManager.Instance.CauseDamageOnSystem(flockSettings.attackFocus, flockSettings.damageOnSystem);

            startLaser = false;
        }


        laserTimer += Time.deltaTime;
        float scale = (Time.deltaTime / flockSettings.activeDuration);
        laserFx.transform.localScale -= new Vector3(scale*5, scale*5, scale*5);
        //Positionne le laser a la base de l'arme (GunPos) et l'oriente dans la direction du point visée par le joueur
        laser.transform.position = Vector3.Lerp(animator.transform.position, target.position, .5f);
        laser.transform.LookAt(new Vector3(target.position.x,target.position.y-5,target.position.z));

        //Scale en Z le laser pour l'agrandir jusqu'a ce qu'il touche le point visée par le joueur (C STYLE TAHU)
        laser.transform.localScale = new Vector3(laser.transform.localScale.x +scale,
                                laser.transform.localScale.y + scale,
                                Vector3.Distance(transform.position, target.transform.position));

        laserSC.DisplayFlockLaser();

        if (laserTimer >= flockSettings.activeDuration)
        {
            DestroyFx();
            EndOfAttack();
        }

        //INSERT LASER SHIT

    }
    #endregion

    public void DestroyFx()
    {
        isFiring = false;
        if(laser != null)
        {
            laser.transform.localScale = new Vector3(0, 0, 0);
            laser.transform.position = new Vector3(0, -2000, 0);
            laserSC.DisplayFlockLaser();

        }
        if (laserFx != null)
        {
            laserFx.transform.localScale = new Vector3(0, 0, 0);
            laserFx.transform.position = new Vector3(0, -2000, 0);
        }
        
    }



    void EndOfAttack()
    {
        this.GetComponent<SC_FlockManager>().EndAttack();
        Reset();
    }

    void Reset()
    {
        laserFire = false;
        n_CurBullet = 0;
        nbBulletFire = 0;
        timer = 0;
        laserTimer = 0;
        isFiring = false;
    }
}
