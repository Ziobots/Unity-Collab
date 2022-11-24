/*******************************************************************************
* Name : heavyBullets.cs
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

[CreateAssetMenu(menuName = "ScriptableObjects/Perk/heavyBullets")]
public class heavyBullets : perkData
{
    public float speedMultiple = 0.7f;
    public float damageMultiple = 1.3f;
    public float sizeMultiple = 1.15f;
    public float addWeight = 1.5f;

    public override void shootEvent(Dictionary<string, GameObject> objDictionary,int Count,bool initialize) {
        bulletSystem bulletStats = getBulletStats(objDictionary);

        if (bulletStats){
            // Add the Damage
            bulletStats.bulletDamage *= damageMultiple;
            bulletStats.bulletSize *= sizeMultiple;
            bulletStats.bulletSpeed *= speedMultiple;
            bulletStats.bulletWeight += addWeight;
        }
    }
}
