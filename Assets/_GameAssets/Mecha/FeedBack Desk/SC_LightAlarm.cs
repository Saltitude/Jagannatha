using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_LightAlarm : MonoBehaviour
{
    #region Singleton

    private static SC_LightAlarm _instance;
    public static SC_LightAlarm Instance { get { return _instance; } }

    #endregion

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

    public void BreakDownLight()
    {
        this.GetComponent<Animator>().SetBool("isBreak", true);
    }
    public void ClearLight()
    {
        this.GetComponent<Animator>().SetBool("isBreak", false);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
