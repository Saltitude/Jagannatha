using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour {

    BoidSettings settings;
    bool isKoa = false;
    bool koaTargetWeight = false;
    public bool isActive = false;
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
     
    // Cached

    Transform cachedTransform; //To define
    public Transform target; //Target

    void Awake () {
        cachedTransform = transform; //Position tampon
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
    public void Initialize (BoidSettings settings, Transform target, bool isKoa=false)
    {
        this.isKoa = isKoa;
        this.target = target; //Peut être null
        this.settings = settings; //Scriptable object

        position = cachedTransform.position; //Déplacement à la position tampon
        forward = cachedTransform.forward; //Direction selon axe X

        float startSpeed = (settings.minSpeed + settings.maxSpeed) / 2; //Vitesse d'initialisation
        velocity = transform.forward * startSpeed; //Stockage de la vélocité selon la vitesse de départ

        isActive = true;
    }
    /// <summary>
    /// Changer la couleur de chaque boid |
    /// Sécurité
    /// </summary>
    /// <param name="col"></param>
    /// 



    /// <summary>
    /// Update fait maison |
    /// Appelé à chaque frame dans l'update du BoidManager
    /// </summary>
    public void UpdateBoid () {

        if(isActive)
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
      
    }

    /// <summary>
    /// Detection de collision |
    /// Raycast en sphere |
    /// Return bool Collide
    /// </summary>
    /// <returns></returns>
    bool IsHeadingForCollision () {
        RaycastHit hit;
        if (Physics.SphereCast (position, settings.boundsRadius, forward, out hit, settings.collisionAvoidDst, settings.obstacleMask)) {
            return true;
        } else { }
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
            if (!Physics.SphereCast (ray, settings.boundsRadius, settings.collisionAvoidDst, settings.obstacleMask)) {
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

    
    public void DestroyBoid()
    {
        isActive = false;
    }
  

}