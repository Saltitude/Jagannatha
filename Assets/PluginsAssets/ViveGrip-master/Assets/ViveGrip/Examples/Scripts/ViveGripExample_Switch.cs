using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViveGripExample_Switch : MonoBehaviour, IInteractible
{
    public bool isEnPanne = false;

    private bool curState = false;

    private bool desiredValue = false;

    [Range(0,1)]
    public float probability = 1;

    private GameObject Mng_SyncVar;
    private IF_SyncVar_Sliders sc_syncvar;
    public GameObject LocalBreakdownMng;

    public int index;


    public enum TargetSystem { Display, Movement }
    public TargetSystem MovementState = TargetSystem.Display;

    [SerializeField]
    GameObject gem;
    float limitOfGemmeZ;
    float intervalle;
    void Start() {
        GetReferences();
        limitOfGemmeZ = gem.transform.localPosition.z;
        intervalle = Mathf.Abs(limitOfGemmeZ / 5);
    }

    void GetReferences()
    {
        if (LocalBreakdownMng == null)
            LocalBreakdownMng = this.transform.parent.parent.gameObject;
        if (Mng_SyncVar == null)
            Mng_SyncVar = GameObject.FindGameObjectWithTag("Mng_SyncVar");
        if (Mng_SyncVar != null && sc_syncvar == null)
        {
            if (MovementState == TargetSystem.Display)
                sc_syncvar = Mng_SyncVar.GetComponent<SC_SyncVar_BreakdownDisplay>();
            else if (MovementState == TargetSystem.Movement)
                sc_syncvar = Mng_SyncVar.GetComponent<SC_SyncVar_BreakdownMovement>();
        }
            

       
    }

    public void Update()
    {
        
       

        

        if (Input.GetKeyDown(KeyCode.P))
        {
            /*
            SC_SyncVar_BreakdownDisplay.Pow babar = sc_syncvar.m_pows[index] ;
            babar.setPower(3);
            Debug.Log("Valeur de babar  : " + babar.power);
            Debug.Log("wesh : " + sc_syncvar.m_pows[index].power);
            sc_syncvar.m_pows.RemoveAt(index);
            sc_syncvar.m_pows.Insert(index, babar);
            Debug.Log("Valeur de sync  : " + sc_syncvar.m_pows[index].power);
            */
        }

        IsValueOk();
    }

    public void Flip()
    {

        //Vector3 rotation = transform.localEulerAngles;
        //rotation.x *= -1;
        //transform.localEulerAngles = rotation;


        curState = !curState;
        sendToSynchVar(curState);
        StartCoroutine(animGemme());

        //SON
        if (curState == false)
        {
            CustomSoundManager.Instance.PlaySound(gameObject, "SFX_p_click_button_1", false, 1, true,0.05f, 0.5f);

        }
        else
        {
            CustomSoundManager.Instance.PlaySound(gameObject, "SFX_p_click_button_2", false, 1, true, 0.05f, 0.4f);

        }
        


  }

    IEnumerator animGemme()
    {
        bool isOnAnim = true;
        while(isOnAnim)
        {
            gem.transform.localPosition -= new Vector3(0, 0,Time.deltaTime*0.1f);
            yield return new WaitForSeconds(0);
            if(gem.transform.localPosition.z < -0.02f)
            {
                isOnAnim = false;
            }
        }
        while(!isOnAnim)
        {
            gem.transform.localPosition += new Vector3(0, 0, Time.deltaTime* 0.1f);
            yield return new WaitForSeconds(0);
            if (gem.transform.localPosition.z >= 0)
            {
                StopAllCoroutines();
            }
        }
    }


    public bool isBreakdown()
    {
        return isEnPanne;
    }


    void sendToSynchVar(bool value)
    {

        if (sc_syncvar != null)
        {

            sc_syncvar.SwitchChangeValue(index,value);
            

        }
        else
            GetReferences();

    }

    public void ChangeDesired()
    {

        desiredValue = !curState;

        SetIsEnPanne(true);


        sc_syncvar.SwitchChangeValueWanted(index, desiredValue);
        sc_syncvar.SwitchChangeIsPanne(index, true);


    }

    public void Repair()
    {

        desiredValue = curState;

        SetIsEnPanne(false);


        sc_syncvar.SwitchChangeValueWanted(index, desiredValue);
        sc_syncvar.SwitchChangeIsPanne(index, false);


    }

    public void IsValueOk()
    {
        if (desiredValue == curState)
        {
            if (isEnPanne)
            {         

                if (sc_syncvar != null)
                {
                    SetIsEnPanne(false);
                    sc_syncvar.SwitchChangeIsPanne(index, false);
                }
                else
                    GetReferences();
            }          

        }
        else 
        {

            if (!isEnPanne)
            {              

                if (sc_syncvar != null)
                {
                    SetIsEnPanne(true);
                    sc_syncvar.SwitchChangeIsPanne(index, true);
                }
                else
                    GetReferences();
            }          

        }
    }

 
    void SetIsEnPanne(bool value)
    {

        isEnPanne = value;
        LocalBreakdownMng.GetComponent<IF_BreakdownManager>().CheckBreakdown();

    }

   
   public bool testAgainstOdds()
    {
        float rand = Random.Range(0f, 1f);

        if (rand < probability)
            return true;
        else
            return false;

        
    }

    public void ForceSync()
    {
        sendToSynchVar(curState);
    }

}
