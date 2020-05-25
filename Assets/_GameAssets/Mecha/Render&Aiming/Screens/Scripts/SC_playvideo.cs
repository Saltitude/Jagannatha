using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_playvideo : MonoBehaviour
{

    [SerializeField]
    bool b_OnPlay = false;

    void Start()
    {
        //PlayVideo();
    }

    public void PlayVideo()
    {
        
        ((MovieTexture)GetComponent<Renderer>().material.mainTexture).loop = true;

        if(!((MovieTexture)GetComponent<Renderer>().material.mainTexture).isPlaying)
            ((MovieTexture)GetComponent<Renderer>().material.mainTexture).Play();

        b_OnPlay = true;

    }

    public void StopVideo()
    {
        
        ((MovieTexture)GetComponent<Renderer>().material.mainTexture).Stop();

        b_OnPlay = false;
        
    }

}
