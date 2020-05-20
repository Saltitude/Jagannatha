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
    [SerializeField]
    SC_UI_WireBlink BlinkMaster;

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

        SetWireBlink();

    }

    public void DisplayProgression()
    {

        int PlayerSeqLenght = SC_SyncVar_MovementSystem.Instance.CurPilotSeqLenght;

        for (int i = 0; i < PlayerSeqLenght; i++)
        {
            tab_Progress[i].GetComponent<Image>().material = ProgressOn;
            BlinkMaster.SetBreakDown(i, false);
        }    

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

        SetWireBlink();

    }

    void SetWireBlink()
    {

        int BreakdownLevel = SC_SyncVar_MovementSystem.Instance.n_BreakDownLvl;
        bool b_InBreakDown = false;

        if (BreakdownLevel > 0)
            b_InBreakDown = true;

        if (b_InBreakDown)
        {
            for (int i = 0; i < BreakdownLevel; i++)
            {
                switch (i)
                {

                    case 0:

                        BlinkMaster.ShutDownWire(1, false);
                        BlinkMaster.ShutDownWire(2, false);
                        BlinkMaster.ShutDownWire(4, false);

                        BlinkMaster.SetBreakDown(0, b_InBreakDown);
                        BlinkMaster.SetBreakDown(1, false);
                        BlinkMaster.SetBreakDown(2, false);
                        BlinkMaster.SetBreakDown(3, b_InBreakDown);
                        BlinkMaster.SetBreakDown(4, false);
                        BlinkMaster.SetBreakDown(5, b_InBreakDown);

                        BlinkMaster.ShutDownWire(1, true);
                        BlinkMaster.ShutDownWire(2, true);
                        BlinkMaster.ShutDownWire(4, true);

                        break;

                    case 1:

                        BlinkMaster.ShutDownWire(1, false);
                        BlinkMaster.ShutDownWire(4, false);

                        BlinkMaster.SetBreakDown(1, b_InBreakDown);
                        BlinkMaster.SetBreakDown(3, false);
                        BlinkMaster.SetBreakDown(4, b_InBreakDown);

                        BlinkMaster.ShutDownWire(3, true);

                        break;

                    case 2:

                        BlinkMaster.ShutDownWire(2, false);

                        BlinkMaster.SetBreakDown(2, b_InBreakDown);
                        BlinkMaster.SetBreakDown(4, false);

                        BlinkMaster.ShutDownWire(4, true);

                        break;

                }
            }
        }
        else
        {

            BlinkMaster.SetBreakDown(0, false);
            BlinkMaster.SetBreakDown(1, false);
            BlinkMaster.SetBreakDown(2, false);
            BlinkMaster.SetBreakDown(3, false);
            BlinkMaster.SetBreakDown(4, false);
            BlinkMaster.SetBreakDown(5, false);

            BlinkMaster.ShutDownWire(0, false);
            BlinkMaster.ShutDownWire(1, false);
            BlinkMaster.ShutDownWire(2, false);
            BlinkMaster.ShutDownWire(3, false);
            BlinkMaster.ShutDownWire(4, false);
            BlinkMaster.ShutDownWire(5, false);

        }

    }

    void RepearWireBlink()
    {

        BlinkMaster.SetBreakDown(0, false);
        BlinkMaster.SetBreakDown(1, false);
        BlinkMaster.SetBreakDown(2, false);
        BlinkMaster.SetBreakDown(3, false);
        BlinkMaster.SetBreakDown(4, false);
        BlinkMaster.SetBreakDown(5, false);

        BlinkMaster.ShutDownWire(0, false);
        BlinkMaster.ShutDownWire(1, false);
        BlinkMaster.ShutDownWire(2, false);
        BlinkMaster.ShutDownWire(3, false);
        BlinkMaster.ShutDownWire(4, false);
        BlinkMaster.ShutDownWire(5, false);

    }

}
