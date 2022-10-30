/*******************************************************************************
* Name : remoteBullet.cs
* Section Description : Bullets will aim towards the mouse cursor mid flight
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 10/30/22  0.10                 DS              Made the thing
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Perk/remoteBullet")]
public class remoteBullet : perkData
{
    public override void updateBullet(Dictionary<string, GameObject> objDictionary,int Count,bool initialize) {
        if (objDictionary.ContainsKey("Bullet") && initialize){
            Debug.Log("homing");
            // Get Mouse Position
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            GameObject bulletObj = objDictionary["Bullet"];

            // Get the direction
            Vector2 cursorDirection = (mousePosition - (Vector2)bulletObj.transform.position).normalized;

            // turn speed is based on how many of this perk you have
            float alphaSpeed = ((float)Count) * 5f;
            
            bulletObj.transform.right = Vector2.Lerp(bulletObj.transform.right,cursorDirection,Time.fixedDeltaTime * alphaSpeed);
        }
    }
}
