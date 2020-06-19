using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class SC_TurboTraveling : MonoBehaviour
{

    [SerializeField]
    BezierSolution.BezierSpline[] tab_Spline;
    LookAtConstraint lookAt;
    

    BezierSolution.BezierWalkerWithSpeed SC_walker;

    // Start is called before the first frame update
    void Start()
    {
        SC_walker = GetComponent<BezierSolution.BezierWalkerWithSpeed>();
        AlignTabSpline();
        lookAt = GetComponent<LookAtConstraint>();
        FollowSpline(0);
    }

    // Update is called once per frame
    void Update()
    {
        SC_walker.Execute(Time.deltaTime);


        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.S))
        {
            AlignTabSpline();
            FollowSpline(0);
        }
    }

    void FollowSpline(int Index, float speed = 0.15f,bool bLookAt = true)
    {


        SC_walker.SetNewSpline(tab_Spline[Index]);
        SC_walker.speed = speed;
        SC_walker.travelMode = BezierSolution.TravelMode.Loop;

        if(bLookAt)
        {
            lookAt.constraintActive = true;
        }
        else
        {
            lookAt.constraintActive = false;
        }
    }

    void AlignTabSpline()
    {
        Debug.Log("AlignTabSpline");
        tab_Spline = SC_SplinesOnSplines.Instance.tab;
    }

}
