using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Perk/healthUp")]
public class healthUp : perkData
{
    public int healthInc = 1;

    public override void loadPerk(GameObject player,int Count){

    }
}
