﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sur Mng_CheckList | 
/// Quand les mng sont Start il donne leur réferences a ce script | 
/// Permet ensuite recupere une reference depuis n'importe ou.
/// </summary>
public class SC_CheckList : MonoBehaviour
{

    public GameObject Mng_Network = null;
    public GameObject NetworkPlayerPilot = null;
    public GameObject NetworkPlayerOperator = null;

    public GameObject Mng_Device = null;
    public GameObject Mng_Scene = null;   
    public GameObject Mng_SyncVar = null;
    public GameObject Mng_Audio = null;

    public Camera Cam_Mecha = null;
    public Camera Cam_VR = null;
    public Camera Cam_FPS = null;

    public GameObject GetNetworkManager()
    {
        return Mng_Network;
    }

    public GameObject GetNetworkPlayerPilot()
    {
        return NetworkPlayerPilot;
    }

    public GameObject GetNetworkPlayerOperator()
    {
        return NetworkPlayerOperator;
    }

    public GameObject GetMngDevice()
    {
        return Mng_Device;
    }

    public GameObject GetMngScene()
    {
        return Mng_Scene;
    }

    public GameObject GetMngSyncVar()
    {
        return Mng_SyncVar;
    }

    public GameObject GetMngAudio()
    {
        return Mng_Audio;
    }

    public Camera GetCamMecha()
    {
        return Cam_Mecha;
    }

    public Camera GetCamVR()
    {
        return Cam_VR;
    }

    public Camera GetCamFPS()
    {
        return Cam_FPS;
    }

}
