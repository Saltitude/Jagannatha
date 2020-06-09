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
    bool[] sourcePlaying;

    [SerializeField]
    float ambianceVolume = 1;

    [SerializeField]
    float fadeSpeed = 1;


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
        sourcePlaying = new bool[soundSource.Length];
        for (int i = 0; i < CustomSoundManager.Instance.tAmbiancePilot.Length;i++)
        {
            var tamp =  CustomSoundManager.Instance.PlayAmbiance(source, i, true, 0);
            soundSource[i] = tamp.GetComponent<AudioSource>();
            sourcePlaying[i] = false;
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
        //InputTest();
    }

    void InputTest()
    {
        if(Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            index++;
            if(index > (int)Ambiance.ClimaxMaxMax)
            {
                index = 0;
            }
            Debug.Log(index + " :" + (Ambiance)index);

        }
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            index--;
            if(index < 0)
            {
                index = (int)Ambiance.ClimaxMaxMax;
            }
            Debug.Log(index + " :" + (Ambiance)index);
        }
        if(Input.GetKeyDown(KeyCode.KeypadEnter))
        {

            PlayAmbiance((Ambiance)index);

            //if (soundSource[index].volume == 0)
            //    soundSource[index].volume = 1;

            //else if (soundSource[index].volume == 1)
            //    soundSource[index].volume = 0;
        }
    }

    public void PlayAmbiance(Ambiance newAmbiance)
    {
        //source actuel
        List<AudioSource> curSources = new List<AudioSource>();
        //Nouvelle sources
        int[] newSource = AmbianceTotal[(int)newAmbiance];

        //Source restant actives
        List<AudioSource> lastingSource = new List<AudioSource>();

        //Récupère toutes les sources active
        for(int i = 0;i<soundSource.Length;i++)
        {
            if(sourcePlaying[i])
            {
                curSources.Add(soundSource[i]);
            }
        }

        //Récupère les lasting sources et lance la désactivation des sources inutiles
        foreach(AudioSource source in curSources)
        {
            bool lasting = false;
            for(int i = 0; i< newSource.Length; i++)
            {
                if (source == soundSource[newSource[i]])
                {
                    lasting = true;


                }
            }
            if(lasting)
            {
                lastingSource.Add(source);
            }
            else
            {
                for(int i =0;i<soundSource.Length;i++)
                {
                    if(source == soundSource[i])
                    {
                        StartCoroutine(FadeOut(i));
                    }

                }
            }
        }



        //Play new sounds
        for (int j = 0; j < newSource.Length; j++)
        {
            bool lasting = false;
            for(int k = 0; k < lastingSource.Count;k++)
            {

                if (soundSource[newSource[j]] == lastingSource[k])
                    lasting = true;
            }
            if(!lasting)
            {
                StartCoroutine(FadeIn(newSource[j]));
            }
        }
           

    }

    IEnumerator FadeIn(int sourceIndex)
    {
        sourcePlaying[sourceIndex] = true;
        AudioSource newSource = soundSource[sourceIndex];
        while(newSource.volume != ambianceVolume)
        {
            newSource.volume += (Time.deltaTime*fadeSpeed);
            if(newSource.volume > ambianceVolume)
            {
                newSource.volume = ambianceVolume;
            }
            yield return 0;
        }
    }

    IEnumerator FadeOut(int sourceIndex)
    {
        sourcePlaying[sourceIndex] = false;
        AudioSource newSource = soundSource[sourceIndex];
        while (newSource.volume != 0)
        {
            newSource.volume -= (Time.deltaTime * fadeSpeed);
            if (newSource.volume < 0)
            {
                newSource.volume = 0;
            }
            yield return 0;
        }
    }
}
