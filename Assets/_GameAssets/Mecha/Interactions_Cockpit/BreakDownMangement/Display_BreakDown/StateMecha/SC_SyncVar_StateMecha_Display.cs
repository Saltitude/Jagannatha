using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SC_SyncVar_StateMecha_Display : NetworkBehaviour
{
    
    public int nbDisplay = 13;

    //bool[] display;

    public SyncListBool displayAll = new SyncListBool();

    // Start is called before the first frame update
    void Start()
    {
        if (isServer)
        {
            for (int i = 0; i < nbDisplay; i++)
            {
                displayAll.Insert(i, false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Keypad1))
        {
            randomDisplayDesactivate();
        }
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            reactivateAllDispay();
        }
    }

    void reactivateAllDispay()
    {

        for (int i = 0; i < nbDisplay; i++)
        {
            displayAll[i] = false;
        }
    }

    void randomDisplayDesactivate()
    {
        float randomDisplay = Random.Range(0, nbDisplay);
        int randInt = (int)Mathf.Round(randomDisplay);

        if(displayAll[randInt] == false)
        {
            displayAll[randInt] = true;
        }
        else
        {
            randomDisplay = Random.Range(0, nbDisplay);
            randInt = (int)Mathf.Round(randomDisplay);
        }
        
    }
}
