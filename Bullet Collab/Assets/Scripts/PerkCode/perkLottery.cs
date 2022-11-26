/*******************************************************************************
* Name : perkLottery.cs
* Section Description : chance that other perks wont disapper when chosen
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 11/2/22  0.10                 DS              Made the thing
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Perk/perkLottery")]
public class perkLottery : perkData
{
    public override void perkCollect(Dictionary<string, GameObject> objDictionary,int Count,bool initialize) {
        if (objDictionary.ContainsKey("PerkObj") && initialize){
            bool wonPerks = false;

            // do a 50% chance for # of this perk they have
            for (int i = 1; i <= Count; i++){
                if (Random.Range(1,101) <= 25){
                    wonPerks = true;
                    break;
                }
            }

            // remove the perks destroy list if they win
            if (wonPerks && objDictionary["PerkObj"] != null){
                objDictionary["PerkObj"].GetComponent<perkPickup>().perkObjList = new List<GameObject>();
            }
        }
    }
}
