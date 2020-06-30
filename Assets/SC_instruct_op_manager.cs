using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SC_instruct_op_manager : MonoBehaviour
{

    #region Singleton

    private static SC_instruct_op_manager _instance;
    public static SC_instruct_op_manager Instance { get { return _instance; } }

    #endregion


    public GameObject[] instruc_Go;

    [SerializeField]
    Image[] instruc_Img;

    public enum ChangeMat
    {
        ReturnDisplay,
        ReturnWeapon,
        ReturnMotion
    }

    [SerializeField]
    Transform returnDisplay;
    [SerializeField]
    Transform returnWeapon;
    [SerializeField]
    Transform returnMotion;

    [SerializeField]
    Material matLogoReturn;

    [SerializeField]
    Material matUIOn;
    [SerializeField]
    Material matUIInizialized;
    
    List<Image> initialized_img;
    
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
        initialized_img = new List<Image>();
        Image[] All = FindObjectsOfType<Image>();
        foreach (Image b in All)
        {
            if (b.material == matUIInizialized)
            {
                initialized_img.Add(b);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Activate(int index)
    {
        if (index < instruc_Go.Length)
        {

            instruc_Go[index].SetActive(true);

        }


    }

    public void Deactivate(int index)
    {
        if (index < instruc_Go.Length)
        {

            instruc_Go[index].SetActive(false);

        }
    }


    public void ActivateImage(int index)
    {
        if (index < instruc_Go.Length)
        {

            instruc_Img[index].enabled = true;

        }


    }

    public void DeactivateImage(int index)
    {
        if (index < instruc_Go.Length)
        {

            instruc_Img[index].enabled = false;

        }


    }


    public void ChangeMaterial(ChangeMat matToChange)
    {
        switch(matToChange)
        {
            case ChangeMat.ReturnDisplay:

                for(int i = 0; i < returnDisplay.childCount;i++)
                {
                    returnDisplay.GetChild(i).GetComponent<MeshRenderer>().material = matLogoReturn;
                }
                

                break;   
            
            case ChangeMat.ReturnMotion:

                for(int i = 0; i < returnMotion.childCount; i++)
                {
                    returnMotion.GetChild(i).GetComponent<MeshRenderer>().material = matLogoReturn;
                }

                break;
            
            case ChangeMat.ReturnWeapon:

                for(int i = 0; i < returnWeapon.childCount; i++)
                {
                    returnWeapon.GetChild(i).GetComponent<MeshRenderer>().material = matLogoReturn;
                }

                break;
        }
    }

    public void ChangeUIOnMat()
    {
        foreach(Image i in initialized_img)
        {
            i.material = matUIOn;
        }
    }

}
