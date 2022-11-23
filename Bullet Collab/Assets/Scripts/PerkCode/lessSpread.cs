/*******************************************************************************
* Name : lessSpread.cs
* Section Description : lessSpread
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 11/23/22  0.10                 DS              Made the thing
*******************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Perk/lessSpread")]
public class lessSpread : perkData
{
    public float addDelay = 0.07f;
    public float spreadMultiple = 0.7f;

    public override void addedEvent(Dictionary<string, GameObject> objDictionary,int Count,bool initialize) {
        Entity entityStats = getEntityStats(objDictionary);

        if (entityStats){
            // Add the player stats
            entityStats.bulletTime += addDelay;
            entityStats.bulletSpread *= spreadMultiple;
        }
    }
}
