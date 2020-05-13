using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_CordViewver : MonoBehaviour
{

    #region Singleton

    private static SC_CordViewver _instance;
    public static SC_CordViewver Instance { get { return _instance; } }

    #endregion

    [SerializeField]
    float ConstraintRange;
    [SerializeField]
    float DeadZone;
    [SerializeField]
    float AddMaxRange;

    [SerializeField]
    float CordRatio01;
    [SerializeField]
    float CordRatio02;
    [SerializeField]
    float CordRatio03;

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
        CordRatio01 = SC_SyncVar_MovementSystem.Instance.CordLenght01 / ConstraintRange + DeadZone + AddMaxRange;
        CordRatio02 = SC_SyncVar_MovementSystem.Instance.CordLenght02 / ConstraintRange + DeadZone + AddMaxRange;
        CordRatio03 = SC_SyncVar_MovementSystem.Instance.CordLenght03 / ConstraintRange + DeadZone + AddMaxRange;
    }

}
