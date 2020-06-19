using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_SplinesOnSplines : MonoBehaviour
{

    #region Singleton

    private static SC_SplinesOnSplines _instance;
    public static SC_SplinesOnSplines Instance { get { return _instance; } }

    #endregion

    [SerializeField]
    public BezierSolution.BezierSpline[] tab;

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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

}
