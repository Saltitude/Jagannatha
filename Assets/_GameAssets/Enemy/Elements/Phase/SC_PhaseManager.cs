﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Script gerant l'enchainement des Waves dans une phase
///  | Sur le prefab EnemyManager(à instantié une fois)
///  | Auteur : Zainix
/// </summary>
public class SC_PhaseManager : MonoBehaviour
{

    #region Singleton

    private static SC_PhaseManager _instance;
    public static SC_PhaseManager Instance { get { return _instance; } }

    #endregion
    PhaseSettings curPhaseSettings;
    public WaveSettings[] waves;


    int curWaveIndex;
    // Start is called before the first frame update
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

        resetVariables();
    }

    public void Initialize(PhaseSettings newPhaseSettigns)
    {

        curPhaseSettings = newPhaseSettigns;
        resetVariables();
        waves = newPhaseSettigns.waves;
        
        SC_WaveManager.Instance.InitializeWave(waves[curWaveIndex]);
    }

    // Update is called once per frame
    void Update()
    {
    
    }
           
    public void EndWave()
    {
        curWaveIndex++;
        if(curWaveIndex<waves.Length)
        {

            SC_WaveManager.Instance.InitializeWave(waves[curWaveIndex]);
        }
        else
        {

            SC_EnemyManager.Instance.EndPhase();
        }
    }

    void resetVariables()
    {
        curWaveIndex = 0;
    }
}
