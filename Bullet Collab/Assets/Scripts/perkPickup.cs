using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class perkPickup : MonoBehaviour
{
    public perkData perk;

    public void onPickup(GameObject player) {
        if (perk != null) {
            perk.loadPerk(player,1);
        }
    }
}
