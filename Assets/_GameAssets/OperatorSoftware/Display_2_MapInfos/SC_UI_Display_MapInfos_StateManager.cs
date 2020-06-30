using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_UI_Display_MapInfos_StateManager : MonoBehaviour
{

    #region Singleton

    private static SC_UI_Display_MapInfos_StateManager _instance;
    public static SC_UI_Display_MapInfos_StateManager Instance { get { return _instance; } }

    #endregion


    enum StateOfCanvas
    {
        neutral,
        koaView,
    }

    StateOfCanvas curState;

    public SC_RaycastOPMapPerspective scriptRaycast; //Sur camera Full view

    int numChild;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    void Start()
    {
        curState = StateOfCanvas.neutral;
        numChild = this.transform.childCount;
        checkState();
    }

    public void checkState()
    {
            if (scriptRaycast.objectOnclic == null || SC_UI_Display_MapInfos_KoaState.Instance.curState == SC_UI_Display_MapInfos_KoaState.KoaState.Death)
            {
                curState = StateOfCanvas.neutral;
                checkChild(0);
                SC_UI_Display_MapInfos_KoaState.Instance.activated = false;
                SC_UI_Display_Flock.Instance.desactivateRender();

                //patch de la gruge de l'infini de ta mère pour effacer le koa en memoire et permettre la discrimination des conditions
                SC_UI_Display_MapInfos_KoaState.Instance.curState = SC_UI_Display_MapInfos_KoaState.KoaState.Spawning;
                scriptRaycast.objectOnclic = null;
                if(SC_GameStates.Instance.CurState >= SC_GameStates.GameState.Tutorial2)
                {
                    SC_TargetMap.Instance.SetFont(SC_TargetMap.FontList.VoiceActivated);
                    SC_TargetMap.Instance.SetText("No Target Selected");
                }

                return;
            }


            if (scriptRaycast.objectOnclic != null)
            {
                if (scriptRaycast.objectOnclic.tag == "Koa")
                {
                    curState = StateOfCanvas.koaView;
                    checkChild(1);
                    SC_UI_Display_MapInfos_KoaState.Instance.activated = true;
                    SC_UI_Display_Flock.Instance.activateRender();

                    /* if (SC_GameStates.Instance.CurTutoState == SC_GameStates.TutorialState.StartTutorial2)
                         SC_CheckList.Instance.NetworkPlayerPilot.GetComponent<SC_Net_Player_TutoState>().CmdChangeTutoState(SC_GameStates.TutorialState.Tutorial2_2);*/
                }
            }
        
      
    }

    void checkChild(int indexToActivate)
    {
        for(int i = 0; i<numChild;i++)
        {
            if(i == indexToActivate)
            {
                activateChild(this.transform.GetChild(i));
                
            }
            else
            {
                desactivateChild(this.transform.GetChild(i));
            }
        }
 
    }
    
    void activateChild(Transform child)
    {
        child.transform.localPosition = Vector3.zero;
    }
    void desactivateChild(Transform child)
    {
        child.transform.localPosition = new Vector3(0,0,200);
        
    }

}
