/*******************************************************************************
* Name : barrage.cs
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

[CreateAssetMenu(menuName = "ScriptableObjects/Perk/barrage")]
public class barrage : perkData
{
    public int addFireCount = 2;
    public int addAmmo = 3;
    public float addSpread = 3f;
    public float addReload = 0.4f;

    public float sizeMultiple = 0.85f;
    public float damageMultiple = 0.85f;

    public override void addedEvent(Dictionary<string, GameObject> objDictionary,int Count,bool initialize) {
        Entity entityStats = getEntityStats(objDictionary);

        if (entityStats){
            // Add the player stats
            entityStats.fireCount += addFireCount;
            entityStats.reloadTime += addReload;
            entityStats.bulletSpread += addSpread;
            entityStats.maxAmmo += addAmmo;
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
