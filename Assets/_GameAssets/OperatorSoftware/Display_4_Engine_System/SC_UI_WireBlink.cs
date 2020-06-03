using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SC_UI_WireBlink : MonoBehaviour
{
    [SerializeField]
    Image[] img_ToBreakDown;

    Material[] wireSafe;

    [SerializeField]
    Material wireBreakdown;
    [SerializeField]
    Material wireShutdown;


    public bool[] IndexToActivate;

    int[] IndexValue;


    // Start is called before the first frame update
    void Start()
    {

        wireSafe = new Material[img_ToBreakDown.Length];
        IndexToActivate = new bool[img_ToBreakDown.Length];
        IndexValue = new int[img_ToBreakDown.Length];

        for (int i = 0; i < wireSafe.Length; i++)
        {
            wireSafe[i] = img_ToBreakDown[i].material;
            IndexValue[i] = 0;
        }

        StartCoroutine(RedWireCoro());
    }


    public void SetBreakDown(int index, bool activate)
    {

        if (activate)
        {
            
            IndexValue[index]++;
            if(!IndexToActivate[index])
            {
                img_ToBreakDown[index].material = wireBreakdown;
            }
   
        }
        else
        {
            IndexValue[index]--;
         
            if (IndexValue[index] <= 0)
            {
                IndexValue[index] = 0;
                EndCoroutine(index);

            }
        }
        IndexToActivate[index] = activate;

    }

    public void ShutDownWire(int index, bool activate)
    {
        if(activate)
            img_ToBreakDown[index].material = wireShutdown;
        else
            img_ToBreakDown[index].material = wireSafe[index];


    }


    void EndCoroutine(int index)
    {
        img_ToBreakDown[index].material = wireSafe[index];
        img_ToBreakDown[index].color = Color.white;
    }


    IEnumerator RedWireCoro()
    {
        float animTime = 0.5f;
        float maxOpacity = 1;
        float minOpacity = 0f;
        float ratePerSec = (maxOpacity - minOpacity / animTime) * 2;
        float curOpacity;
        bool Add = true;
        float t = 0;

        Vector4 ColorTampon = Color.white;
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

                for (int i = 0; i < img_ToBreakDown.Length; i++)
                {
                    if(IndexValue[i] > 0)
                    img_ToBreakDown[i].color = new Vector4(ColorTampon.x, ColorTampon.y, ColorTampon.z, curOpacity);
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
