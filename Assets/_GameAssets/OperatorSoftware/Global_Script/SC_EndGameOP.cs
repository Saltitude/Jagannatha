using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_EndGameOP : MonoBehaviour
{

    #region Singleton

    private static SC_EndGameOP _instance;
    public static SC_EndGameOP Instance { get { return _instance; } }

    #endregion

    [Header("Drop the InGame Childs")]
    [Tooltip("Tableau avec tous les child in")]
    [SerializeField]
    GameObject[] tab_InGame; //Tableau des canvas

    [Header("Drop the EndGame Childs")]
    [Tooltip("Tableau avec tous les child end")]
    [SerializeField]
    GameObject[] tab_EndGame; //Tableau des canvas

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

    // Update is called once per frame
    //void Update()
    //{

    //    //if (Input.GetKeyDown(KeyCode.F11))
    //    //    InGameDisplay();

    //    //if (Input.GetKeyDown(KeyCode.F12))
    //    //    EndGameDisplay();

    //}

    public void InGameDisplay()
    {

        for (int i = 0; i < tab_EndGame.Length; i++)
        {
            tab_EndGame[i].SetActive(false);
        }

        for (int i = 0; i < tab_InGame.Length; i++)
        {
            tab_InGame[i].SetActive(true);
        }

    }

    public void EndGameDisplay()
    {

        for (int i = 0; i < tab_EndGame.Length; i++)
        {
            tab_EndGame[i].SetActive(true);
        }

        for (int i = 0; i < tab_InGame.Length; i++)
        {
            tab_InGame[i].SetActive(false);
        }

        SC_UI_OngletContainer.Instance.EndScreenUnzoom();

    }  

}
