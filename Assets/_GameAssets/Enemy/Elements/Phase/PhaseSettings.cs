using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PhaseSettings : ScriptableObject
{

    [Header("Waves")]
    public WaveSettings[] waves;

    public enum Orientation { NorthDefault, East, South, West }
    public Orientation[] WavesOrientation;


}