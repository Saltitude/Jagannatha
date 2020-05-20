using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SC_Display_flux_bar : MonoBehaviour
{
    [Range(0, 1)]
    [SerializeField]
    public float simpleValue;
    float fRatioValue;
    [SerializeField]
    float speedBar;
    int nbImage;

    Image[] tabImage;
    // Start is called before the first frame update
    void Start()
    {
        nbImage = transform.childCount;
        tabImage = new Image[nbImage];

        for (int i = 0; i < nbImage; i++)
        {
            tabImage[i] = transform.GetChild(i).GetComponent<Image>();
        }
    }

    // Update is called once per frame
    void Update()
    {
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

        fRatioValue = Mathf.Lerp(fRatioValue, ratio(simpleValue, 10, nbImage, 0, 0), Time.deltaTime * speedBar * 5);


        int ratioValue = Mathf.RoundToInt(fRatioValue);

        //Debug.Log(simpleValue);

        //if (ratioValue != 0)
        //{

        for (int i = ratioValue - 1; i >= 0; i--)
        {
            tabImage[i].enabled = true;
        }

        //}

        if (simpleValue - 1 != nbImage - 1)
        {
            for (int i = nbImage - 1; i >= ratioValue; i--)
            {
                tabImage[i].enabled = false;
            }
        }
    }
}
