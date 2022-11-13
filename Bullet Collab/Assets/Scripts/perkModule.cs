/*******************************************************************************
* Name : perkModule.cs
* Section Description : 
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 10/26/22  0.10                 DS              Made the thing
* 11/02/22  0.10                 DS              short list function
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class perkModule : MonoBehaviour
{
    private bool loadedPerks = false;
    private perkData[] perkObjects;
    private Dictionary<string,perkData> perkIDDictionary;

    private void loadPerkFolder(){
        if (!loadedPerks){
            loadedPerks = true;
            Object[] perkLoad = Resources.LoadAll("PerkFolder");
            perkObjects = new perkData[perkLoad.Length];
            perkLoad.CopyTo(perkObjects, 0);

            perkIDDictionary = new Dictionary<string,perkData>();
            foreach (perkData perk in perkObjects){
                if (!perkIDDictionary.ContainsKey(perk.name)){
                    perkIDDictionary.Add(perk.name,perk);
                }
            }
        }
    }

    public perkData getPerk(string perkID){
        // get all of the perks to go through

        // Only load perks onces, optimization, before it loaded them everytime lagging the game when called too often
        loadPerkFolder();

        // dont loop if not needed
            return perkIDDictionary[perkID];
    }

    public Rarity GetRarity(int value,levelData level){
        int[] valueList = {40,70,90,100};
        if (level && level.valueList != null && level.valueList.Length > 0){
            valueList = level.valueList;
        }

        if (value <= valueList[0]){ // 40%
            return Rarity.Common;
        }else if (value <= valueList[1]){ // 30%
            return Rarity.Uncommon;
        }else if (value <= valueList[2]){ // 25 %
            return Rarity.Rare;
        }else if (value <= valueList[3]){ // 5%
            return Rarity.Legendary;
        }

        return Rarity.Common;
    }

    public perkData getRandomPerk(int perkSeed,List<string> blackList,levelData level){
        // load all the perks to sort through
        loadPerkFolder();

        // get perks and put them into each rarity list
        Dictionary<Rarity, List<perkData>> rarityChoices = new Dictionary<Rarity, List<perkData>>();
        foreach (Rarity tierEnum in System.Enum.GetValues(typeof(Rarity))){
            rarityChoices.Add(tierEnum,new List<perkData>());
        }

        foreach (perkData perk in perkObjects){
            if (perk && !blackList.Contains(perk.name)){
                rarityChoices[perk.perkRarity].Add(perk);
            }
        }

        // get random perk
        System.Random randomGen = new System.Random(perkSeed);
        Rarity chosenTier = GetRarity(randomGen.Next(0,100),level);
        if (rarityChoices[chosenTier].Count <= 0){
            chosenTier = Rarity.Common;
        }

        // return the chosen perk
        return rarityChoices[chosenTier].Count > 0 ? rarityChoices[chosenTier][randomGen.Next(0,rarityChoices[chosenTier].Count)] : null;
    }

    public Dictionary<string, int> countPerks(List<string> perkIDList){
        Dictionary<string, int> perkCounts = new Dictionary<string, int>();

        // count all perks
        foreach (string perkID in perkIDList){
            // Check if key exists then create/add one 
            perkCounts[perkID] = (perkCounts.ContainsKey(perkID) ? perkCounts[perkID] : 0) + 1;
        }

        return perkCounts;
    }

    public List<string> shortenList(List<string> perkIDList){
        List<string> shortPerkList = new List<string>();

        // go through all perks
        foreach (string perkID in perkIDList){
            // Check if key exists then create if none
            if (!shortPerkList.Contains(perkID)){
                shortPerkList.Add(perkID);
            }
        }

        return shortPerkList;
    }

    // This function will fix entity stats after having an perk applied, ex - max ammo should never be less than one
    public void fixEntity(Entity entityInfo){
        if (entityInfo){
            // stats that can be messed up when offset that need fixed
            
            if (entityInfo.maxAmmo <= 1)
                entityInfo.maxAmmo = 1;

            if (entityInfo.currentAmmo > entityInfo.maxAmmo)
                entityInfo.currentAmmo = entityInfo.maxAmmo;

            if (entityInfo.maxHealth <= 1)
                entityInfo.maxHealth = 1;

            if (entityInfo.currentHealth > entityInfo.maxHealth)
                entityInfo.currentHealth = entityInfo.maxHealth;

            if (entityInfo.walkSpeed <= 1)
                entityInfo.walkSpeed = 1;

            if (entityInfo.fireCount <= 1)
                entityInfo.fireCount = 1;
        }
    }

    // This function will fix bullet stats after having an perk applied, ex - damage should never be less than 0.1f
    public void fixBullet(bulletSystem bulletInfo){
        if (bulletInfo){
            // stats that can be messed up when offset that need fixed
            
            if (bulletInfo.bulletSize <= 0.05f)
                bulletInfo.bulletSize = 0.05f;

            if (bulletInfo.bulletDamage <= 0.1f)
                bulletInfo.bulletDamage = 0.1f;

            if (bulletInfo.bulletBounces < 0)
                bulletInfo.bulletBounces = 0;
        }
    }

    public void applyPerk(List<string> perkIDList,string perkType,Dictionary<string, GameObject> objDictionary){
        if (perkIDList == null){
            return;
        }

        // Dictionaries to keep track of perks we sorted through
        Dictionary<string, int> perkCounts = countPerks(perkIDList);
        Dictionary<string, bool> initialize = new Dictionary<string, bool>();

        // go through each perk id in the list
        foreach (string perkID in perkIDList){
            // get the perk obj
            perkData perk = getPerk(perkID);

            // keep track if we already setup the perk
            bool initializePerk = !initialize.ContainsKey(perkID);
            initialize[perkID] = true;
            
            if (perk != null){
                var getType = typeof(perkData);
                // check for each type of perk and run code
                switch(perkType){
                    // Unique Cases
                    case "Update_Bullet":
                        perk.updateBullet(objDictionary,perkCounts[perkID],initializePerk);
                        break;
                    case "Update_Entity":
                        perk.updateEntity(objDictionary,perkCounts[perkID],initializePerk);
                        break;
                    case "Perk_Collect":// when an entity collects a perk
                        perk.perkCollect(objDictionary,perkCounts[perkID],initializePerk);
                        break;
                    // Normal Cases
                    case "Shoot":// when an entity fires a bullet
                        perk.shootEvent(objDictionary,perkCounts[perkID],initializePerk);
                        break;
                    case "Reload":// when an entity fires a bullet
                        perk.reloadGunEvent(objDictionary,perkCounts[perkID],initializePerk);
                        break;
                    case "Hit":// when a bullet hits an entity, this includes walls and stuff, use getTargetStats to check if target is entity
                        perk.hitEvent(objDictionary,perkCounts[perkID],initializePerk);
                        break;
                    case "Bounce":// when a bullet bounces of a surface
                        perk.bounceEvent(objDictionary,perkCounts[perkID],initializePerk);
                        break;
                    case "Damaged":// when an entity receives damage, cannot get bullet or entity that hit this entity: refer to hit event
                        perk.damagedEvent(objDictionary,perkCounts[perkID],initializePerk);
                        break;
                    case "Killed":// when an entitys health becomes zero
                        perk.killedEvent(objDictionary,perkCounts[perkID],initializePerk);
                        break;

                    // Enemy based perk events
                    case "Target_Change":// when an entitys health becomes zero
                        perk.enemy_targetChange(objDictionary,perkCounts[perkID],initializePerk);
                        break;
                }
            }
            
            
            // fix any stats that are really bad to prevent game from breaking
            if (objDictionary.ContainsKey("Owner") && objDictionary["Owner"] != null){
                fixEntity(objDictionary["Owner"].GetComponent<Entity>());
            }

            if (objDictionary.ContainsKey("Bullet") && objDictionary["Bullet"] != null){
                fixBullet(objDictionary["Bullet"].GetComponent<bulletSystem>());
            }

            if (objDictionary.ContainsKey("Target") && objDictionary["Target"] != null){
                fixEntity(objDictionary["Target"].GetComponent<Entity>());
            }
        }
    }
}
