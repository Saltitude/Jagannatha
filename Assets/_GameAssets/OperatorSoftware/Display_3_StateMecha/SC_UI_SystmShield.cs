using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SC_UI_SystmShield : MonoBehaviour
{
    [Range(0, 1)]
    [SerializeField]
    public float simpleValue;
    float fRatioValue = 0;
    [SerializeField]
    float speedBar;
    int nbImage;


    //Variable buffer vie 0
    public int bufferCounter = 0;

    public bool bufferIsRunning = false;

    Image[] tabImage;

    [SerializeField] SC_KoaSettingsOP koaSettings;
    // Start is called before the first frame update
    void Start()
    {
        nbImage = transform.childCount;
        tabImage = new Image[nbImage];

        for (int i = 0; i < nbImage; i++)
        {
            tabImage[i] = transform.GetChild(i).GetComponent<Image>();
        }

        //for (int i = nbImage - 1; i >= 0; i--)
        //{
        //    tabImage[i].enabled = true;
        //}
        //fRatioValue = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if(koaSettings != null)
        simpleValue = ratio(koaSettings.GetCurKoaLife(),10,1);
        if (bufferIsRunning == false)
            displayBar();
    }

    float ratio(float inputValue, float inputMax, float outputMax, float inputMin = 0.0f, float outputMin = 0.0f)
    {
        float product = (inputValue - inputMin) / (inputMax - inputMin);
        float output = ((outputMax - outputMin) * product) + outputMin;
        return output;
    }

    void displayBar()
    {
        fRatioValue = Mathf.Lerp(fRatioValue, ratio(simpleValue, 1, nbImage - 1, 0, 0), Time.deltaTime * speedBar);
        int ratioValue = Mathf.RoundToInt(fRatioValue);


        if (ratioValue != 0)
        {
            for (int i = ratioValue; i >= 0; i--)
            {
                tabImage[i].enabled = true;
            }
        }
        if (ratioValue != nbImage - 1)
        {
            for (int i = nbImage - 1; i >= ratioValue; i--)
            {
                tabImage[i].enabled = false;
            }
        }
    }

    public void LaunchCoroutine()
    {
        StartCoroutine(LifeToZero());

    }

    private IEnumerator LifeToZero()
    {
        bufferIsRunning = true;
        while (bufferCounter > 0)
        {
            int _ratioValue = Mathf.RoundToInt(fRatioValue);
            
            while (_ratioValue != 0)
            {
                
                fRatioValue = Mathf.Lerp(fRatioValue, 0, Time.deltaTime * speedBar);
                _ratioValue = Mathf.RoundToInt(fRatioValue);

                for (int i = nbImage - 1; i >= 0; i--)
                {
                    if (i >= _ratioValue)
                        tabImage[i].enabled = false;
                    else
                        tabImage[i].enabled = true;
                }

                yield return 0;
            }
            for (int i = nbImage-1; i >= 0; i--)
            {
                tabImage[i].enabled = true;
            }
            fRatioValue = nbImage-1;
            bufferCounter--;

            yield return 0;

        }
        bufferIsRunning = false;
        StopAllCoroutines();

    }


}
