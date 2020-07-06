using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class SC_SceneManager : NetworkBehaviour
{
    
    #region Singleton

    private static SC_SceneManager _instance;
    public static SC_SceneManager Instance { get { return _instance; } }

    #endregion

    GameObject Mng_CheckList = null;

    [SyncVar]
    public int n_ConnectionsCount = 0;
    float countTime = 0;

    [SerializeField, Range(0, 1)]
    float f_LoadingProgress;
    [SyncVar]
    public bool b_PilotReadyToLoad = false;
    [SyncVar]
    public bool b_OperatorReadyToLoad = false;
    [SyncVar]
    public bool b_LoadingAllowed = false;

    [SerializeField]
    SC_passwordLock _SC_PasswordLock;

    /*
    [SerializeField]
    Scene LobbyPilot;
    [SerializeField]
    Scene LobbyOpe;
    [SerializeField]
    Scene GamePilot;
    [SerializeField]
    Scene GameOpe;
    */

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

        IsCheck();

        if (SceneManager.GetActiveScene().name == "Lobby" || SceneManager.GetActiveScene().name == "Lobby Opé" || SceneManager.GetActiveScene().name == "Lobby Opé 1 screen")
            StartCoroutine(PreLoadScene());

    }

    void IsCheck()
    {
        Mng_CheckList = GameObject.FindGameObjectWithTag("Mng_CheckList");
        Mng_CheckList.GetComponent<SC_CheckList>().Mng_Scene = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Lobby Opé" || SceneManager.GetActiveScene().name == "Lobby" || SceneManager.GetActiveScene().name == "Lobby Opé 1 screen")
            LobbyUpdate();

    }

    void LobbyUpdate()
    {

        // on lit le nombre de connexions
        if (isServer)
            n_ConnectionsCount = NetworkServer.connections.Count;

        //si les deux joueurs sont connectés
        if (n_ConnectionsCount >= 2)
        {

            //si pas Server on load la scène opérateur
            if (!isServer && _SC_PasswordLock != null)
            {

                if (SC_DeviceManager.Instance != null)
                    SC_DeviceManager.Instance.GetJoyStickToUse();

                _SC_PasswordLock.validatePassword();

            }                        

            if (isServer && b_OperatorReadyToLoad && b_PilotReadyToLoad && !b_LoadingAllowed)
                RpcAllowChangeScene();

        }

    }

    IEnumerator PreLoadScene()
    {

        yield return null;

        //Debug.Log("PreLoadScene ");

        AsyncOperation asyncOperation = null;

        /*
        if (SceneManager.GetActiveScene().buildIndex == 1)
            asyncOperation = SceneManager.LoadSceneAsync(3);

        else if (SceneManager.GetActiveScene().buildIndex == 2)
            asyncOperation = SceneManager.LoadSceneAsync(4);
        */

        if (SceneManager.GetActiveScene().name == "Lobby")
            asyncOperation = SceneManager.LoadSceneAsync("Tuto_Pilot");

        else if (SceneManager.GetActiveScene().name == "Lobby Opé")
            asyncOperation = SceneManager.LoadSceneAsync("Tuto_Operator");
        else if(SceneManager.GetActiveScene().name == "Lobby Opé 1 screen")
            asyncOperation = SceneManager.LoadSceneAsync("Tuto_Operator_1_screen");

        asyncOperation.allowSceneActivation = false;

        f_LoadingProgress = asyncOperation.progress;

        //When the load is still in progress, output the Text and progress bar
        while (!asyncOperation.isDone)
        {

            //Debug.Log("notDone");

            if (asyncOperation.progress < 0.9f)
                //Debug.LogError("f_LoadingProgress = " +  f_LoadingProgress);

            //Output the current progress (In Inspector)
            f_LoadingProgress = asyncOperation.progress;

            // Check if the load has finished
            if (asyncOperation.progress >= 0.9f && n_ConnectionsCount >= 2)
            {

                //Debug.LogError("Progress >= 0.9");

                if (SceneManager.GetActiveScene().name == "Lobby" && !b_PilotReadyToLoad)
                    b_PilotReadyToLoad = true;

                else if ((SceneManager.GetActiveScene().name == "Lobby Opé" || SceneManager.GetActiveScene().name == "Lobby Opé 1 screen") && !b_OperatorReadyToLoad)
                    SendReadyOP();

                //Activate the Scene
                if (b_LoadingAllowed && b_PilotReadyToLoad && b_OperatorReadyToLoad)                    
                    asyncOperation.allowSceneActivation = true;

            }

            yield return null;

        }
    }

    void SendReadyOP()
    {
        //Debug.LogError("SendReadyOP");
        Mng_CheckList.GetComponent<SC_CheckList>().NetworkPlayerPilot.GetComponent<SC_NetScene>().CmdSendReadyOP();
    }

    [ClientRpc]
    void RpcAllowChangeScene()
    {
        //Debug.LogError("RpcAllowChangeScene");
        b_LoadingAllowed = true;
    }

    #region OldMethods

    public void LoadTutoLobby()
    {
        SceneManager.LoadScene("Lobby");
    }

    public void LoadTutoLobbyOpe()
    {
        SceneManager.LoadScene("Lobby Opé");
        //SC_NetPlayerInit_OP.Instance.CmdSendForceUpdate();
        //Mng_CheckList.GetComponent<SC_CheckList>().NetworkPlayerOperator.GetComponent<SC_NetPlayerInit_OP>().CmdSendForceUpdate();
    }
    public void LoadTutoLobbyOpe1Screen()
    {
        SceneManager.LoadScene("Lobby Opé 1 screen");
        //SC_NetPlayerInit_OP.Instance.CmdSendForceUpdate();
        //Mng_CheckList.GetComponent<SC_CheckList>().NetworkPlayerOperator.GetComponent<SC_NetPlayerInit_OP>().CmdSendForceUpdate();
    }

    void LoadTutoPilot()
    {
        SceneManager.LoadScene(3);
    }

    void LoadTutoOperator()
    {
        SceneManager.LoadScene(4);
    }

    #endregion OldMethods

}
