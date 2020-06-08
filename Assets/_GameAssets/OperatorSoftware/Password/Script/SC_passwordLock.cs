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
    InputField Field01;
    [SerializeField]
    Text objectPassword; //Contient le texte du inputField
    [SerializeField]
    InputField Field02;
    [SerializeField]
    Text objectPassword02;
    [SerializeField]
    InputField Field03;
    [SerializeField]
    Text objectPassword03;
    [SerializeField]
    InputField Field04;
    [SerializeField]
    Text objectPassword04;

    private bool FieldSecu01 = false;
    private bool FieldSecu02 = false;
    private bool FieldSecu03 = false;
    private bool FieldSecu04 = false;

    //string s_password = "LV426"; //Mot de passe à taper

    [SerializeField]
    GameObject canvasMng; //récupération du DisplayManager

    [SerializeField]
    Text textFeedback; //Texte de feedback 

    [SerializeField]
    NetworkManager manager;

    float countTime = 0; //Compteur 
    public bool unlock = false; //Sécurité
    public bool b_IsConnected = false;
    [SerializeField]
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

        CheckFocus();

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) //Validation
        {

            // Debug.Log("Engage password");
            manager.networkAddress = objectPassword.text + "." + objectPassword02.text + "." + objectPassword03.text + "." + objectPassword04.text;

            unlock = true;
            b_IsConnected = true;
            secu = true;

        }

        if (countTime > 0.001f) //Fin de compteur
        {

            if(secu)
            {
                manager.StartClient();
                secu = false;
            }

            else if (countTime > 2f && SC_SceneManager.Instance.n_ConnectionsCount < 2)
            {
                //Debug.Log("Connection Failed");
                
                failPassword();
                countTime = 0; //RaZ compteur
                unlock = false; //Sécurité
                secu = false;
                manager.StopClient();
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
        textFeedbackFunction("Valid address", new Color32(0, 255, 0, 255)); //Feedback textuel vert
        CustomSoundManager.Instance.PlaySound(gameObject, "SFX_o_opening", false, 0.4f);
    }

    void failPassword()
    {
        textFeedbackFunction("Wrong address", new Color32(255, 0, 0, 255)); //Feedback textuel vert
        
    }

    void CheckFocus()
    {

        //
        if (Field01.isFocused && Input.GetKeyDown(KeyCode.Tab) && Input.GetKey(KeyCode.LeftShift))
        {
            FieldSecu01 = true;
            Field04.Select();
        }

        if (Field01.isFocused && objectPassword.text.Length < 3 && FieldSecu01)
            FieldSecu01 = false;

        if ( ( Field01.isFocused && ( ( objectPassword.text.Length >= 3 && !FieldSecu01 ) || ( Input.GetKeyDown(KeyCode.Tab) && !Input.GetKey(KeyCode.LeftShift) ) ) ) )
        {
            FieldSecu01 = true;
            Field02.Select();
        }

        //
        if(Field02.isFocused && ((Input.GetKeyDown(KeyCode.Tab) && Input.GetKey(KeyCode.LeftShift)) || (objectPassword02.text.Length == 0 && Input.GetKeyDown(KeyCode.Backspace))))
        {
            FieldSecu02 = true;
            Field01.Select();
        }

        if (Field02.isFocused && objectPassword02.text.Length < 3 && FieldSecu01)
            FieldSecu02 = false;

        if (Field02.isFocused && ((objectPassword02.text.Length >= 3 && !FieldSecu02) || (Input.GetKeyDown(KeyCode.Tab) && !Input.GetKey(KeyCode.LeftShift))))
        {
            FieldSecu02 = true;
            Field03.Select();
        }

        //
        if (Field03.isFocused && ((Input.GetKeyDown(KeyCode.Tab) && Input.GetKey(KeyCode.LeftShift)) || (objectPassword03.text.Length == 0 && Input.GetKeyDown(KeyCode.Backspace))))
        {
            FieldSecu03 = true;
            Field02.Select();
        }

        if (Field03.isFocused && objectPassword03.text.Length < 3 && FieldSecu03)
            FieldSecu03 = false;

        if (Field03.isFocused && ((objectPassword03.text.Length >= 3 && !FieldSecu03) || (Input.GetKeyDown(KeyCode.Tab) && !Input.GetKey(KeyCode.LeftShift))))
        {
            FieldSecu03 = true;
            Field04.Select();
        }

        //
        if (Field04.isFocused && ((Input.GetKeyDown(KeyCode.Tab) && Input.GetKey(KeyCode.LeftShift)) || (objectPassword04.text.Length == 0 && Input.GetKeyDown(KeyCode.Backspace))))
        {
            FieldSecu04 = true;
            Field03.Select();
            //Field03.selectionAnchorPosition.ToString(" ");
        }

        if (Field04.isFocused && Input.GetKeyDown(KeyCode.Tab))
        {
            Field01.Select();
        }

    }

}
