using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatArea : MonoBehaviour
{
    /// <summary>
    /// Parent
    /// </summary>
    public CombatZone parentCombatZone;

    private void OnTriggerEnter(Collider other)
    {
        if (parentCombatZone != null && other.tag == "Player")
        {
            parentCombatZone.PlayerEnteredCombatZone();
            parentCombatZone.CombatArea = this.gameObject;
        }
     }
}
