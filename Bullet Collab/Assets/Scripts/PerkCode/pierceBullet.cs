/*******************************************************************************
* Name : pierceBullet.cs
* Section Description : pierceBullet
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 11/23/22  0.10                 DS              Made the thing
*******************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Perk/pierceBullet")]
public class pierceBullet : perkData
{
    public int addPierce = 3;
    public int subAmmo = 1;
    public float addReload = 0.15f;
    public float damageMultiple = 0.8f;

    public override void addedEvent(Dictionary<string, GameObject> objDictionary, int Count, bool initialize){
        Entity entityInfo = getEntityStats(objDictionary);
        if (entityInfo){
            entityInfo.maxAmmo -= subAmmo;
        }
    }

    public override void shootEvent(Dictionary<string, GameObject> objDictionary,int Count,bool initialize) {
        bulletSystem bulletStats = getBulletStats(objDictionary);

        if (bulletStats){
            // Add the stats
            bulletStats.bulletDamage *= damageMultiple;
            bulletStats.bulletPierce += addPierce;
        }
    }
}
