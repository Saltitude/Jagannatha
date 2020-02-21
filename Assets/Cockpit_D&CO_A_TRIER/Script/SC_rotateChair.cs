﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_rotateChair : MonoBehaviour
{
    [SerializeField]
    GameObject chair;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        chair.transform.LookAt(transform.position);
        chair.transform.localEulerAngles = new Vector3(0, 0, chair.transform.localEulerAngles.z+90);
       
    }
}
