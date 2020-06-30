using UnityEngine;
using System.Collections;

public class ViveGripExample_Button : MonoBehaviour {
  private const float SPEED = 0.1f;
  private float distance;
  private int direction = 1;
    [SerializeField]
  private int VIBRATION_DURATION_IN_MILLISECONDS = 25;
    [SerializeField]
private float VIBRATION_STRENGTH = 2f;
    [SerializeField]
    int index;
    [SerializeField]
    GameObject HumptyTheBoss;

    Vector3  InitPos;
    Quaternion  InitRot;


    void Start () {
        if (index == 1)
        {
            InitPos = HumptyTheBoss.transform.position;
            InitRot = HumptyTheBoss.transform.rotation;
        }
        ResetDistance();
         
  }

  void ViveGripInteractionStart(ViveGrip_GripPoint gripPoint) {
    gripPoint.controller.Vibrate(VIBRATION_DURATION_IN_MILLISECONDS, VIBRATION_STRENGTH);
    //GetComponent<ViveGrip_Interactable>().enabled = false;
    StartCoroutine("Move");
  }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.S) && index == 1)
        {
            StartCoroutine("Move");
        }
        //Debug.Log(UmptyTheBoss.transform.position);
    }

    IEnumerator Move() {
        if (index == 1)
        {
            HumptyTheBoss.transform.position = InitPos;
            HumptyTheBoss.transform.rotation = InitRot;

        }

        while (distance > 0)
        {
            Increment();
            yield return null;
        }
    yield return StartCoroutine("MoveBack");
  }

  IEnumerator MoveBack() {
    direction *= -1;
    ResetDistance();
    while (distance > 0) {
      Increment();
      yield return null;
    }
    direction *= -1;
    ResetDistance();
    //GetComponent<ViveGrip_Interactable>().enabled = true;
  }

  void Increment() {
    float increment = Time.deltaTime * SPEED;
    increment = Mathf.Min(increment, distance);
        if(index == 1)
            transform.Translate(0, increment * -direction, 0);
        else
            transform.Translate(0, 0, increment * direction);
        distance -= increment;
  }

  void ResetDistance() {
    distance = 0.03f;
  }
}
