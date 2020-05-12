using System.Collections;
using UnityEngine;

public class SC_FogBreakDown : MonoBehaviour
{
    #region Singleton

    private static SC_FogBreakDown _instance;
    public static SC_FogBreakDown Instance { get { return _instance; } }

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

    // Update is called once per frame
    void Update()
    {

    }

    public void BreakDownDensity()
    {
        this.GetComponent<Animator>().SetBool("isDown", true);
    }
    public void ClearDensity()
    {
        this.GetComponent<Animator>().SetBool("isDown", false);
    }
}
