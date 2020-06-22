 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class FlockSettings : ScriptableObject
{
    [Header("General Attack Settings")]
    public FlockType attackType;

    public enum FlockType
    {
        Bullet,
        Laser,
        Kamikaze,
        Boss,

        none
    }
    public enum BossAttackType
    {
        Bullet,
        Laser,
        Both
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

    [Tooltip("in Second")]
    public int timeBeforeFirstAttack = -1;


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
    [Tooltip("Pourcentage de sensibilité a partir duquel le flock réagit au tir")]
    public int flightReactionMinSensibility = 75;

    [Tooltip("in second")]
    public float hitReactionDelay = 2;
    [Tooltip("Pourcentage de sensibilité a partir duquel le flock ne réagit plus au tir")]
    public int hitReactionMaxSensibility = 50;

    public AttackFocus attackFocusHitReaction = AttackFocus.Display;
    public int damageOnSystemHitReaction = 5;
    public float laserDurationHitReaction = 2;


    [Header("Boss Specifics")]

    public BossAttackType bossAttackType;
    public float startingLife = 10;
    public float fleeingLife = 0;



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

    [Range(10,1000)]
    public int boidSpawn;

    [Range(10,1000)]
    public int maxBoid;

    [Tooltip("boids per min")]
    public int regenerationRate;

    [Tooltip("1 is normal"), Range(0.01f, 5f)]
    public float damageMultiplicator = 1;
}