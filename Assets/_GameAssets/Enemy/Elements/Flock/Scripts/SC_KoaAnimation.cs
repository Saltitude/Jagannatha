using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_KoaAnimation : MonoBehaviour
{

    [SerializeField]
    Color color;

    Transform papa;
    
    // Start is called before the first frame update

    Renderer[] renderer;
    void Start()
    {
        papa = transform.parent;
        renderer = papa.GetComponentsInChildren<Renderer>();
    
    }

    // Update is called once per frame
    void Update()
    {

        for (int i = 0; i < renderer.Length; i++)
        {
            renderer[i].material.SetColor("_EmissionColor", color);
        }
    }
}
