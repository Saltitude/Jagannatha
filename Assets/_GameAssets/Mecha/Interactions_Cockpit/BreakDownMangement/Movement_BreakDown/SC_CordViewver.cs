using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_CordViewver : MonoBehaviour
{

    #region Singleton

    private static SC_CordViewver _instance;
    public static SC_CordViewver Instance { get { return _instance; } }

    #endregion

    [Header("Basics Infos")]
    [SerializeField]
    float ConstraintRange;
    [SerializeField]
    float DeadZone;
    [SerializeField]
    float AddMaxRange;

    [Header("Ratio Infos")]
    [SerializeField]
    float CordRatio01;
    [SerializeField]
    bool b_Cord01Valid;
    [SerializeField]
    float CordRatio02;
    [SerializeField]
    bool b_Cord02Valid;
    [SerializeField]
    float CordRatio03;
    [SerializeField]
    bool b_Cord03Valid;

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
        GetBaseValue();
    }

    // Update is called once per frame
    void Update()
    {
        ComputeRatio();
    }

    public void GetBaseValue()
    {
        ConstraintRange = SC_SyncVar_MovementSystem.Instance.ConstraintRange;
        DeadZone = SC_SyncVar_MovementSystem.Instance.DeadZone;
        AddMaxRange = SC_SyncVar_MovementSystem.Instance.AddMaxRange;
        ComputeRatio();
    }

    public void ComputeRatio()
    {

        CordRatio01 = SC_SyncVar_MovementSystem.Instance.CordLenght01 / (ConstraintRange + DeadZone + AddMaxRange);
        CordRatio02 = SC_SyncVar_MovementSystem.Instance.CordLenght02 / (ConstraintRange + DeadZone + AddMaxRange);
        CordRatio03 = SC_SyncVar_MovementSystem.Instance.CordLenght03 / (ConstraintRange + DeadZone + AddMaxRange);

        if (SC_SyncVar_MovementSystem.Instance.CordLenght01 > (ConstraintRange + DeadZone) && !b_Cord01Valid)
            b_Cord01Valid = true;
        else if(b_Cord01Valid)
            b_Cord01Valid = false;

        if (SC_SyncVar_MovementSystem.Instance.CordLenght02 > (ConstraintRange + DeadZone) && !b_Cord02Valid)
            b_Cord02Valid = true;
        else if (b_Cord01Valid)
            b_Cord02Valid = false;

        if (SC_SyncVar_MovementSystem.Instance.CordLenght03 > (ConstraintRange + DeadZone) && !b_Cord03Valid)
            b_Cord03Valid = true;
        else if (b_Cord03Valid)
            b_Cord03Valid = false;

    }

}
