using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_ShowTourbiLvl_OP : MonoBehaviour
{

    [Header("References")]
    [SerializeField]
    RectTransform[] tab_TorbiBar;

    [SerializeField]
    float f_MaxLenght;

    //BlinkDanceFLoor
    [SerializeField]
    SC_UI_WireBlink BlinkMaster;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateBar();
        SetWireBlink();
    }

    void UpdateBar()
    {
        for(int i = 0; i < tab_TorbiBar.Length; i++)
        {

            //On recupere le Statut du Tourbilol
            float TargetValue = SC_SyncVar_BreakdownWeapon.Instance.SL_Tourbilols[i].valueWanted;
            float CurrentValue = SC_SyncVar_BreakdownWeapon.Instance.SL_Tourbilols[i].value;

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
        for (int i = 0; i < tab_TorbiBar.Length; i++)
        {

            if (SC_SyncVar_BreakdownWeapon.Instance.SL_Tourbilols[i].isEnPanne)
            {

                //Wire
                BlinkMaster.SetBreakDown(9, true);

                switch (i)
                {

                    //Container du Haut
                    case 0:

                        BlinkMaster.SetBreakDown(0, true);

                        switch (SC_SyncVar_BreakdownWeapon.Instance.SL_Tourbilols[i].valueWanted)
                        {

                            //Gauche
                            case 1:
                                //Gods
                                BlinkMaster.SetBreakDown(3, true);
                                //BlinkMaster.SetBreakDown(4, false);
                                BlinkMaster.ShutDownWire(4, true);
                                //Wire
                                BlinkMaster.SetBreakDown(7, true);
                                break;

                            //Droite
                            case -1:
                                //Gods
                                //BlinkMaster.SetBreakDown(3, false);
                                BlinkMaster.ShutDownWire(3, true);
                                BlinkMaster.SetBreakDown(4, true);
                                //Wire
                                BlinkMaster.SetBreakDown(8, true);
                                break;

                        }

                        break;

                    //Container du Bas
                    case 1:

                        BlinkMaster.SetBreakDown(1, true);

                        switch (SC_SyncVar_BreakdownWeapon.Instance.SL_Tourbilols[i].valueWanted)
                        {

                            //Gauche
                            case 1:
                                //Gods
                                BlinkMaster.SetBreakDown(5, true);
                                //BlinkMaster.SetBreakDown(6, false);
                                BlinkMaster.ShutDownWire(6, true);
                                //Wire
                                BlinkMaster.SetBreakDown(7, true);
                                break;

                            //Droite
                            case -1:
                                //Gods
                                //BlinkMaster.SetBreakDown(5, false);
                                BlinkMaster.ShutDownWire(5, true);
                                BlinkMaster.SetBreakDown(6, true);
                                //Wire
                                BlinkMaster.SetBreakDown(8, true);
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
                        break;

                    //Container du Bas
                    case 1:
                        BlinkMaster.SetBreakDown(1, false);
                        BlinkMaster.SetBreakDown(5, false);
                        BlinkMaster.SetBreakDown(6, false);
                        BlinkMaster.ShutDownWire(5, false);
                        BlinkMaster.ShutDownWire(6, false);
                        break;

                }

                //System entier
                if(!SC_SyncVar_BreakdownWeapon.Instance.SL_Tourbilols[0].isEnPanne && !SC_SyncVar_BreakdownWeapon.Instance.SL_Tourbilols[1].isEnPanne)
                {
                    BlinkMaster.SetBreakDown(7, false);
                    BlinkMaster.SetBreakDown(8, false);
                    BlinkMaster.SetBreakDown(9, false);
                }

            }
            
        }
    }

}
