/*******************************************************************************
* Name : noMelee.cs
* Section Description : perk code
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 11/23/22  0.10                 DS              Made the thing
*******************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Perk/noMelee")]
public class noMelee : perkData
{
    public float addMelee = 0.5f;

    public override void addedEvent(Dictionary<string, GameObject> objDictionary, int Count, bool initialize){
        Entity entityInfo = getEntityStats(objDictionary);
        if (entityInfo){
            entityInfo.meleeDamage += (addMelee * Count);
        }
    }
}
