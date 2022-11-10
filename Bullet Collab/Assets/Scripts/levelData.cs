/*******************************************************************************
* Name : levelData.cs
* Section Description : This code holds any code or information relating to levels.
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 11/10/22  0.10                 DS              Made the thing
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class levelData : MonoBehaviour
{
    public int waveCount = 1;

    // Level Folders
    public Transform spawnPoints;

    public void loadLevel(){
        if (spawnPoints){
            // hide all spawn point images
            foreach (Transform point in spawnPoints){
                if (point && point.gameObject){
                    SpriteRenderer image = point.gameObject.GetComponent<SpriteRenderer>();
                    if (image){
                        image.color = new Color32(0,0,0,0);
                    }
                }
            }
        }
    }
}
