using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_BarreulRowll : MonoBehaviour
{
    float speed = 100;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(0, speed * Time.deltaTime, 0);
    }
}
