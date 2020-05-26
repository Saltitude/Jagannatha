using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_UI_Cockpit_FrequenceLine : MonoBehaviour
{
    #region Singleton

    private static SC_UI_Cockpit_FrequenceLine _instance;
    public static SC_UI_Cockpit_FrequenceLine Instance { get { return _instance; } }

    #endregion

    LineRenderer line;

    //[Range(0.1f, 1.5f)]
    [SerializeField]
    float amplitude = 0.8f; //Hauteur de la courbe
    float curAmplitude;

    //[Range(0.1f, 200)]
    [SerializeField]
    float taille = 102;

    float curPhase;

    //[Range(40, 180)]
    [SerializeField]
    float frequence = 110; //Frequence de la courbe 
    float curFrequence;

    //[SerializeField]
    int speed = 1;

    public Color32 curColor;
    public Color32 Color1;
    public Color32 Color2;
    public Color32 Color3;
    public Color32 Color4;
    public Color32 Color5;
    public Color32 Color6;

    [SerializeField]
    Color32 colorBreakdown;

    Renderer lineRenderer;


    GameObject Mng_SyncVar = null;
    SC_SyncVar_calibr sc_syncvar;

    public int indexDouble1;
    public int indexDouble2;

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
        line = this.gameObject.GetComponent<LineRenderer>(); //Stockage de lui-meme
        Mng_SyncVar = GameObject.FindGameObjectWithTag("Mng_SyncVar");
        GetReferences();

        curColor = new Color32();
        lineRenderer = line.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!SC_MainBreakDownManager.Instance.b_BreakEngine)
            updateLineRender();
        else
            UpdateLineBreakdown();

        if (sc_syncvar == null || Mng_SyncVar == null)
        {
            GetReferences();

        }
    }
    void updateLineRender()
    {/*
        //LIGNE JOUEUR
        line.positionCount = Mathf.CeilToInt(frequence); //Configuration du nombre 
        for (int i = 0; i < line.positionCount; i++)
        {
            float x = taille / frequence * i; //Valeur de X
            float y = Mathf.Sin(Time.time + i * speed) * amplitude; //Valeur de Y
            line.SetPosition(i, new Vector3(y, 0f, x)); //Distribution des valeurs dans le tableau (index, Vector3)
        }
        */
        ////////////////////////
        
        curAmplitude = ratio(sc_syncvar.CalibrInts[0], 6, 1.5f, 0, 0.1f);
        curFrequence = ratio(sc_syncvar.CalibrInts[1], 6, 0.33f, 0, 0.05f);
        curPhase = ratio(sc_syncvar.CalibrInts[2], 6, 20, 0, 0);

        //curColor.b = (byte)ratio(sc_syncvar.CalibrInts[2], 6, 255, 0, 0);
        //curColor.r = (byte)(255 - curColor.b);

        if(sc_syncvar.CalibrInts [2] == 1)
        {
            curColor.r = Color1.r;
            curColor.g = Color1.g;
            curColor.b = Color1.b;
        }
        else if (sc_syncvar.CalibrInts [2] == 2)
        {
            curColor.r = Color2.r;
            curColor.g = Color2.g;
            curColor.b = Color2.b;
        }
        else if (sc_syncvar.CalibrInts [2] == 3)
        {
            curColor.r = Color3.r;
            curColor.g = Color3.g;
            curColor.b = Color3.b;
        }
        else if (sc_syncvar.CalibrInts [2] == 4)
        {
            curColor.r = Color4.r;
            curColor.g = Color4.g;
            curColor.b = Color4.b;
        }
        else if (sc_syncvar.CalibrInts [2] == 5)
        {
            curColor.r = Color5.r;
            curColor.g = Color5.g;
            curColor.b = Color5.b;
        }
        else
        {
            curColor.r = Color6.r;
            curColor.g = Color6.g;
            curColor.b = Color6.b;
        }


        line.positionCount = 300; //Configuration du nombre 
        for (int i = 0; i < line.positionCount; i++)
        {
            float x = (i * taille / line.positionCount); //Valeur de X
            float y = Mathf.Sin((Time.time * speed) + (i + curPhase) * curFrequence) * curAmplitude; //0;// Mathf.Sin(Time.time + i * speed) * amplitude; //Valeur de Y
            line.SetPosition(i, new Vector3(y, 0f, x)); //Distribution des valeurs dans le tableau (index, Vector3)

            lineRenderer.material.color = curColor;

        }

        Sc_LaserFeedBack.Instance.SetColor(curColor);
        SC_WeaponLaserGun.Instance.AlignColor(curColor);

    }

    void UpdateLineBreakdown()
    {
        curColor.r = colorBreakdown.r;
        curColor.g = colorBreakdown.g;
        curColor.b = colorBreakdown.b; 

        line.positionCount = 2; //Configuration du nombre 
        for (int i = 0; i < line.positionCount; i++)
        {
            float x = (i * taille); //Valeur de X
            float y = 0;
            line.SetPosition(i, new Vector3(y, 0f, x)); //Distribution des valeurs dans le tableau (index, Vector3)

            lineRenderer.material.color = curColor;

        }
    }

    void GetReferences()
    {
        if (Mng_SyncVar == null)
            Mng_SyncVar = GameObject.FindGameObjectWithTag("Mng_SyncVar");
        if (Mng_SyncVar != null && sc_syncvar == null)
            sc_syncvar = Mng_SyncVar.GetComponent<SC_SyncVar_calibr>();
    }




    float ratio(float inputValue, float inputMax, float outputMax, float inputMin = 0.0f, float outputMin = 0.0f)
    {
        float product = (inputValue - inputMin) / (inputMax - inputMin);
        float output = ((outputMax - outputMin) * product) + outputMin;
        return output;
    }
}
