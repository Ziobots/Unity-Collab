/*******************************************************************************
* Name : healthUp.cs
* Section Description : This is an example perk to permanently increase Player Health
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 10/23/22  0.10                 DS              Made the thing
*******************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Perk/healthUp")]
public class healthUp : perkData
{
    public int healthInc = 1;

    public override void loadPerk(Dictionary<string, dynamic> objDictionary,int Count,bool initialize) {

    }
}
