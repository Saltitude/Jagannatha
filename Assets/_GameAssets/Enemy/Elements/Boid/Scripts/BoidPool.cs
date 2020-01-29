using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidPool : MonoBehaviour
{
    [SerializeField]
    int maxBoidNumber;

    [SerializeField]
    Boid _boidPrefab;

    Boid[] boidPool;

    int curPoolindex;


    // Start is called before the first frame update
    void Start()
    {
        curPoolindex = 0;
        boidPool = new Boid[maxBoidNumber];
        for(int i =0; i<maxBoidNumber;i++)
        {
            Boid curBoid = Instantiate(_boidPrefab);
            curBoid.transform.position = new Vector3(0, -2000, 0);
            boidPool[i] = curBoid;
        }
    }


    public Boid[] GetBoid(int nbBoids)
    {

        return boidPool;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
