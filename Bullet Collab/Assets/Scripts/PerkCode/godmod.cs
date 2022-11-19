/*******************************************************************************
* Name : godmod.cs
* Section Description : this is used for testing
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 11/19/22  0.10                 DS              Made the thing
*******************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Perk/godmod")]
public class godmod : perkData
{
    public override void shootEvent(Dictionary<string, GameObject> objDictionary,int Count,bool initialize) {
        if (initialize && objDictionary.ContainsKey("GameManager")){
            objDictionary["GameManager"].GetComponent<gameLoader>().clearGameObj();
        }
    }
}
