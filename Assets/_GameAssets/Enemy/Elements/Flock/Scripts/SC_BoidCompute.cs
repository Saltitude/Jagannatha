using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Le Koa Manager gère tout les Boids et le Koa, exécute les ordres du Flock Manager 
///  | Sur le prefab Flock
///  | Auteur : Zainix
/// </summary>
public class SC_BoidCompute : MonoBehaviour
{

    //Tkt ca marche
    const int threadGroupSize = 3;
    ComputeBuffer boidBuffer;
    BoidData[] boidData;

    [SerializeField]
    ComputeShader compute;

    Boid[] _boidsTab; //Tableau contenant les boids

    [SerializeField]
    BoidSettings basiqueSettings; //Tableau contenant les boids
    Coroutine m_boidCor = null;



    public void Initialize(Boid[] boidPool)
    {
        _boidsTab = boidPool;


        boidData = new BoidData[_boidsTab.Length];
        boidBuffer = new ComputeBuffer(_boidsTab.Length, BoidData.Size);
        m_boidCor = StartCoroutine(corBoid());
    }



    IEnumerator corBoid()
    {
        //Sécurité
        if (_boidsTab != null)
        {

            while (true)
            {

                //Ajoute le Koa a la nuée avant les calcules de Flock 
                //_boidsList.Add(_koa);

                Array.Clear(boidData,0,boidData.Length);
                //BURST ?
                //Création d'un variable (Type BoidData) contenant un tableau avec le nombre d'éléments actuels


                for (int i = 0; i < _boidsTab.Length; i++)
                {
                    boidData[i].active = 0;
                    if (_boidsTab[i].isActive)
                    {
                        boidData[i].active = 1;
                        boidData[i].position = _boidsTab[i].position; //Chaque élément déjà positionné est stocké
                        boidData[i].direction = _boidsTab[i].forward; //Stockage direction
                    } 
                }

                boidBuffer.SetData(boidData);//Dans boidBuffer (type ComputeBuffer), stocakge data du tableau actuel

                //ComputeShader compute est stockage du Shader

                //Buffer Configuration | 
                //para1 : kernelIndex (ordre de la fonction dans ComputeShader)
                //para2 : name ID (cf ComputeShader ligne 14)
                //para3 : buffer 
                compute.SetBuffer(0, "boids", boidBuffer);

                //Configuration des variables du ComputeShader
                compute.SetInt("numBoids", _boidsTab.Length);
                compute.SetFloat("viewRadius", basiqueSettings.perceptionRadius);
                compute.SetFloat("avoidRadius", basiqueSettings.avoidanceRadius);


                int threadGroups = Mathf.CeilToInt(_boidsTab.Length / (float)threadGroupSize); //Nombre de groupe = nombre éléments / nombre tkt
                compute.Dispatch(0, threadGroups, 1, 1); //Execute le Shader

                yield return 0;



                boidBuffer.GetData(boidData); //Récupère le résultat du Shader

                //Cf script Boid
                for (int i = 0; i < _boidsTab.Length; i++)
                {
                    if(_boidsTab[i].isActive)
                    {
                        _boidsTab[i].avgFlockHeading = boidData[i].flockHeading; //Stockage pour chaque boid : moyenne devant lui
                        _boidsTab[i].centreOfFlockmates = boidData[i].flockCentre; //Stockage pour chaque boid : moyenne à côté
                        _boidsTab[i].avgAvoidanceHeading = boidData[i].avoidanceHeading; //Stockage pour chaque boid : moyenne des éléments à éviter
                        _boidsTab[i].numPerceivedFlockmates = boidData[i].numFlockmates; //Stockage pour chaque boid : nombre de mate autour

                        _boidsTab[i].UpdateBoid(); //Update les boidss
                    }

                }

                yield return 0;

            }
        }
    }

    /// <summary>
    /// Structure envoyée dans le ComputeShader 
    /// </summary>
    public struct BoidData
    {
        public int active;

        public Vector3 position;
        public Vector3 direction;

        public Vector3 flockHeading;
        public Vector3 flockCentre;
        public Vector3 avoidanceHeading;

        public int numFlockmates;

        //sizeof => retourne la mémoire en bit, pour un type de variable
        //Ici float* 3(Vector3) * 5(nombre de valeurs) + int * 1
        public static int Size
        {
            get
            {
                return sizeof(float) * 3 * 5 + sizeof(int)*2;
            }
        }
    }
}