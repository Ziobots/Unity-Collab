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
        Debug.Log("crystal event");
        if (initialize && objDictionary.ContainsKey("GameManager")){
            Entity ownerInfo = getEntityStats(objDictionary);
            if (ownerInfo){
                ownerInfo.currentHealth = ownerInfo.maxHealth;
                ownerInfo.gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("crystalOn");
            }

            objDictionary["GameManager"].GetComponent<gameLoader>().showContinue();
        }
    }
}
