using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script de Navigation dans les Menus du System |
/// Made by Leni |
/// </summary>
public class SC_SystemNavigate : MonoBehaviour
{

    [Header("Drop the Systems")]
    [Tooltip("Tableau avec tout les System")]
    [SerializeField]
    GameObject[] tab_System; //Tableau des Systems

    public void GoToSystem(int n_SystemIndex)
    {
        for (int i = 0; i < tab_System.Length ; i++)
        {
            if (i != n_SystemIndex)
            {
                tab_System[i].SetActive(false);
            }
            else if (i == n_SystemIndex)
            {
                tab_System[i].SetActive(true);
            }
        }
    }

}
