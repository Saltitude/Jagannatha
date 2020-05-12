using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_Flux_jaugeDisplay : MonoBehaviour
{
    // Start is called before the first frame update
    public ViveGripExample_Slider FluxSlider;
    int FluxValue;
    Transform[] JaugeUnits;
    int MaxNumbJauge;

    void Start()
    {
        MaxNumbJauge = transform.childCount;
        JaugeUnits = new Transform [MaxNumbJauge];

        for(int i = 0; i < MaxNumbJauge; i ++)
        {
            JaugeUnits[i] = transform.GetChild(i);
        }

    }

    // Update is called once per frame
    void Update()
    {
        FluxValue = FluxSlider.curValue;
        
        for (int i = 0; i < MaxNumbJauge; i++)
        {
            if (i < FluxValue)
                JaugeUnits [i].GetComponent<MeshRenderer>().enabled = true;
            else
                JaugeUnits [i].GetComponent<MeshRenderer>().enabled = false;

            if(FluxValue == 10)
                JaugeUnits[i].GetComponent<MeshRenderer>().enabled = true;

        }
    }
}
