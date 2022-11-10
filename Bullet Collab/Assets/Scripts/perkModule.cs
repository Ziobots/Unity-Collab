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
    public perkData getPerk(string perkID){
        // get all of the perks to go through
        Object[] perkLoad = Resources.LoadAll("PerkFolder");
        perkData[] perkObjects = new perkData[perkLoad.Length];
        perkLoad.CopyTo(perkObjects, 0);

        foreach (perkData perk in perkObjects){
            if (perk.name == perkID){
                return perk;
            }
        }

        return null;
    }

    public Rarity GetRarity(int value){
        if (value <= 40){ // 40%
            return Rarity.Common;
        }else if (value <= 70){ // 30%
            return Rarity.Uncommon;
        }else if (value <= 95){ // 25 %
            return Rarity.Rare;
        }else if (value <= 100){ // 5%
            return Rarity.Legendary;
        }

        return Rarity.Common;
    }

    public perkData getRandomPerk(int perkSeed,List<string> blackList){
        // load all the perks to sort through
        Object[] perkLoad = Resources.LoadAll("PerkFolder");
        perkData[] perkObjects = new perkData[perkLoad.Length];
        perkLoad.CopyTo(perkObjects, 0);

        // get perks and put them into each rarity list
        Dictionary<Rarity, List<perkData>> rarityChoices = new Dictionary<Rarity, List<perkData>>();
        foreach (Rarity tierEnum in System.Enum.GetValues(typeof(Rarity))){
            rarityChoices.Add(tierEnum,new List<perkData>());
        }

        foreach (perkData perk in perkObjects){
            if (perk && !blackList.Contains(perk.name)){
                rarityChoices[perk.perkRarity].Add(perk);
                print("added "+perk.name+" to "+perk.perkRarity+" :c= " + rarityChoices[perk.perkRarity].Count);
            }
        }

        // get random perk
        Random.InitState(perkSeed);
        Rarity chosenTier = GetRarity(Random.Range(0,100));
        if (rarityChoices[chosenTier].Count <= 0){
            chosenTier = Rarity.Common;
        }

        // return the chosen perk
        return rarityChoices[chosenTier].Count > 0 ? rarityChoices[chosenTier][Random.Range(0,rarityChoices[chosenTier].Count - 1)] : null;
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
                    case "Hit":// when a bullet hits an entity
                        perk.hitEvent(objDictionary,perkCounts[perkID],initializePerk);
                        break;
                    case "Bounce":// when a bullet bounces of a surface
                        perk.bounceEvent(objDictionary,perkCounts[perkID],initializePerk);
                        break;
                    case "Damaged":// when an entity receives damage
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
        }
    }
}
