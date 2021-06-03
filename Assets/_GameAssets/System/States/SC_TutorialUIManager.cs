using System;
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
    Image[][] imageBlink;

    [SerializeField]
    Image[][] imageState;

    Material[,] imageMaterial;

    [SerializeField]
    Image[] displayHub;
    [SerializeField]
    Image[] weaponHub; 
    [SerializeField]
    Image[] motionHub;

    [SerializeField]
    Image[] rebootHub;

    [SerializeField]
    GameObject displayState;
    [SerializeField]
    GameObject weaponState; 
    [SerializeField]
    GameObject motionState;


    [SerializeField]
    Material matBreakdown;

    List<Image> img_Blink;

    [SerializeField]
    ParticleSystem[] PS_RipleTuto;

    public enum System
    {
        Display = 0,
        Weapon,
        Motion,
        Reboot
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
        SetTab();
        ActivateSystem(System.Display, false);
        ActivateSystem(System.Weapon, false);
        ActivateSystem(System.Motion, false);
        StartCoroutine(BlinkCoro());
    }

    void SetTab()
    {
        img_Blink = new List<Image>();
        imageBlink = new Image[4][];
        imageState = new Image[3][];
        imageMaterial = new Material[4,2];

        imageBlink[0] = displayHub;
        imageBlink[1] = weaponHub;
        imageBlink[2] = motionHub;
        imageBlink[3] = rebootHub;  
        
        

        for (int i = 0; i < imageBlink.GetLength(0); i++)
        {
            for (int j = 0; j < imageBlink[i].Length; j++)
            {
                imageMaterial[i,j] = imageBlink[i][j].material;

            }
        }

        imageState[0] = displayState.GetComponentsInChildren<Image>();
        imageState[1] = weaponState.GetComponentsInChildren<Image>();
        imageState[2] = motionState.GetComponentsInChildren<Image>();

        for (int i = 0; i < PS_RipleTuto.Length; i++)
        {
            PS_RipleTuto[i].Stop();
        }

    }


    void DeactivateImage(Image image)
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0.1f);
      
    }


    void ActivateImage(Image image)
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
    }

    void ActivateBlink(Image image)
    {
        img_Blink.Add(image);
        image.material = matBreakdown;
    }

    void EndBlink(Image image)
    {
        foreach(Image i in img_Blink)
        {
            if(i == image)
            {
                i.color = new Color(i.color.r, i.color.g, i.color.b, 1);

                for (int u = 0; u < imageBlink.GetLength(0); u++)
                {
                    for (int v = 0; v < imageBlink[u].Length; v++)
                    {
                        if(imageBlink[u][v] == i)
                        imageBlink[u][v].material = imageMaterial[u,v];
                    }
                }

                img_Blink.Remove(i);
                break;
            }
        }
    }
    
    public void ActivateSystem(System syst, bool activation)
    {

        for(int i = 0;i < imageState[(int)syst].Length; i++)
        {
            if (activation)
                ActivateImage(imageState[(int)syst][i]);
            else
                DeactivateImage(imageState[(int)syst][i]);
        }
        for(int i = 0;i < imageBlink[(int)syst].Length; i++)
        {
            if (activation)
                ActivateImage(imageBlink[(int)syst][i]);
            else
                DeactivateImage(imageBlink[(int)syst][i]);
            
        }

    }

    public void ActivateBlink(System syst, bool blink)
    {


        for (int i = 0; i < imageBlink[(int)syst].Length; i++)
        {
            if (blink)
                ActivateBlink(imageBlink[(int)syst][i]);

            else
                EndBlink(imageBlink[(int)syst][i]);
        }

        CTA(syst, blink);
    }


    IEnumerator BlinkCoro()
    {
        float animTime = 0.5f;
        float maxOpacity = 1;
        float minOpacity = 0f;
        float ratePerSec = ((maxOpacity - minOpacity) / animTime);
        float curOpacity;
        bool Add = true;
        float t = 0;

        curOpacity = minOpacity;

        while (true)
        {
            if (t < animTime)
            {
                t += Time.deltaTime;
                if (Add)
                {

                    if (curOpacity < maxOpacity)
                        curOpacity = Mathf.Lerp(curOpacity, maxOpacity, ratePerSec * Time.deltaTime);
                }
                else
                {

                    if (curOpacity > minOpacity)
                        curOpacity = Mathf.Lerp(curOpacity, minOpacity, ratePerSec * Time.deltaTime);

                }

                for (int i = 0; i < img_Blink.Count; i++)
                {

                    img_Blink[i].color = new Vector4(img_Blink[i].color.r, img_Blink[i].color.g, img_Blink[i].color.b, curOpacity);
                }

            }
            else
            {
                Add = !Add;
                t = 0;
            }
            yield return 0;
        }

    }


    public void CTA(System syst,bool activate, bool btnReturn = false)
    {
        int index = (int)syst;
        if (btnReturn) index += 3;

        //Debug.Log(index);
        //Debug.Log(activate);

        PS_RipleTuto[index].Stop();

        if (activate)
        {
            PS_RipleTuto[index].Play();
        }
    }

}
