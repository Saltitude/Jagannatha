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
        if (SceneManager.GetActiveScene().name == "Pre_Lobby")
        {
            if (XRSettings.isDeviceActive)
            {
                SC_SceneManager.Instance.LoadTutoLobby();
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
    void Start()
    {



    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
