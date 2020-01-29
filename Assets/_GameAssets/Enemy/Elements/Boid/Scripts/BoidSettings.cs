﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BoidSettings : ScriptableObject {
    // Settings
    public float minSpeed = 2;
    public float maxSpeed = 5;
    public float perceptionRadius = 2.5f;
    public float avoidanceRadius = 1;
    public float maxSteerForce = 3;

    public float alignWeight = 1;
    public float cohesionWeight = 1;
    public float seperateWeight = 1;

    public float targetWeight = 1;

    [Header ("Collisions")]
    public LayerMask obstacleMask;
    public float boundsRadius = .27f;
    public float avoidCollisionWeight = 10;
    public float collisionAvoidDst = 5;

    [Header("Split Curve")]

    public bool split;
    public int splitNumber;

    public AnimationCurve curveX;
    public AnimationCurve curveY;
    public AnimationCurve curveZ;

    public Vector3 amplitude;
    public Vector3 frequence;

    [Tooltip("Inverse la valeur de position d'un guide sur 2")]
    public bool invert;
    [Tooltip("Axe d'inversion")]
    public Vector3 invertAxis;

}