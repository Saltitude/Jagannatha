using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SC_UI_OngletSelection : MonoBehaviour, IF_clicableAction, IF_Hover
{
    public int index;
    public SC_UI_OngletContainer ongletContainer;
    SC_UI_WireBlink wireBlink;


    [SerializeField]
    GameObject animated;
    [SerializeField]
    GameObject additionalAnimated;

    Animator animator;
    Animator additionalAnimator;

    [SerializeField]
    int[] wireIndex;



    // Start is called before the first frame update
    void Start()
    {
        ongletContainer = SC_UI_OngletContainer.Instance;
        if(animated != null)
        animator = animated.GetComponent<Animator>();
        if(additionalAnimated != null)
        additionalAnimator = additionalAnimated.GetComponent<Animator>();
        wireBlink = GetComponentInParent<SC_UI_WireBlink>();


    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Action()
    {
        SC_UI_OngletContainer.Window newWindow = (SC_UI_OngletContainer.Window)index;
        if(index == 0 && SC_GameStates.Instance.CurTutoState == SC_GameStates.TutorialState.Tutorial1_3)
        {
                SC_CheckList.Instance.NetworkPlayerOperator.GetComponent<SC_Net_Player_TutoState>().CmdChangeTutoState(SC_GameStates.TutorialState.Tutorial1_4);
        }
        else if (index == 1 && SC_GameStates.Instance.CurTutoState == SC_GameStates.TutorialState.Tutorial1_3)
        {
                SC_CheckList.Instance.NetworkPlayerOperator.GetComponent<SC_Net_Player_TutoState>().CmdChangeTutoState(SC_GameStates.TutorialState.Tutorial1_5);
        }
        else if (index == 2 && SC_GameStates.Instance.CurTutoState == SC_GameStates.TutorialState.Tutorial1_3)
        {
                SC_CheckList.Instance.NetworkPlayerOperator.GetComponent<SC_Net_Player_TutoState>().CmdChangeTutoState(SC_GameStates.TutorialState.Tutorial1_6);
        }
        if(index == 3 && SC_GameStates.Instance.CurTutoState == SC_GameStates.TutorialState.Tutorial1_7)
        {
            SC_CheckList.Instance.NetworkPlayerOperator.GetComponent<SC_Net_Player_TutoState>().CmdChangeTutoState(SC_GameStates.TutorialState.Tutorial1_8);
        }
        ongletContainer.ChangeWindow(newWindow);
        OnClicAnimation();
        #region AnimIn&Out
        
        if(index == 0)
        {
         //   if (animator.GetBool("ClicDisplay") == true)
            ongletContainer.DisplayIn();
            animator.SetBool("ActivateDisplay", true);
        }
        if (index == 1)
        {
          //  if (animator.GetBool("ClicWeapon") == true)
                ongletContainer.WeaponIn();
                animator.SetBool("ActivateWeapon", true);
        }
        if (index == 2)
        {
          //  if (animator.GetBool("ClicMove") == true)
                ongletContainer.MoveIn();
                animator.SetBool("ActivateMove", true);
        }



        if (index == 3)
        {
            ongletContainer.DisplayOut();
            additionalAnimator.SetBool("ActivateDisplay", false);
        }
        if (index == 4)
        {
            ongletContainer.WeaponOut();
            additionalAnimator.SetBool("ActivateWeapon", false);
        }
        if (index == 5)
        {
            ongletContainer.MoveOut();
            additionalAnimator.SetBool("ActivateMove", false);
        }
        #endregion
    }

    public void HoverAction()
    {
        IsHover();
    }


    public void isBreakdownSystem(bool state)
    {
        
        for (int i = 0; i < wireIndex.Length; i++)
        {
            wireBlink.SetBreakDown(wireIndex[i], state);
        }

    }


    void IsHover()
    {
        if (animator != null)
        {
          
                animator.SetBool("Hover", true);
               StartCoroutine(EndCoroutine("Hover"));
            
            //if (index == 0)
            //{
            //    animator.SetBool("HoverDisplay", true);
            //    StartCoroutine(EndCoroutine("HoverDisplay"));
            //}
            //if (index == 1)
            //{
            //    animator.SetBool("HoverWeapon", true);
            //    StartCoroutine(EndCoroutine("HoverWeapon"));
            //}
            //if (index == 2)
            //{
            //    animator.SetBool("HoverMove", true);
            //    StartCoroutine(EndCoroutine("HoverMove"));
            //}

        }

    }
 

    void OnClicAnimation()
    {
       /*
            animator.SetBool("Clic", true);
            StartCoroutine(EndCoroutine("Clic"));*/
        
        //if (animator != null)
        //{
        //    if (index == 0)
        //    {
        //        animator.SetBool("ClicDisplay", true);
        //        StartCoroutine(EndCoroutine("ClicDisplay"));
        //    }
        //    if (index == 1)
        //    {
        //        animator.SetBool("ClicWeapon", true);
        //        StartCoroutine(EndCoroutine("ClicWeapon"));
        //    }
        //    if (index == 2)
        //    {
        //        animator.SetBool("ClicMove", true);
        //        StartCoroutine(EndCoroutine("ClicMove"));
        //    }

        //}
    }

    IEnumerator EndCoroutine(string Bool)
    {
        yield return new WaitForEndOfFrame();
        animator.SetBool(Bool, false);
    }
}
