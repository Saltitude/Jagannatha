using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SC_showIP : MonoBehaviour
{
    [SerializeField]
    SC_IPManager _SC_IPManager;

    [SerializeField]
    TextMeshProUGUI textContainer;
    // Start is called before the first frame update
    void Start()
    {
        if(_SC_IPManager.localComputerIP != null)
        {
            textContainer.text = "Address : " + _SC_IPManager.GetLocalIPAddress();
            //Debug.Log(_SC_IPManager.GetLocalIPAddress());
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
