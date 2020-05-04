using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEngine.UI;

/// <summary>
/// Script de gestion du Display Operateur |
/// Made by Tienou & Modif de Leni |
/// </summary>
public class SC_CheckDevice : MonoBehaviour
{

    public enum DisplayType { monoscreen, fourScreen } //Différentes configurations d'écrans

    [Header("Configuration des écrans")]
    public DisplayType displayMode; //Modifiable dans l'éditeur | Change selon 1 ecran ou 4 ecrans
    public bool b_InputToScreen = false;

    [Header("Drop the Camera")]
    [Tooltip("Tableau avec toute les Cameras")]
    [SerializeField]
    Camera[] tab_Cam; //Tableau des cameras

    [Header("Drop the Canvas")]
    [Tooltip("Tableau avec tous les canvas")]
    [SerializeField]
    Canvas[] tab_Canv; //Tableau des canvas

    Vector2 CursorPos = new Vector2(0, 0);
    int displayIndex = 0;



    // Start is called before the first frame update
    void Start()
    {

        if (displayMode == DisplayType.monoscreen)
            MonoScreen();

        if (displayMode == DisplayType.fourScreen)
        {
            MultiScreens();
            if (b_InputToScreen)
                DiseableRaycaster(2);
        }

    }

    void Update()
    {
        if (b_InputToScreen)
            InputToScreen();
    }

    void MonoScreen()
    {
        
        for (int i = 0; i < tab_Canv.Length ; i++)
        {
            tab_Canv[i].GetComponent<Canvas>().worldCamera = tab_Cam[0];
        }

        for (int i = 1; i < tab_Cam.Length ; i++)
        {
            tab_Cam[i].enabled = false;
        }

        ChangeOrder(2);

    }

    void MultiScreens()
    {
        
        for (int i = 1; i < Display.displays.Length ; i++)
        {
            Display.displays[i].Activate(1920, 1080, 120); //Active les écrans (build)
        }

        tab_Cam[0].enabled = false;
        tab_Canv[0].enabled = false;

    }

    /// <summary>
    /// Change les Order in Layer des Canvas pour affiché le Canvas voulu |
    /// </summary>
    /// <param name="TargetCanvIndex"></param>
    public void ChangeOrder(int TargetCanvIndex)
    {
        for (int i = 1; i < tab_Canv.Length ; i++)
        {
            if (i != TargetCanvIndex)
            {
                tab_Canv[i].sortingOrder = 0;
            }
            else if (i == TargetCanvIndex)
            {
                tab_Canv[i].sortingOrder = 10;
            }
        }
    }

    public void DiseableRaycaster(int TargetCanvIndex)
    {
        for (int i = 1; i < tab_Canv.Length; i++)
        {
            if (i != TargetCanvIndex)
            {
                tab_Canv[i].GetComponent<GraphicRaycaster>().enabled = false;
            }
            else if (i == TargetCanvIndex)
            {
                tab_Canv[i].GetComponent<GraphicRaycaster>().enabled = true;
            }
        }
    }

    void InputToScreen()
    {
        if (Input.GetKey(KeyCode.F1))
            DiseableRaycaster(1);
        if (Input.GetKey(KeyCode.F2))
            DiseableRaycaster(2);
        if (Input.GetKey(KeyCode.F3))
            DiseableRaycaster(3);
        if (Input.GetKey(KeyCode.F4))
            DiseableRaycaster(4);
    }

}
