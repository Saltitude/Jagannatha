using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PhaseSettings : ScriptableObject
{

    [Header("Waves")]
    public WaveSettings[] waves;

    public enum Orientation
    {
        NorthDefault = 0,
        NorthWest = 1,
        West = 2,
        SouthWest = 3,
        South = 4,
        SouthEast = 5,
        East = 6,
        NorthEast = 7
    } 

    public Orientation[] WavesOrientation;

    void Awake()
    {
        //SetOrientedSpawnpos()
    }

    /*
    void SetOrientedSpawnpos()
    {

        for(int i = 0; i < waves.Length; i++)
        {

            waves[i].OrientedSpawnPosition = new int[waves[i].initialSpawnPosition.Length];
            for (int j = 0; j < waves[i].OrientedSpawnPosition.Length; j++)
                waves[i].OrientedSpawnPosition[j] = (waves[i].initialSpawnPosition[j] + (int)WavesOrientation[i]) % 8;

            waves[i].backupOrientedSpawnPosition = new int[waves[i].backupSpawnPosition.Length];
            for (int j = 0; j < waves[i].backupSpawnPosition.Length; j++)
                waves[i].backupOrientedSpawnPosition[j] = (waves[i].backupSpawnPosition[j] + (int)WavesOrientation[i]) % 8;

        }

    }
    */

}