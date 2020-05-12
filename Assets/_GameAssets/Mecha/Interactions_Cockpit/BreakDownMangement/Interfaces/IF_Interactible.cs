using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractible
{
    void ChangeDesired();
    void Repair();
    void ForceSync();
    bool isBreakdown();

    //effectue un test de probabilité et retourne le resultat
    bool testAgainstOdds();

}
