/*******************************************************************************
* Name : gameLoader.cs
* Section Description : This code handles any game events, like loading enemies and perks.
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 11/09/22  0.10                 DS              Made the thing
* 11/10/22  0.10                 DS              Added enemy spawning + perks
* 11/11/22  0.10                 DS              added level loading + next button
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
    public Transform debriFolder;
    public GameObject levelObj;
    public GameObject playerObj;

    // ui obj
    public GameObject transitioner;   
    public GameObject continueButton;   

    // Game Data
    public int gameSeed;
    public string currentArea = "baseGame";

    [HideInInspector] public bool waveStarted = false;
    [HideInInspector] public bool spawningEnemies = false;
    [HideInInspector] public bool spawnedPerks = false;

    public int currentRoom = 1;
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
                entityInfo.debriFolder = debriFolder;
                entityInfo.gameManager = gameObject;

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
        Random.InitState(gameSeed + currentWave + (currentRoom * 1000) + 444);

        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");

        // spawn enemy at each point
        foreach (GameObject point in spawnPoints){
            if (point){
                enemyList pointData = point.GetComponent<enemyList>();
                if (pointData){
                    string chosenID = pointData.enemySpawns.Count > 0 ? pointData.enemySpawns[0] : "default";
                    if (pointData.enemySpawns.Count > 1){
                        chosenID = pointData.enemySpawns[Random.Range(0,pointData.enemySpawns.Count)];
                    }

                    createEnemy(chosenID,point);
                }
            }
        }
    }

    // get all rooms of room type
    public List<GameObject> getRooms(RoomType getType){
        List<GameObject> roomList = new List<GameObject>();

        Object[] roomLoad = Resources.LoadAll("Levels");
        GameObject[] levelObj = new GameObject[roomLoad.Length];
        roomLoad.CopyTo(levelObj, 0);

        // go through each room and check if it is avaliable
        foreach (GameObject level in roomLoad){
            if (level){
                levelData levelInfo = level.GetComponent<levelData>();
                if (levelInfo){
                    // check if the room can appear
                    if (levelInfo.type == getType || levelInfo.allowLevel()){
                        roomList.Add(level);
                    }
                }
            }
        }

        // add a room to empty lists just in case
        if (roomList.Count <= 0 && levelObj.Length > 0){
            roomList.Add(levelObj[0]);
        }

        return roomList;
    }

    // Clear Folders
    public void clearFolder(Transform folder){
        if (folder != null){
            foreach (Transform chlid in folder){
                GameObject.Destroy(chlid.gameObject);
            }
        }
    }

    public GameObject createRoom(GameObject roomBase){
        // remove the old room
        if (levelObj != null){
            levelData oldData = levelObj.GetComponent<levelData>();
            if (oldData){
                oldData.unLoadLevel();
            }

            Destroy(levelObj);
            levelObj = null;
        }

        // Remove Bullets and Enemies
        clearFolder(bulletFolder);
        clearFolder(enemyFolder);

        // Reset Position of Player to 0,0,0
        if (playerObj != null){
            playerObj.transform.position = new Vector2(0,0);
        }

        if (roomBase != null){
            levelObj = Instantiate(Resources.Load("Levels/"+roomBase.name),new Vector3(0,0,0),new Quaternion()) as GameObject;
            if (levelObj != null){
                levelData levelInfo = levelObj.GetComponent<levelData>();
                if (levelInfo){
                    // set the level info
                    // no info yet

                    // finish setting up the level
                    levelInfo.loadLevel();

                    return levelObj;
                }
            }
        }

        return null;
    }

    // find the next room for the player, based on seed
    public void nextRoom(){
        currentRoom++;
        RoomType nextType = RoomType.Enemy;
        if (currentRoom % 10 == 0){
            nextType = RoomType.Boss;
        }else if (currentRoom % 10 == 5){
            nextType = RoomType.Shop;
        }

        List<GameObject> roomList = getRooms(nextType);
        if (roomList != null && roomList.Count > 0){
            Random.InitState(gameSeed + (currentRoom * 1000) + 111); // gotta offset from the original seed a bit for uniqueness
            GameObject chosenRoom = roomList[Random.Range(0,roomList.Count)];
            if (chosenRoom){
                createRoom(chosenRoom);
            }
        }

        sceneStartWave = currentWave;
        spawningEnemies = false;
        waveStarted = false;
        spawnedPerks = false;
    }

    public void showContinue(){
        if (continueButton != null){
            nextWave continueData = continueButton.GetComponent<nextWave>();
            if (continueData){
                // set the function to be done on press
                continueData.showButton(delegate{
                    // next wave room
                    print("continue to next wave");
                    
                    // start room transition
                    transitioner.GetComponent<fadeTransition>().startFade(delegate{
                        continueData.hideButton();
                        nextRoom();
                    },false);
                });
            }
        }
    }

    public void spawnPerks(){
        if (playerObj){
            Player playerData = playerObj.GetComponent<Player>();
            if (playerData){
                int maxColumn = 7;
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

                for (int i = 0; i < playerData.perkCount; i++){
                    // get the position of the perk
                    Vector3 perkPosition = new Vector3(0,0,0);
                    perkPosition.x = -Mathf.Ceil((float)columnCount / (float)2f) + ((i % columnCount) + 1);
                    //perkPosition.y = -Mathf.Floor((float)rowCount / (float)2f) + Mathf.Floor((float)i/(float)columnCount);
                    perkPosition.y = Mathf.Floor((float)i/(float)columnCount);

                    perkPickup newPerk = Instantiate(perkPrefab,perkPosition * 2.5f,new Quaternion(),debriFolder);
                    if (newPerk != null){
                        // set the default perk stats
                        newPerk.dataManager = dataManager;
                        newPerk.uiManager = uiManager;

                        newPerk.cost = 0;
                        newPerk.count = 1;
                        newPerk.addFolder = debriFolder;
                        newPerk.perkObjList = perkObjList;

                        // get the perk
                        int perkSeed = gameSeed + (currentWave * 10) + (i * 100) + (currentRoom * 1000);
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
        gameSeed = Mathf.Abs((int)System.DateTime.Now.Ticks);
        
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
            levelData levelInfo = levelObj.GetComponent<levelData>();
            if (levelInfo){
                if (currentWave - sceneStartWave > levelInfo.waveCount){
                    if (!spawnedPerks){
                        spawnedPerks = true;
                        currentWave++;
                        spawnPerks();
                        showContinue();

                        // pop all bullets, aka: player shouldnt be in danger once all the enemies are gone (unless they hurt themself)
                        foreach (Transform child in bulletFolder){
                            if (child.gameObject){
                                bulletSystem bulletData = child.gameObject.GetComponent<bulletSystem>();
                                if (bulletData){
                                    bulletData.removeBullet(null);
                                }else{
                                    GameObject.Destroy(child.gameObject);
                                }
                            }
                        }
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
