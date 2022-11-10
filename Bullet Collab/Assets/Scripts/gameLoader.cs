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

    // Game Objects
    public Transform enemyFolder;
    public Transform bulletFolder;
    public GameObject levelObj;
    public GameObject playerObj;

    // Game Data
    public int gameSeed;
    public string currentArea = "baseGame";

    [HideInInspector] public bool waveStarted = false;
    [HideInInspector] public bool spawningEnemies = false;
    public int currentWave = 0;
    public int sceneStartWave = 0;

    // prefabs
    public perkPickup perkPrefab;

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

                return newEnemy;
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

    public void spawnPerks(){
        if (playerObj){
            Player playerData = playerObj.GetComponent<Player>();
            if (playerData){
                int maxColumn = 10;
                int columnCount = Mathf.Clamp(playerData.perkCount,1,maxColumn);
                int rowCount = (int) Mathf.Ceil(playerData.perkCount/columnCount);
                List<GameObject> perkObjList = new List<GameObject>();

                // should not spawn perks that cant stack that the player has
                List<string> blackList = new List<string>();
                foreach (string perkID in playerObj.GetComponent<Entity>().perkIDList){
                    perkData perk = gameObject.GetComponent<perkModule>().getPerk(perkID);
                    if (perk && !perk.stackablePerk){
                        blackList.Add(perkID);
                    }
                }


                for (int i = 1; i <= playerData.perkCount; i++){
                    Vector3 perkPosition = new Vector3(0,0,0);
                    perkPosition.x = -((float)columnCount / (float)2f) + i;
                    perkPosition.y = -((float)rowCount / (float)2f) + Mathf.Ceil((float)i/(float)columnCount);

                    perkPickup newPerk = Instantiate(perkPrefab,perkPosition * 2.5f,new Quaternion(),bulletFolder);
                    if (newPerk != null){
                        // set the default perk stats
                        newPerk.dataManager = dataManager;
                        newPerk.uiManager = uiManager;

                        newPerk.cost = 0;
                        newPerk.count = 1;
                        newPerk.addFolder = bulletFolder;
                        newPerk.perkObjList = perkObjList;

                        // get the perk
                        int perkSeed = gameSeed + (currentWave * 10) + (i * 100);
                        perkData chosenPerk = gameObject.GetComponent<perkModule>().getRandomPerk(perkSeed,blackList);
                        if (chosenPerk){
                            newPerk.perkID = chosenPerk.name;

                            // check if should add to blacklist
                            if (chosenPerk && !chosenPerk.stackablePerk){
                                blackList.Add(chosenPerk.name);
                            }
                        }

                        // finish setting up the perk
                        newPerk.setupPickup();
                        perkObjList.Add(newPerk.gameObject);
                    }
                }

                foreach (GameObject perkObj in perkObjList){
                    perkObj.GetComponent<perkPickup>().perkObjList = perkObjList;
                }
            }
        }
    }

    // Start is called before the first frame update
    private void Start(){
        gameSeed = (int)System.DateTime.Now.Ticks;
        
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
    private bool spawnedPerks = false;
    private void FixedUpdate() {
        if (waveStarted){
            if (getEnemies().Count <= 0){
                print("END WAVE");
                waveStarted = false;
                spawningEnemies = false;
            }
        }else{
            levelData levelInfo = levelObj.GetComponent<levelData>();
            if (levelInfo){
                if (currentWave - sceneStartWave > levelInfo.waveCount){
                    if (!spawnedPerks){
                        spawnedPerks = true;
                        currentWave++;
                        print("Spawn perks");
                        spawnPerks();
                    }
                }else if(!spawningEnemies){
                    print("SPAWN ENEMIES");
                    currentWave++;
                    spawningEnemies = true;
                    spawnEnemies();
                    waveStarted = true;
                }
            }
        }
    }
}
