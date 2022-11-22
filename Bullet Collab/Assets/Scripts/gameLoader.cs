/*******************************************************************************
* Name : gameLoader.cs
* Section Description : This code handles any game events, like loading enemies and perks.
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 11/09/22  0.10                 DS              Made the thing
* 11/10/22  0.20                 DS              Added enemy spawning + perks
* 11/11/22  0.30                 DS              added level loading + next button
* 11/14/22  0.40                 DS              added game start + end + stats
* 11/16/22  0.50                 DS              added music
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
    public GameObject pathGrid;

    // ui obj
    public GameObject transitioner;   
    public GameObject continueButton;   
    public GameObject bossHealthBar;

    public GameObject loadUIScreen;
    public GameObject gameEndScreen;
    public GameObject mainMenu;
    public GameObject gameMenu;
    public GameObject pauseUI;

    public GameObject errorMenu;
    public GameObject cursorObj;

    // Game Data
    public int gameSeed;
    public string currentArea = "baseGame";

    [HideInInspector] public bool waveStarted = false;
    [HideInInspector] public bool spawningEnemies = false;
    [HideInInspector] public bool spawnedPerks = false;
    [HideInInspector] public bool roomLoaded = false;
    [HideInInspector] public bool gameLoaded = false;

    public int currentRoom = 1;
    public int currentWave = 1;
    public float roomStartTime = 0;
    public bool ContinueVisible = false;

    // prefabs
    public perkPickup perkPrefab;

    // Sound Stuff
    public AudioSource musicMenu;
    public AudioSource musicGame;
    public AudioSource musicShop;
    public AudioSource musicBoss;
    public AudioSource currentSource;
    public AudioSource collectNoise;
    public AudioSource errorNoise;
    public AudioSource buyNoise;
    public GameObject currentCamera;

    // music functions

    public void switchMusic(AudioSource setMusic,float fadeTime){
        if (fadeTime < 0){
            fadeTime = 0.5f;
        }

        if (currentSource != setMusic){
            if (currentSource != null){
                LeanTween.cancel(currentSource.gameObject);
                LeanTween.value(currentSource.gameObject,0.1f,0f,fadeTime).setIgnoreTimeScale(true).setEaseLinear().setOnUpdate(delegate(float value){
                    currentSource.volume = value;
                }).setOnComplete(delegate(){
                    currentSource.Stop();
                    currentSource = setMusic;
                    if (currentSource != null){
                        LeanTween.cancel(currentSource.gameObject);
                        LeanTween.value(currentSource.gameObject,0f,0.1f,fadeTime).setIgnoreTimeScale(true).setEaseLinear().setOnUpdate(delegate(float value){
                            currentSource.volume = value;
                        });

                        currentSource.Play();
                    }
                });
            }else{
                currentSource = setMusic;
                if (currentSource != null){
                    LeanTween.cancel(currentSource.gameObject);
                    LeanTween.value(currentSource.gameObject,0f,0.1f,fadeTime).setIgnoreTimeScale(true).setEaseLinear().setOnUpdate(delegate(float value){
                        currentSource.volume = value;
                    });

                    currentSource.Play();
                }
            }
        }
    }

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

    public GameObject createEnemy(string enemyID,GameObject spawnPoint,System.Random randomGen){
        if (randomGen == null){
            randomGen = new System.Random(gameSeed + (currentWave * 2) + (currentRoom * 1000) + 1234);
        }

        GameObject newEnemy = Instantiate(Resources.Load("Enemies/"+enemyID),spawnPoint.transform.position,new Quaternion(),enemyFolder) as GameObject;
        if (newEnemy != null){
            Enemy entityInfo = newEnemy.GetComponent<Enemy>();
            if (entityInfo){
                List<string> spawnPerkIDs = new List<string>();
                List<string> blackList = new List<string>();

                // set the enemy info
                entityInfo.dataManager = dataManager;
                entityInfo.uiManager = uiManager;
                entityInfo.bulletFolder = bulletFolder;
                entityInfo.debriFolder = debriFolder;
                entityInfo.gameManager = gameObject;
                entityInfo.levelObj = levelObj;
                entityInfo.currentCamera = currentCamera;
                
                // put the default perks into the spawn list
                if (entityInfo.perkIDList != null && entityInfo.perkIDList.Count > 0){
                    foreach (string perkID in entityInfo.perkIDList){
                        perkData perk = gameObject.GetComponent<perkModule>().getPerk(perkID);
                        if (perk){
                            spawnPerkIDs.Add(perkID);
                            if (!perk.stackablePerk){
                                blackList.Add(perkID);
                            }
                        }
                    }
                }

                // increase difficulty of enemies by giving them perks after room 10
                if (levelObj != null && currentRoom > 10){
                    levelData levelInfo = levelObj.GetComponent<levelData>();
                    if (levelInfo){
                        // random number of perks based on room number
                        int perkCount = randomGen.Next(0,(int) Mathf.Ceil(currentRoom/10));
                        for (int i = 1; i < perkCount; i++){
                            int perkSeed = gameSeed + randomGen.Next(0,10000) + (i * 150);
                            perkData chosenPerk = gameObject.GetComponent<perkModule>().getRandomPerk(perkSeed,blackList,levelInfo);
                            if (chosenPerk){
                                spawnPerkIDs.Add(chosenPerk.name);
                                if (!chosenPerk.stackablePerk){
                                    blackList.Add(chosenPerk.name);
                                }
                            }
                        }
                    }
                }

                // overwrite the default perk list
                entityInfo.perkIDList = new List<string>();

                // finish setting up the enemy
                entityInfo.setupEntity();

                // initialize perks on the enemy
                foreach (string perkID in spawnPerkIDs){
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

                        // fix any stats that are really bad
                        gameObject.GetComponent<perkModule>().fixEntity(entityInfo);
                    }
                }

                if (bossHealthBar != null && entityInfo.myType == EnemyType.Boss){
                    bossHealthBar.GetComponent<bossHP>().displayHealthBar(entityInfo);
                }

                return newEnemy;
            }
        }

        return null;
    }

    private bool resourceLoadedEnemies = false;
    GameObject[] enemyObjLoad;

    public List<string> getSpawnEnemies(EnemyType type,int roomNumber){
        List<string> returnList = new List<string>();

        if (!resourceLoadedEnemies){
            resourceLoadedEnemies = true;
            Object[] enemyLoad = Resources.LoadAll("Enemies");
            enemyObjLoad = new GameObject[enemyLoad.Length];
            enemyLoad.CopyTo(enemyObjLoad, 0);
        }

        foreach (GameObject enemyObj in enemyObjLoad){
            if (enemyObj){
                Enemy enemyData = enemyObj.GetComponent<Enemy>();
                if (enemyData && enemyData.myType == type && enemyData.roomSpawnMinimum <= roomNumber){
                    if (enemyData.roomSpawnMaximum <= -1 || enemyData.roomSpawnMaximum > roomNumber){
                        returnList.Add(enemyData.gameObject.name);
                    }
                }
            }
        }

        return returnList;
    }

    public void spawnEnemies(){
        // same enemies each wave for same seed
        System.Random randomGen = new System.Random(gameSeed + (currentWave * 2) + (currentRoom * 1000) + 444);

        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");

        // spawn enemy at each point
        foreach (GameObject point in spawnPoints){
            if (point){
                enemyList pointData = point.GetComponent<enemyList>();
                if (pointData){
                    
                    // check if point is active
                    if (pointData.minWave >= 0){
                        if (currentWave < pointData.minWave){
                            continue;
                        }
                    }

                    if (pointData.maxWave >= 0){
                        if (currentWave > pointData.maxWave){
                            continue;
                        }
                    }

                    List<string> choiceList = pointData.enemySpawns;
                    string chosenID = choiceList.Count > 0 ? pointData.enemySpawns[0] : "default";

                    if (pointData.enemySpawns.Count <= 0 && pointData.spawnType != EnemyType.None){
                        choiceList = getSpawnEnemies(pointData.spawnType,currentRoom);
                    }

                    if (choiceList.Count > 0){
                        chosenID = choiceList[randomGen.Next(0,choiceList.Count)];
                    }

                    createEnemy(chosenID,point,randomGen);
                }
            }
        }
    }

    // get all rooms of room type
    private bool resourceLevelLoaded = false;
    private GameObject[] levelResourceObj;

    public void loadRooms(){
        if (!resourceLevelLoaded){
            resourceLevelLoaded = true;
            Object[] roomLoad = Resources.LoadAll("Levels");
            levelResourceObj = new GameObject[roomLoad.Length];
            roomLoad.CopyTo(levelResourceObj, 0);
        }
    }

    public GameObject getRoom(string roomID){
        loadRooms();

        // search for room from id
        foreach (GameObject level in levelResourceObj){
            if (level && level.name == roomID){
                return level;
            }
        }

        return null;
    }

    public List<string> getRoomList(RoomType getType){
        List<string> roomList = new List<string>();
        loadRooms();

        // go through each room and check if it is avaliable
        foreach (GameObject level in levelResourceObj){
            if (level){
                levelData levelInfo = level.GetComponent<levelData>();
                if (levelInfo){
                    // check if the room can appear
                    if (levelInfo.type == getType || levelInfo.allowLevel()){
                        roomList.Add(level.name);
                    }
                }
            }
        }

        // add a room to empty lists just in case
        if (roomList.Count <= 0 && levelResourceObj.Length > 0){
            roomList.Add(levelResourceObj[0].name);
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

    // Remove Bullets and Enemies and other stuff
    public void clearGameObj(){
        clearFolder(bulletFolder);
        clearFolder(enemyFolder);
        clearFolder(debriFolder);
    }

    public GameObject createRoom(string roomID){
        GameObject roomBase = getRoom(roomID);

        // remove the old room
        if (levelObj != null){
            levelData oldData = levelObj.GetComponent<levelData>();
            if (oldData){
                oldData.unLoadLevel();
            }

            Destroy(levelObj);
            levelObj = null;
        }

        // Remove Bullets and Enemies and other stuff
        clearGameObj();

        if (roomBase != null){
            levelObj = Instantiate(Resources.Load("Levels/"+roomBase.name),new Vector3(0,0,0),new Quaternion()) as GameObject;
            if (levelObj != null){
                levelData levelInfo = levelObj.GetComponent<levelData>();
                if (levelInfo){
                    // set the level info

                    // set the music
                    if (levelInfo.type == RoomType.Shop){
                        switchMusic(musicShop,0.25f);
                    }else if (levelInfo.type == RoomType.Boss){
                        switchMusic(musicBoss,0.25f);
                    }else{
                        switchMusic(musicGame,0.25f);
                    }

                    if (levelObj.transform.Find("BreakableObj")){
                        foreach (Transform destructable in levelObj.transform.Find("BreakableObj")){
                            Breakable breakInfo = destructable.gameObject.GetComponent<Breakable>();
                            if (breakInfo != null){
                                breakInfo.gameManager = gameObject;
                                breakInfo.dataManager = dataManager;
                                breakInfo.uiManager = uiManager;
                                breakInfo.debriFolder = debriFolder;
                                breakInfo.bulletFolder = bulletFolder;
                                breakInfo.currentCamera = currentCamera;

                                breakInfo.Start();
                            }
                        }
                    }

                    // finish setting up the level
                    levelInfo.loadLevel();

                    // Reset Position of Player to 0,0,0
                    if (playerObj != null && currentCamera != null){
                        Vector3 spawnPosition = new Vector3(0,0,0);
                        if (levelObj != null && levelObj.transform.Find("spawnPosition")){
                            spawnPosition = levelObj.transform.Find("spawnPosition").position;
                        }

                        playerObj.transform.position = spawnPosition;
                        currentCamera.GetComponent<CameraBehavior>().instantJump = true;
                    }   


                    if (pathGrid != null){
                        pathGrid.GetComponent<AstarPath>().Scan();
                    }

                    return levelObj;
                }
            }
        }

        return null;
    }

    // find the next room for the player, based on seed
    public void nextRoom(string setID){
        currentRoom++;
        RoomType nextType = RoomType.Enemy;
        if ((currentRoom + 1) % 10 == 0 && currentRoom > 5){
            nextType = RoomType.Boss;
        }else if ((currentRoom + 1) % 10 == 5){
            nextType = RoomType.Shop;
        }

        if (currentRoom > 1){
            dataInfo.totalScore += 1000;
        }

        List<string> roomList = getRoomList(nextType);
        print(currentRoom + " ROOM LIST: " + roomList.Count + "_" + nextType);

        if (roomList != null && roomList.Count > 0){
            System.Random randomGen = new System.Random(gameSeed + (currentRoom * 1000) + 111);// gotta offset from the original seed a bit for uniqueness
            string chosenRoomID = roomList[0];
            if (roomList.Count > 1){
                chosenRoomID = roomList[randomGen.Next(0,roomList.Count)];
            }

            // check for forced room
            if (setID != null && setID != ""){
                foreach (string roomID in roomList){
                    if (roomID == setID){
                        chosenRoomID = roomID;
                        break;
                    }
                }
            }

            if (currentRoom <= 0 && (setID == null  || setID == "")){
                chosenRoomID = "tutorial";
            }

            if (chosenRoomID != null && chosenRoomID != ""){
                dataInfo.currentRoomID = chosenRoomID;
                createRoom(chosenRoomID);
            }
        }

        roomLoaded = false;
        currentWave = 0;
        spawningEnemies = false;
        waveStarted = false;
        spawnedPerks = false;

        dataInfo.currentRoom = currentRoom;
        dataInfo.currentWave = currentWave;

        // dont spawn enemies immediately
        StartCoroutine(doWait(delegate{
            roomLoaded = true;
        },0.5f));
    }

    public void hideContinue(){
        nextWave continueData = continueButton.GetComponent<nextWave>();
        ContinueVisible = false;
        continueData.hideButton();
        if (bossHealthBar != null){
            bossHealthBar.GetComponent<bossHP>().hideHealthBar();
        }
    }

    public void showContinue(){
        if (bossHealthBar != null){
            bossHealthBar.GetComponent<bossHP>().hideHealthBar();
        }

        if (continueButton != null && !ContinueVisible){
            nextWave continueData = continueButton.GetComponent<nextWave>();
            if (continueData){
                ContinueVisible = true;

                if (levelObj != null){
                    levelData levelInfo = levelObj.GetComponent<levelData>();
                    if (levelInfo){
                        levelInfo.onLevelClear();
                    }
                }

                // set the function to be done on press
                continueData.showButton(delegate{
                    // start room transition
                    transitioner.GetComponent<fadeTransition>().startFade(delegate{
                        if (playerObj.GetComponent<Entity>().currentHealth > 0){
                            hideContinue();
                            nextRoom(null);

                            // save the run data each time they enter a new room
                            dataInfo.canDoSave = true;
                            dataInfo.saveTemporaryData(null);
                        }
                    },false);
                });
            }
        }
    }

    public void spawnPerks(){
        if (playerObj && levelObj){
            Player playerData = playerObj.GetComponent<Player>();
            levelData levelInfo = levelObj.GetComponent<levelData>();
            if (playerData && levelInfo){
                dataInfo.saveTemporaryData(null);
                dataInfo.canDoSave = false;
                
                List<string> blackList = new List<string>();
                int spawnPerkCount = playerData.perkCount;

                if (levelInfo.type == RoomType.Shop){
                    spawnPerkCount = 5;
                    blackList.Add("COST_ONLY_PERK");
                }else if (levelInfo.type == RoomType.Boss){
                    spawnPerkCount += 1;
                }

                int maxColumn = 7;
                int columnCount = Mathf.Clamp(spawnPerkCount,1,maxColumn);
                int rowCount = (int) Mathf.Ceil(spawnPerkCount/columnCount);
                List<GameObject> perkObjList = new List<GameObject>();

                // should not spawn perks that cant stack that the player has
                foreach (string perkID in playerObj.GetComponent<Entity>().perkIDList){
                    perkData perk = gameObject.GetComponent<perkModule>().getPerk(perkID);
                    if (perk && !perk.stackablePerk){
                        blackList.Add(perkID);
                    }
                }

                // spawn the perks
                for (int i = 0; i < spawnPerkCount; i++){
                    // get the position of the perk
                    Vector3 perkBasePosition = new Vector3(0,0,0);
                    Vector3 perkPosition = new Vector3(0,0,0);

                    if (levelInfo.type == RoomType.Shop){
                        // change to item shop
                        if (i >= spawnPerkCount - 2){
                            blackList.Add("SHOP_ONLY_PERK");
                        }
                    }

                    if (levelObj != null && levelObj.transform.Find("spawnPosition")){
                        perkBasePosition = levelObj.transform.Find("spawnPosition").position;
                    }

                    if (levelObj && levelObj.transform.Find("shopPosition")){
                        perkBasePosition = levelObj.transform.Find("shopPosition").position;
                    }

                    float offsetX = columnCount % 2 == 0 ? 0.5f : 1f;
                    perkPosition.x = -Mathf.Ceil((float)columnCount / (float)2f) + ((i % columnCount) + offsetX);
                    perkPosition.y = Mathf.Floor((float)i/(float)columnCount);

                    perkPickup newPerk = Instantiate(perkPrefab,perkBasePosition + (perkPosition * 2.5f),new Quaternion(),debriFolder);

                    if (newPerk != null){
                        // set the default perk stats
                        newPerk.dataManager = dataManager;
                        newPerk.gameManager = gameObject;
                        newPerk.uiManager = uiManager;

                        newPerk.cost = 0;
                        newPerk.count = 1;
                        newPerk.addFolder = debriFolder;
                        newPerk.perkObjList = perkObjList;
                        newPerk.collectNoise = collectNoise;
                        newPerk.errorNoise = errorNoise;

                        newPerk.perkGet = delegate{
                            currentWave++;
                            dataInfo.currentRoom = currentRoom;
                            dataInfo.currentWave = currentWave;
                        };

                        // get the perk
                        int perkSeed = gameSeed + (i * 100) + (currentRoom * 1000) + 894636;
                        perkData chosenPerk = gameObject.GetComponent<perkModule>().getRandomPerk(perkSeed,blackList,levelInfo);
                        if (levelInfo.type == RoomType.Boss){
                            if (i == 0){
                                chosenPerk = gameObject.GetComponent<perkModule>().getPerk("maxHPIncrease");
                            }
                        }
                        
                        if (chosenPerk){
                            newPerk.perkID = chosenPerk.name;
                            if (levelInfo.type == RoomType.Shop){
                                newPerk.cost = chosenPerk.perkCost;
                                newPerk.collectNoise = buyNoise;
                            }

                            // check if should add to blacklist
                            if (chosenPerk && !chosenPerk.stackablePerk){
                                blackList.Add(chosenPerk.name);
                            }
                        }

                        // finish setting up the perk
                        newPerk.setupPickup();
                        if (levelInfo.type != RoomType.Shop){
                            perkObjList.Add(newPerk.gameObject);
                        }
                    }
                }

                if (levelInfo.type != RoomType.Shop){
                    foreach (GameObject perkObj in perkObjList){
                        perkObj.GetComponent<perkPickup>().perkObjList = perkObjList;
                    }
                }
            }
        }
    }

    private IEnumerator doWait(System.Action onComplete,float waitTime){
        yield return new WaitForSecondsRealtime(waitTime);
        // run on complete
        onComplete();
    }

    public void newGameTransition(){
        // show load screen
        loadUIScreen.SetActive(true);

        gameLoaded = false;
        startNewGame();

        // wait for game to load
        StartCoroutine(doWait(delegate{
            transitioner.GetComponent<fadeTransition>().startFade(delegate{
                loadUIScreen.SetActive(false);
                // wait a bit before loading enemies
                StartCoroutine(doWait(delegate{
                    gameLoaded = true;
                },1));
            },false);
        },1));
    }

    public void startNewGame(){
        Setup();

        if (dataInfo != null){
            print("START NEW GAME");
            hideContinue();
            if (bossHealthBar != null){
                bossHealthBar.GetComponent<bossHP>().hideHealthBar();
            }

            roomStartTime = 0;

            bool createNewGame = false;

            if (dataInfo.currentTempData == null){
                createNewGame = true;
            }else if (dataInfo.currentTempData.room < 0){
                createNewGame = true;
            }

            if (createNewGame){
                dataInfo.currentTempData = new tempDataClass();
            }

            // get a seed for randomly generated content
            gameSeed = dataInfo.currentTempData.seed;
            dataInfo.seed = gameSeed;
            //gameSeed = 0;// just for testing if seeds work
            print("SEED: " + gameSeed);

            Random.InitState(gameSeed);

            currentRoom = dataInfo.currentTempData.room - 1;
            nextRoom(dataInfo.currentTempData.roomID);
            currentWave = dataInfo.currentTempData.wave;

            if (levelObj != null && currentWave > 0){
                levelData levelInfo = levelObj.GetComponent<levelData>();
                if (levelInfo){
                    levelInfo.onNextWave(currentWave);
                }
            }

            // reset data for obj
            dataInfo.enemiesKilled = dataInfo.currentTempData.enemiesKilled;
            dataInfo.totalScore = dataInfo.currentTempData.totalScore;
            dataInfo.currentRoom = currentRoom;
            dataInfo.currentWave = currentWave;
            dataInfo.elapsedTime = dataInfo.currentTempData.elapsedTime;

            dataInfo.overwriteEntity(playerObj,dataInfo.currentTempData);

            // camera cursor stuff
            if (currentCamera != null){
                currentCamera.GetComponent<CameraBehavior>().factorMouse = true;
                currentCamera.GetComponent<CameraBehavior>().extraZoom = 0;
                currentCamera.GetComponent<CameraBehavior>().zoomSpeed = 1f;
                currentCamera.GetComponent<CameraBehavior>().instantJump = true;
            }
            
            mouseCursor cursorData = cursorObj.GetComponent<mouseCursor>();
            cursorData.reticleActive = true;
            cursorData.updateHover(false);

            // show game menu
            mainMenu.SetActive(false);
            gameEndScreen.SetActive(false);
            gameMenu.SetActive(true);
            pauseUI.GetComponent<pauseButton>().resumeGame();

            // enable the player controller
            if (playerObj != null){
                playerObj.SetActive(true);
            }

            // update the ui
            if (uiUpdate != null){
                uiUpdate.updateGameUI();
                uiUpdate.updateHealth();
            }

            //switchMusic(musicGame,0.5f);
        }
    }

    public void endGame(){
        Setup();
        if (dataInfo != null){
            switchMusic(musicMenu,0.2f);
            dataInfo.sendLeaderboard(dataInfo.totalScore);
            dataInfo.currentTempData = new tempDataClass();
            dataInfo.currentTempData.room = -1;

            // change perm stats
            dataInfo.statKillCount += dataInfo.enemiesKilled;
            dataInfo.statPerkCount += gameObject.GetComponent<perkModule>().countPerks(dataInfo.perkIDList,false);
            dataInfo.statRoomCount += dataInfo.currentRoom;

            if (dataInfo.currenthealth > 0){
                dataInfo.statWinCount++;
            }

            dataInfo.statRunCount++;
            roomStartTime = 0;

            if (dataInfo.statHighscore < dataInfo.totalScore){
                dataInfo.statHighscore = dataInfo.totalScore;
            }

            // overwrite the current run data
            dataInfo.canDoSave = true;
            dataInfo.saveTemporaryData(dataInfo.currentTempData);
            dataInfo.canDoSave = false;

            if (gameEndScreen != null){
                gameEndScreen.GetComponent<endScreen>().loadMenu();
            }
        }
    }

    public void Setup(){
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

    // Start is called before the first frame update
    private void Start(){
        Setup();
    }

    // Update is called once per frame
    private void FixedUpdate() {
        if (waveStarted){
            if (getEnemies().Count <= 0){
                waveStarted = false;
                spawningEnemies = false;
            }
        }else if (levelObj != null && gameLoaded){
            levelData levelInfo = levelObj.GetComponent<levelData>();
            if (levelInfo){
                if (currentWave >= levelInfo.waveCount){
                    // get time spent clearing room
                    if (roomStartTime != 0){
                        dataInfo.elapsedTime += Time.time - roomStartTime;
                        roomStartTime = 0;
                    }

                    if (!spawnedPerks && currentWave <= (levelInfo.waveCount + 1)){
                        spawnedPerks = true;

                        if (!levelInfo.skipPerks || levelInfo.type == RoomType.Shop){
                            spawnPerks();
                        }

                        if (!levelInfo.skipPerks){
                            showContinue();
                        }

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
                    }else if (!spawnedPerks){
                        spawnedPerks = true;
                        if (!levelInfo.skipPerks){
                            showContinue();
                        }
                    }
                }else if(!spawningEnemies && roomLoaded && gameLoaded){
                    if (roomStartTime == 0){
                        roomStartTime = Time.time;
                    }
                    
                    currentWave++;
                    spawningEnemies = true;
                    dataInfo.totalScore += 100;
                    spawnEnemies();
                    waveStarted = true;

                    dataInfo.currentRoom = currentRoom;
                    dataInfo.currentWave = currentWave;

                    if (levelObj != null){
                        if (levelInfo){
                            levelInfo.onNextWave(currentWave);
                        }
                    }
                }
            }
        }
    }
}
