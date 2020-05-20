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

    // valeur de rotation normalisée entre -1 et 1
    [SerializeField]
    float normalizedAngle = 0f;

    public int index = 0;

    [SerializeField]
    public bool isEnPanne = false;

    [SerializeField]
    private float desiredValue = 0f;

    //angle total dans un sens et dans l'autre requis par le joueur pour atteindre les bornes
    [SerializeField]
    int maxAngleBorne = 360;


    [SerializeField]
    ViveGrip_Grabbable _handle1_grabbable;

    [SerializeField]
    ViveGrip_Grabbable _handle2_grabbable;

    //marge pour le total angle => si le joueur dépasse la valeur affichable, celle-ci est toujours prise en compte, cela veut dire qu'il devra effectuer margeSimpson dans l'autre sens avant de la voir evoluer à nouveau
    [SerializeField]
    float margeSimpson = 30f;

    enum tourbiType { tourbiFirst, tourbiSecond }

    [SerializeField]
    tourbiType type;
  
    void Start()
    {
        f_InitRot = this.transform.localEulerAngles.z;
        oldRot = f_InitRot;

        sc_syncvar = SC_SyncVar_BreakdownWeapon.Instance;
    }

    //Ici on update la rotation du tourbilol 
    void Update()
    {

        if (this.transform.localEulerAngles.z != oldRot)
            UpdateAngle();

        if (Input.GetKeyDown(KeyCode.Y))
            DebugValue();
        
    }
    
    //met à jour le total angle et le normalize avant de l'envoyer dans la sync var et faire un test de panne
    void UpdateAngle()
    {

        curRot = this.transform.localEulerAngles.z;

        if (Mathf.Abs(oldRot - curRot) < 260)
            totalAngle += oldRot - curRot;


        //sécurité pour que le joueur ne s'enfonce pas dans les meandres
        if (totalAngle > maxAngleBorne + margeSimpson)
            totalAngle = maxAngleBorne + margeSimpson;
        else if (totalAngle <- (maxAngleBorne + margeSimpson))
            totalAngle = -(maxAngleBorne + margeSimpson);

        normalizedAngle = NormalizeAngle(totalAngle);

        sendToSynchVar(normalizedAngle);

        ///////////vibration pti crancrans
        if (Mathf.Abs(Mathf.Abs(oldRot) - Mathf.Abs(curRot)) > 2)
        {
            //vibrate
            _handle1_grabbable.Vibrate(10, 0.02f);
            _handle2_grabbable.Vibrate(10, 0.02f);
        }


        // vibrate quand on atteint le bout

        if (Mathf.Abs(normalizedAngle) == 1)
        {
            _handle1_grabbable.Vibrate(10, 2f);
            _handle2_grabbable.Vibrate(10, 2f);
        }


        oldRot = curRot;

        IsValueOk();

    }

    //clamp l'angle d'après l'angle max et normalise l'angle total entre -1 et 1
    private float NormalizeAngle(float _angle)
    {
        float _normalizedAngle = 0;

        _normalizedAngle = Ratio(Mathf.Clamp(_angle, -maxAngleBorne, maxAngleBorne), maxAngleBorne, 1, -maxAngleBorne, -1);


        return _normalizedAngle;
    }


    public void IsValueOk()
    {

        if (normalizedAngle == desiredValue)
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

        // on remet à zero la position de départ
        totalAngle = 0;
        normalizedAngle = 0;
        sendToSynchVar(0);

        //rand pour décider du côté
        int rand = Random.Range(0, 2);

        if (rand == 0)
            desiredValue = 1;
        else
            desiredValue = -1;


        SetIsEnPanne(true);

        sc_syncvar.TourbilolChangeValueWanted(index, desiredValue);
        sc_syncvar.TourbilolChangeIsPanne(index, true);




    }

    public bool testAgainstOdds()
    {
        return true;
    }

    #region Breakdown

    public void Repair()
    {

        desiredValue = normalizedAngle;

        SetIsEnPanne(false);

        sc_syncvar.TourbilolChangeValueWanted(index, normalizedAngle);
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
            Debug.Log("total : " + normalizedAngle);
            Debug.Log("desired : " + desiredValue);
        }
    }

    #endregion DebugMethods

    public void ForceSync()
    {
        sendToSynchVar(normalizedAngle);
    }

    float Ratio(float inputValue, float inputMax, float outputMax, float inputMin = 0.0f, float outputMin = 0.0f)
    {
        float product = (inputValue - inputMin) / (inputMax - inputMin);
        float output = ((outputMax - outputMin) * product) + outputMin;
        return output;
    }

}
