/*******************************************************************************
* Name : glassCannon.cs
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

[CreateAssetMenu(menuName = "ScriptableObjects/Perk/glassCannon")]
public class glassCannon : perkData
{
    public float reloadMultiple = 0.7f;
    public float damageMultiple = 3f;
    public float sizeMultiple = 2f;

    public override void addedEvent(Dictionary<string, GameObject> objDictionary,int Count,bool initialize) {
        Entity entityStats = getEntityStats(objDictionary);

        if (entityStats){
            // Add the player stats
            entityStats.reloadTime *= reloadMultiple;
            entityStats.maxHealth = 1;
            entityStats.currentHealth = 1;
        }
    }

    public override void shootEvent(Dictionary<string, GameObject> objDictionary,int Count,bool initialize) {
        bulletSystem bulletStats = getBulletStats(objDictionary);

        if (bulletStats){
            // Add the Damage
            bulletStats.bulletDamage *= damageMultiple;
            bulletStats.bulletSize *= sizeMultiple;
        }
    }
}
