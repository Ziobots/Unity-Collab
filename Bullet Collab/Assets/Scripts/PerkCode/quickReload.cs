/*******************************************************************************
* Name : quickReload.cs
* Section Description : quickReload
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 11/23/22  0.10                 DS              Made the thing
*******************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Perk/quickReload")]
public class quickReload : perkData
{
    public float delayMultiple = 0.95f;
    public float reloadMultiple = 0.8f;

    public override void addedEvent(Dictionary<string, GameObject> objDictionary,int Count,bool initialize) {
        Entity entityStats = getEntityStats(objDictionary);

        if (entityStats){
            // Add the player stats
            entityStats.bulletTime *= delayMultiple;
            entityStats.reloadTime *= reloadMultiple;
        }
    }
}
