/*******************************************************************************
* Name : remoteBullet.cs
* Section Description : Bullets will aim towards the mouse cursor mid flight
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 10/30/22  0.10                 DS              Made the thing
* 11/07/22  0.20                 DS              enemy support
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Perk/remoteBullet")]
public class remoteBullet : perkData
{
    public float speedMultiple = 0.8f;

    public override void shootEvent(Dictionary<string, GameObject> objDictionary,int Count,bool initialize) {
        bulletSystem bulletStats = getBulletStats(objDictionary);

        if (bulletStats){
            // Add the stats
            bulletStats.bulletSpeed *= speedMultiple;
        }
    }
    
    public override void updateBullet(Dictionary<string, GameObject> objDictionary,int Count,bool initialize) {
        if (objDictionary.ContainsKey("Bullet") && initialize){
            // Get Mouse Position
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (objDictionary.ContainsKey("Owner") && objDictionary["Owner"] != null){
                Enemy enemyData = objDictionary["Owner"].GetComponent<Enemy>();
                if (enemyData != null){
                    if (enemyData.currentTarget){
                        mousePosition = (Vector2)enemyData.currentTarget.transform.position;
                    }else{
                        return;
                    }
                }
            }

            GameObject bulletObj = objDictionary["Bullet"];
            if (bulletObj != null){
                // Get the direction
                Vector2 cursorDirection = (mousePosition - (Vector2)bulletObj.transform.position).normalized;

                // turn speed is based on how many of this perk you have
                float alphaSpeed = ((float)Count) * 5f;
                float alpha = Time.fixedDeltaTime * alphaSpeed;
                bulletObj.transform.right = Vector2.Lerp(bulletObj.transform.right,cursorDirection,alpha);
            }
        }
    }
}
