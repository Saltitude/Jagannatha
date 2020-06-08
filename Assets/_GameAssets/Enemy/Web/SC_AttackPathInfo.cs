using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script permettant de stocker les index du Circle et de la Line pour entrer sur l'AttackPath contenant le script
///  | Sur tout les AttackPath possible avec en paramètre l'index circle et l'index Line correspondant au point d'entrée de la spline
///  | Auteur : Zainix
/// </summary>

public class SC_AttackPathInfo : MonoBehaviour
{
    [SerializeField]
    int _circleEnterIndex;//Index cercle d'entrée


    [SerializeField]
    int _lineEnterIndex;//Index de line d'entrée

    SC_CheckList_Mecha checklist;

    Transform Player;


    void Start()
    {
        
        //Player = SC_CheckList_Mecha.Instance.MechCollider.transform;

    }

    //Get des index par PathBehavior
    public int GetCircleEnterIndex()
    {
        return _circleEnterIndex;
    }
    public int GetLineEnterIndex()
    {
        return _lineEnterIndex;
    }


    void Update()
    {
        if (Player != null)
            transform.position = new Vector3(Player.transform.position.x, Player.transform.position.y +55, Player.transform.position.z);
    }


}
