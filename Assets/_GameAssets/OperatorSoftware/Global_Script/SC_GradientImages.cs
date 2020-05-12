using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[ExecuteInEditMode]
public class SC_GradientImages : MonoBehaviour
{
    //[SerializeField]
    Color32 startColor = new Color32(221,22,22, 255);

    //[SerializeField]

    Color32 endColor = new Color32(255, 159, 0, 255);

    float factor = 1f;

    int nbImage;
    Image[] tabImage;

    // Start is called before the first frame update
    void Start()
    {
        nbImage = transform.childCount;
        tabImage = new Image[nbImage];
        float red = startColor.r;
        float green = startColor.g;
        float blue = startColor.b;
        float deltaRed = (startColor.r - endColor.r) / nbImage ;
        
        float deltaGreen = (startColor.g - endColor.g) / nbImage;
        float deltaBlue = (startColor.b - endColor.b) / nbImage;

        for (int i = 0; i < nbImage; i++)
        {
            tabImage[i] = transform.GetChild(i).GetComponent<Image>();
            float newRed = red + (-deltaRed) * i ;
            float newGreen = green+ (-deltaGreen) * i ;
            float newBlue = blue+ (-deltaBlue) * i ;
            tabImage[i].color = new Color32 ((byte)newRed, (byte)newGreen, (byte)newBlue, 255);
        }
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
