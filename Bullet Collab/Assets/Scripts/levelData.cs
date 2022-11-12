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

public enum RoomType {None,Shop,Boss,Enemy,Other};

public class levelData : MonoBehaviour
{
    // Room Data
    public RoomType type = RoomType.None;
    public int waveCount = 1;

    // Reward
    public bool skipPerks = false;
    public int[] valueList = null;//{40,70,90,100};

    // Level Folders
    public Transform spawnPoints;

    // load in the level, include scan for pathfinding here?
    public virtual void loadLevel(){
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

    // undo any changes made to the scene
    public virtual void unLoadLevel(){

    }

    // this is for unique levels or something for subclass levels
    public virtual bool allowLevel(){
        return false;// do not change this
    }
}
