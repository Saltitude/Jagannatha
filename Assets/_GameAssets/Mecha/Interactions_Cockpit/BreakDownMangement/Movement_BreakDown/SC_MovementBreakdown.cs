using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_MovementBreakdown : MonoBehaviour, IF_BreakdownManager
{

    #region Singleton

    private static SC_MovementBreakdown _instance;
    public static SC_MovementBreakdown Instance { get { return _instance; } }

    #endregion

    #region Variables

    [Header("BreakDown Parameters")]
    [SerializeField]
    bool b_ResetSeqIfFail = false;

    [Header("BreakDown Infos")]
    public bool b_MaxBreakdown = false;
    [SerializeField]
    int n_MaxBreakdownLvl = 3;
    [SerializeField, Range(0, 3)]
    public int n_BreakDownLvl = 0;
    public int n_InteractibleInBreakDown = 0;

    [Header("Sequences Infos")]
    [SerializeField]
    int[] tab_BreakdownSequence;
    [SerializeField]
    int[] tab_PilotSequence;
    [SerializeField]
    int CurPilotSeqLenght = 0;
    [SerializeField]
    public bool b_SeqIsCorrect = false;

    [Header("Interactibles"), SerializeField]
    public GameObject[] interactible;

    #endregion Variables

    #region Init

    void Awake()
    {

        InitSingleton();

        Invoke("Demarage", 0.5f);

    }

    void InitSingleton()
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

    void Demarage()
    {
        StartNewBreakdown(3);
    }

    #endregion Init

    #region MainFunctions

    public void StartNewBreakdown(int nbBreakdown)
    {
        //Debug.Log("StartNewBdMov");

        if (!b_MaxBreakdown)
        {

            int n_BreakDownLvlTemp = n_BreakDownLvl + nbBreakdown;
            SetBreakdownLvl(n_BreakDownLvlTemp);

            SetSequences();

            CheckBreakdown();

        }
    }

    void SetSequences()
    {

        ResizeTab();
        InitPilotSeq();
        //CurPilotSeqLenght = 0;
        SyncCurPilotSeqLenght();
        SetSequenceState(false);

        int oldrnd = 0;

        for (int i = 0; i < n_BreakDownLvl; i++)
        {
            int rnd = Random.Range(1, 4);

            if (rnd != oldrnd)
            {
                tab_BreakdownSequence[i] = rnd;
                oldrnd = rnd;
            }
            else
                i--;

        }

        SendSequences();    

        //Call les Cords

    }

    void CheckSequences(int CheckLenght)
    {

        //Debug.Log("Start CheckSq Mov " + CheckLenght);

        bool b_isCorrect = true;

        for (int i = 0; i < CheckLenght; i++)
        {
            if (tab_BreakdownSequence[i] != tab_PilotSequence[i])
                b_isCorrect = false;
        }

        //Debug.Log("CheckSq Mov " + b_isCorrect);

        if (b_isCorrect)
        {
            if (CheckLenght == tab_BreakdownSequence.Length)
            {
                //Debug.Log("isCorrect Mov");
                CurPilotSeqLenght = 0;
                SyncCurPilotSeqLenght();
                SetSequenceState(true);
                //Ranger les Cords              
            }
        }
        else
        {
            if(b_ResetSeqIfFail)
                SetSequences();
            else
                InitPilotSeq();
        }

        CheckBreakdown();

    }

    public void CheckBreakdown()
    {

        //Debug.Log("Check Mov");

        if (n_BreakDownLvl == n_MaxBreakdownLvl)
            SetMaxBreakdown(true);

        //Normal Breakdown
        //if (!SC_MainBreakDownManager.Instance.b_BreakEngine && n_BreakDownLvl > 0 && b_SeqIsCorrect )
        if (n_BreakDownLvl > 0 && b_SeqIsCorrect )
            EndBreakdown();

        else
        {
            SC_JoystickMove.Instance.AlignBreakdownLevel(n_BreakDownLvl);
            SyncSystemState();
            SC_MainBreakDownManager.Instance.CheckBreakdown();
        }

    }

    public void EndBreakdown()
    {

        //Debug.Log("End Mov Bd");

        SetMaxBreakdown(false);
        SetBreakdownLvl(0);
        ResizeTab();

        int rnd = Random.Range(0, 2);
        if(rnd == 0)
            SC_JoystickMove.Instance.SetBrokenDir(SC_JoystickMove.Dir.Left);       
        else
            SC_JoystickMove.Instance.SetBrokenDir(SC_JoystickMove.Dir.Right);

        SC_JoystickMove.Instance.AlignBreakdownLevel(n_BreakDownLvl);

        SyncSystemState();
        SC_MainBreakDownManager.Instance.CheckBreakdown();

    }

    #endregion MainFunctions

    #region OtherFunctions

    public void InitPilotSeq()
    {
        tab_PilotSequence = new int[n_BreakDownLvl];
        CurPilotSeqLenght = 0;
    }

    public void AddToPilotSeq(int CordIndex)
    {
        if (CurPilotSeqLenght < tab_BreakdownSequence.Length)
        {
            tab_PilotSequence[CurPilotSeqLenght] = CordIndex;
            CurPilotSeqLenght++;
            SyncCurPilotSeqLenght();
            CheckSequences(CurPilotSeqLenght);
        }
    }

    void ResizeTab()
    {
        tab_BreakdownSequence = new int[n_BreakDownLvl];
        tab_PilotSequence = new int[n_BreakDownLvl];
    }

    #endregion OtherFunctions

    #region SyncFunctions

    void SyncSystemState()
    {
        if (n_BreakDownLvl > 0)
            SC_SyncVar_Main_Breakdown.Instance.onPanneMovementChange(true);
        else
            SC_SyncVar_Main_Breakdown.Instance.onPanneMovementChange(false);
    }

    void SetMaxBreakdown(bool TargetState)
    {
        b_MaxBreakdown = TargetState;
        SC_SyncVar_MovementSystem.Instance.b_MaxBreakdown = TargetState;
    }

    void SetBreakdownLvl(int TargetLvl)
    {
        n_BreakDownLvl = TargetLvl;
        SC_SyncVar_MovementSystem.Instance.n_BreakDownLvl = TargetLvl;
    }

    void SetSequenceState(bool State)
    {
        b_SeqIsCorrect = State;
        SC_SyncVar_MovementSystem.Instance.b_SeqIsCorrect = State;
    }

    void SendSequences()
    {

        //Debug.Log("Send Sequences");

        SC_SyncVar_MovementSystem.Instance.b_SeqIsSync = false;

        SC_SyncVar_MovementSystem.Instance.BreakdownList.Clear();

        for (int i = 0; i < tab_BreakdownSequence.Length; i++)
            SC_SyncVar_MovementSystem.Instance.BreakdownList.Add(tab_BreakdownSequence[i]);

        SC_SyncVar_MovementSystem.Instance.b_SeqIsSync = true;

    }

    void SyncCurPilotSeqLenght()
    {
        SC_SyncVar_MovementSystem.Instance.CurPilotSeqLenght = CurPilotSeqLenght;
    }

    #endregion SyncFunctions

    #region DebugMethod

    /// <summary>
    /// Focntion permettant de réparer tous les boutons automatiquement
    /// </summary>
    public void RepairBreakdownDebug()
    {

        for (int i = 0; i < tab_BreakdownSequence.Length; i++)
        {
            tab_PilotSequence[i] = tab_BreakdownSequence[i];
        }

        CurPilotSeqLenght = tab_BreakdownSequence.Length;

        CheckSequences(CurPilotSeqLenght);

    }

    public void RepairSingleBreakdownDebug()
    {

        List<GameObject> list = new List<GameObject>();
        for (int i = 0; i < interactible.Length; i++)
        {

            if (interactible[i].GetComponent<IInteractible>().isBreakdown())
            {
                list.Add(interactible[i]);
            }

        }

        int rnd = Random.Range(0, list.Count);
        list[rnd].GetComponent<IInteractible>().Repair();

    }

    #endregion DebugMethod

    public void ForceUpdate()
    {
        SyncCurPilotSeqLenght();
        SyncSystemState();
        SendSequences();

        for (int j = 0; j < interactible.Length; j++)
        {
            interactible[j].GetComponent<IInteractible>().ForceSync();
        }

    }

}
