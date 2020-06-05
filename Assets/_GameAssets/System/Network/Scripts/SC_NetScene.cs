using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SC_NetScene : NetworkBehaviour
{

    [Command]
    public void CmdSendReadyOP()
    {
        //Debug.LogError("CmdSendReadyOP");
        SC_SceneManager.Instance.b_OperatorReadyToLoad = true;
    }

}
