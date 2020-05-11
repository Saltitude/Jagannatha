 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class FlockSettings : ScriptableObject
{
    [Header("General Attack Settings")]
    public AttackType attackType;

    public enum AttackType
    {
        Bullet,
        Laser,
        Kamikaze,

        none
    }

    public AttackFocus attackFocus ;
    public int damageOnSystem;
    public enum AttackFocus
    {
        Display,
        Movement,
        Weapon,

        none
    }

    [Tooltip("in Second")]
    public int timeBetweenAttacks;


    [Header("Bullet")]
    
    [Tooltip("bullet per sec")]
    [Range(0f,5f)]
    public float fireRate = 0;
    [Tooltip("nb total de bullet a tirer avant de retourner en roam")]
    public float nbBulletToShoot = 0;

    [Header("Laser")]
    [Tooltip("in Second")]
    public float chargingAttackTime = 0;
    [Tooltip("in Second, avant de retourner en roam")]
    public float activeDuration = 0;

    [Header("Kamikaze")]
    public float speedToTarget;


    [Header("Reaction")]

    [Tooltip("in second")]
    public float flightDuration = 3;
    [Tooltip("Pourcentage de sensibilité a partir duquel le flock ne réagit plus au tir")]
    public int hitReactionMaxSensibility = 50;

    [Tooltip("in second")]
    public float hitReactionDelay = 2;
    [Tooltip("Pourcentage de sensibilité a partir duquel le flock réagit au tir")]
    public int flightReactionMinSensibility = 75;

    public AttackFocus attackFocusHitReaction = AttackFocus.Display;
    public int damageOnSystemHitReaction = 5;



    [Header("Boids")]


    [Tooltip("Index 0 : Spawn | Index 1 : Roam | Index 2 : Attack | Index 3 : Destruction | Index 4 : Réaction")]
    public BoidSettings[] spawnSettings;
    public BoidSettings[] roamSettings;
    public BoidSettings[] attackSettings;
    public BoidSettings[] destructionSettings;
    public BoidSettings[] getAwaySettings;
    public BoidSettings[] hitReactionSettings;


    public BezierSolution.BezierSpline[] splines;


    public int spawnTimer = 10;


    [Range(10,200)]
    public int boidSpawn;

    [Range(10,200)]
    public int maxBoid;

    [Tooltip("boids per min")]
    public int regenerationRate;


}