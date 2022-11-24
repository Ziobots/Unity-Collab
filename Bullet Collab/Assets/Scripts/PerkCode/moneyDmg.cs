/*******************************************************************************
* Name : moneyDmg.cs
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

[CreateAssetMenu(menuName = "ScriptableObjects/Perk/moneyDmg")]
public class moneyDmg : perkData
{
    public int addMoney = 15;

    public override void damagedEvent(Dictionary<string, GameObject> objDictionary,int Count,bool initialize) {
        Entity entityStats = getEntityStats(objDictionary);

        if (entityStats){
            entityStats.currency += addMoney;      
        }
    }
}
