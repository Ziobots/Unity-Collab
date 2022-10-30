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

    public void applyPerk(List<string> perkIDList,string perkType,Dictionary<string, GameObject> objDictionary){
        if (perkIDList == null){
            return;
        }

        Dictionary<string, int> perkCounts = new Dictionary<string, int>();

        // go through each perk id in the list
        foreach (string perkID in perkIDList){
            // get the perk obj
            perkData perk = getPerk(perkID);
            // keep track if we already setup the perk
            bool initializePerk = !perkCounts.ContainsKey(perkID);
            perkCounts[perkID] = (!initializePerk ? perkCounts[perkID] : 0) + 1;

            if (perk != null){
                // check for each type of perk and run code
                switch(perkType){
                    case "Shoot":// when an entity fires a bullet
                        perk.shootEvent(objDictionary,perkCounts[perkID],initializePerk);
                        break;
                    case "Hit":// when a bullet hits an entity
                        perk.hitEvent(objDictionary,perkCounts[perkID],initializePerk);
                        break;
                    case "Bounce":// when a bullet bounces of a surface
                        print("BOUNCE EVENT");
                        perk.bounceEvent(objDictionary,perkCounts[perkID],initializePerk);
                        break;
                    case "Damaged":// when an entity receives damage
                        perk.damagedEvent(objDictionary,perkCounts[perkID],initializePerk);
                        break;
                    case "Killed":// when an entitys health becomes zero
                        perk.killedEvent(objDictionary,perkCounts[perkID],initializePerk);
                        break;
                }
            }
        }
    }
}
