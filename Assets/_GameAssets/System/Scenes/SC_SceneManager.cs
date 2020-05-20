using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class SC_SceneManager : NetworkBehaviour
{

    GameObject Mng_CheckList = null;

    [SyncVar]
    public int n_ConnectionsCount = 0;

    float countTime = 0;

    [SerializeField]
    SC_passwordLock _SC_PasswordLock;
    // Start is called before the first frame update
    void Start()
    {
        IsCheck();
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
            if (!isServer)
            {
                //_SC_PasswordLock.b_IsConnected = true;
                LoadTutoOperator();

                if (_SC_PasswordLock != null)
                {
                    _SC_PasswordLock.validatePassword();
                        
                }
            }
                

            //si server on invoke le chargement de la scène pilote (la scène opérateur necessitant de se lancer en première)
            //sécurisé par b_hasInvoked pour n'être appelé qu'une fois
            if (isServer)
            {
                LoadTutoPilot();
            }

        }

    }

    void LoadTutoLobby()
    {
        SceneManager.LoadScene(1);
    }
    void LoadTutoLobbyOpe()
    {
        SceneManager.LoadScene(2);
        //SC_NetPlayerInit_OP.Instance.CmdSendForceUpdate();
        Mng_CheckList.GetComponent<SC_CheckList>().NetworkPlayerOperator.GetComponent<SC_NetPlayerInit_OP>().CmdSendForceUpdate();


    }

    void LoadTutoPilot()
    {

        SceneManager.LoadScene(3);
       

    }

    void LoadTutoOperator()
    {
        SceneManager.LoadScene(4);
    }

    //void LoadGamePilot()
    //{
    //    SceneManager.LoadScene(4);
    //}

    //void LoadGameOperator()
    //{
    //    SceneManager.LoadScene(5);
    //}

}
