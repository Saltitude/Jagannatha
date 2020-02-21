﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_UI_Cockpit_FrequenceLine : MonoBehaviour
{
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





    GameObject Mng_SyncVar = null;
    SC_SyncVar_calibr sc_syncvar;

    public int indexDouble1;
    public int indexDouble2;

    void Start()
    {
        line = this.gameObject.GetComponent<LineRenderer>(); //Stockage de lui-meme
        Mng_SyncVar = GameObject.FindGameObjectWithTag("Mng_SyncVar");
        GetReferences();
    }

    // Update is called once per frame
    void Update()
    {
        updateLineRender();
        if (sc_syncvar == null || Mng_SyncVar == null)
        {
            GetReferences();

        }
        if (sc_syncvar != null)
        {
            

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
        curFrequence = ratio(sc_syncvar.CalibrInts[1], 6, 0.33f, 0, 0);
        curPhase = ratio(sc_syncvar.CalibrInts[2], 6, 20, 0, 0);

        line.positionCount = 300; //Configuration du nombre 
        for (int i = 0; i < line.positionCount; i++)
        {
            float x = (i * taille / line.positionCount); //Valeur de X
            float y = Mathf.Sin((Time.time * speed) + (i + curPhase) * curFrequence) * curAmplitude; //0;// Mathf.Sin(Time.time + i * speed) * amplitude; //Valeur de Y
            line.SetPosition(i, new Vector3(y, 0f, x)); //Distribution des valeurs dans le tableau (index, Vector3)

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
