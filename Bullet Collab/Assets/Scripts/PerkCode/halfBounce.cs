/*******************************************************************************
* Name : halfBounce.cs
* Section Description : perk
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 11/25/22  0.10                 DS              Made the thing
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Perk/halfBounce")]
public class halfBounce : perkData
{
    public int addBounce = 2;
    public float addReload = 0.2f;
    public float delayMultiple = 0.8f;
    public float speedMultiple = 0.5f;

    public override void addedEvent(Dictionary<string, GameObject> objDictionary,int Count,bool initialize) {
        Entity entityStats = getEntityStats(objDictionary);

        if (entityStats){
            // Add the player stats
            entityStats.reloadTime += addReload;
            entityStats.bulletTime *= delayMultiple;
        }
    }

    public override void shootEvent(Dictionary<string, GameObject> objDictionary,int Count,bool initialize) {
        if (objDictionary.ContainsKey("Bullet")){
            // Add the Bounces
            bulletSystem bulletInfo = getBulletStats(objDictionary);
            if (bulletInfo){
                bulletInfo.bulletBounces += addBounce;
            }
        }
    }

    public override void bounceEvent(Dictionary<string, GameObject> objDictionary,int Count,bool initialize) {
        bulletSystem bulletInfo = getBulletStats(objDictionary);
        if (bulletInfo != null){
            bulletInfo.bulletSpeed *= speedMultiple;
        }
    }
}
