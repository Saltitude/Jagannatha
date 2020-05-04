using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_tourbilol_handle_kinematic : MonoBehaviour
{

    public Rigidbody otherHandle;


    private ViveGrip_ControllerHandler controller;
    private Rigidbody thisHandle;


    // Start is called before the first frame update
    void Start()
    {
        thisHandle = gameObject.GetComponent<Rigidbody>();
    }


    void ViveGripGrabStart(ViveGrip_GripPoint gripPoint)
    {
        controller = gripPoint.controller;
        otherHandle.isKinematic = false;
        thisHandle.isKinematic = false;

    }

    void ViveGripGrabStop()
    {
        controller = null;
        otherHandle.isKinematic = true;
        thisHandle.isKinematic = true;
    }





   

    // Update is called once per frame
    void Update()
    {
        
    }
}
