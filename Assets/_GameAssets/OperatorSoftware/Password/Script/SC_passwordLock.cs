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
    bool secu = false;
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
            if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) //Validation
            {
                Debug.Log("Engage password");
                manager.networkAddress = objectPassword.GetComponent<Text>().text;
                unlock = true;
                b_IsConnected = true;
                secu = true;
            }

            if (countTime > 0.1f) //Fin de compteur
            {
                if(secu)
                {
                    
                    manager.StartClient();
                    secu = false;
                }
                //SC_instruct_op_manager.Instance.Deactivate(6);
                
                
            if (countTime > 3f)
            {
                Debug.Log("Connection Failed");
                failPassword();
                countTime = 0; //RaZ compteur
                unlock = false; //Sécurité
                secu = false;
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
    
    public void validatePassword()
    {
        textFeedbackFunction("Valid password", new Color32(0, 255, 0, 255)); //Feedback textuel vert
        CustomSoundManager.Instance.PlaySound(gameObject, "SFX_o_opening", false, 0.4f);
    }

    void failPassword()
    {
        textFeedbackFunction("Wrong password", new Color32(255, 0, 0, 255)); //Feedback textuel vert
        
    }
}
