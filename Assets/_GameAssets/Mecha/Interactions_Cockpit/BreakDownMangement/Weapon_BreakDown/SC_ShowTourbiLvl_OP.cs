using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SC_ShowTourbiLvl_OP : MonoBehaviour
{

    [Header("References")]
    [SerializeField]
    RectTransform[] tab_TorbiBar;

    [SerializeField]
    float f_MaxLenght;

    //BlinkDanceFLoor
    SC_ShowTourbiLvl_OP BlinkMaster;

    bool secu1_1 = false;
    bool secu1_2 = false;
    bool secu2_1 = false;
    bool secu2_2 = false;

    /// Blink Part ///
    bool[] IndexToActivate;
    [SerializeField]
    Image[] img_ToBreakDown;
    Material[] wireSafe;
    [SerializeField]
    Material wireBreakdown;
    [SerializeField]
    Material wireShutdown;


    bool LeftGods = false;
    bool RightGods = false;

    [SerializeField]
    GameObject sparkleUmTara;
    [SerializeField]
    GameObject sparkleChenRezig;
    void Start()
    {

        BlinkMaster = this;

        wireSafe = new Material[img_ToBreakDown.Length];
        IndexToActivate = new bool[img_ToBreakDown.Length];
        for (int i = 0; i < wireSafe.Length; i++)
        {
            wireSafe[i] = img_ToBreakDown[i].material;
           
        }

        StartCoroutine(RedWireCoro());
    }

    // Update is called once per frame
    void Update()
    {
        UpdateBar();
        //SetWireBlink();
    }

    void UpdateBar()
    {
        for(int i = 0; i < tab_TorbiBar.Length; i++)
        {

            //On recupere le Statut du Tourbilol
            float TargetValue = SC_SyncVar_BreakdownWeapon.Instance.SL_Tourbilols[i].valueWanted;
            float CurrentValue = SC_SyncVar_BreakdownWeapon.Instance.SL_Tourbilols[i].value;

            switch(i)
            {
                case 0:

                    if (TargetValue != CurrentValue && !secu1_1)
                    {
                        secu1_1 = true;
                        secu2_1 = false;
                        SetWireBlink();
                    }
                    if (TargetValue == CurrentValue && !secu2_1)
                    {
                        secu2_1 = true;
                        secu1_1 = false;
                        SetWireBlink();
                    }

                    break;

                case 1:

                    if (TargetValue != CurrentValue && !secu1_2)
                    {
                        secu1_2 = true;
                        secu2_2 = false;
                        SetWireBlink();
                    }
                    if (TargetValue == CurrentValue && !secu2_2)
                    {
                        secu2_2 = true;
                        secu1_2 = false;
                        SetWireBlink();
                    }

                    break;
            }
    

            //Repositionnement selon la valeur actuelle pour pouvoir Scale dans la direction souhaité
            if (CurrentValue >= 0)
            {
                tab_TorbiBar[i].pivot = new Vector2(1, 0.5f);
                tab_TorbiBar[i].localPosition = new Vector2(0, tab_TorbiBar[i].localPosition.y);
            }       
            else
            {
                tab_TorbiBar[i].pivot = new Vector2(0, 0.5f);
                tab_TorbiBar[i].localPosition = new Vector2(0, tab_TorbiBar[i].localPosition.y);
            }

            //On garde une valeur positive
            if (CurrentValue < 0)
                CurrentValue *= -1;

            //On rescale la barre selon cette valeur
            tab_TorbiBar[i].SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, CurrentValue * f_MaxLenght);

        }
    }

    void SetWireBlink()
    {


        int BreakLvl = 0;

        

        for (int i = 0; i < tab_TorbiBar.Length; i++)
        {

            if (SC_SyncVar_BreakdownWeapon.Instance.SL_Tourbilols[i].isEnPanne)
            {

                BreakLvl++;

                //Wire
                BlinkMaster.SetBreakDown(9, true);

                if (SC_SyncVar_BreakdownWeapon.Instance.SL_Tourbilols[i].isEnPanne && SC_SyncVar_BreakdownWeapon.Instance.SL_Tourbilols[i].valueWanted == 1)
                {
                    LeftGods = true;
                }
                if (SC_SyncVar_BreakdownWeapon.Instance.SL_Tourbilols[i].isEnPanne && SC_SyncVar_BreakdownWeapon.Instance.SL_Tourbilols[i].valueWanted == -1)
                {
                    RightGods = true;
                }

                if (BreakLvl >= 2)
                {
               


                    if (LeftGods) BlinkMaster.SetBreakDown(7, true); else BlinkMaster.ShutDownWire(7, true);
                    if (RightGods) BlinkMaster.SetBreakDown(8, true); else BlinkMaster.ShutDownWire(8, true);

                }
                else
                {
                    BlinkMaster.SetBreakDown(7, false);
                    BlinkMaster.SetBreakDown(8, false);
                    BlinkMaster.ShutDownWire(7, false);
                    BlinkMaster.ShutDownWire(8, false);
                }

                switch (i)
                {

                    //Container du Haut
                    case 0:

                        BlinkMaster.SetBreakDown(0, true);
                        sparkleUmTara.SetActive(false);
                        switch (SC_SyncVar_BreakdownWeapon.Instance.SL_Tourbilols[i].valueWanted)
                        {

                            //Gauche
                            case 1:
                                //Gods
                                BlinkMaster.SetBreakDown(3, true);
                                //BlinkMaster.SetBreakDown(4, false);
                                BlinkMaster.ShutDownWire(4, true);
                                //Wire
                                //LeftGods++;
                                //if (LeftGods >= 2)
                                //    BlinkMaster.ShutDownWire(8, true);
                                
                                break;

                            //Droite
                            case -1:
                                //Gods
                               
                                BlinkMaster.ShutDownWire(3, true);
                                BlinkMaster.SetBreakDown(4, true);
                                //Wire
                                //RightGods++;
                                //if (RightGods >= 2)
                                //    BlinkMaster.ShutDownWire(7, true);                                 
                                break;

                        }

                        break;

                    //Container du Bas
                    case 1:

                        
                        BlinkMaster.SetBreakDown(1, true);
                        sparkleChenRezig.SetActive(false);
                        switch (SC_SyncVar_BreakdownWeapon.Instance.SL_Tourbilols[i].valueWanted)
                        {

                            //Gauche
                            case 1:
                                //Gods
                               
                                BlinkMaster.SetBreakDown(5, true);
                                BlinkMaster.ShutDownWire(6, true);
                                //Wire
                                //LeftGods++;
                                //if (LeftGods >= 2)
                                //    BlinkMaster.ShutDownWire(8, true);                                  
                                break;

                            //Droite
                            case -1:
                                //Gods
                                BlinkMaster.ShutDownWire(5, true);
                                BlinkMaster.SetBreakDown(6, true);
                                ////Wire
                                //RightGods++;
                                //if (RightGods >= 2)
                                //    BlinkMaster.ShutDownWire(7, true);         
                                break;

                        }

                        break;

                }
              
            }

            else
            {

                switch (i)
                {

                    //Container du Haut
                    case 0:
                        BlinkMaster.SetBreakDown(0, false);    
                        BlinkMaster.SetBreakDown(3, false);
                        BlinkMaster.SetBreakDown(4, false);
                        BlinkMaster.ShutDownWire(3, false);
                        BlinkMaster.ShutDownWire(4, false);
                        sparkleUmTara.SetActive(true);
                        break;

                    //Container du Bas
                    case 1:
                       
                        BlinkMaster.SetBreakDown(1, false);    
                        BlinkMaster.SetBreakDown(5, false);
                        BlinkMaster.SetBreakDown(6, false);
                        BlinkMaster.ShutDownWire(5, false);
                        BlinkMaster.ShutDownWire(6, false);
                        sparkleChenRezig.SetActive(true);
                        break;

                }

                //System entier
                if(!SC_SyncVar_BreakdownWeapon.Instance.SL_Tourbilols[0].isEnPanne && !SC_SyncVar_BreakdownWeapon.Instance.SL_Tourbilols[1].isEnPanne)
                {
                    BlinkMaster.SetBreakDown(7, false);
                    BlinkMaster.SetBreakDown(8, false);
                    BlinkMaster.SetBreakDown(9, false);
                    BlinkMaster.ShutDownWire(7, false);
                    BlinkMaster.ShutDownWire(8, false);
                    LeftGods = false;
                    RightGods = false;
                }

            }
            
        }
    }
    #region Blink
    //////////////////////---BLINK---//////////////////////////////
    void SetBreakDown(int index, bool activate)
    {

        if (activate)
        {

            if (!IndexToActivate[index])
            {
                img_ToBreakDown[index].material = wireBreakdown;
            }

        }
        else
        {
            if (IndexToActivate[index])
                EndCoroutine(index);

        }
        IndexToActivate[index] = activate;

    }
    void ShutDownWire(int index, bool activate)
    {
        if (activate)
            img_ToBreakDown[index].material = wireShutdown;
        else
            img_ToBreakDown[index].material = wireSafe[index];


    }


    void EndCoroutine(int index)
    {
        img_ToBreakDown[index].material = wireSafe[index];
        img_ToBreakDown[index].color = Color.white;
    }


    IEnumerator RedWireCoro()
    {
        float animTime = 0.5f;
        float maxOpacity = 1;
        float minOpacity = 0f;
        float ratePerSec = (maxOpacity - minOpacity / animTime) * 2;
        float curOpacity;
        bool Add = true;
        float t = 0;

        Vector4 ColorTampon = Color.white;
        curOpacity = minOpacity;

        while (true)
        {
            if (t < animTime)
            {
                t += Time.deltaTime;
                if (Add)
                {

                    if (curOpacity < maxOpacity)
                        curOpacity = Mathf.Lerp(curOpacity, maxOpacity, ratePerSec * Time.deltaTime);
                }
                else
                {

                    if (curOpacity > minOpacity)
                        curOpacity = Mathf.Lerp(curOpacity, minOpacity, ratePerSec * Time.deltaTime);

                }

                for (int i = 0; i < img_ToBreakDown.Length; i++)
                {
                    if (IndexToActivate[i])
                        img_ToBreakDown[i].color = new Vector4(ColorTampon.x, ColorTampon.y, ColorTampon.z, curOpacity);
                }

            }
            else
            {
                Add = !Add;
                t = 0;
            }
            yield return 0;
        }

    }
    #endregion
}
