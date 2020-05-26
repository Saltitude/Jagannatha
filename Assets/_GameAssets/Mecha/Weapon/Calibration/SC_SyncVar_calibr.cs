using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SC_SyncVar_calibr : NetworkBehaviour
{
    /// <summary>
    /// //////////////////////////////////List struct des Sliders
    /// </summary>
    /// 
    public int nbOfIntButtons;

    //nombre de sliders pour l'init
    public SyncListInt CalibrInts = new SyncListInt();



    void Start()
    {
        if (isServer)
        {
            for (int i = 0; i < nbOfIntButtons; i++)
            {
                CalibrInts.Insert(i, 0);
            }
        }
    }


}
