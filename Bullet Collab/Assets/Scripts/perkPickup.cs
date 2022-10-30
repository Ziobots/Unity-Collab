/*******************************************************************************
* Name : perkPickup.cs
* Section Description : 
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 10/23/22  0.10                 DS              Made the thing
*******************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class perkPickup : MonoBehaviour
{
    public perkData perk;

    public void onPickup(GameObject player) {
        if (perk != null) {
            //perk.loadPerk(player,1);
        }
    }
}
