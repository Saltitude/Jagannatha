using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_weapon_puzzle_op_display : MonoBehaviour
{

    #region Variables

    //Bar Variables
    [Header("References")]
    [SerializeField]
    GameObject[] rotBarTab;
    [SerializeField]
    Material[] rotBarMatTab;
    public Material[] solutions_mat;
    [SerializeField]
    Material neutral_mat;
    public MeshRenderer solution;
    GameObject[] tableau_barres;
    float[] tableau_init_rot_z;
    Quaternion[] tableau_old_rot;
    Quaternion[] tableau_new_rot;  
    float init_rot_cylindre;

    //Coroutine
    [Header("Coroutines Parameters")]
    [SerializeField]
    float f_AngleMainCorDuration = 0.2f;
    [SerializeField]
    float f_AngleBarCorDuration = 0.2f;
    [SerializeField]
    AnimationCurve Acceleration;
    Coroutine CurAngleMainCoro;
    Coroutine[] CurAngleBarCoro;

    //Solutions
    int oldSolutionNb = 40;
    int solutionNb;

    //Angles
    Quaternion oldAngleMain;
    Quaternion newAngleMain;
    Vector3 oldDirMain;
    Vector3 newDirMain;
    Quaternion CoroAngleMain;
    Quaternion[] CoroAngleBar;

    [SerializeField]
    float valueTest;
    #endregion Variables

    private void Awake()
    {

        tableau_barres = new GameObject[gameObject.transform.childCount];
        tableau_old_rot = new Quaternion[gameObject.transform.childCount];
        tableau_new_rot = new Quaternion[gameObject.transform.childCount];
        tableau_init_rot_z = new float[gameObject.transform.childCount];
        CoroAngleBar = new Quaternion[gameObject.transform.childCount];
        CurAngleBarCoro = new Coroutine[gameObject.transform.childCount];

        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            tableau_barres[i] = gameObject.transform.GetChild(i).gameObject;
            tableau_init_rot_z[i] = gameObject.transform.GetChild(i).localEulerAngles.y;
            tableau_old_rot[i] = tableau_barres[i].transform.localRotation;
            CoroAngleBar[i] = tableau_barres[i].transform.localRotation;
        }

        init_rot_cylindre = gameObject.transform.eulerAngles.y;

        CoroAngleMain = transform.rotation;

    }

    void Update()
    {

        UpdateAngleBar();

        UpdateAngleMain();

        SetSolution();

        ApplySolution();

    }   

    void UpdateAngleBar()
    {

        for (int i = 0; i < tableau_barres.Length; i++)
        {

            tableau_new_rot[i] = Quaternion.Euler(-90, 0, tableau_init_rot_z[i] + SC_SyncVar_BreakdownWeapon.Instance.SL_Tourbilols[0].value * 22.5f + 22.5f);
            tableau_old_rot[i] = tableau_barres[i].transform.localRotation;


            //Old Method
            tableau_barres[i].transform.localEulerAngles = Vector3.Slerp(tableau_old_rot[i].eulerAngles, tableau_new_rot[i].eulerAngles, 0.25f);
 
            /*
            //Coroutine Method
            if (tableau_new_rot[i] != tableau_old_rot[i] && tableau_new_rot[i] != CoroAngleBar[i])
            {

                if (CurAngleBarCoro[i] != null)
                    StopCoroutine(CurAngleBarCoro[i]);

                CurAngleBarCoro[i] = StartCoroutine(GoTargetRotBarAngle(f_AngleBarCorDuration, tableau_new_rot[i], i));

            }
            */

        }

    }

    IEnumerator GoTargetRotBarAngle(float Duration, Quaternion TargetRot, int Index)
    {

        float t = 0;
        float rate = 1 / Duration;

        Quaternion StartRot = tableau_barres[Index].transform.rotation;
        CoroAngleBar[Index] = TargetRot;

        while (t < 1)
        {

            t += Time.deltaTime * rate;
            float Lerp = Acceleration.Evaluate(t);

            tableau_barres[Index].transform.rotation = Quaternion.Slerp(StartRot, CoroAngleBar[Index], Lerp);

            yield return 0;

        }

    }

    void UpdateAngleMain()
    {

        oldAngleMain = this.transform.rotation;
        oldDirMain = this.transform.rotation * Vector3.forward;
        Debug.Log("SyncVar1 = " + SC_SyncVar_BreakdownWeapon.Instance.SL_Tourbilols[1].value);
        switch (SC_SyncVar_BreakdownWeapon.Instance.SL_Tourbilols[1].value)
        {

            //solution1
            case -4:

                newAngleMain = Quaternion.Euler(0, init_rot_cylindre + 255 , 0);

                break;

            case -3:

                newAngleMain = Quaternion.Euler(0, 35, 0);

                break;

            //solution2
            case -2:

                newAngleMain = Quaternion.Euler(0, 135, 0);

                break;

            case -1:

                newAngleMain = Quaternion.Euler(0, 100, 0);

                break;

            case 0:

                newAngleMain = Quaternion.Euler(0, 140, 0);

                break;
            //solution3 
            case 1:

                newAngleMain = Quaternion.Euler(0, valueTest, 0);

                break;

            case 2:

                newAngleMain = Quaternion.Euler(0, 220, 0);

                break;

            case 3:

                newAngleMain = Quaternion.Euler(0, 300, 0);

                break;

        }

        //Old Method
        //C'EST LUI LE BATARD QUAND -4 EST IMPLIQUER

        //this.transform.eulerAngles = Vector3.Slerp(oldAngleMain.eulerAngles, newAngleMain.eulerAngles, 0.25f);
        newDirMain = newAngleMain * Vector3.forward;
        Quaternion newRot = Quaternion.FromToRotation(oldDirMain, newDirMain);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, newRot, 0.25f);
        

    }

    IEnumerator GoTargetRotMainAngle(float Duration, Quaternion TargetRot)
    {

        float t = 0;
        float rate = 1 / Duration;

        Quaternion StartRot = transform.rotation;
        CoroAngleMain = TargetRot;

        while (t < 1)
        {

            t += Time.deltaTime * rate;
            float Lerp = Acceleration.Evaluate(t);

            this.transform.rotation = Quaternion.Slerp(StartRot, CoroAngleMain, Lerp);

            yield return 0;

        }

    }

    void SetSolution()
    {

        if (SC_SyncVar_BreakdownWeapon.Instance.SL_Tourbilols[1].valueWanted == -4)
        {

            if (SC_SyncVar_BreakdownWeapon.Instance.SL_Tourbilols[0].valueWanted == -1)
            {
                solutionNb = 0;
            }

            else
                solutionNb = 1;

        }

        else if (SC_SyncVar_BreakdownWeapon.Instance.SL_Tourbilols[1].valueWanted == -2)
        {

            if (SC_SyncVar_BreakdownWeapon.Instance.SL_Tourbilols[0].valueWanted == -1)
            {
                solutionNb = 2;
            }

            else
                solutionNb = 3;

        }

        else if (SC_SyncVar_BreakdownWeapon.Instance.SL_Tourbilols[1].valueWanted == 1)
        {

            if (SC_SyncVar_BreakdownWeapon.Instance.SL_Tourbilols[0].valueWanted == -1)
            {
                solutionNb = 5;
            }

            else
                solutionNb = 4;

        }

        if (!SC_SyncVar_BreakdownWeapon.Instance.SL_Tourbilols[1].isEnPanne && !SC_SyncVar_BreakdownWeapon.Instance.SL_Tourbilols[0].isEnPanne)
        {
            solutionNb = 420;
            checkColorBar(0);
        }

        else
            checkColorBar(1);

    }

    void ApplySolution()
    {

        if (solutionNb != oldSolutionNb)
        {

            switch (solutionNb)
            {

                case 0:
                    solution.material = solutions_mat[0];
                    break;

                case 1:
                    solution.material = solutions_mat[1];
                    break;

                case 2:
                    solution.material = solutions_mat[2];
                    break;

                case 3:
                    solution.material = solutions_mat[3];
                    break;

                case 4:
                    solution.material = solutions_mat[4];
                    break;

                case 5:
                    solution.material = solutions_mat[5];
                    break;

                default:
                    solution.material = neutral_mat;
                    break;

            }

            oldSolutionNb = solutionNb;

        }    

    }

    void checkColorBar(int indexMax)
    {

        for (int i = 0; i < rotBarTab.Length; i++)
        {
            rotBarTab[i].GetComponent<MeshRenderer>().material = rotBarMatTab[indexMax];
        }

        this.GetComponent<MeshRenderer>().material = rotBarMatTab[indexMax];

    }

}
