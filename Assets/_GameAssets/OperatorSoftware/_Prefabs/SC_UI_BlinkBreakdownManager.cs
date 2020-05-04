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
    Color32 color;

    [Range (0,1)]
    public float opacity;

   
    Image[] All;
    List<Image> toAnimate;

    void Start()
    {
        toAnimate = new List<Image>();
        All = FindObjectsOfType<Image>();
        foreach(Image b in All)
        {
            if(b == matToBlink)
            {
                toAnimate.Add(b);
            }
        }
        color = matToBlink.color;
    }

    // Update is called once per frame
    void Update()
    {

        for(int i = 0; i<toAnimate.Count; i++)
        {
            opacity *= 255;
            byte opacityB = (byte)Mathf.RoundToInt(opacity);
            Debug.Log(opacityB);

            toAnimate[i].material.color = new Color32(color.r, color.g, color.b, opacityB);
     
        }

    }
}
