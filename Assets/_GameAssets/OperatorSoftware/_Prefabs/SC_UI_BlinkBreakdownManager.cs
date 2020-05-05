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
    Color color;

    [Range (0.2f,0.75f)]
    public float opacity;

   
    Image[] All;
    List<Image> toAnimate;

    void Start()
    {
        toAnimate = new List<Image>();
        All = FindObjectsOfType<Image>();
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
        for (int i = 0; i<toAnimate.Count; i++)
        {
            color = new Color(color.r, color.g, color.b, opacity);
            toAnimate[i].color = color;
     
        }

    }
}
