/*******************************************************************************
* Name : extraPerk.cs
* Section Description : perk
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 11/23/22  0.10                 DS              Made the thing
*******************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Perk/extraPerk")]
public class extraPerk : perkData
{
    public override void addedEvent(Dictionary<string, GameObject> objDictionary,int Count,bool initialize) {
        Player playerStats = getPlayerStats(objDictionary);

        if (playerStats){
            // Add the player stats
            playerStats.perkCount += 1;
        }
    }
}
