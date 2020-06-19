using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// Calcul des déplacement sur la spline voulue
///  | Sur le prefab Flock
///  | Auteur : Bezier Solution (Adapté par Zainix)
/// </summary>
namespace BezierSolution
{
    public class BezierWalkerWithSpeed : BezierWalker
	{

        //---------------------------- VARIABLES MANIPULABLES ------------------------------//

        BezierSpline curSpline; //Spline acutel surlequel le flock va se déplacer
		public TravelMode travelMode; //Type de déplacement sur cette spline | Ping-Pong, Loop et Once
        GameObject target;
        public LookAtMode lookAt = LookAtMode.Forward; //Type de lookAt de l'objet suivant la spline | Forward, None et Extra-Data(tkt moi non plus j'ai pas compris)
        public float speed = 5f; //Vitesse de déplacement sur cette spline
        public float rotationLerpModifier = 10f;//Fidélité de la rotation sur la spline
        bool isGoingForward = true; //Sens de déplacement sur la spline (Forward ou Backward)

        GameObject TargetSlineExtraData;

        [SerializeField]
		[Range( 0f, 1f )]
		private float m_normalizedT = 0f; //Position actuel du flock sur la curSpline

        //---------------------------- GET ------------------------------//
        public override BezierSpline Spline { get { return curSpline; } set { curSpline = value; }}
		public override float NormalizedT
		{
			get { return m_normalizedT; }
			set { m_normalizedT = value; }
		}
		public override bool MovingForward {
            get { return ( speed > 0f ) == isGoingForward; }
            set { isGoingForward = value; }
        }


        //TKT FRATE CA MARCHE TOUT SEUL
		public UnityEvent onPathCompleted = new UnityEvent();
		private bool onPathCompletedCalledAt1 = false;
		private bool onPathCompletedCalledAt0 = false;



        void Start()
        {
            target = GameObject.FindGameObjectWithTag("Player");
        }

        /// <summary>
        /// Change la sline de déplacement actuel | Param : BezierSolution.BezierSpline nouvelle spline a suivre (peut etre null)
        /// </summary>
        /// <param name="newSpline"></param>
        public void SetNewSpline(BezierSpline newSpline)
        {
            //Actualisation de la spline a suivre
            curSpline = newSpline;
            NormalizedT = 0;

            //Si la spline existe
            if(newSpline != null)
            {
                //Récupère le point de la spline le plus proche du flock
                Vector3 v3 = curSpline.FindNearestPointTo(transform.position, out float normalizedT, 1000f);
  
                //Se place sur la spline a la position de ce point
                NormalizedT = normalizedT;
            }
        }


        public void SetNewTarget(GameObject newTarget)
        {
            target = newTarget;
        }

        /// <summary>
        /// Effectue le déplacement sur la spline actuel, code conçu par les auteurs du plugg-in, ne pas me demandé
        /// </summary>
        /// <param name="deltaTime"></param>
		public override void Execute( float deltaTime )
		{
            if(curSpline != null)
            {
                float targetSpeed = (isGoingForward) ? speed : -speed;

                Vector3 targetPos = curSpline.MoveAlongSpline(ref m_normalizedT, targetSpeed * deltaTime);

                transform.position = targetPos;
                //transform.position = Vector3.Lerp( transform.position, targetPos, movementLerpModifier * deltaTime );

                bool movingForward = MovingForward;

                if (lookAt == LookAtMode.Forward)
                {
                    Quaternion targetRotation;
                    if (movingForward)
                        targetRotation = Quaternion.LookRotation(curSpline.GetTangent(m_normalizedT));
                    else
                        targetRotation = Quaternion.LookRotation(-curSpline.GetTangent(m_normalizedT));

                    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationLerpModifier * deltaTime);
                }
                else if (lookAt == LookAtMode.SplineExtraData)
                target.transform.rotation = Quaternion.Lerp(target.transform.rotation, target.transform.rotation, rotationLerpModifier * deltaTime);

                if (movingForward)
                {
                    if (m_normalizedT >= 1f)
                    {
                        if (!onPathCompletedCalledAt1)
                        {
                            onPathCompletedCalledAt1 = true;
#if UNITY_EDITOR
                            if (UnityEditor.EditorApplication.isPlaying)
#endif
                                onPathCompleted.Invoke();
                        }

                        if (travelMode == TravelMode.Once)
                            m_normalizedT = 1f;
                        else if (travelMode == TravelMode.Loop)
                            m_normalizedT -= 1f;
                        else
                        {
                            m_normalizedT = 2f - m_normalizedT;
                            isGoingForward = !isGoingForward;
                        }
                    }
                    else
                    {
                        onPathCompletedCalledAt1 = false;
                    }
                }
                else
                {
                    if (m_normalizedT <= 0f)
                    {
                        if (!onPathCompletedCalledAt0)
                        {
                            onPathCompletedCalledAt0 = true;
#if UNITY_EDITOR
                            if (UnityEditor.EditorApplication.isPlaying)
#endif
                                onPathCompleted.Invoke();
                        }

                        if (travelMode == TravelMode.Once)
                            m_normalizedT = 0f;
                        else if (travelMode == TravelMode.Loop)
                            m_normalizedT += 1f;
                        else
                        {
                            m_normalizedT = -m_normalizedT;
                            isGoingForward = !isGoingForward;
                        }
                    }
                    else
                    {
                        onPathCompletedCalledAt0 = false;
                    }
                }

            }

        }
	}
}