using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SC_RaycastOPMapPerspective : MonoBehaviour
{
    #region Singleton

    private static SC_RaycastOPMapPerspective _instance;
    public static SC_RaycastOPMapPerspective Instance { get { return _instance; } }

    #endregion
    //camera cockpit
    public GameObject Cam_Map;
    private Ray ray;
    public GameObject objectOnclic = null;
    GameObject OldObjectClic;
    public Text debugText;


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

    /// <summary>
    /// récupère le hit du cockpit et tire un rayon depuis les coordonnées UV de la collision avec l'écran.
    /// </summary>
    public void castRayInWorld(RaycastHit hit)
    {
       
        ray = this.GetComponent<Camera>().ViewportPointToRay(hit.textureCoord);

        //Debug.Log(hit.textureCoord);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.GetComponent<IF_Hover>() != null)
            {
                hit.collider.GetComponent<IF_Hover>().HoverAction();
            }
            if (Input.GetMouseButtonDown(0))
            {
                if (hit.collider.GetComponent<IF_KoaForOperator>() != null)
                {
                    if (hit.collider.GetComponent<SC_KoaSettingsOP>() != null)
                    {
                        if (objectOnclic != null)
                        {
                            OldObjectClic = objectOnclic;
                            var OldSelect = OldObjectClic.GetComponent<SC_KoaSettingsOP>();
                            OldSelect.bSelected = false;
                            OldSelect.SetMaterial(SC_KoaSettingsOP.koaSelection.None);
                        }

                        SC_UI_Display_MapInfos_KoaState.Instance.SetNewKoaSettings(hit.collider.GetComponent<SC_KoaSettingsOP>());

                        objectOnclic = hit.collider.gameObject;
                        var script = objectOnclic.GetComponent<SC_KoaSettingsOP>();
                        script.bSelected = true;
                        script.SetMaterial(SC_KoaSettingsOP.koaSelection.Selected);

                    }
                }
                //else
                //{
                //    Debug.Log("Clic on nothing on Map");
                //    objectOnclic = null;
                //    SC_UI_Display_MapInfos_KoaState.Instance.activated = false;
                //}
            }
            

             
            
        }
        SC_UI_Display_MapInfos_StateManager.Instance.checkState();
    }
}
