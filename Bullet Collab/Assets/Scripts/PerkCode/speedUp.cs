using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Perk/speedUp")]
public class speedUp : perkData
{
    public int speedInc = 1;

    public override void loadPerk(GameObject player,int Count){

    }
}
