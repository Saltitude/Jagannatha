using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_UI_BlinkBreakdownManager : MonoBehaviour
{
    #region Singleton

    private static SC_UI_BlinkBreakdownManager _instance;
    public static SC_UI_BlinkBreakdownManager Instance { get { return _instance; } }

    #endregion

    private void Awake()
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
