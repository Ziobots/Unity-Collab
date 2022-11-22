/*******************************************************************************
* Name : kill_enemy.cs
* Section Description : this is used for not perks but functionality
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 11/19/22  0.10                 DS              Made the thing
*******************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Perk/kill_enemy")]
public class kill_enemy : perkData
{
    public override void damagedEvent(Dictionary<string, GameObject> objDictionary,int Count,bool initialize) {
        if (initialize && objDictionary.ContainsKey("GameManager")){
            Entity ownerInfo = getEntityStats(objDictionary);
            if (ownerInfo){
                ownerInfo.currentHealth = ownerInfo.maxHealth;
                ownerInfo.gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("crystalOn");
            }

            gameLoader gameInfo = objDictionary["GameManager"].GetComponent<gameLoader>();
            GameObject levelObj = gameInfo.levelObj;
            if (levelObj != null && !gameInfo.spawnedPerks){
                levelData levelInfo = levelObj.GetComponent<levelData>();
                if (levelInfo && !levelInfo.skipPerks && levelInfo.type != RoomType.Shop){
                    objDictionary["GameManager"].GetComponent<gameLoader>().spawnPerks();
                }
            }

            gameInfo.spawnedPerks = true;
            gameInfo.showContinue();
        }
    }
}
