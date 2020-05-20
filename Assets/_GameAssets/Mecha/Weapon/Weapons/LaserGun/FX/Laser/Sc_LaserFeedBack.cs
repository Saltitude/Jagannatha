using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_LaserFeedBack : MonoBehaviour
{

    #region Singleton

    private static Sc_LaserFeedBack _instance;
    public static Sc_LaserFeedBack Instance { get { return _instance; } }

    #endregion

    public SC_WeaponLaserGun MainLaserScript;
    public GameObject FirePoint;
    GameObject SFX_LaserBeam;
    int SoundSourceNumb;
    [SerializeField]
    Color CurColor;
    public GameObject Laser;
    public GameObject EnergyBall;
    public GameObject ChargeSpark;
    public GameObject Fioriture;
    public GameObject Ondes;
    public GameObject Elice;
    public GameObject EliceDark;
    [SerializeField]
    SC_WeaponLaserGun WeapMainSC;
    public ParticleSystem.MainModule LaserPS;
    public ParticleSystem.MainModule EnergyBallPS;
    public ParticleSystem.MainModule ChargeSparkPS;
    public ParticleSystem.MainModule FioriturePS;
    public ParticleSystem.MainModule OndesPS;
    public ParticleSystem.MainModule ElicePS;
    public ParticleSystem.MainModule EliceDarkPS;
    [SerializeField]
    Animator Kahme;

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

    private void Update()
    {
        if (SFX_LaserBeam != null)
        {
            SFX_LaserBeam.transform.position = new Vector3(Laser.transform.position.x, -1000, Laser.transform.position.z);
            //Debug.Log(SFX_LaserBeam.transform.position);
        }
    }
    public void EnableLaser(RaycastHit hit)
    {
        if (SoundSourceNumb == 0)
        {
            SFX_LaserBeam = CustomSoundManager.Instance.PlaySound(gameObject, "SFX_p_LaserBeam2", true, 0.1f);
            SoundSourceNumb += 1;
        }

        Kahme.SetBool("IsFire", true);
    }

    public void DiseableLaser()
    {
        if(SFX_LaserBeam != null && SFX_LaserBeam.GetComponent<AudioSource>().isPlaying)
        {
            SFX_LaserBeam.GetComponent<AudioSource>().Stop();
            SoundSourceNumb = 0;
        }
        Kahme.SetBool("IsFire", false);
    }

    public void SetLaserSize(int value)
    {
        Kahme.SetInteger("CalibValue",value);
    }

    public void SetColor(Color32 NewColor)
    {

        if (CurColor != NewColor)
        {
            CurColor = NewColor;

            Gradient gradiend = new Gradient();
            GradientColorKey[] colorKeys = new GradientColorKey[3];
            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];

            alphaKeys[0].time = 0;
            alphaKeys[0].alpha = 1;

            alphaKeys[1].time = 1;
            alphaKeys[1].alpha = 1;

            colorKeys[0].color = NewColor;
            colorKeys[1].color = NewColor;
            colorKeys[2].color = NewColor;

            gradiend.SetKeys(colorKeys, alphaKeys);
            gradiend.SetKeys(colorKeys, alphaKeys);

            LaserPS = Laser.GetComponent<ParticleSystem>().main;
            ChargeSparkPS = ChargeSpark.GetComponent<ParticleSystem>().main;
            //EnergyBallPS = EnergyBall.GetComponent<ParticleSystem>().main;
            FioriturePS = Fioriture.GetComponent<ParticleSystem>().main;
            OndesPS = Ondes.GetComponent<ParticleSystem>().main;
            ElicePS = Elice.GetComponent<ParticleSystem>().main;
            EliceDarkPS = EliceDark.GetComponent<ParticleSystem>().main;

            LaserPS.startColor = gradiend;
            ChargeSparkPS.startColor = gradiend;
            //EnergyBallPS.startColor = gradiend;
            FioriturePS.startColor = gradiend;
            OndesPS.startColor = gradiend;
            ElicePS.startColor = gradiend;
            EliceDarkPS.startColor = gradiend;

        }

    }

}
