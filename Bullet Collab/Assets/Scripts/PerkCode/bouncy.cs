/*******************************************************************************
* Name : bouncy.cs
* Section Description : This is a perk
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 11/11/22  0.10                 DS              Made the thing
*******************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Perk/bouncy")]
public class bouncy : perkData
{
    public int addBounces = 2;
    public float sizeMultiple = 1.15f;
    public float damageMultiple = 1.2f;
    public float addReload = 0.2f;

    public override void addedEvent(Dictionary<string, GameObject> objDictionary,int Count,bool initialize) {
        Entity entityStats = getEntityStats(objDictionary);

        if (entityStats){
            // Add the player stats
            entityStats.reloadTime += addReload;
        }
    }

    public override void shootEvent(Dictionary<string, GameObject> objDictionary,int Count,bool initialize) {
        bulletSystem bulletStats = getBulletStats(objDictionary);

        if (bulletStats){
            // Add the Damage
            bulletStats.bulletDamage *= damageMultiple;
            bulletStats.bulletSize *= sizeMultiple;
            bulletStats.bulletBounces += addBounces;
        }
    }
}
