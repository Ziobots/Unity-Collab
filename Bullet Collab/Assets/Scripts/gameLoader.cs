/*******************************************************************************
* Name : gameLoader.cs
* Section Description : This code handles any game events, like loading enemies and perks.
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 11/09/22  0.10                 DS              Made the thing
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameLoader : MonoBehaviour
{
    // Base Data Stuff
    public GameObject dataManager;
    [HideInInspector] public sharedData dataInfo;

    // UI Stuff 
    public GameObject uiManager;
    [HideInInspector] public UIManager uiUpdate;

    // Game Folders
    public Transform enemyFolder;
    public Transform bulletFolder;

    // Game Data
    public int gameSeed;

    [HideInInspector] public bool waveStarted = false;
    [HideInInspector] public bool spawningEnemies = false;
    public int currentWave = 0;

    // game checks
    public List<GameObject> getEnemies(){
        List<GameObject> enemyList = new List<GameObject>();

        foreach (Transform enemyTransform in enemyFolder.transform){
            if (enemyTransform && enemyTransform.gameObject){
                Entity enemyData = enemyTransform.gameObject.GetComponent<Entity>();
                if (enemyData && enemyData.currentHealth > 0){
                    enemyList.Add(enemyTransform.gameObject);
                }
            }
        }

        print("NUM OF ENEMY:" + enemyList.Count);

        return enemyList;
    }

    public GameObject createEnemy(string enemyID,GameObject spawnPoint){
        GameObject newEnemy = Instantiate(Resources.Load("Enemies/"+enemyID),spawnPoint.transform.position,new Quaternion(),enemyFolder) as GameObject;
        if (newEnemy != null){
            Entity entityInfo = newEnemy.GetComponent<Entity>();
            if (entityInfo){
                // set the enemy info
                entityInfo.dataManager = dataManager;
                entityInfo.uiManager = uiManager;
                entityInfo.bulletFolder = bulletFolder;

                // finish setting up the enemy
                entityInfo.setupEntity();

                // Add modifiers
                foreach (string perkID in entityInfo.perkIDList){
                    perkData perk = gameObject.GetComponent<perkModule>().getPerk(perkID);
                    if (perk != null){
                        // Add to list one by one just in case
                        entityInfo.perkIDList.Add(perkID);

                        // create the dictionary for on add
                        Dictionary<string, GameObject> editList = new Dictionary<string, GameObject>();
                        editList.Add("Owner", newEnemy);
                        editList.Add("PerkObj", null);

                        // This event should only run here on pickup, 3 parameter should always be true here?
                        perk.addedEvent(editList,gameObject.GetComponent<perkModule>().countPerks(entityInfo.perkIDList)[perkID],true);
                    }
                }
            }
        }

        return null;
    }

    public void spawnEnemies(){
        // same enemies each wave for same seed
        Random.InitState(gameSeed + currentWave);

        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");

        // spawn enemy at each point
        foreach (GameObject point in spawnPoints){
            if (point){
                enemyList pointData = point.GetComponent<enemyList>();
                if (pointData){
                    string chosenID = pointData.enemySpawns.Count > 0 ? pointData.enemySpawns[0] : "default";
                    if (pointData.enemySpawns.Count > 1){
                        chosenID = pointData.enemySpawns[Random.Range(0,pointData.enemySpawns.Count - 1)];
                    }

                    createEnemy(chosenID,point);
                }
            }
        }
    }

    // Start is called before the first frame update
    private void Start(){
        // Get data management script
        if (dataManager != null){
            dataInfo = dataManager.GetComponent<sharedData>();
        }

        // Get UI management script
        if (uiManager != null){
            uiUpdate = uiManager.GetComponent<UIManager>();
        }

        // should persist
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    private void FixedUpdate() {
        if (waveStarted){
            if (getEnemies().Count <= 0){
                print("END WAVE");
                waveStarted = false;
                spawningEnemies = false;
            }
        }else{
            if (!spawningEnemies){
                print("SPAWN ENEMIES");
                currentWave++;
                spawningEnemies = true;
                spawnEnemies();
                waveStarted = true;
            }
        }
    }
}
