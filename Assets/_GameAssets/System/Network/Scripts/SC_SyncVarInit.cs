﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_SyncVarInit : MonoBehaviour
{

    GameObject Mng_CheckList = null;

    // Start is called before the first frame update
    void Start()
    {
        IsCheck();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void IsCheck()
    {
        Mng_CheckList = GameObject.FindGameObjectWithTag("Mng_CheckList");
        Mng_CheckList.GetComponent<SC_CheckList>().Mng_SyncVar = this.gameObject;
    }

}
