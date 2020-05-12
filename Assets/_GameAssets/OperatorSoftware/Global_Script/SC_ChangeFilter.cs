using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Change les Contents Actifs et Inactifs des Canvas Map et MapInfos selon le Filtre voulu |
/// Script by Leni |
/// </summary>
public class SC_ChangeFilter : MonoBehaviour
{

    public GameObject Canv_Map;
    public GameObject Canv_MapInfos;

    [Header("Map Filters")]
    [Tooltip("Tableau avec tout les Filtres de Canv_Map")]
    [SerializeField]
    GameObject[] tab_MapFilters; //Tableau des Filtres

    [Header("MapInfos Filters")]
    [Tooltip("Tableau avec tout les Filtres de Canv_MapInfos")]
    [SerializeField]
    GameObject[] tab_MapInfosFilters; //Tableau des Filtres

    [Header("Buttons Filters")]
    [Tooltip("Tableau avec tout les Btn Filtres")]
    [SerializeField]
    GameObject[] tab_BtnFilters; //Tableau des Btn

    public enum ButtonSide { Left, Right } //Différentes configurations d'écrans

    [Header("Configuration des écrans")]
    public ButtonSide curBtnSide; //Modifiable dans l'éditeur | Change selon 1 ecran ou 4 ecrans

    float f_LeftPos = -690;
    float f_RightPos = 690;
    public float f_SidePos = 0;

    int n_CurIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (curBtnSide == ButtonSide.Left)
            f_SidePos = f_LeftPos;

        if (curBtnSide == ButtonSide.Right)
            f_SidePos = f_RightPos;

        tab_BtnFilters[n_CurIndex].transform.localPosition = new Vector3(0, 0, 0);

        SetFilter(n_CurIndex);

    }
    
    /// <summary>
    /// Active le Filtre Correspondant a l'Index |
    /// 0 = MechFilter |
    /// 1 = PathFilter |
    /// 2 = HeartFilter |
    /// </summary>
    /// <param name="n_FilterIndex"></param>
    public void SetFilter (int n_FilterIndex)
    {
        //Active les Contents du Filtre voulu et desactive le reste
        for (int i = 0; i < tab_MapFilters.Length; i++)
        {
            if (i != n_FilterIndex)
            {
                tab_MapFilters[i].SetActive(false);
                tab_MapInfosFilters[i].SetActive(false);
            }
            else if (i == n_FilterIndex)
            {
                tab_MapFilters[i].SetActive(true);
                tab_MapInfosFilters[i].SetActive(true);
            }
        }

        //Active tout les btn
        tab_BtnFilters[n_CurIndex].SetActive(true);
        tab_BtnFilters[n_FilterIndex].SetActive(true);

        //Enregistre leur position
        Vector3 Vt3_CurBtnPos_Temp = new Vector3(f_SidePos, tab_BtnFilters[n_CurIndex].transform.localPosition.y, tab_BtnFilters[n_CurIndex].transform.localPosition.z);
        Vector3 Vt3_TargetBtnPos_Temp = new Vector3(f_SidePos, tab_BtnFilters[n_FilterIndex].transform.localPosition.y, tab_BtnFilters[n_FilterIndex].transform.localPosition.z);

        //Echange les Pos
        tab_BtnFilters[n_CurIndex].transform.localPosition = Vt3_TargetBtnPos_Temp;
        tab_BtnFilters[n_FilterIndex].transform.localPosition = Vt3_CurBtnPos_Temp;

        tab_BtnFilters[n_CurIndex].SetActive(true);
        tab_BtnFilters[n_FilterIndex].SetActive(false);

        n_CurIndex = n_FilterIndex;

    }

}
