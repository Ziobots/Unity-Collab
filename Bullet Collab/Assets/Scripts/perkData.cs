using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class perkData : ScriptableObject
{
    public string perkName;
    public int perkCost;
    [TextArea]
    public string perkDescription;
    public float perkRarity;
    public bool stackablePerk = true;

    public virtual void loadPerk(){

    }
}

public class perkSaved : MonoBehaviour
{
    // Variables used when saving
    public int perkID;
    public int perkCount;
}