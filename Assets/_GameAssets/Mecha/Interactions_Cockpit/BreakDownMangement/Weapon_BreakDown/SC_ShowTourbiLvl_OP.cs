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

            float TargetValue = SC_SyncVar_BreakdownWeapon.Instance.SL_Tourbilols[i].valueWanted;
            float CurrentValue = SC_SyncVar_BreakdownWeapon.Instance.SL_Tourbilols[i].value;

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

            if (CurrentValue < 0)
                CurrentValue *= -1;

            tab_TorbiBar[i].SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, CurrentValue * f_MaxLenght);

        }
    }

    void SetWireBlink()
    {
        for (int i = 0; i < tab_TorbiBar.Length; i++)
        {

            if (SC_SyncVar_BreakdownWeapon.Instance.SL_Tourbilols[i].isEnPanne)
            {
                switch (i)
                {
                    case 0:

                        switch (SC_SyncVar_BreakdownWeapon.Instance.SL_Tourbilols[i].valueWanted)
                        {
                            case -1:

                                BlinkMaster.ShutDownWire(1, false);
                                BlinkMaster.ShutDownWire(4, false);
                                BlinkMaster.ShutDownWire(5, false);
                                BlinkMaster.ShutDownWire(6, false);
                                BlinkMaster.ShutDownWire(8, false);
                                

                                BlinkMaster.SetBreakDown(0, true);
                                BlinkMaster.SetBreakDown(3, true);
                                BlinkMaster.SetBreakDown(7, true);
                                BlinkMaster.SetBreakDown(9, true);

                                BlinkMaster.SetBreakDown(1, false);
                                BlinkMaster.SetBreakDown(4, false);
                                BlinkMaster.SetBreakDown(5, false);
                                BlinkMaster.SetBreakDown(6, false);
                                BlinkMaster.SetBreakDown(8, false);

                                BlinkMaster.ShutDownWire(1, true);
                                BlinkMaster.ShutDownWire(4, true);
                                BlinkMaster.ShutDownWire(5, true);
                                BlinkMaster.ShutDownWire(6, true);
                                BlinkMaster.ShutDownWire(8, true);



                                break;

                            case 1:

                                BlinkMaster.ShutDownWire(1, false);
                                BlinkMaster.ShutDownWire(3, false);
                                BlinkMaster.ShutDownWire(5, false);
                                BlinkMaster.ShutDownWire(6, false);
                                BlinkMaster.ShutDownWire(7, false);

                                BlinkMaster.SetBreakDown(0, true);
                                BlinkMaster.SetBreakDown(4, true);
                                BlinkMaster.SetBreakDown(8, true);
                                BlinkMaster.SetBreakDown(9, true);

                                BlinkMaster.SetBreakDown(1, false);
                                BlinkMaster.ShutDownWire(3, false);
                                BlinkMaster.SetBreakDown(5, false);
                                BlinkMaster.SetBreakDown(6, false);
                                BlinkMaster.SetBreakDown(7, false);

                                BlinkMaster.ShutDownWire(1, true);
                                BlinkMaster.ShutDownWire(3, true);
                                BlinkMaster.ShutDownWire(5, true);
                                BlinkMaster.ShutDownWire(6, true);
                                BlinkMaster.ShutDownWire(7, true);

                                break;

                        }

                        break;

                    case 1:

                        switch (SC_SyncVar_BreakdownWeapon.Instance.SL_Tourbilols[i].valueWanted)
                        {
                            case -1:


                                BlinkMaster.ShutDownWire(0, false);
                                BlinkMaster.ShutDownWire(3, false);
                                BlinkMaster.ShutDownWire(4, false);
                                BlinkMaster.ShutDownWire(6, false);
                                BlinkMaster.ShutDownWire(8, false);


                                BlinkMaster.SetBreakDown(1, true);
                                BlinkMaster.SetBreakDown(5, true);
                                BlinkMaster.SetBreakDown(7, true);
                                BlinkMaster.SetBreakDown(9, true);

                                BlinkMaster.SetBreakDown(0, false);
                                BlinkMaster.SetBreakDown(3, false);
                                BlinkMaster.SetBreakDown(4, false);
                                BlinkMaster.SetBreakDown(6, false);
                                BlinkMaster.SetBreakDown(8, false);

                                BlinkMaster.ShutDownWire(0, true);
                                BlinkMaster.ShutDownWire(3, true);
                                BlinkMaster.ShutDownWire(4, true);
                                BlinkMaster.ShutDownWire(6, true);
                                BlinkMaster.ShutDownWire(8, true);

                                break;

                            case 1:

                                BlinkMaster.ShutDownWire(0, false);
                                BlinkMaster.ShutDownWire(3, false);
                                BlinkMaster.ShutDownWire(4, false);
                                BlinkMaster.ShutDownWire(5, false);
                                BlinkMaster.ShutDownWire(7, false);


                                BlinkMaster.SetBreakDown(1, true);
                                BlinkMaster.SetBreakDown(6, true);
                                BlinkMaster.SetBreakDown(8, true);
                                BlinkMaster.SetBreakDown(9, true);

                                BlinkMaster.SetBreakDown(0, false);
                                BlinkMaster.SetBreakDown(3, false);
                                BlinkMaster.SetBreakDown(4, false);
                                BlinkMaster.SetBreakDown(5, false);
                                BlinkMaster.SetBreakDown(7, false);

                                BlinkMaster.ShutDownWire(0, true);
                                BlinkMaster.ShutDownWire(3, true);
                                BlinkMaster.ShutDownWire(4, true);
                                BlinkMaster.ShutDownWire(5, true);
                                BlinkMaster.ShutDownWire(7, true);

                                break;

                        }

                        break;
                }
              
            }
            else
            {
                BlinkMaster.ShutDownWire(0, false);
                BlinkMaster.ShutDownWire(1, false);
                BlinkMaster.ShutDownWire(2, false);
                BlinkMaster.ShutDownWire(3, false);
                BlinkMaster.ShutDownWire(4, false);
                BlinkMaster.ShutDownWire(5, false);
                BlinkMaster.ShutDownWire(6, false);
                BlinkMaster.ShutDownWire(7, false);
                BlinkMaster.ShutDownWire(8, false);
                BlinkMaster.ShutDownWire(9, false);
            }
            
        }
    }

}
