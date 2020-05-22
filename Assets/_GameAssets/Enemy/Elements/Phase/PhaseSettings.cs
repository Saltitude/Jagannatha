using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PhaseSettings : ScriptableObject
{

    [Header("Waves")]
    public WaveSettings[] waves;

    public enum Orientation { NorthDefault, West, South, East }
    public Orientation[] WavesOrientation;


}