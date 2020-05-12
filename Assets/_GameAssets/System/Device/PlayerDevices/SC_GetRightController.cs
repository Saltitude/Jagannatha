using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_GetRightController : MonoBehaviour
{

    #region Singleton

    private static SC_GetRightController _instance;
    public static SC_GetRightController Instance { get { return _instance; } }

    #endregion

    [SerializeField]
    GameObject GripPoint;

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

    public GameObject getGameObject()
    {
        return gameObject;
    }

}
