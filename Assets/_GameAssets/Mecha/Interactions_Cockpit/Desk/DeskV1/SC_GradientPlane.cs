using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[ExecuteInEditMode]
public class SC_GradientPlane : MonoBehaviour
{
    //[SerializeField]
    Color32 startColor = new Color32(204,61,00, 255);

    //[SerializeField]

    Color32 endColor = new Color32(253, 255, 185, 255);

    float factor = 1f;

    int nbPlane;
    MeshRenderer[] tabObject;

    // Start is called before the first frame update
    void Start()
    {
        nbPlane = transform.childCount;
        tabObject = new MeshRenderer[nbPlane];
        float red = startColor.r;
        float green = startColor.g;
        float blue = startColor.b;
        float deltaRed = (startColor.r - endColor.r) / nbPlane ;
        
        float deltaGreen = (startColor.g - endColor.g) / nbPlane;
        float deltaBlue = (startColor.b - endColor.b) / nbPlane;

        for (int i = 0; i < nbPlane; i++)
        {
            tabObject[i] = transform.GetChild(i).GetComponent<MeshRenderer>();
            float newRed = red + (-deltaRed) * i;
            float newGreen = green + (-deltaGreen) * i;
            float newBlue = blue + (-deltaBlue) * i;
            tabObject[i].GetComponent<MeshRenderer>().material.color = new Color32((byte)newRed, (byte)newGreen, (byte)newBlue, 255);
            //tabMat[i].color = new Color32 ((byte)newRed, (byte)newGreen, (byte)newBlue, 255);
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
