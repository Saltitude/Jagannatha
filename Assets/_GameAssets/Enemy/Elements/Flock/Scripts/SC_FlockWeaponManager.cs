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

    Animator mainAnimator;
    Animator emissiveAnimator;

    Coroutine resetBoolCoro;
    Coroutine superLaserCoro;

    bool animation = false;

    bool laserBoss;

    bool canFire = true;
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
    public void Initialize(FlockSettings curFlockSettings,Animator mainAnimator, Animator emissiveAnimator)
    {
        flockSettings = curFlockSettings;
        switch (flockSettings.attackType)
        {
            case FlockSettings.AttackType.Bullet: //Bullet
                InitBulletPool();
                break;

            case FlockSettings.AttackType.none: //Bullet
                InitBulletPool();
                break;

            case FlockSettings.AttackType.Laser: //Laser
                InitLaser();
                break;

            case FlockSettings.AttackType.Boss:
                InitBulletPool();
                InitLaser();

                break;
        }
        this.mainAnimator = mainAnimator;
        this.emissiveAnimator = emissiveAnimator;
    }

    void GetReferences()
    {
        if (NetPlayerP == null)
            NetPlayerP = SC_CheckList.Instance.NetworkPlayerPilot;
        if (NetPlayerP != null && NetPFloackSC == null)
            NetPFloackSC = NetPlayerP.GetComponent<SC_NetPlayer_Flock_P>();
    }

    public void StartFire(bool isBoss = false, bool laserBoss = false)
    {
        isFiring = true;
        startLaser = true;
        animation = false;
        this.laserBoss = laserBoss;
        if (flockSettings.attackType == FlockSettings.AttackType.Laser || (isBoss && laserBoss))
        {
            emissiveAnimator.SetBool("LaserCharge", true);

        }
    }

    // Update is called once per frame
    void Update()
    {

        if (NetPlayerP == null || NetPFloackSC == null)
            GetReferences();
        if(canFire)
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

                    if(emissiveAnimator != null)
                        emissiveAnimator.SetBool("Bullet", true);

                    if (mainAnimator != null)
                        mainAnimator.SetBool("Bullet", true);

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

                    
                    if(timer >= flockSettings.chargingAttackTime -1f)
                    {
                        if(!animation)
                        {
                            emissiveAnimator.SetBool("Laser", true);
                            mainAnimator.SetBool("Laser", true);
                            resetBoolCoro = StartCoroutine(ResetBool());
                            animation = true;
                        }

                        
                    }

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
                case FlockSettings.AttackType.Boss:
                    {
                        if (!laserBoss)
                        {
                            if (emissiveAnimator != null)
                                emissiveAnimator.SetBool("Bullet", true);

                            if (mainAnimator != null)
                                mainAnimator.SetBool("Bullet", true);

                            if (timer >= 1 / flockSettings.fireRate)
                            {
                                FireBullet(false);
                                timer = 0;
                                if (nbBulletFire >= flockSettings.nbBulletToShoot)
                                {
                                    EndOfAttack();
                                }
                            }
                        }
                        else
                        {
                            if (timer >= flockSettings.chargingAttackTime - 1f)
                            {
                                if (!animation)
                                {
                                    emissiveAnimator.SetBool("Laser", true);
                                    mainAnimator.SetBool("Laser", true);
                                    resetBoolCoro = StartCoroutine(ResetBool());
                                    animation = true;
                                }
                            }

                            if (timer >= flockSettings.chargingAttackTime)
                            {
                                FireLaser();
                            }
                        }
                    }
                    break;

            }
        }
    }
    IEnumerator ResetBool()
    {
        yield return 0;
        emissiveAnimator.SetBool("LaserCharge", false);
        emissiveAnimator.SetBool("Laser", false);
        mainAnimator.SetBool("Laser", false);
        StopCoroutine(resetBoolCoro);
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

        if (bulletPool[n_CurBullet] != null && mainAnimator != null)
            bulletPool[n_CurBullet].transform.position = mainAnimator.transform.position;

        if (bulletPool[n_CurBullet] != null)
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
            case FlockSettings.AttackType.none: //Bullet

                FireBullet(true);

                break;
            case FlockSettings.AttackType.Laser:

                Reset();
                startLaser = true;
                superLaserCoro = StartCoroutine(SuperLaserCoroutine());


                break;
        }
    }

    IEnumerator SuperLaserCoroutine()
    {
        while(true)
        {
            laserFire = true;
            if (startLaser)
            {

                Sc_ScreenShake.Instance.ShakeIt(0.025f, flockSettings.laserDurationHitReaction);
                SC_CockpitShake.Instance.ShakeIt(0.025f, flockSettings.laserDurationHitReaction);
                //SC_HitDisplay.Instance.Hit(transform.position);
                SC_MainBreakDownManager.Instance.CauseDamageOnSystem(flockSettings.attackFocusHitReaction, flockSettings.damageOnSystemHitReaction);

                startLaser = false;
            }


            laserTimer += Time.deltaTime;
            float scale = (Time.deltaTime / flockSettings.laserDurationHitReaction);
            //Positionne le laser a la base de l'arme (GunPos) et l'oriente dans la direction du point visée par le joueur
            Vector3 TargetPos = new Vector3(target.position.x, target.position.y - 5, target.position.z);
            laser.transform.position = Vector3.Lerp(mainAnimator.transform.position, TargetPos, .5f);
            laser.transform.LookAt(TargetPos);

            //Scale en Z le laser pour l'agrandir jusqu'a ce qu'il touche le point visée par le joueur (C STYLE TAHU)
            laser.transform.localScale = new Vector3(laser.transform.localScale.x + scale,
                                    laser.transform.localScale.y + scale,
                                    Vector3.Distance(transform.position, target.transform.position));

            laserSC.DisplayFlockLaser();

            if (laserTimer >= flockSettings.laserDurationHitReaction)
            {
                DestroyFx();
                GetComponent<SC_FlockManager>().EndReaction();


                StopCoroutine(superLaserCoro);
            }

            yield return new WaitForEndOfFrame();
        }
       
    }

    #endregion

    #region Laser
    void InitLaser()
    {
        //laser = Instantiate(laserPrefab);
        laser = NetPFloackSC.SpawnLaserF();
        laserSC = laser.GetComponent<SC_LaserFlock>();
    }



    void FireLaser()
    {
        laserFire = true;
        if(startLaser)
        {

            Sc_ScreenShake.Instance.ShakeIt(0.025f, flockSettings.activeDuration);
            SC_CockpitShake.Instance.ShakeIt(0.025f, flockSettings.activeDuration);
            //SC_HitDisplay.Instance.Hit(transform.position);
            SC_MainBreakDownManager.Instance.CauseDamageOnSystem(flockSettings.attackFocus, flockSettings.damageOnSystem);

            startLaser = false;
        }


        laserTimer += Time.deltaTime;
        float scale = (Time.deltaTime / flockSettings.activeDuration);
        //Positionne le laser a la base de l'arme (GunPos) et l'oriente dans la direction du point visée par le joueur
        Vector3 TargetPos = new Vector3(target.position.x, target.position.y - 5, target.position.z);
        laser.transform.position = Vector3.Lerp(mainAnimator.transform.position, TargetPos, .5f);
        laser.transform.LookAt(TargetPos);

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

    public void SetBoolCanFire(bool value)
    {
        canFire = value;
    }
}
