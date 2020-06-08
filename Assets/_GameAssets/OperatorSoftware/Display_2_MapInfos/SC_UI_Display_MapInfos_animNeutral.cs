using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SC_UI_Display_MapInfos_animNeutral : MonoBehaviour
{
    [SerializeField]
    Text neutralText;
    bool onUp = true;
    float compt;
    float delay = 1 ;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(OpacityCoro());
    }

    // Update is called once per frame
    void Update()
    {
      

    }
    /*
    void animText(Text textToAnim)
    {
        if (onUp)
        {
            compt += Time.deltaTime;
            if (compt < delay)
            {
                textToAnim.transform.localScale += new Vector3(0.01f, 0.01f, 0.01f);
            }
            else if (compt >= delay)
            {
                onUp = false;
            }
        }
        else
        {
            compt -= Time.deltaTime;
            if(compt > 0)
            {
                textToAnim.transform.localScale -= new Vector3(0.01f, 0.01f, 0.01f);
            }
            else if(compt <= 0)
            {
                onUp = true;
            }
        }
        
        
    }*/

    IEnumerator OpacityCoro()
    {
        float animTime = 1f;
        float maxOpacity = 1;
        float minOpacity = 0f;
        float ratePerSec = ((maxOpacity - minOpacity) / animTime);
        float curOpacity;
        bool Add = true;
        float t = 0;

        Vector4 ColorTampon = neutralText.color;
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

                neutralText.color = new Vector4(ColorTampon.x, ColorTampon.y, ColorTampon.z, curOpacity); 

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
