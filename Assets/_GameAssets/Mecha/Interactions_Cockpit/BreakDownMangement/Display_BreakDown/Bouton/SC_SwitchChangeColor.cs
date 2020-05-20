using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_SwitchChangeColor : MonoBehaviour
{
    // Start is called before the first frame update
    bool b_light;

    void Start()
    {
        b_light = false;
        
    }

    // Update is called once per frame
    void Update()
    {


    }
    public void LightOnOFF()
    {
        StartCoroutine(SwichBlink(0.3f));
    }
    IEnumerator SwichBlink(float duration)
    {

        gameObject.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
        yield return new WaitForSeconds(duration);
        gameObject.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
        StopAllCoroutines();
    }

}
