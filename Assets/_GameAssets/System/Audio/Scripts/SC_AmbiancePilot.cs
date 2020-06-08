using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_AmbiancePilot : MonoBehaviour
{
    #region Singleton

    private static SC_AmbiancePilot _instance;
    public static SC_AmbiancePilot Instance { get { return _instance; } }

    #endregion

    [SerializeField]
    GameObject source;

    AudioSource[] soundSource;

    int index = 0;

    [SerializeField]
    int[] Ambiance1;
    [SerializeField]
    int[] Ambiance2;
    [SerializeField]
    int[] Ambiance3;
    [SerializeField]
    int[] Ambiance4;
    [SerializeField]
    int[] Ambiance5;
    [SerializeField]
    int[] Ambiance6;
    [SerializeField]
    int[] Ambiance7;
    [SerializeField]
    int[] Ambiance8;

    int[][] AmbianceTotal;
    public enum Ambiance
    {
        TutoRepa,
        TutoFlock,
        Rising1,
        Rising2,
        Climax1,
        Slow1,
        Rising3,
        ClimaxMaxMax
    }

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

    void Start()
    {
        AmbianceTotal = new int[8][];
        soundSource = new AudioSource[CustomSoundManager.Instance.tAmbiancePilot.Length];
        for(int i = 0; i < CustomSoundManager.Instance.tAmbiancePilot.Length;i++)
        {
            var tamp =  CustomSoundManager.Instance.PlayAmbiance(source, i, true, 0);
            soundSource[i] = tamp.GetComponent<AudioSource>();
        }

        AmbianceTotal[0] = Ambiance1;
        AmbianceTotal[1] = Ambiance2;
        AmbianceTotal[2] = Ambiance3;
        AmbianceTotal[3] = Ambiance4;
        AmbianceTotal[4] = Ambiance5;
        AmbianceTotal[5] = Ambiance6;
        AmbianceTotal[6] = Ambiance7;
        AmbianceTotal[7] = Ambiance8;


    }

    // Update is called once per frame
    void Update()
    {
        InputTest();
    }

    void InputTest()
    {
        if(Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            index++;
            if(index > soundSource.Length-1)
            {
                index = 0;
            }
            Debug.Log(index + " :" + soundSource[index].clip.name);

        }
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            index--;
            if(index < 0)
            {
                index = soundSource.Length-1;
            }
            Debug.Log(index + " :" +soundSource[index].clip.name);
        }
        if(Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (soundSource[index].volume == 0)
                soundSource[index].volume = 1;

            else if (soundSource[index].volume == 1)
                soundSource[index].volume = 0;
        }
    }

    public void PlayAmbiance(Ambiance newAmbiance)
    {      
        for(int i =0; i <soundSource.Length; i++)
        {
            soundSource[i].volume = 0;
        }
        var curTab = AmbianceTotal[(int)newAmbiance];
        for(int j = 0; j < curTab.Length; j++)
        {
            soundSource[curTab[j]].volume = 1;
        }
    }
}
