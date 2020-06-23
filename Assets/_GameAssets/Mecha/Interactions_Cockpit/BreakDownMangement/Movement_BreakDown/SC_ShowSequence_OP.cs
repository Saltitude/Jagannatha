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

    SC_ShowSequence_OP BlinkMaster;

    [Header("Debug References")]
    [SerializeField]
    bool b_UseDebugContent = false;
    [SerializeField]
    GameObject[] DebugContents;


    /// Blink Part ///
    bool[] IndexToActivate;
    [SerializeField]
    Image[] img_ToBreakDown;
    Material[] wireSafe;
    [SerializeField]
    Material wireBreakdown;
    [SerializeField]
    Material wireShutdown;

    [SerializeField]
    GameObject sparkleSeq1;
    [SerializeField]
    GameObject sparkleSeq2;
    [SerializeField]
    GameObject sparkleSeq3;

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
        BlinkMaster = this;

        if (!b_UseDebugContent)
            for (int i = 0; i < DebugContents.Length; i++)
                DebugContents[i].SetActive(false);


        wireSafe = new Material[img_ToBreakDown.Length];
        IndexToActivate = new bool[img_ToBreakDown.Length];
    
        for (int i = 0; i < wireSafe.Length; i++)
        {
            wireSafe[i] = img_ToBreakDown[i].material;
        }

        StartCoroutine(RedWireCoro());

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

            switch (i)
            {

                case 0:
                    BlinkMaster.SetBreakDown(6, false);
                    BlinkMaster.SetBreakDown(7, false);
                    sparkleSeq1.SetActive(true);
                    break;

                case 1:
                    BlinkMaster.SetBreakDown(8, false);
                    BlinkMaster.SetBreakDown(9, false);
                    sparkleSeq1.SetActive(false);
                    sparkleSeq2.SetActive(true);
                    break;

                case 2:
                    BlinkMaster.SetBreakDown(10, false);
                    BlinkMaster.SetBreakDown(11, false);
                    sparkleSeq3.SetActive(true);
                    break;

            }

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

                        //Logo
                        BlinkMaster.SetBreakDown(6, b_InBreakDown);
                        BlinkMaster.SetBreakDown(7, b_InBreakDown);
                        sparkleSeq1.SetActive(false);
                        break;

                    case 1:

                        BlinkMaster.ShutDownWire(1, false);
                        BlinkMaster.ShutDownWire(4, false);

                        BlinkMaster.SetBreakDown(1, b_InBreakDown);
                        BlinkMaster.SetBreakDown(3, false);
                        BlinkMaster.SetBreakDown(4, b_InBreakDown);

                        BlinkMaster.ShutDownWire(3, true);

                        //Logo);
                        BlinkMaster.SetBreakDown(8, b_InBreakDown);
                        BlinkMaster.SetBreakDown(9, b_InBreakDown);
                        sparkleSeq2.SetActive(false);
                        break;

                    case 2:

                        BlinkMaster.ShutDownWire(2, false);

                        BlinkMaster.SetBreakDown(2, b_InBreakDown);
                        BlinkMaster.SetBreakDown(4, false);

                        BlinkMaster.ShutDownWire(4, true);

                        //Logo
                        BlinkMaster.SetBreakDown(10, b_InBreakDown);
                        BlinkMaster.SetBreakDown(11, b_InBreakDown);
                        sparkleSeq3.SetActive(false);
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

            BlinkMaster.SetBreakDown(6, false);
            BlinkMaster.SetBreakDown(7, false);
            BlinkMaster.SetBreakDown(8, false);
            BlinkMaster.SetBreakDown(9, false);
            BlinkMaster.SetBreakDown(10, false);
            BlinkMaster.SetBreakDown(11, false);

            BlinkMaster.ShutDownWire(0, false);
            BlinkMaster.ShutDownWire(1, false);
            BlinkMaster.ShutDownWire(2, false);
            BlinkMaster.ShutDownWire(3, false);
            BlinkMaster.ShutDownWire(4, false);
            BlinkMaster.ShutDownWire(5, false);

        }

    }

    #region Blink
    //////////////////////---BLINK---//////////////////////////////
    void SetBreakDown(int index, bool activate)
    {

        if (activate)
        {

            if (!IndexToActivate[index])
            {
                img_ToBreakDown[index].material = wireBreakdown;
            }

        }
        else
        {
            if(IndexToActivate[index])
                EndCoroutine(index);

        }
        IndexToActivate[index] = activate;

    }
    void ShutDownWire(int index, bool activate)
    {
        if (activate)
            img_ToBreakDown[index].material = wireShutdown;
        else
            img_ToBreakDown[index].material = wireSafe[index];


    }


    void EndCoroutine(int index)
    {
        img_ToBreakDown[index].material = wireSafe[index];
        img_ToBreakDown[index].color = Color.white;
    }


    IEnumerator RedWireCoro()
    {
        float animTime = 0.5f;
        float maxOpacity = 1;
        float minOpacity = 0f;
        float ratePerSec = (maxOpacity - minOpacity / animTime) * 2;
        float curOpacity;
        bool Add = true;
        float t = 0;

        Vector4 ColorTampon = Color.white;
        curOpacity = minOpacity;

        while (true)
        {
            if (t < animTime)
            {
                t += Time.deltaTime;
                if (Add)
                {

                    if (curOpacity < maxOpacity)
                        curOpacity = Mathf.Lerp(curOpacity, maxOpacity, ratePerSec * Time.deltaTime);
                }
                else
                {

                    if (curOpacity > minOpacity)
                        curOpacity = Mathf.Lerp(curOpacity, minOpacity, ratePerSec * Time.deltaTime);

                }

                for (int i = 0; i < img_ToBreakDown.Length; i++)
                {
                    if (IndexToActivate[i])
                        img_ToBreakDown[i].color = new Vector4(ColorTampon.x, ColorTampon.y, ColorTampon.z, curOpacity);
                }

            }
            else
            {
                Add = !Add;
                t = 0;
            }
            yield return 0;
        }

    }
    #endregion
}
