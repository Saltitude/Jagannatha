using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SC_UI_BlinkBreakdownManager : MonoBehaviour
{
    #region Singleton

    private static SC_UI_BlinkBreakdownManager _instance;
    public static SC_UI_BlinkBreakdownManager Instance { get { return _instance; } }

    #endregion

    bool breakdown = false;

    [SerializeField]
    public Image vision;
    private void Awake()
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

    [SerializeField]
    Material matToBlink;
    Color color;

    [Range (0.2f,0.75f)]
    public float opacity;

   
    Image[] All;
    List<Image> toAnimate;

    void Start()
    {
        toAnimate = new List<Image>();
        All = FindObjectsOfType<Image>();
        vision = GameObject.FindGameObjectWithTag("VisionCone").GetComponent<Image>();
        foreach(Image b in All)
        {
            if (b.material == matToBlink)
            {
                toAnimate.Add(b);
            }
        }
        color = Color.white;
    }

    // Update is called once per frame
    void Update()
    {
        if(breakdown)
        {
            color = new Color(color.r, color.g, color.b, opacity);

            for (int i = 0; i < toAnimate.Count; i++)
            {
                toAnimate[i].color = color;

            }
            vision.color = new Color(0, 0,0, 0.75f-(opacity*0.7f));
        }


    }

    public void SetBreakdown(bool b)
    {
        breakdown = b;
        if(b == false)
        {
            StartCoroutine(LerpOpacity());
        }
    }

    IEnumerator LerpOpacity()
    {
        while(color.a <0.99)
        {
            float curOpacity = Mathf.Lerp(color.a, 1f, 0.10f); 
            for (int i = 0; i < toAnimate.Count; i++)
            {
                color = new Color(color.r, color.g, color.b, curOpacity);
                toAnimate[i].color = color;

            }
            Color colorVision = new Color(0, 0, 0, 0.75f - (curOpacity *0.7f));
            vision.color = colorVision;

            yield return 0;
        }
        for (int i = 0; i < toAnimate.Count; i++)
        {
            color = new Color(color.r, color.g, color.b, 1);
            toAnimate[i].color = color;

        }
        vision.color = new Color(0, 0, 0,  0);
        StopAllCoroutines();

    }

}
