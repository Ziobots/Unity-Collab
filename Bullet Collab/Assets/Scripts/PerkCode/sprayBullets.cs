/*******************************************************************************
* Name : sprayBullets.cs
* Section Description : This is an example perk to permanently increase Player Speed
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 10/23/22  0.10                 DS              Made the thing
*******************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Perk/spray")]
public class sprayBullets : perkData
{
    public int addBullets = 10;
    public float damageMultiple = 0.7f;
    public float delayMultiple = 3f;
    public float reloadMultiple = 1.25f;

    public override void addedEvent(Dictionary<string, GameObject> objDictionary,int Count,bool initialize) {
        Entity entityStats = getEntityStats(objDictionary);

        if (entityStats){
            entityStats.maxAmmo += addBullets;
            entityStats.bulletDamage *= damageMultiple;
            entityStats.bulletTime *= delayMultiple;
            entityStats.reloadTime *= reloadMultiple;
            entityStats.automaticGun = true;
        }
    }
}
