using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SC_TutorialUIManager : MonoBehaviour
{
    #region Singleton

    private static SC_TutorialUIManager _instance;
    public static SC_TutorialUIManager Instance { get { return _instance; } }

    #endregion

    [SerializeField]
    Image[][] ImageTab;
    Color[] InitColorTab;

    [SerializeField]
    Image[] Display;
    [SerializeField]
    Image[] Weapon; 
    [SerializeField]
    Image[] Motion;

    public enum System
    {
        Display = 0,
        Weapon,
        Motion
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

    void Start()
    {
        ImageTab = new Image[3][];
        ImageTab[0] = Display;
        ImageTab[1] = Weapon;
        ImageTab[2] = Motion;
    }

    void DeactivateImage(Image image)
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0.1f);
      ;
    }


    void ActivateImage(Image image)
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
    }

    
    public void ActivateSystem(System syst, bool activation)
    {
        for(int i = 0; i < ImageTab[(int)syst].Length; i++)
        {
            if(activation)
                ActivateImage(ImageTab[(int)syst][i]); 

            else
                DeactivateImage(ImageTab[(int)syst][i]);
        }

    }



}
