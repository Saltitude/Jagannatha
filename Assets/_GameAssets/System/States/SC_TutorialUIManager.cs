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

    [SerializeField]
    Image[] displayHub;
    [SerializeField]
    Image[] weaponHub; 
    [SerializeField]
    Image[] motionHub;  
    
    [SerializeField]
    GameObject displayState;
    [SerializeField]
    GameObject weaponState; 
    [SerializeField]
    GameObject motionState;

    List<Image> img_Blink;

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
        img_Blink = new List<Image>();
        imageBlink = new Image[3][];
        imageState = new Image[3][];

        imageBlink[0] = displayHub;
        imageBlink[1] = weaponHub;
        imageBlink[2] = motionHub;

        imageState[0] = displayState.GetComponentsInChildren<Image>();
        imageState[1] = weaponState.GetComponentsInChildren<Image>();
        imageState[2] = motionState.GetComponentsInChildren<Image>();

        StartCoroutine(BlinkCoro());
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
    }

    void EndBlink(Image image)
    {
        foreach(Image i in img_Blink)
        {
            if(i == image)
            {
                i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
                img_Blink.Remove(i);
                break;
            }
        }
    }
    
    public void ActivateSystem(System syst, bool activation)
    {
        for(int i = 0; i < imageBlink[(int)syst].Length; i++)
        {
            if(activation)
                ActivateImage(imageBlink[(int)syst][i]); 

            else
                DeactivateImage(imageBlink[(int)syst][i]);
        }
        for(int i = 0;i < imageState[(int)syst].Length; i++)
        {
            if (activation)
                ActivateImage(imageState[(int)syst][i]);

            else
                DeactivateImage(imageState[(int)syst][i]);
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
    }


    IEnumerator BlinkCoro()
    {
        float animTime = 0.5f;
        float maxOpacity = 1;
        float minOpacity = 0f;
        float ratePerSec = (maxOpacity - minOpacity / animTime) * 2;
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


}
