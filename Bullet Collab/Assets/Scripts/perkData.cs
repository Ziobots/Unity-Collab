/*******************************************************************************
* Name : perkData.cs
* Section Description : This is the abstract class for each perk the player can pick up.
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 10/23/22  0.10                 DS              Made the thing
* 11/03/22  0.10                 DS              Added new events + useful functions
* 11/05/22  0.10                 DS              Added get bullet system
*******************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Perk Base Info - do not edit, all perks inherit these fields
// INITIALIZE bool is used if a perk has code you only want executed once
// ex: with homing it has to calculate stuff but if it did it twice it would cause problems

public enum Rarity {Unobtainable,Common,Rare,Legendary,ShopOnly};

 [CreateAssetMenu(menuName = "ScriptableObjects/Perk/noPerk_Display")]
public class perkData : ScriptableObject
{
    public Sprite perkIcon;
    public string perkName;
    public int perkCost;
    [TextArea]
    public string perkDescription;
    public Rarity perkRarity;
    public bool stackablePerk = true;
    public bool enemyPerk = true;

    // This event fires when the player picks up a perk, use it to adjust player stats
    public virtual void addedEvent(Dictionary<string, GameObject> objDictionary,int Count,bool initialize) {}

    // This event fires when the player buys a perk, use it to adjust player stats
    public virtual void buyEvent(Dictionary<string, GameObject> objDictionary,int Count,bool initialize) {}

    // This event fires when the player shoots a bullet, use it to modify bullets when they are created
    public virtual void shootEvent(Dictionary<string, GameObject> objDictionary,int Count,bool initialize) {}

    // This event fires when the player reloads their gun
    public virtual void reloadGunEvent(Dictionary<string, GameObject> objDictionary,int Count,bool initialize) {}

    // This event fires when an entity is hit
    public virtual void hitEvent(Dictionary<string, GameObject> objDictionary,int Count,bool initialize) {}

    // This event is fired when the bullet bounces off of a surface
    public virtual void bounceEvent(Dictionary<string, GameObject> objDictionary,int Count,bool initialize) {}

    // This event is fired when the player takes direct damage, you can check the entity.damageAmount to see how much damage they took
    public virtual void damagedEvent(Dictionary<string, GameObject> objDictionary,int Count,bool initialize) {}

    // This event is fired when an entity is killed
    public virtual void killedEvent(Dictionary<string, GameObject> objDictionary,int Count,bool initialize) {}

    // This event is fired every frame that a bullet exists, use it to modify the behavior of a bullet 
    public virtual void updateBullet(Dictionary<string, GameObject> objDictionary,int Count,bool initialize) {}

    // This event is fired every frame that the player exists
    public virtual void updateEntity(Dictionary<string, GameObject> objDictionary,int Count,bool initialize) {}

    // This event is fired when a perk is collected by the player, it also lets you know which perk was picked up
    // This event fires after the added event
    public virtual void perkCollect(Dictionary<string, GameObject> objDictionary,int Count,bool initialize) {}

    // enemy based perk events

    // This event is fired when an enemy changes targets, get target or no target
    public virtual void enemy_targetChange(Dictionary<string, GameObject> objDictionary,int Count,bool initialize) {}

    /********************************************************************************************************/

    // functions for use in perks
    public Entity getEntityStats(Dictionary<string, GameObject> objDictionary){
        if (objDictionary.ContainsKey("Owner")){
            GameObject entityObj = objDictionary["Owner"];
            if (entityObj != null){
                Entity entityStats = entityObj.GetComponent<Entity>();
                return entityStats;
            }
        }

        return null;
    }

    public Player getPlayerStats(Dictionary<string, GameObject> objDictionary){
        if (objDictionary.ContainsKey("Owner")){
            GameObject entityObj = objDictionary["Owner"];
            if (entityObj != null){
                Player playerStats = entityObj.GetComponent<Player>();
                return playerStats;
            }
        }

        return null;
    }

    public Entity getTargetStats(Dictionary<string, GameObject> objDictionary){
        if (objDictionary.ContainsKey("Target")){
            GameObject entityObj = objDictionary["Target"];
            if (entityObj != null){
                Entity entityStats = entityObj.GetComponent<Entity>();
                return entityStats;
            }
        }

        return null;
    }

    public bulletSystem getBulletStats(Dictionary<string, GameObject> objDictionary){
        if (objDictionary.ContainsKey("Bullet")){
            GameObject bulletObj = objDictionary["Bullet"];
            if (bulletObj != null){
                bulletSystem bulletStats = bulletObj.GetComponent<bulletSystem>();
                return bulletStats;
            }
        }

        return null;
    }
}