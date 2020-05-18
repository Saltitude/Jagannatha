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
#if UNITY_EDITOR
        if(index == 3 || index == 4 || index == 5)
        {
            this.GetComponent<BoxCollider>().center = new Vector3(-11.49976f, 4.058487f, -22.14557f);
            this.GetComponent<BoxCollider>().size = new Vector3(162.1187f, 163.5854f, 54.29114f);
        }

#endif

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Action()
    {
        SC_UI_OngletContainer.Window newWindow = (SC_UI_OngletContainer.Window)index;
        if(index == 0 && SC_GameStates.Instance.CurTutoState == SC_GameStates.TutorialState.RepairDisplay)
        {
            // SC_CheckList.Instance.NetworkPlayerPilot.GetComponent<SC_Net_Player_TutoState>().CmdChangeTutoState(SC_GameStates.TutorialState.Tutorial1_4);
            ongletContainer.DisplayIn();
            animator.SetBool("ActivateDisplay", true);
        }
        else if (index == 1 && SC_GameStates.Instance.CurTutoState == SC_GameStates.TutorialState.RepairWeapon)
        {
            //SC_CheckList.Instance.NetworkPlayerPilot.GetComponent<SC_Net_Player_TutoState>().CmdChangeTutoState(SC_GameStates.TutorialState.Tutorial1_5);

            ongletContainer.WeaponIn();
            animator.SetBool("ActivateWeapon", true);
        }
        else if (index == 2 && SC_GameStates.Instance.CurTutoState == SC_GameStates.TutorialState.RepairDisplay)
        {
            //SC_CheckList.Instance.NetworkPlayerPilot.GetComponent<SC_Net_Player_TutoState>().CmdChangeTutoState(SC_GameStates.TutorialState.Tutorial1_6);
            ongletContainer.MoveIn();
            animator.SetBool("ActivateMove", true);
        }

        ongletContainer.ChangeWindow(newWindow);
        #region AnimIn&Out
        
        if(SC_GameStates.Instance.CurState == SC_GameStates.GameState.Game)
        {
            if (index == 0)
            {
                ongletContainer.DisplayIn();
                animator.SetBool("ActivateDisplay", true);
            }
            if (index == 1)
            {
                ongletContainer.WeaponIn();
                animator.SetBool("ActivateWeapon", true);
            }
            if (index == 2)
            {
                ongletContainer.MoveIn();
                animator.SetBool("ActivateMove", true);
            }

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
        if (SC_GameStates.Instance.CurState == SC_GameStates.GameState.Game)
            IsHover();
        else
            IsHoverTuto();
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
        }

    } 
    void IsHoverTuto()
    {
        if (animator != null)
        {

            if ((index == 0 || index == 3)&& SC_GameStates.Instance.CurTutoState == SC_GameStates.TutorialState.RepairDisplay)
            {
                animator.SetBool("Hover", true);
                StartCoroutine(EndCoroutine("Hover"));
            }
            if ((index == 1 || index == 4)&& SC_GameStates.Instance.CurTutoState == SC_GameStates.TutorialState.RepairWeapon)
            {
                animator.SetBool("Hover", true);
                StartCoroutine(EndCoroutine("Hover"));
            }
            if ((index == 2 || index == 5)&& SC_GameStates.Instance.CurTutoState == SC_GameStates.TutorialState.RepairMotion)
            {
                animator.SetBool("Hover", true);
                StartCoroutine(EndCoroutine("Hover"));
            }
         

        }

    }
 

    IEnumerator EndCoroutine(string Bool)
    {
        yield return new WaitForEndOfFrame();
        animator.SetBool(Bool, false);
    }
}
