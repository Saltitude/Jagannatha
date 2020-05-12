using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SC_UI_RaycastOPManager : MonoBehaviour
{
    #region Singleton

    private static SC_UI_RaycastOPManager _instance;
    public static SC_UI_RaycastOPManager Instance { get { return _instance; } }

    #endregion

    private RaycastHit hit;

    [SerializeField]
    Camera camRaycast;

    [SerializeField]
    Camera[] otherCam;
    void Start()
    {
        //camRaycast = GetComponent<Camera>();
    }
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


    // Update is called once per frame
    void Update()
    {

        Ray ray = camRaycast.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {

            SC_RaycastOPMapPerspective.Instance.castRayInWorld(hit);

            if (hit.collider.GetComponent<IF_Hover>() != null)
            {
                hit.collider.GetComponent<IF_Hover>().HoverAction();
            }
            //else
            //    hit.collider.GetComponent<IF_Hover>().OutAction();

            //Cast un ray à partir du casque
            if (Input.GetMouseButtonDown(0))
            {
                
                if (hit.collider.GetComponent<IF_clicableAction>() != null)
                {
                    hit.collider.GetComponent<IF_clicableAction>().Action();
                }

            }
        }


     
        #region CheatChode

        if (Input.GetKeyDown(KeyCode.F5))
        {
            camRaycast = otherCam[0];
        }
        if (Input.GetKeyDown(KeyCode.F6))
        {
            camRaycast = otherCam[1];
        }
        if (Input.GetKeyDown(KeyCode.F7))
        {
            camRaycast = otherCam[2];
        }
        if (Input.GetKeyDown(KeyCode.F8))
        {
            camRaycast = otherCam[3];
        }

        #endregion

    }

}
