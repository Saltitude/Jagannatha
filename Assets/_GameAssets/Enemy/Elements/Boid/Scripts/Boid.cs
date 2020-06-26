using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour {

    BoidSettings settings;
    bool isKoa = false;
    bool koaTargetWeight = false;
    public bool isActive = false;
    bool DestructionAnim = false;
    float destructionTimer;
    float curTimer;
    int TotalFlick =1;
    int curFlick;
    int type;
    

    SC_KoaManager koaManager;

    Vector3 initScale;

    Material baseMat;


    [SerializeField]
    Material[] M_tabHit;
    MeshRenderer meshRenderer;

    [SerializeField]
    Material bossMat;

    Vector3 deathPos;
    int life = 2;
    Vector3Int sensitivity;
    // State
    [HideInInspector]
    public Vector3 position;
    [HideInInspector]
    public Vector3 forward;
    Vector3 velocity;

    // To update:
    Vector3 acceleration; //Acceleraton
    [HideInInspector]
    public Vector3 avgFlockHeading; //Position moyenne de la tête
    [HideInInspector]
    public Vector3 avgAvoidanceHeading; //Evitement moyen de la tête
    [HideInInspector]
    public Vector3 centreOfFlockmates; //Position des mate capté
    [HideInInspector]
    public int numPerceivedFlockmates; //Nombre de mate dans le radius de détection
     

    public enum DestructionType
    {
        Solo,
        Massive,
        none
    }

    DestructionType destructionType;
    // Cached

    Transform cachedTransform; //To define
    public Transform target; //Target

    void Awake () {
        cachedTransform = transform; //Position tampon
        initScale = transform.localScale;
        meshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
        baseMat = meshRenderer.material;
    }
    public void SetNewSettings(BoidSettings newSettings, bool koaTargetWeight = false)
    {
        settings = newSettings;
        this.koaTargetWeight = koaTargetWeight;
    }
    /// <summary>
    /// Initialisation
    /// </summary>
    /// <param name="settings"></param>
    /// <param name="target"></param>
    public void Initialize (BoidSettings settings, Transform target,Vector3Int sensitivity, SC_KoaManager koaManager, int type)
    {
        this.koaManager = koaManager;
        destructionType = DestructionType.none;
        curFlick = 0;
        transform.localScale = initScale;
        this.target = target; //Peut être null
        this.settings = settings; //Scriptable object
        this.sensitivity = sensitivity;
        position = cachedTransform.position; //Déplacement à la position tampon
        forward = cachedTransform.forward; //Direction selon axe X
        
        float startSpeed = (settings.minSpeed + settings.maxSpeed) / 2; //Vitesse d'initialisation
        velocity = transform.forward * startSpeed; //Stockage de la vélocité selon la vitesse de départ
        destructionTimer = 3f;
        curTimer = 0;
        if (type == 4) meshRenderer.material = bossMat;
        else meshRenderer.material = baseMat;
        isActive = true;
        this.type = type;
    }
    /// <summary>
    /// Changer la couleur de chaque boid |
    /// Sécurité
    /// </summary>
    /// <param name="col"></param>
    /// 
    private void OnTriggerEnter(Collider other)
    {
        //JE TOUCHE LE PLAYER 
        if (other.gameObject.layer == 20)
        {
            Sc_ScreenShake.Instance.ShakeIt(0.010f, 0.1f);
            SC_CockpitShake.Instance.ShakeIt(0.0075f, 0.1f,false);
            //SC_HitDisplay.Instance.Hit(transform.position);

            DestroyBoid(DestructionType.Solo);
        }
    }


        /// <summary>
        /// Update fait maison |
        /// Appelé à chaque frame dans l'update du BoidManager
        /// </summary>
        public void UpdateBoid () {

        if(isActive && (!DestructionAnim || destructionType == DestructionType.Massive))
        {
            Vector3 acceleration = Vector3.zero; //RaZ de l'accélération

            if (target != null) //Absence de target
            {
                float curTargetWeight = settings.targetWeight;
                if (isKoa && koaTargetWeight)
                {
                    curTargetWeight *= 3;

                }
                Vector3 offsetToTarget = (target.position - position); //Calcul de la différence entre boid et Target
                acceleration = SteerTowards(offsetToTarget) * curTargetWeight; //Acceleration = Résultat de SteerToward * Attraction de la Target 

            }

            if (numPerceivedFlockmates != 0) //Si des mates sont dans la zone de détection
            {
                centreOfFlockmates /= numPerceivedFlockmates; //Position des autres flock / nombre de flock autour
                Vector3 offsetToFlockmatesCentre = (centreOfFlockmates - position); //Offset selon position mate - position actuel
                
                var alignmentForce = SteerTowards(avgFlockHeading) * settings.alignWeight; //Vector3 Alignement
                var cohesionForce = SteerTowards(offsetToFlockmatesCentre) * settings.cohesionWeight; //Vector3 Cohesion
                var seperationForce = SteerTowards(avgAvoidanceHeading) * settings.seperateWeight; //Vector3 separation

                //Application des forces (Vector3)
                acceleration += alignmentForce;
                acceleration += cohesionForce;
                acceleration += seperationForce;
            }

            //Detection de collision (return bool)
            if (IsHeadingForCollision())
            {
                //Nouvelle direction pour éviter obstacle
                Vector3 collisionAvoidDir = ObstacleRays();
                //Nouveau déplacement selon la nouvelle direction
                Vector3 collisionAvoidForce = SteerTowards(collisionAvoidDir) * settings.avoidCollisionWeight;
                //Ajout de l'évitement dans le déplacement
                acceleration += collisionAvoidForce;
            }

            velocity += acceleration * Time.deltaTime; //MaJ de la vélocité du boid
            float speed = velocity.magnitude; //Récupère magnitude de la vélocité (longueur du vecteur)
            Vector3 dir = velocity / speed; //Nouvelle direction car Velocité / magnitude : direction
            speed = Mathf.Clamp(speed, settings.minSpeed, settings.maxSpeed); //Vélocité clampée entre les bornes (sécurité)
            velocity = dir * speed; //MaJ de la vélocité

            if (this != null)
            {
                cachedTransform.position += velocity * Time.deltaTime; //Position tampon MaJ
                cachedTransform.forward = dir; //Orientation du Tampon selon direction
                position = cachedTransform.position; //Position du boid MaJ selon tampon
                forward = dir; //Orientation du boid MaJ selon direction Tampon
            }
        }
        if(DestructionAnim)
        {
            curTimer += Time.deltaTime;

            if(destructionType == DestructionType.Solo) transform.position = new Vector3(deathPos.x, transform.position.y- 75 * Time.deltaTime, deathPos.z);

            float scale = (cachedTransform.localScale.x /destructionTimer);
            scale *= Time.deltaTime;
            transform.localScale -= new Vector3(scale, scale, scale);


            if (curTimer > destructionTimer)
            {
                transform.position = new Vector3(0, -2000, 0);
                destructionType = DestructionType.none;
                isActive = false;
                DestructionAnim = false;
       
            }
        }
      
    }

    /// <summary>
    /// Detection de collision |
    /// Raycast en sphere |
    /// Return bool Collide
    /// </summary>
    /// <returns></returns>
    bool IsHeadingForCollision () {
        RaycastHit hit;

        /*
        if (Physics.SphereCast (position, settings.boundsRadius, forward, out hit, settings.collisionAvoidDst, settings.obstacleMask)) {
            return true;
        }*/
        if (Physics.Raycast (position, forward, out hit, settings.collisionAvoidDst, settings.obstacleMask)) {
            return true;
        }

        else { }
        return false;
    }

    /// <summary>
    /// Détection autour du flock
    /// </summary>
    /// <returns></returns>
    Vector3 ObstacleRays () {
        Vector3[] rayDirections = BoidHelper.directions; //Calcul selon sphere Raycast

        //Pour chaque raycast
        for (int i = 0; i < rayDirections.Length; i++)
        {
            //Nouvelle Direction : position Tampon convertit de local à World
            Vector3 dir = cachedTransform.TransformDirection (rayDirections[i]); 
            //
            Ray ray = new Ray (position, dir);

            /*
            if (!Physics.SphereCast (ray, settings.boundsRadius, settings.collisionAvoidDst, settings.obstacleMask)) {
                return dir;
            }*/
            
            if (!Physics.Raycast (ray, settings.collisionAvoidDst, settings.obstacleMask)) {
                return dir;
            }
        }
        return forward;
    }

    /// <summary>
    /// Sécurité | 
    /// IN : Current Vector3 | 
    /// OUT : Vector3 Future destination 
    /// </summary>
    /// <returns> </returns>
    Vector3 SteerTowards (Vector3 vector)
    {
        Vector3 v = vector.normalized * settings.maxSpeed - velocity; //Normalization du vector3 * VitesseMax - Vitesse actuelle
        return Vector3.ClampMagnitude (v, settings.maxSteerForce); //Retourne le vector3 clampé à la force Maximum 
    }

    
    public void HitBoid(Vector3Int gunSensitivity)
    {
        float x = Mathf.Abs((int)gunSensitivity.x - (int)sensitivity.x);
        float y = Mathf.Abs((int)gunSensitivity.y - (int)sensitivity.y);
        float z = Mathf.Abs((int)gunSensitivity.z - (int)sensitivity.z);


        float power = 18 - (x + y + z);

        float powerPerCent = (power / 18) * 100;

        int rnd = Random.Range(0, 101);
        if(rnd <= powerPerCent)
        {
            DestroyBoid(DestructionType.Solo);
        }



        if(powerPerCent > 90)
        {
            SC_HitMarker.Instance.HitMark(SC_HitMarker.HitType.Critical);
            koaManager.StopRegeneration();
            
        }
        else
        {
            SC_HitMarker.Instance.HitMark(SC_HitMarker.HitType.Normal);
        }
        koaManager.BoidHit(gunSensitivity);


    }

    public void DestroyBoid(DestructionType destructionType)
    {
        this.destructionType = destructionType;
        switch (destructionType)
        {
            case DestructionType.Solo:

                StartCoroutine(ImpactFrame());
                deathPos = transform.position;
                DestructionAnim = true;

                break;


            case DestructionType.Massive:

                StartCoroutine(ImpactFrame());
                deathPos = transform.position;
                DestructionAnim = true;

                break;
        }


    }


    IEnumerator ImpactFrame()
    {
        if(type != 4)
        {
            while (true)
            {
                if (curFlick % 2 == 0)
                {
                    meshRenderer.material = M_tabHit[1];
                }
                else
                {

                    meshRenderer.material = M_tabHit[0];
                }


                if (curFlick == TotalFlick)
                {

                    StopCoroutine(ImpactFrame());
                    break;
                }


                curFlick++;
                yield return new WaitForSeconds(0.1f);
            }
        }
      
    }
}