/*******************************************************************************
* Name : speedUp.cs
* Section Description : This is an example perk to permanently increase Player Speed
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 10/23/22  0.10                 DS              Made the thing
*******************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Perk/speedUp")]
public class speedUp : perkData
{
    public int speedInc = 1;

    public override void loadPerk(Dictionary<string, GameObject> objDictionary,int Count,bool initialize) {

    }
}
