using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.SceneManagement;

public class SC_CheckVr : MonoBehaviour
{
    [SerializeField]
    Button FourScreen;
    [SerializeField]
    GameObject Btn4;
    [SerializeField]
    GameObject Btn1;
    // Start is called before the first frame update
    private void Awake()
    {

    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "Pre_Lobby")
        {
            if (XRSettings.isDeviceActive)
            {
                LoadTutoLobby();
            }
            else
            {
                Btn4.SetActive(true);
                Btn1.SetActive(true);
                if (Display.displays.Length != 4)
                {
                    FourScreen.interactable = false;
                }
            }
        }
    }
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
}
