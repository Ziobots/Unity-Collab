/*******************************************************************************
* Name : levelData.cs
* Section Description : This code holds any code or information relating to levels.
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 11/10/22  0.10                 DS              Made the thing
* 11/12/22  0.30                 KJ              Made some test levels for the game
* 11/21/22  0.30                 KJ              Made a couple of levels for the game
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// the types of rooms that can spawn
public enum RoomType {None,Shop,Boss,Enemy,Other};

public class levelData : MonoBehaviour
{
    // Room Data
    public RoomType type = RoomType.None;
    public int roomSpawnMinimum = 0;
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

    // fires when the wave changes
    public virtual void onNextWave(int waveNumber){

    }

    // fires when the room is cleared
    public virtual void onLevelClear(){

    }

    // this is for unique levels or something for subclass levels
    public virtual bool allowLevel(int currentRoom){
        return false;// do not change this
    }
}
