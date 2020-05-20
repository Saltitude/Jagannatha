using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SC_ShowSequence_OP : MonoBehaviour
{

    #region Singleton

    private static SC_ShowSequence_OP _instance;
    public static SC_ShowSequence_OP Instance { get { return _instance; } }

    #endregion

    [Header("References")]
    [SerializeField]
    GameObject[] SeqParts;
    [SerializeField] //Order is Circle Triangle Square
    Sprite[] tab_Sprites;
    [SerializeField]
    GameObject[] tab_Progress;
    [SerializeField]
    Material ProgressOn;
    [SerializeField]
    Material ProgressOff;
    [SerializeField]
    Material ProgressDisable;

    [Header("Debug References")]
    [SerializeField]
    bool b_UseDebugContent = false;
    [SerializeField]
    GameObject[] DebugContents;

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

    void Start()
    {

        if (!b_UseDebugContent)
            for (int i = 0; i < DebugContents.Length; i++)
                DebugContents[i].SetActive(false);

    }

    public void DisplaySequence()
    {

        int SequenceLenght = SC_SyncVar_MovementSystem.Instance.BreakdownList.Count;

        for (int i = 0; i < tab_Progress.Length; i++)
        {
            if (i < SequenceLenght)
            {
                SeqParts[i].SetActive(true);
                tab_Progress[i].GetComponent<Image>().material = ProgressOff;              
            }               
            else
            {
                SeqParts[i].SetActive(false);
                tab_Progress[i].GetComponent<Image>().material = ProgressDisable;
            }              
        }

        for (int i = 0; i < SeqParts.Length; i++)
            SeqParts[i].SetActive(false);         

        if (b_UseDebugContent)
            for (int i = 0; i < DebugContents.Length; i++)
                DebugContents[i].SetActive(false); 

        for (int i = 0; i < SequenceLenght; i++)
        {

            SeqParts[i].SetActive(true);
            SeqParts[i].GetComponent<Image>().sprite = tab_Sprites[SC_SyncVar_MovementSystem.Instance.BreakdownList[i]-1];

            if (b_UseDebugContent)
            {
                DebugContents[i].SetActive(true);
                string curValue = SC_SyncVar_MovementSystem.Instance.BreakdownList[i].ToString();
                DebugContents[i].GetComponent<TextMeshPro>().text = curValue;
            }

        }

    }

    public void DisplayProgression()
    {

        int PlayerSeqLenght = SC_SyncVar_MovementSystem.Instance.CurPilotSeqLenght;

        for (int i = 0; i < PlayerSeqLenght; i++)
                tab_Progress[i].GetComponent<Image>().material = ProgressOn;

        //Debug.Log("DispProg | " + PlayerSeqLenght + " | " + SC_SyncVar_MovementSystem.Instance.BreakdownList.Count);

    }

    public void HideSequence()
    {

        //Debug.Log("HideSequences");

        for (int i = 0; i < SeqParts.Length; i++)
        {
            SeqParts[i].SetActive(false);
            tab_Progress[i].GetComponent<Image>().material = ProgressDisable;
        }
    }

}
