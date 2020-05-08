using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_KoaAnimation : MonoBehaviour
{

    [SerializeField]
    int color;

    
    // Start is called before the first frame update

    Renderer[] renderer;
    void Start()
    {
        renderer = GetComponentsInChildren<Renderer>();
     
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(color);
        for (int i = 0; i < renderer.Length; i++)
        {
            //renderer[i].material.SetColor("_EmissionColor", color);

        }
    }
}
