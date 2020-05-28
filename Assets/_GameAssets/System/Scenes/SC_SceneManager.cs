using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class SC_SceneManager : NetworkBehaviour
{
    /*
    #region Singleton

    private static SC_SceneManager _instance;
    public static SC_SceneManager Instance { get { return _instance; } }

    #endregion*/

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
    }*/

    void Start()
    {

        IsCheck();

        if (SceneManager.GetActiveScene().buildIndex == 2 || SceneManager.GetActiveScene().buildIndex == 1)
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

        if (SceneManager.GetActiveScene().buildIndex == 2 || SceneManager.GetActiveScene().buildIndex == 1)
            LobbyUpdate();

        if (Input.GetKeyDown(KeyCode.Space) && isServer)
            RpcAllowChangeScene();

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
                _SC_PasswordLock.validatePassword();                                   
        }

    }

    IEnumerator PreLoadScene()
    {

        yield return null;

        Debug.Log("PreLoadScene ");

        AsyncOperation asyncOperation = null;

        if (SceneManager.GetActiveScene().buildIndex == 1)
            asyncOperation = SceneManager.LoadSceneAsync(3);

        else if (SceneManager.GetActiveScene().buildIndex == 2)
            asyncOperation = SceneManager.LoadSceneAsync(3);

        asyncOperation.allowSceneActivation = false;

        f_LoadingProgress = asyncOperation.progress;

        //When the load is still in progress, output the Text and progress bar
        while (!asyncOperation.isDone)
        {

            Debug.Log("notDone");

            //Output the current progress (In Inspector)
            f_LoadingProgress = asyncOperation.progress;

            // Check if the load has finished
            if (asyncOperation.progress >= 0.9f && n_ConnectionsCount >= 2)
            {

                if (SceneManager.GetActiveScene().buildIndex == 1 && !b_PilotReadyToLoad)
                    b_PilotReadyToLoad = true;

                else if (SceneManager.GetActiveScene().buildIndex == 2 && !b_OperatorReadyToLoad)
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
        Mng_CheckList.GetComponent<SC_CheckList>().NetworkPlayerOperator.GetComponent<SC_NetScene>().CmdSendReadyOP();
    }

    [ClientRpc]
    void RpcAllowChangeScene()
    {
        b_LoadingAllowed = true;
    }

}
