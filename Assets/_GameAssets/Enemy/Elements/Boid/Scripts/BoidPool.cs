using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidPool : MonoBehaviour
{
    [SerializeField]
    int poolNumber;

    [SerializeField]
    Boid _boidPrefab;

    Boid[] boidPool;


    int curPoolindex;


    // Start is called before the first frame update
    void Start()
    {
        curPoolindex = 0;
        boidPool = new Boid[poolNumber];
        for(int i =0; i<poolNumber;i++)
        {
            Boid curBoid = Instantiate(_boidPrefab);
            curBoid.transform.position = new Vector3(0, -2000, 0);
            boidPool[i] = curBoid;
            
        }


        this.GetComponent<SC_BoidCompute>().Initialize(boidPool);
    }


    public Boid[] GetBoid(int boidNumber)
    {
        Boid[] boidTab = new Boid[boidNumber];
        for(int i =0; i<boidNumber; i++)
        {
            boidTab[i] = boidPool[curPoolindex];
            curPoolindex++;
            if (curPoolindex >= poolNumber)
                curPoolindex = 0;
        }
        return boidTab;
    }

}
