using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_JoystickMove : MonoBehaviour, IF_BreakdownSystem
{

    #region Singleton

    private static SC_JoystickMove _instance;
    public static SC_JoystickMove Instance { get { return _instance; } }

    #endregion

    #region Variables

    //Breakdown Infos
    [Header("Breakdown Infos")]
    [SerializeField]
    bool b_InBreakdown = false;
    [SerializeField]
    public bool b_BreakEngine = false;
    [SerializeField, Range(0, 3)]
    int n_BreakDownLvl = 0;
    public enum Dir { None, Left, Right, Off }
    public Dir CurBrokenDir = Dir.Left;

    //Coroutines Infos
    [Header("Acceleration Parameters")]
    [SerializeField]
    bool b_UseCoroutine = false;
    [Range(0, 2)]
    public float f_Duration = 0.5f;
    [SerializeField]
    AnimationCurve Acceleration;
    Coroutine CurMovementCoro;

    [Header("Acceleration Coroutine Infos")]
    public Dir CurDir = Dir.None;
    public Dir TargetDir = Dir.None;
    public Dir CoroDir = Dir.Off;

    //Rotation Horizontale
    [Header("Horizontal Rotation Settings")]
    [SerializeField]
    float f_RotationSpeedZ = 1.0f;
    float f_CurRotationSpeedZ = 1.0f;
    [SerializeField]
    float f_LerpRotZ = 1f;  
    public enum RotationMode { TSR, Torque, Normalize, Higher, Clamp }
    public RotationMode TypeRotationZ;
    float f_TransImpulseZ;    
    Quaternion TargetRotY;
    public float CurImpulse = 0;

    [Header("Horizontal Rotation Infos")]
    [SerializeField]
    int n_JoyNumToUse;
    [SerializeField]
    bool[] tab_TorqueAxes;
    public float f_TorqueImpulseZ;

    //Rotation Verticale
    [Header("Vertical Rotation Settings")]
    [SerializeField]
    bool EnableVerticalMovement = false;
    [SerializeField]
    bool b_InvertAxe = false;  
    [SerializeField]
    Transform TargetTRS;
    [Range(0.0f, 1.0f)]
    public float f_RotationSpeedX = 0.5f;
    [Range(0.0f, 1.0f)]
    public float f_LerpRotX = 1f;
    [Range(0.0f, 0.3f)]
    public float f_MaxRotUpX;
    public float f_ImpulseX;
    Quaternion xQuaternion;

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
    }

    void Start()
    {
        CheckTorqueAxis();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!b_BreakEngine)
        {
            GetImpulses();
            DebugGetImpulses();

            if(EnableVerticalMovement)
                VerticalRot();

            HorizontalRot();
        }        
    }

    #region Moves

    void CheckTorqueAxis()
    {
        n_JoyNumToUse = SC_DeviceManager.Instance.n_JoyNumToUse;
        tab_TorqueAxes = SC_DeviceManager.Instance.tab_TorqueAxesToUse;
    }

    void GetImpulses()
    {

        //Vertical Impulse
        f_ImpulseX = Input.GetAxis("Vertical") * f_RotationSpeedX;

        //Horizontal Impulses
        f_TransImpulseZ = Input.GetAxis("Horizontal") * f_CurRotationSpeedZ;

        if(n_JoyNumToUse == null || n_JoyNumToUse == 0)
            n_JoyNumToUse = SC_DeviceManager.Instance.n_JoyNumToUse;
     
        switch (n_JoyNumToUse)
        {

            case 1:
                f_TorqueImpulseZ = Input.GetAxis("Torque_01") * f_CurRotationSpeedZ;
                break;

            case 2:
                f_TorqueImpulseZ = Input.GetAxis("Torque_02") * f_CurRotationSpeedZ;
                break;

            case 3:
                f_TorqueImpulseZ = Input.GetAxis("Torque_03") * f_CurRotationSpeedZ;
                break;

            case 4:
                f_TorqueImpulseZ = Input.GetAxis("Torque_04") * f_CurRotationSpeedZ;
                break;

        }
        
        //Other Method
        /*
        f_TorqueImpulseZ = 0;

        for (int i = 0; i < tab_TorqueAxes.Length; i++)
        {

            if (tab_TorqueAxes[i])
            {

                switch (i)
                {

                    case 0:
                        f_TorqueImpulseZ += Input.GetAxis("Torque_01");
                        break;

                    case 1:
                        f_TorqueImpulseZ += Input.GetAxis("Torque_02");
                        break;

                    case 2:
                        f_TorqueImpulseZ += Input.GetAxis("Torque_03");
                        break;

                    case 3:
                        f_TorqueImpulseZ += Input.GetAxis("Torque_04");
                        break;

                }

            }                

        }

        f_TorqueImpulseZ *= f_CurRotationSpeedZ;
        */

    }

    void VerticalRot()
    {

        if (f_ImpulseX != 0)
        {

            if (!b_InvertAxe)
            {

                xQuaternion = Quaternion.AngleAxis(f_ImpulseX, Vector3.left);

                if (f_ImpulseX > 0 && TargetTRS.localRotation.x > -f_MaxRotUpX)
                    TargetTRS.localRotation *= Quaternion.Lerp(TargetTRS.localRotation, xQuaternion, f_LerpRotX);

                if (f_ImpulseX < 0 && TargetTRS.localRotation.x < 0)
                    TargetTRS.localRotation *= Quaternion.Lerp(TargetTRS.localRotation, xQuaternion, f_LerpRotX);

            }

            else if (b_InvertAxe)
            {

                xQuaternion = Quaternion.AngleAxis(-f_ImpulseX, Vector3.left);

                if (f_ImpulseX > 0 && TargetTRS.localRotation.x < 0)
                    TargetTRS.localRotation *= Quaternion.Lerp(TargetTRS.localRotation, xQuaternion, f_LerpRotX);

                if (f_ImpulseX < 0 && TargetTRS.localRotation.x > -f_MaxRotUpX)
                    TargetTRS.localRotation *= Quaternion.Lerp(TargetTRS.localRotation, xQuaternion, f_LerpRotX);

            }

        }

    }

    void HorizontalRot()
    {

        //Une Direction Input
        if (f_TorqueImpulseZ != 0 || f_TransImpulseZ != 0)
        {

            Quaternion zQuaternion = new Quaternion();
            float MixImpulseZ;
            

            //Calcul Selon Mode de Rotation
            switch (TypeRotationZ)
            {

                case RotationMode.TSR:
                    zQuaternion = Quaternion.AngleAxis(f_TransImpulseZ, Vector3.up);
                    CurImpulse = f_TransImpulseZ;
                    break;

                case RotationMode.Torque:
                    zQuaternion = Quaternion.AngleAxis(f_TorqueImpulseZ, Vector3.up);
                    CurImpulse = f_TorqueImpulseZ;
                    break;

                case RotationMode.Higher:
                    float absTorque = Mathf.Abs(f_TorqueImpulseZ);
                    float absTrans = Mathf.Abs(f_TransImpulseZ);
                    if (absTorque >= absTrans)
                    {
                        zQuaternion = Quaternion.AngleAxis(f_TorqueImpulseZ, Vector3.up);
                        CurImpulse = f_TorqueImpulseZ;
                    }
                    else
                    {
                        zQuaternion = Quaternion.AngleAxis(f_TransImpulseZ, Vector3.up);
                        CurImpulse = f_TransImpulseZ;
                    }
                    break;

                case RotationMode.Normalize:
                    MixImpulseZ = (Input.GetAxis("Rotation") + Input.GetAxis("Horizontal")) / 2 * f_RotationSpeedZ;
                    zQuaternion = Quaternion.AngleAxis(MixImpulseZ, Vector3.up);
                    CurImpulse = MixImpulseZ;
                    break;

                case RotationMode.Clamp:
                    MixImpulseZ = Input.GetAxis("Rotation") + Input.GetAxis("Horizontal");
                    if (MixImpulseZ > 1)
                        MixImpulseZ = 1;
                    MixImpulseZ *= f_RotationSpeedZ;
                    zQuaternion = Quaternion.AngleAxis(MixImpulseZ, Vector3.up);
                    CurImpulse = MixImpulseZ;
                    break;

                default:
                    break;

            }

            //Direction
            if (CurImpulse > 0)
                TargetDir = Dir.Right;
            else if (CurImpulse < 0)
                TargetDir = Dir.Left;

            //Defini la rotation ciblé
            TargetRotY = this.transform.rotation * zQuaternion;

            if(CurBrokenDir == CurDir)
            {

                if (b_UseCoroutine && CurDir != TargetDir && CoroDir != TargetDir && n_BreakDownLvl < 3)
                {
                    //Debug.Log("CheckDir01");
                    CheckDir();
                }
                    
                else if ( ( !b_UseCoroutine || ( CoroDir == Dir.Off && CurDir == TargetDir ) ) && n_BreakDownLvl < 2 )
                    transform.rotation = Quaternion.Slerp(transform.rotation, TargetRotY, f_LerpRotZ);

            }

            else
            {

                if (b_UseCoroutine && CurDir != TargetDir && CoroDir != TargetDir)
                {
                    //Debug.Log("CheckDir02");
                    CheckDir();
                }      
                
                else if (!b_UseCoroutine || (CoroDir == Dir.Off && CurDir == TargetDir))
                    transform.rotation = Quaternion.Slerp(transform.rotation, TargetRotY, f_LerpRotZ);

            }        

        }
        
        //Pas de Direction
        else
        {

            TargetDir = Dir.None;
            TargetRotY = this.transform.rotation;

            if (b_UseCoroutine && CurDir != TargetDir && CoroDir != TargetDir)
            {
                //Debug.Log("CheckDir03");
                CheckDir();
            }
                
            CurImpulse = 0; 

        }

    }

    #endregion Moves

    #region Coroutines Functions

    void CheckDir()
    {

        if(CurMovementCoro != null)
            StopCoroutine(CurMovementCoro);

        if (TargetDir == Dir.None)
        {
            CurMovementCoro = StartCoroutine(GoTargetRot(f_Duration, Dir.None));
        }         
        else if (CurDir == Dir.None)
        {
            CurMovementCoro = StartCoroutine(GoTargetRot(f_Duration, TargetDir));
        }
        else
        {
            CurMovementCoro = StartCoroutine(GoTargetRot(f_Duration * 2, TargetDir));
        }
            
    }

    IEnumerator GoTargetRot(float Duration, Dir ToDir)
    {

        CoroDir = ToDir;
        SC_SyncVar_MovementSystem.Instance.CoroDir = CoroDir;

        SetCurDir(ToDir);

        float t = 0;
        float rate = 1 / Duration;

        Quaternion StartRot = transform.rotation;

        while (t < 1)
        {

            t += Time.deltaTime * rate;
            float Lerp = Acceleration.Evaluate(t); 

            transform.rotation = Quaternion.Slerp(StartRot, TargetRotY, Lerp);

            yield return 0;

        }

        

        CoroDir = Dir.Off;

        SC_SyncVar_MovementSystem.Instance.CoroDir = CoroDir;

    }

    #endregion

    public void SetCurDir(Dir TargetDir)
    {
        CurDir = TargetDir;
        SC_SyncVar_MovementSystem.Instance.CurDir = TargetDir;
    }

    public void SetBrokenDir(Dir TargetDir)
    {
        CurBrokenDir = TargetDir;
        SC_SyncVar_MovementSystem.Instance.CurBrokenDir = TargetDir;
    }

    #region BreakDown

    public void SetBreakdownState(bool State)
    {
        b_InBreakdown = State;
    }

    public void SetEngineBreakdownState(bool State)
    {
        b_BreakEngine = State;
    }

    public void AlignBreakdownLevel(int n_Level)
    {

        n_BreakDownLvl = n_Level;

        if (n_BreakDownLvl == 0)
            f_CurRotationSpeedZ = f_RotationSpeedZ;
        else
            f_CurRotationSpeedZ = f_RotationSpeedZ / 2;

    }

    #endregion

    #region DebugMethods

    /// <summary>
    /// Get Impulse by Keyboard | 
    /// Overwrite JoyStick Value | 
    /// </summary>
    void DebugGetImpulses()
    {

        //Horizontal Impulses
        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.K))
        {
            f_TorqueImpulseZ = -1 * f_CurRotationSpeedZ;
            f_TransImpulseZ = -1 * f_CurRotationSpeedZ;
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.M))
        {
            f_TorqueImpulseZ = 1 * f_CurRotationSpeedZ;
            f_TransImpulseZ = 1 * f_CurRotationSpeedZ;
        }

        //Vertical Impulse
        if (Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.O))
        {
            f_ImpulseX = 1 * f_RotationSpeedX;
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.L))
        {
            f_ImpulseX = -1 * f_RotationSpeedX;
        }

    }

    #endregion DebugMethods

}