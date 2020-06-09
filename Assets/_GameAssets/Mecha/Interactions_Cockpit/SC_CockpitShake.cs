using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_CockpitShake : MonoBehaviour
{

    #region Singleton

    private static SC_CockpitShake _instance;
    public static SC_CockpitShake Instance { get { return _instance; } }

    #endregion
    // Transform of the camera to shake. Grabs the gameObject's transform
    // if null.
    public Transform screenTransform;
    GameObject SFX_DamageTaken;
    int SoundSourceNumb = 0;
    // How long the object should shake for.
    public float shakeDuration = 0f;
    public float soundDuration = 0f;

    float maxShakeDuration = 2f;
    bool maxShakeReach = false;
    // Amplitude of the shake. A larger value shakes the camera harder.
    public float shakeAmount = 0.7f;
    public float decreaseFactor = 1.0f;

    Vector3 originalPos;

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
        if (screenTransform == null)
        {
            screenTransform = GetComponent(typeof(Transform)) as Transform;
        }
    }

    void OnEnable()
    {
        originalPos = screenTransform.localPosition;
    }

    public void ShakeIt(float amplitude, float duration, bool playSound = true)
    {
        float newShakeDuration = shakeDuration + duration;
    
        if (newShakeDuration < maxShakeDuration && !maxShakeReach)
        {

            shakeAmount = amplitude;
            shakeDuration = shakeDuration + duration;

            if(playSound)
            SFX_DamageTaken = CustomSoundManager.Instance.PlaySound(gameObject, "SFX_p_DamageTaken", false, 0.1f);
            SoundSourceNumb += 1;
            soundDuration = 0.4f;
        }
        else
        {
            maxShakeReach = true;
        }
        
        if(shakeDuration <= 0)
        {
            maxShakeReach = false;
        }



    }
    void Update()
    {
        if (shakeDuration > 0.4f)
        {
            soundDuration = shakeDuration;
        }
        if (shakeDuration > 0)
        {
            screenTransform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;

            shakeDuration -= Time.deltaTime * decreaseFactor;
        }
        else
        {
            shakeDuration = 0f;
            screenTransform.localPosition = originalPos;
        }
        if(soundDuration >= 0)
        {
            soundDuration -= Time.deltaTime;
        }
        else if(soundDuration <= 0)
        {
            if (SFX_DamageTaken != null && SFX_DamageTaken.GetComponent<AudioSource>().isPlaying)
            {
                SFX_DamageTaken.GetComponent<AudioSource>().Stop();
               // SoundSourceNumb = 0;
            }
        }
    }
}