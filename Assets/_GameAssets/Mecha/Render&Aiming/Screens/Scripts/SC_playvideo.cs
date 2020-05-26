using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_playvideo : MonoBehaviour
{

    Renderer renderer;


    void Awake()
    {
        renderer = GetComponent<Renderer>();
        renderer.enabled = true;
    }

    public void PlayVideo()
    {
        
        ((MovieTexture)renderer.material.mainTexture).loop = true;

        if(!((MovieTexture)renderer.material.mainTexture).isPlaying)
            ((MovieTexture)renderer.material.mainTexture).Play();
    }

    public void StopVideo()
    {
        
        ((MovieTexture)renderer.material.mainTexture).Stop();

        
    }

}
