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
using UnityEngine.UI;

// Perk Base Info - do not edit, all perks inherit these fields
// INITIALIZE bool is used if a perk has code you only want executed once
// ex: with homing it has to calculate stuff but if it did it twice it would cause problems


public class perkData : ScriptableObject
{
    public Sprite perkIcon;
    public string perkName;
    public int perkCost;
    [TextArea]
    public string perkDescription;
    public float perkRarity;
    public bool stackablePerk = true;

    public virtual void loadPerk(Dictionary<string, GameObject> objDictionary,int Count,bool initialize) {}

    public virtual void shootEvent(Dictionary<string, GameObject> objDictionary,int Count,bool initialize) {}

    public virtual void hitEvent(Dictionary<string, GameObject> objDictionary,int Count,bool initialize) {}

    public virtual void bounceEvent(Dictionary<string, GameObject> objDictionary,int Count,bool initialize) {}

    public virtual void damagedEvent(Dictionary<string, GameObject> objDictionary,int Count,bool initialize) {}

    public virtual void killedEvent(Dictionary<string, GameObject> objDictionary,int Count,bool initialize) {}

    public virtual void updateBullet(Dictionary<string, GameObject> objDictionary,int Count,bool initialize) {}

    public virtual void updateEntity(Dictionary<string, GameObject> objDictionary,int Count,bool initialize) {}

}