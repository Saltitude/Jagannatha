using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SC_TargetMap : MonoBehaviour
{
    #region Singleton

    private static SC_TargetMap _instance;
    public static SC_TargetMap Instance { get { return _instance; } }

    #endregion

    Text textContainer;

    [SerializeField]
    float animTime;
    [SerializeField]
    float maxOpacity = 1;
    [SerializeField]
    float minOpacity = 0.75f;

    float ratePerSec;

    Vector4 colorTampon;
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

    public void SetText(string txt)
    {
        textContainer.text = txt;
        ratePerSec = ((maxOpacity - minOpacity) / animTime);
        colorTampon = textContainer.color;
    }

    public void StartAnimation()
    {
        StartCoroutine(Animation());
    }

    public void StopAnimation()
    {
        StopAllCoroutines();
    }

    IEnumerator Animation()
    {
        float curOpacity;
        bool Add = false;
        float t = 0;

        curOpacity = maxOpacity;

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

                textContainer.color = new Vector4(colorTampon.x, colorTampon.y, colorTampon.z, curOpacity);
            }
            yield return 0;
        }
    }
}
