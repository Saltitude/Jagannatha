using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SC_TargetMap : MonoBehaviour
{
    #region Singleton

    private static SC_TargetMap _instance;
    public static SC_TargetMap Instance { get { return _instance; } }

    #endregion

    TextMeshPro textContainer;

    [SerializeField]
    float animTime;
    [SerializeField]
    float maxOpacity = 1;
    [SerializeField]
    float minOpacity = 0.75f;

    float ratePerSec;

    Vector4 colorTampon;

    [SerializeField]
    TMP_FontAsset VoiceActivated;
    [SerializeField]
    TMP_FontAsset sanskritFont;

    public enum FontList
    {
        Sanskri,
        VoiceActivated
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

        textContainer = this.GetComponent<TextMeshPro>();

    }

    public void SetText(string txt)
    {
        textContainer.text = txt;
        ratePerSec = ((maxOpacity - minOpacity) / animTime);
        colorTampon = textContainer.color;
    }

    public void SetFont(FontList font)
    {
        switch (font)
        {
            case FontList.Sanskri:

                textContainer.font = sanskritFont;

            break;

            case FontList.VoiceActivated:

                textContainer.font = VoiceActivated;

            break;
        }
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
