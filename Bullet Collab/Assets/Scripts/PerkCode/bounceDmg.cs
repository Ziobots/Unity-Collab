/*******************************************************************************
* Name : bounceDmg.cs
* Section Description : This perk will add bounces to the bullet and increase bullet damage when it does bounce
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 10/30/22  0.10                 DS              Made the thing
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Perk/bounceDmg")]
public class bounceDmg : perkData
{
    public int addBounce = 3;
    public int addDamage = 1;

    public override void shootEvent(Dictionary<string, GameObject> objDictionary,int Count,bool initialize) {
        if (objDictionary.ContainsKey("Bullet")){
            // Add the Bounces
            GameObject bulletObj = objDictionary["Bullet"];
            bulletObj.GetComponent<bulletSystem>().bulletBounces += addBounce;
        }
    }

    public override void bounceEvent(Dictionary<string, GameObject> objDictionary,int Count,bool initialize) {
        if (objDictionary.ContainsKey("Bullet") && initialize){
            // Increase the Damage
            GameObject bulletObj = objDictionary["Bullet"];
            bulletObj.GetComponent<bulletSystem>().bulletDamage += (addDamage * Count);
            bulletObj.GetComponent<bulletSystem>().bulletSize += (0.025f * Count);
            Debug.Log("Bounce");
            Debug.Log(bulletObj.GetComponent<bulletSystem>().bulletBounces);
        }
    }
}
