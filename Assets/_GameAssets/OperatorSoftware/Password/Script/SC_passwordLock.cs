using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public class SC_passwordLock : MonoBehaviour
{

    #region Singleton

    private static SC_passwordLock _instance;
    public static SC_passwordLock Instance { get { return _instance; } }

    #endregion

    [SerializeField]
    GameObject objectPassword; //Contient le texte du inputField

    string s_password = "LV426"; //Mot de passe à taper

    [SerializeField]
    GameObject canvasMng; //récupération du DisplayManager

    [SerializeField]
    Text textFeedback; //Texte de feedback 

    [SerializeField]
    NetworkManager manager;

    float countTime = 0; //Compteur 
    public bool unlock = false; //Sécurité
    public bool b_IsConnected = false;

    //[SerializeField]
    //SC_electricPlug plugObject; //récupération de l'objet prise electrique

    //[SerializeField]
    //GameObject objectElectricPlug;

 
    public bool cheatCode = true;

    void Awake()
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
        //if (cheatCode)

#if UNITY_EDITOR

        //for (int i = 0; i < 4; i++)
        //{
        //    canvasMng.GetComponent<SC_CanvasManager>().activateChildInGame(i);
        //}
        //canvasMng.GetComponent<SC_CanvasManager>().checkTaskBeforeGo();
        //gameObject.SetActive(false);
        //objectElectricPlug.SetActive(false);

        //b_IsConnected = true;
        //unlock = false;
        //countTime = 0;
        //SC_CheckList.Instance.NetworkPlayerOperator.GetComponent<SC_Net_Player_TutoState>().CmdChangeTutoState(SC_GameStates.TutorialState.Tutorial1_2);

#else

        //else

        canvasMng.GetComponent<SC_CanvasManager>().lockScreenDisplay();

#endif
    }

    // Update is called once per frame
    void Update()
    {
        //if (SC_GameStates.Instance.CurState == SC_GameStates.GameState.Tutorial2)
        //    SC_instruct_op_manager.Instance.Deactivate(6);
        //if (cheatCode)
        //{
        //    for (int i = 0; i < 4; i++)
        //    {
        //        canvasMng.GetComponent<SC_CanvasManager>().activateChildInGame(i);
        //    }
        //    canvasMng.GetComponent<SC_CanvasManager>().checkTaskBeforeGo();
        //    gameObject.SetActive(false);
        //    objectElectricPlug.SetActive(false);
        //    SC_instruct_op_manager.Instance.Deactivate(6);
        //    b_IsConnected = true;
        //    unlock = false;
        //    countTime = 0;
        //    SC_CheckList.Instance.NetworkPlayerOperator.GetComponent<SC_Net_Player_TutoState>().CmdChangeTutoState(SC_GameStates.TutorialState.Tutorial1_2);
        //}
        if (string.Equals(objectPassword.GetComponent<Text>().text, s_password, System.StringComparison.CurrentCultureIgnoreCase) /*objectPassword.GetComponent<Text>().text == s_password*/) //Check du mot de passe
        {

            if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) //Validation
            {
                textFeedbackFunction("Valid password", new Color32(0, 255, 0, 255)); //Feedback textuel vert
                unlock = true; //Sécurité
                b_IsConnected = true;
                CustomSoundManager.Instance.PlaySound(gameObject, "SFX_o_opening", false, 0.4f);

              /*  plugObject.GetComponent<SC_electricPlug>().plugConnected();*/ //Animation PLay

               // SC_instruct_op_manager.Instance.Activate(1);
                //if(SC_GameStates.Instance.CurTutoState == SC_GameStates.TutorialState.Tutorial1_1)
                //    SC_CheckList.Instance.NetworkPlayerOperator.GetComponent<SC_Net_Player_TutoState>().CmdChangeTutoState(SC_GameStates.TutorialState.Tutorial1_2);
            }

            if (countTime > 4f) //Fin de compteur
            {
                //canvasMng.GetComponent<SC_CanvasManager>().checkTaskBeforeGo(); //Activation des écrans (cf distributionDisplay)
                gameObject.SetActive(false); //désactivation du canvas de mot de passe (ContainerPassword)
                //objectElectricPlug.SetActive(false);
                unlock = false; //Sécurité
                countTime = 0; //RaZ compteur
                //SC_instruct_op_manager.Instance.Deactivate(6);
                manager.StartClient();
            }

        }

        else
        {
            if (Input.GetKeyDown(KeyCode.Return)) //Mot de passe faux
            {
                textFeedbackFunction("Invalid password", new Color32(255, 0, 0, 255)); //feedback textuel rouge
            }
        }

        if(unlock)
        {
            countTime += Time.deltaTime; //Compteur ++
        }

        else
        {
            countTime = 0; //RaZ compteur
        }

    }
    /// <summary>
    /// Fonction qui permet d'agir sur un texte selon son contenu et sa couleur
    /// </summary>
    /// <param name="texte"></param>
    /// <param name="color"></param>
    void textFeedbackFunction(string texte,Color32 color)
    {
        textFeedback.color = color;
        textFeedback.text = texte;
    }
}
