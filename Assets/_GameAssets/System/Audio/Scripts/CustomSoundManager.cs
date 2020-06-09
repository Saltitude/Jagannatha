using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomSoundManager : MonoBehaviour
{
    #region Singleton

    private static CustomSoundManager _instance;
    public static CustomSoundManager Instance { get { return _instance; } }

    #endregion
    GameObject Mng_CheckList = null;

    [SerializeField] AudioClip[] tSounds = new AudioClip[0];
    [SerializeField] public AudioClip[] tAmbiancePilot = new AudioClip[0];
    [SerializeField] GameObject hAudioSourcePrefab = null;
    [SerializeField] bool bStartWithNoSound = false;
    [SerializeField] float fFirstEnvironmentSoundTimeBeforePlay = 1;
    [SerializeField] int nNbAudioSource = 150;
    [SerializeField] Vector2 vRandomTimeBetweenAmbianceSound = new Vector2(2, 5);
    [SerializeField] float fVolumeAmbiance = 0.5f;
    [SerializeField] Vector2 vRandomAndFixedPitchAmbiance = new Vector2(0.5f, 0.4f);

    GameObject[] hAudioSources;

    float nTimeFade = 5;
    float fCurrentTimer = 0;
    int nSoundDir = 0;

    float nVolumeModifierLocal = 1;

    float fTimerBeforeNextSound = 5;
    float fCurrentTimerSound = 0;



    private void Awake()
    {

        IsCheck();

        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }

        
        hAudioSources = new GameObject[nNbAudioSource];
        for (int i = 0; i < hAudioSources.Length; i++)
        {
            hAudioSources[i] = Instantiate(hAudioSourcePrefab);
            hAudioSources[i].transform.SetParent(this.transform);
        }

        fTimerBeforeNextSound = fFirstEnvironmentSoundTimeBeforePlay;

        if (bStartWithNoSound)
            nVolumeModifierLocal = nVolumeModifierLocal * 0.1f;
        else
            fCurrentTimer = nTimeFade;

    }

    void IsCheck()
    {
        Mng_CheckList = GameObject.FindGameObjectWithTag("Mng_CheckList");
        Mng_CheckList.GetComponent<SC_CheckList>().Mng_Audio = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        //FadeSound(1, 1);
        // ######################################## FADE DE SON  ######################################## //
        #region Fade de Son
        // ----- Enlève l'ancien modifieur local
        if (nVolumeModifierLocal != 0)
        {
            for (int i = 0; i < hAudioSources.Length; i++)
            {
                if (hAudioSources[i] != null)
                    hAudioSources[i].GetComponent<AudioSource>().volume = hAudioSources[i].GetComponent<AudioSource>().volume / nVolumeModifierLocal;
            }
        }

        // ----- Change la valeur du modifieur local
        fCurrentTimer += Time.deltaTime * nSoundDir;
        if (fCurrentTimer > nTimeFade)
        {
            fCurrentTimer = nTimeFade;
            nSoundDir = 0;
            nVolumeModifierLocal = 1;
        }
        else if (fCurrentTimer < 0.001f)
        {
            fCurrentTimer = 0.001f;
            nSoundDir = 0;
            nVolumeModifierLocal = 0.001f;
        }
        else
            nVolumeModifierLocal = fCurrentTimer / nTimeFade;


        // ----- Ré-attribue le modifieur local
        if (nVolumeModifierLocal != 0)
        {
            for (int i = 0; i < hAudioSources.Length; i++)
            {
                if (hAudioSources[i] != null)
                    hAudioSources[i].GetComponent<AudioSource>().volume = hAudioSources[i].GetComponent<AudioSource>().volume * nVolumeModifierLocal;
            }
        }

        #endregion
        /*
        // --- Joue un son d'environnement tout les "fTimerBeforeNextSound"
        fCurrentTimerSound += Time.deltaTime;
        if (fCurrentTimerSound > fTimerBeforeNextSound) environmentSound();

    */
    }
    /*
    void environmentSound()
    {
        if (tSoundsAmbient.Length != 0)
        {
            fCurrentTimerSound -= fTimerBeforeNextSound;
            fTimerBeforeNextSound = Random.Range(vRandomTimeBetweenAmbianceSound.x, vRandomTimeBetweenAmbianceSound.y);
            PlaySound(GameObject.Find("Main Camera"), tSoundsAmbient[Random.Range(0, tSoundsAmbient.Length)], false, fVolumeAmbiance, vRandomAndFixedPitchAmbiance.x, vRandomAndFixedPitchAmbiance.y);
        }
    }*/

    public GameObject PlayAmbiance(GameObject hSource, int indexSound, bool bLoop, float fVolume, string sSoundName = "", bool parent = true, float fPitchRandom = 0, float fPitchConstantModifier = 0)
    {
        int IndexSound = -1;

        if (sSoundName != "")
        {
            for (int i = 0; i < tAmbiancePilot.Length; i++)
            {
                if (tAmbiancePilot[i].name == sSoundName)
                {
                    IndexSound = i;
                    break;
                }
            }

        }
        else
        {
            IndexSound = indexSound;
        }

        for (int i = 0; i < hAudioSources.Length; i++)
        {

            if (!hAudioSources[i].GetComponent<AudioSource>().isPlaying && hAudioSources[i] != null)
            {
                AudioSource curSource = hAudioSources[i].GetComponent<AudioSource>();
                if (parent == true) hAudioSources[i].transform.SetParent(hSource.transform);

                hAudioSources[i].transform.position = hSource.transform.position;
                curSource.clip = tAmbiancePilot[IndexSound];
                curSource.loop = bLoop;
                curSource.pitch = 1 + Random.Range(-fPitchRandom, fPitchRandom) + fPitchConstantModifier;
                curSource.volume = fVolume;
                curSource.Play();
                return hAudioSources[i];
                
            }
        }
        return null;
    }

    public GameObject PlaySound(GameObject hSource, string sSoundName, bool bLoop, float fVolume, bool parent = true, float fPitchRandom = 0, float fPitchConstantModifier = 0)
    {
        int IndexSound = -1;
        for (int i = 0; i < tSounds.Length; i++)
        {
            if (tSounds[i].name == sSoundName)
            {
                IndexSound = i;
                break;
            }
        }
        for (int i = 0; i < hAudioSources.Length; i++)
        {
            if (!hAudioSources[i].GetComponent<AudioSource>().isPlaying && hAudioSources[i] != null)
            {
                if(parent == true) hAudioSources[i].transform.SetParent(hSource.transform);

                AudioSource curSource = hAudioSources[i].GetComponent<AudioSource>();
                hAudioSources[i].transform.position = hSource.transform.position;
                curSource.clip = tSounds[IndexSound];
                curSource.loop = bLoop;
                curSource.pitch = 1 + Random.Range(-fPitchRandom, fPitchRandom) + fPitchConstantModifier;
                curSource.volume = fVolume;
                curSource.Play();
                return hAudioSources[i];
                
            }
        }
        return null;
    }

    public void FadeSound(int nDir, float fTime)
    {
        if (nDir != 0)
        {
            if (nDir > 1) nDir = 1;
            if (nDir < -1) nDir = -1;
        }
        if (fTime < 0) fTime = fTime * -1;
        fCurrentTimer = fCurrentTimer * fTime / nTimeFade;
        nSoundDir = nDir;
        nTimeFade = fTime;

    }
}
