using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IF_SyncVar_Sliders
{

    void SwitchChangeValue(int index, bool newValue);

    void SwitchChangeValueWanted(int index, bool newValue);

    void SwitchChangeIsPanne(int index, bool newValue);

}
