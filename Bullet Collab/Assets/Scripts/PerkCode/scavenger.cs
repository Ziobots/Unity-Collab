/*******************************************************************************
* Name : scavenger.cs
* Section Description : perk code
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 11/11/22  0.10                 DS              Made the thing
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Perk/scavenger")]
public class scavenger : perkData
{
    public float addReload = 0.5f;

    public override void addedEvent(Dictionary<string, GameObject> objDictionary,int Count,bool initialize) {
        Entity entityStats = getEntityStats(objDictionary);

        if (entityStats){
            // Add the player stats
            entityStats.reloadTime += addReload;
        }
    }

    public override void hitEvent(Dictionary<string, GameObject> objDictionary, int Count, bool initialize){
        Entity entityStats = getEntityStats(objDictionary);
        Entity targetStats = getTargetStats(objDictionary);

        if (entityStats != null && targetStats != null){
            entityStats.setCurrentAmmo(entityStats.maxAmmo);
        }
    }
}
