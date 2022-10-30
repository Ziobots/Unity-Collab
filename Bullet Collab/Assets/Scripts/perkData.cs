/*******************************************************************************
* Name : perkData.cs
* Section Description : This is the abstract class for each perk the player can pick up.
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 10/23/22  0.10                 DS              Made the thing
*******************************************************************************/
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

    public virtual void loadPerk(Dictionary<string, GameObject> objDictionary,int Count,bool initialize) {

    }

    public virtual void shootEvent(Dictionary<string, GameObject> objDictionary,int Count,bool initialize) {

    }

    public virtual void hitEvent(Dictionary<string, GameObject> objDictionary,int Count,bool initialize) {

    }

    public virtual void bounceEvent(Dictionary<string, GameObject> objDictionary,int Count,bool initialize) {

    }

    public virtual void damagedEvent(Dictionary<string, GameObject> objDictionary,int Count,bool initialize) {

    }

    public virtual void killedEvent(Dictionary<string, GameObject> objDictionary,int Count,bool initialize) {

    }
}