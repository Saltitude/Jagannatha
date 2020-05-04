using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_AberationAmount : MonoBehaviour
{
    Renderer screen;
    float speedHorizontal;
    float speedVertical;
    // Start is called before the first frame update
    void Start()
    {
        screen = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        speedHorizontal = ratio(-(SC_JoystickMove.Instance.CurImpulse), 1, 0.002f, 0, 0);
        speedVertical = ratio((SC_JoystickMove.Instance.f_ImpulseX), 1, 0.003f, 0, 0);
        //Debug.Log(speedHorizontal);
        screen.material.SetFloat("_AmountH", speedHorizontal);
        screen.material.SetFloat("_AmountV", speedVertical);

    }

    float ratio(float inputValue, float inputMax, float outputMax, float inputMin = 0.0f, float outputMin = 0.0f)
    {
        float product = (inputValue - inputMin) / (inputMax - inputMin);
        float output = ((outputMax - outputMin) * product) + outputMin;
        return output;
    }
}
