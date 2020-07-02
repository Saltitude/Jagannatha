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

    [SerializeField]
    GameObject[] BtnReturnComponent;

    // Start is called before the first frame update
    void Start()
    {
        ongletContainer = SC_UI_OngletContainer.Instance;
        if (animated != null)
            animator = animated.GetComponent<Animator>();
        if (additionalAnimated != null)
            additionalAnimator = additionalAnimated.GetComponent<Animator>();
        wireBlink = GetComponentInParent<SC_UI_WireBlink>();
#if UNITY_EDITOR
        if (index == 3 || index == 4 || index == 5)
        {
            this.GetComponent<BoxCollider>().center = new Vector3(-11.49976f, 4.058487f, -22.14557f);
            this.GetComponent<BoxCollider>().size = new Vector3(162.1187f, 163.5854f, 54.29114f);
        }

#endif

    }


    public void Action()
    {
        SC_UI_OngletContainer.Window newWindow = (SC_UI_OngletContainer.Window)index;

        if (SC_GameStates.Instance.CurState != SC_GameStates.GameState.Tutorial)
            ActionGame();
        else
            ActionTuto();


        ongletContainer.ChangeWindow(newWindow);

    }

    void ActionGame()
    {

        if (ongletContainer.canGoBack)
        {
            //ONGLET IN
            if (index == 0)
            {
                CustomSoundManager.Instance.PlaySound(gameObject, "SFX_o_clickOnglet", false, 1f);
                ongletContainer.DisplayIn();
                animator.SetBool("ActivateDisplay", true);
                ongletContainer.SetBack(false);
                ongletContainer.SetBack(true);



            }
            if (index == 1)
            {
                CustomSoundManager.Instance.PlaySound(gameObject, "SFX_o_clickOnglet", false, 1f);
                ongletContainer.WeaponIn();
                animator.SetBool("ActivateWeapon", true);
                ongletContainer.SetBack(false);
                ongletContainer.SetBack(true);


            }
            if (index == 2)
            {
                CustomSoundManager.Instance.PlaySound(gameObject, "SFX_o_clickOnglet", false, 1f);
                ongletContainer.MoveIn();
                animator.SetBool("ActivateMove", true);
                ongletContainer.SetBack(false);
                ongletContainer.SetBack(true);

            }

            //ONGLET OUT
            if (index == 3)
            {
                CustomSoundManager.Instance.PlaySound(gameObject, "SFX_o_clickReturn", false, 1f);
                ongletContainer.DisplayOut();
                additionalAnimator.SetBool("ActivateDisplay", false);
                ongletContainer.SetBack(false);
                ongletContainer.SetBack(true);


            }
            if (index == 4)
            {
                CustomSoundManager.Instance.PlaySound(gameObject, "SFX_o_clickReturn", false, 1f);
                ongletContainer.WeaponOut();
                additionalAnimator.SetBool("ActivateWeapon", false);
                ongletContainer.SetBack(false);
                ongletContainer.SetBack(true);



            }
            if (index == 5)
            {
                CustomSoundManager.Instance.PlaySound(gameObject, "SFX_o_clickReturn", false, 1f);
                ongletContainer.MoveOut();
                additionalAnimator.SetBool("ActivateMove", false);
                ongletContainer.SetBack(false);
                ongletContainer.SetBack(true);



            }
        }



    }
    void ActionTuto()
    {
        if (ongletContainer.canGoBack)
        {
            if (index == 0 && (int)SC_GameStates.Instance.CurTutoState >= (int)SC_GameStates.TutorialState.StartRepairDisplay)
            {
                CustomSoundManager.Instance.PlaySound(gameObject, "SFX_o_clickOnglet", false, 1f);
                ongletContainer.DisplayIn();
                animator.SetBool("ActivateDisplay", true);

                ongletContainer.SetBack(false);
                ongletContainer.SetBack(true);
            }
            else if (index == 1 && (int)SC_GameStates.Instance.CurTutoState >= (int)SC_GameStates.TutorialState.StartRepairWeapon)
            {
                CustomSoundManager.Instance.PlaySound(gameObject, "SFX_o_clickOnglet", false, 1f);
                ongletContainer.WeaponIn();
                animator.SetBool("ActivateWeapon", true);

                ongletContainer.SetBack(false);
                ongletContainer.SetBack(true);
            }
            else if (index == 2 && (int)SC_GameStates.Instance.CurTutoState >= (int)SC_GameStates.TutorialState.StartRepairMotion)
            {
                CustomSoundManager.Instance.PlaySound(gameObject, "SFX_o_clickOnglet", false, 1f);
                ongletContainer.MoveIn();
                animator.SetBool("ActivateMove", true);

                ongletContainer.SetBack(false);
                ongletContainer.SetBack(true);
            }

            //ONGLET OUT
            if (index == 3 && (int)SC_GameStates.Instance.CurTutoState >= (int)SC_GameStates.TutorialState.StartRepairWeapon)
            {
                CustomSoundManager.Instance.PlaySound(gameObject, "SFX_o_clickReturn", false, 1f);
                SC_Weapon_MechState.Instance.UpdateVar();
                ongletContainer.DisplayOut();
                additionalAnimator.SetBool("ActivateDisplay", false);

                ongletContainer.SetBack(false);
                ongletContainer.SetBack(true);
            }
            if (index == 4 && (int)SC_GameStates.Instance.CurTutoState >= (int)SC_GameStates.TutorialState.StartRepairMotion)
            {
                CustomSoundManager.Instance.PlaySound(gameObject, "SFX_o_clickReturn", false, 1f);
                SC_Movement_MechState.Instance.UpdateVar();
                ongletContainer.WeaponOut();
                additionalAnimator.SetBool("ActivateWeapon", false);

                ongletContainer.SetBack(false);
                ongletContainer.SetBack(true);
            }
            if (index == 5 && (int)SC_GameStates.Instance.CurTutoState >= (int)SC_GameStates.TutorialState.Reboot)
            {
                CustomSoundManager.Instance.PlaySound(gameObject, "SFX_o_clickReturn", false, 1f);
                ongletContainer.MoveOut();
                additionalAnimator.SetBool("ActivateMove", false);

                ongletContainer.SetBack(false);
                ongletContainer.SetBack(true);
            }
        }
    }

    public void HoverAction()
    {
        if (SC_GameStates.Instance.CurState != SC_GameStates.GameState.Tutorial)
            IsHover();
        else
            IsHoverTuto();
    }


    public void isBreakdownSystem(bool state)
    {
        if (SC_GameStates.Instance.CurState == SC_GameStates.GameState.Game)
        {
            for (int i = 0; i < wireIndex.Length; i++)
            {
                wireBlink.SetBreakDown(wireIndex[i], state);
            }
            CustomSoundManager.Instance.PlaySound(gameObject, "SFX_o_panneSoundOpe", false, 1f);
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
            if (index == 0 && (int)SC_GameStates.Instance.CurTutoState >= (int)SC_GameStates.TutorialState.StartRepairDisplay)
            {
                animator.SetBool("Hover", true);
                StartCoroutine(EndCoroutine("Hover"));
            }
            if (index == 3 && (int)SC_GameStates.Instance.CurTutoState >= (int)SC_GameStates.TutorialState.EndRepairDisplay)
            {
                animator.SetBool("Hover", true);
                StartCoroutine(EndCoroutine("Hover"));
            }

            if (index == 1 && (int)SC_GameStates.Instance.CurTutoState >= (int)SC_GameStates.TutorialState.StartRepairWeapon)
            {
                animator.SetBool("Hover", true);
                StartCoroutine(EndCoroutine("Hover"));
            }
            if (index == 4 && (int)SC_GameStates.Instance.CurTutoState >= (int)SC_GameStates.TutorialState.EndRepairWeapon)
            {
                animator.SetBool("Hover", true);
                StartCoroutine(EndCoroutine("Hover"));
            }
            if (index == 2 && (int)SC_GameStates.Instance.CurTutoState >= (int)SC_GameStates.TutorialState.StartRepairMotion)
            {
                animator.SetBool("Hover", true);
                StartCoroutine(EndCoroutine("Hover"));
            }
            if (index == 5 && (int)SC_GameStates.Instance.CurTutoState >= (int)SC_GameStates.TutorialState.EndRepairMotion)
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
