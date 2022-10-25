using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Perk Base Info - do not edit, all perks inherit these fields
public class perkData : ScriptableObject
{
    public string perkName;
    public int perkCost;
    [TextArea]
    public string perkDescription;
    public float perkRarity;
    public bool stackablePerk = true;

    public virtual void loadPerk(GameObject player,int Count) {

    }
}