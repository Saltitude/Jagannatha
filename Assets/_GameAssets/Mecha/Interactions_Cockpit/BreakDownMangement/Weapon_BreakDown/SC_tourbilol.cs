using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_tourbilol : MonoBehaviour, IInteractible
{

    private SC_SyncVar_BreakdownWeapon sc_syncvar;

    float f_InitRot;
    float oldRot;
    float curRot;

    float totalAngle = 0;

    public int index = 0;

    public bool isEnPanne = false;

    private float desiredValue = 0f;

    [SerializeField]
    ViveGrip_Grabbable _handle1_grabbable;

    [SerializeField]
    ViveGrip_Grabbable _handle2_grabbable;

    enum tourbiType { tourbiFirst, tourbiSecond }

    [SerializeField]
    tourbiType type;
  
    void Start()
    {
        f_InitRot = this.transform.localEulerAngles.z;
        oldRot = f_InitRot;
        sc_syncvar = SC_SyncVar_BreakdownWeapon.Instance;
    }

    //Il faut faire 1/4 de tour de valve pour changer d'un cran la valeur signifiante
    void Update()
    {

        if (this.transform.localEulerAngles.z != oldRot)
            UpdateAngle();

        if (Input.GetKeyDown(KeyCode.Y))
            DebugValue();
        
    }
    
    void UpdateAngle()
    {

        curRot = this.transform.localEulerAngles.z;

        if (Mathf.Abs(oldRot - curRot) < 260)
            totalAngle += oldRot - curRot;

        //si on veut limiter à un tour           
        if (totalAngle < -360)
            totalAngle = 359;

        else if (totalAngle > 360)
            totalAngle = -360;

        //FORMATAGE ET ENVOIE COTE OP
        //Ici on crante par 90°
        if (oldRot != curRot)
        {
            sendToSynchVar(Mathf.Floor(totalAngle / 90));

            if (Mathf.Abs(Mathf.Abs(oldRot)- Mathf.Abs(curRot)) > 2)
            {
                //vibrate
                _handle1_grabbable.Vibrate(10, 0.1f);
                _handle2_grabbable.Vibrate(10, 0.1f);
            }

            IsValueOk();
        }

        if (Mathf.Floor(oldRot / 90) != Mathf.Floor(curRot / 90))
        {
            //vibrate
            _handle1_grabbable.Vibrate(50, 1.5f);
            _handle2_grabbable.Vibrate(50, 1.5f);

            Debug.Log("cran");

        }

        oldRot = curRot;

    }//0.06859592 // 0.1714907 // 0.1611188

    public void IsValueOk()
    {

        if (/*Mathf.Abs(totalAngle - desiredValue) < 89*/ Mathf.Floor(totalAngle / 90) == Mathf.Floor(desiredValue / 90))
        {

            if (isEnPanne)
            {
                SetIsEnPanne(false);
                sc_syncvar.TourbilolChangeIsPanne(index, false);
            }

        }

        else if(!isEnPanne)
        {
            SetIsEnPanne(true);
            sc_syncvar.TourbilolChangeIsPanne(index, true);
        }

    }

    void sendToSynchVar(float value)
    {
        sc_syncvar.TourbilolChangeValue(index, value);
    }

    public void ChangeDesired()
    {

        // pour l'instant je pars sur 2 tours pour obtenir la totalité des positions possibles avec un changement d'état tous les 90°

        bool good = false;

        int count = 0;
        while (good == false)
        {

            if(type == tourbiType.tourbiFirst)
            {
                int rand = Random.Range(0,2);

                if (rand == 0)
                    desiredValue = 3 * 90;

                else
                    desiredValue = -90;

            }

            else if (type == tourbiType.tourbiSecond)
            {

                int rand = Random.Range(0, 3);

                switch(rand)
                {

                    case 0:

                        desiredValue = -4 * 90;

                        break;

                    case 1:

                        desiredValue = -2 * 90;

                        break;

                    case 2:

                        desiredValue = 1 * 90;

                        break;

                }

            }

            else
            {
                desiredValue = Random.Range(-4, 3) * 90;
            }
                
            if (Mathf.Abs(desiredValue - totalAngle) > 90 )
            {
                good = true;
            }

            count++;

            if (count > 10)
            {
                Debug.LogError("La boucle elle est FUCKEE");
                break;              
            }

        }

        SetIsEnPanne(true);

        sc_syncvar.TourbilolChangeValueWanted(index, desiredValue/90);
        sc_syncvar.TourbilolChangeIsPanne(index, true);

    }

    public bool testAgainstOdds()
    {
        return true;
    }

    #region Breakdown

    public void Repair()
    {

        desiredValue = totalAngle;

        SetIsEnPanne(false);

        sc_syncvar.TourbilolChangeValueWanted(index, Mathf.Floor(desiredValue / 90));
        sc_syncvar.TourbilolChangeIsPanne(index, false);

    }

    public bool isBreakdown()
    {
        return isEnPanne;
    }

    void SetIsEnPanne(bool value)
    {
        isEnPanne = value;
        SC_WeaponBreakdown.Instance.CheckBreakdown();
    }

    #endregion Breakdown

    #region DebugMethods

    void DebugValue()
    {
        if (index == 0)
        {
            Debug.Log("total : " + totalAngle);
            Debug.Log("desired : " + desiredValue);
        }
    }

    #endregion DebugMethods

    public void ForceSync()
    {
        sendToSynchVar(Mathf.Floor(totalAngle / 90));
    }

}
