using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_UI_Debug_NoBuild : MonoBehaviour
{
    [Header("Normal")]
    [Tooltip("Normal = à activer quand build")]
    [SerializeField]
    GameObject[] childNormal;

    [Header("Invert")]
    [Tooltip("Invert = à desactiver quand build")]
    [SerializeField]
    GameObject[] childInvert;

    [SerializeField]
    SC_passwordLock _SC_passwordLock;
    // Start is called before the first frame update
    void Start()
    {
        // NORMAL ----------------------------------------
        for (int i = 0; i < childNormal.Length; i++)
        {
            Desactivate(childNormal[i]);
        }
        //_SC_passwordLock.cheatCode = false;

#if UNITY_EDITOR


        for (int i = 0; i < childNormal.Length; i++)
        {
            Activate(childNormal[i]);
        }
        //_SC_passwordLock.cheatCode = true;
#endif


        //////SAME SHIT BUT INVERT --------------------------------------
        for (int i = 0; i < childInvert.Length; i++)
        {
            
            Activate(childInvert[i]);
        }
#if UNITY_EDITOR

        for (int i = 0; i < childInvert.Length; i++)
        {
            Desactivate(childInvert[i]);
        }
        //_SC_passwordLock.cheatCode = true;
#endif

        //Debug.Log(_SC_passwordLock.cheatCode);
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }

    void Activate(GameObject objet)
    {
        objet.SetActive(true);
    }

    void Desactivate(GameObject objet)
    {
        objet.SetActive(false);
    }


}
