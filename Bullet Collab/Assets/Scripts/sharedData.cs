/*******************************************************************************
* Name : sharedData.cs
* Section Description : This is where player data will be stored. Scoreboard Database 
* will reference this for login, name, score, etc.
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 10/23/22  0.10                 DS              Made the thing
* 10/26/22  0.20                 DS              Added Health to connect to healthbar
* 11/04/22  0.30                 DS              Added saving system + json
* 11/14/22  0.40                 DS              Added game handler + stats + reset functions
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PlayFab;
using PlayFab.ClientModels;

using Newtonsoft.Json;

// use this to store default values
public class tempDataClass{
    // player stats
    public List<string> perkIDList = new List<string>();
    public float currenthealth = 6;
    public int currency = 0;
    public int seed = 0;
    public string roomID = "";

    // stats
    public int enemiesKilled = 0;
    public int totalScore = 0;
    public int wave = 1;
    public int room = 0;
    public float startTime = 0;

    public tempDataClass(){
        seed = Mathf.Abs((int)System.DateTime.Now.Ticks);
    }
}

public class persistDataClass{
    // user stats
    public int statHighscore = 0;
    public int statRunCount = 0;
    public int statWinCount = 0;
    public int statKillCount = 0;
    public int statPerkCount = 0;
    public int statRoomCount = 0;
    // Settings
    public float masterVolume = 1f;
    public float musicVolume = 1f;
    public float gameVolume = 1f;
    public bool mobileControls = false;
    public bool particleFX = true;

    public persistDataClass(){

    }
}

public class sharedData : MonoBehaviour
{
    // reference for use in other scripts
    public sharedData dataInstance;
    public GameObject playerObj;

    // UI Stuff
    public GameObject uiManager;
    [HideInInspector] public UIManager uiUpdate;

    // User Information
    public string sessionTicket;
    public string userName;
    public string userID; 
    public bool loggedIn = false;
    public bool connectedToPlayfab = false;

    // Persistant Data
    public int statHighscore = 0;
    public int statRunCount = 0;
    public int statWinCount = 0;
    public int statKillCount = 0;
    public int statPerkCount = 0;
    public int statRoomCount = 0;
    // Settings
    public float masterVolume = 0.5f;
    public float musicVolume = 0.5f;
    public float gameVolume = 0.5f;
    public bool mobileControls = false;
    public bool particleFX = true;

    // Temporary Data
    public float gameStartTime;
    public float gameEndTime;
    public int totalScore = 0;
    public int currentRoom = 1;
    public int currentWave = 1;
    public int seed;
    public int wave;
    public string currentRoomID;
    public float currenthealth;
    public float maxHealth;
    public int enemiesKilled = 0;
    public int currency;
    public List<string> perkIDList = new List<string>();
    [HideInInspector] public tempDataClass currentTempData = new tempDataClass();
    [HideInInspector] public persistDataClass currentPersistData = new persistDataClass();

    // Temporary Bullet - mainly for ui
    public int maxAmmo;
    public int currentAmmo;
    public int fireCount;
    public float reloadTime;
    public float bulletTime;
    public float bulletSpread;
    public float bulletDamage;

    // Leaderboard
    public GetLeaderboardResult leaderboardData;
    public GetLeaderboardAroundPlayerResult clientLeaderboardData;
    public float lastLeaderboardTime = -50000f;
    public float lastClientRankTime = -50000f;
    public float resetLeaderTime = 60 * 10;

    // time vars
    [HideInInspector] public float reloadStartTime = 0;
    [HideInInspector] public float delayStartTime = 0;

    // Data Receive function variables
    [HideInInspector] public System.Action onDataGet = null;
    [HideInInspector] public System.Action onLeaderGet = null;
    [HideInInspector] public System.Action onRankGet = null;

    // Reset the Player obj
    public void resetPlayerObj(GameObject playerObj) {
        if (playerObj != null && playerObj.tag == "Player"){
            Entity entityInfo = playerObj.GetComponent<Entity>();
            if (entityInfo){
                // set the default values
                entityInfo.perkIDList = new List<string>();
                entityInfo.maxHealth = 6;
                entityInfo.currentHealth = entityInfo.maxHealth;
                entityInfo.currency = 0;

                // non saved stats, these are changed by perks which are saved though
                entityInfo.maxAmmo = 3;
                entityInfo.fireCount = 1;
                entityInfo.currentAmmo = entityInfo.maxAmmo;
                entityInfo.reloadTime = 0.7f;
                entityInfo.bulletTime = 0.25f;
                entityInfo.bulletSpread = 2;
                entityInfo.automaticGun = false;
                entityInfo.reloadingGun = false;
                entityInfo.weight = 5f;
                entityInfo.walkSpeed = 6f;
                entityInfo.damagedBy = null;

                // player obj unique stats
                Player playerInfo = playerObj.GetComponent<Player>();
                if (playerInfo != null){
                    playerInfo.hitTime = 0;
                    playerInfo.iFrames = 0.5f;
                    playerInfo.perkCount = 3;
                }
            }
        }
    }

    public void overwriteEntity(GameObject playerObj,tempDataClass overwriteData){
        // Get UI management script
        if (uiManager != null){
            uiUpdate = uiManager.GetComponent<UIManager>();
        }

        // check if player object
        if (playerObj != null && playerObj.tag == "Player" && overwriteData != null){
            Entity entityInfo = playerObj.GetComponent<Entity>();
            if (entityInfo){
                // reset the player obj default values
                resetPlayerObj(playerObj);

                foreach (string perkID in overwriteData.perkIDList){
                    perkData perk = gameObject.GetComponent<perkModule>().getPerk(perkID);
                    if (perk != null){
                        // Add to list one by one just in case
                        entityInfo.perkIDList.Add(perkID);

                        // create the dictionary for on add
                        Dictionary<string, GameObject> editList = new Dictionary<string, GameObject>();
                        editList.Add("Owner", playerObj);
                        editList.Add("PerkObj", null);

                        // This event should only run here on pickup, 3 parameter should always be true here?
                        perk.addedEvent(editList,gameObject.GetComponent<perkModule>().countPerks(entityInfo.perkIDList)[perkID],true);

                        // fix any stats that are really bad
                        gameObject.GetComponent<perkModule>().fixEntity(entityInfo);
                    }
                }

                // overwrite the health with their old health
                entityInfo.currentHealth = Mathf.Clamp(overwriteData.currenthealth,1,entityInfo.maxHealth);
            }
                
            // apply any changes to the data
            updateEntityData(playerObj);
        }
    }

    public void updateEntityData(GameObject playerObj){
        if (playerObj != null && playerObj.tag == "Player"){
            Entity entityInfo = playerObj.GetComponent<Entity>();
            if (entityInfo){
                perkIDList = entityInfo.perkIDList;
                currenthealth = entityInfo.currentHealth;
                maxHealth = entityInfo.maxHealth;
                currency = entityInfo.currency;
                maxAmmo = entityInfo.maxAmmo;
                fireCount = entityInfo.fireCount;
                currentAmmo = entityInfo.currentAmmo;
                reloadTime = entityInfo.reloadTime;
                bulletTime = entityInfo.bulletTime;
                bulletSpread = entityInfo.bulletSpread;
                reloadStartTime = entityInfo.reloadStartTime;
                delayStartTime = entityInfo.delayStartTime;

                // update the ui
                if (uiUpdate != null){
                    uiUpdate.updateGameUI();
                }
            }
        }
    }

    public persistDataClass getPersistJSON(){
        persistDataClass permData = new persistDataClass();
        permData.masterVolume = masterVolume;
        permData.mobileControls = mobileControls;
        permData.musicVolume = musicVolume;
        permData.gameVolume = gameVolume;
        permData.particleFX = particleFX;
        permData.statHighscore = statHighscore;
        permData.statKillCount = statKillCount;
        permData.statPerkCount = statPerkCount;
        permData.statRoomCount = statRoomCount;
        permData.statRunCount = statRunCount;
        permData.statWinCount = statWinCount;

        return permData;
    }

    public tempDataClass getTemporaryJSON(){
        tempDataClass tempData = new tempDataClass();
        tempData.perkIDList = perkIDList;
        tempData.currenthealth = currenthealth;
        tempData.currency = currency;
        tempData.wave = wave;
        tempData.roomID = currentRoomID;
        tempData.seed = seed;
        tempData.room = currentRoom;
        tempData.wave = currentWave;
        tempData.enemiesKilled = enemiesKilled;
        tempData.totalScore = totalScore;
        tempData.startTime = gameStartTime;

        return tempData;
    }

    public void getTemporaryData(){
        if (loggedIn && connectedToPlayfab){
            PlayFabClientAPI.GetUserData(new GetUserDataRequest(), onDataReceive, onDataError);
        }
    }

    public void saveTemporaryData(tempDataClass forceSave){
        if (loggedIn && connectedToPlayfab){
            persistDataClass permSave = getPersistJSON();
            tempDataClass dataToSave = getTemporaryJSON();

            if (forceSave != null){
                dataToSave = forceSave;
            }

            var request = new UpdateUserDataRequest{
                Data = new Dictionary<string, string>{
                    {"Temp_Data", JsonConvert.SerializeObject(dataToSave)},
                    {"Perm_Data", JsonConvert.SerializeObject(permSave)}
                }
            };

            PlayFabClientAPI.UpdateUserData(request,onDataSend,onDataError);
        }
    }

    public void getClientRank(){
        // only get the leaderboard every 10 minutes
        if ((loggedIn && connectedToPlayfab) &&  (userID != null && (clientLeaderboardData != null || Time.realtimeSinceStartup - lastClientRankTime >= resetLeaderTime))){
            lastClientRankTime = Time.realtimeSinceStartup;
            
            var request = new GetLeaderboardAroundPlayerRequest{
                StatisticName = "Score",
                PlayFabId = userID,
                MaxResultsCount = 1
            };

            PlayFabClientAPI.GetLeaderboardAroundPlayer(request,onClientRankGet,onClientRankError);
        }else{
            onClientRankGet(clientLeaderboardData);
        }
    }

    public void getLeaderboard(){
        // only get the leaderboard every 10 minutes
        if (connectedToPlayfab && (leaderboardData != null || Time.realtimeSinceStartup - lastLeaderboardTime >= resetLeaderTime)){
            print("Call Playfab for leaderboard");
            lastLeaderboardTime = Time.realtimeSinceStartup;
            
            var request = new GetLeaderboardRequest{
                StatisticName = "Score",
                StartPosition = 0,
                MaxResultsCount = 99
            };

            PlayFabClientAPI.GetLeaderboard(request,onLeaderboardGet,onLeaderboardError);
        }else{
            onLeaderboardGet(leaderboardData);
        }
    }

    public void sendLeaderboard(int score){
        if (loggedIn && connectedToPlayfab){
            var request = new UpdatePlayerStatisticsRequest{
                Statistics = new List<StatisticUpdate>{
                    new StatisticUpdate{
                        StatisticName = "Score",
                        Value = score
                    }
                }
            };

            PlayFabClientAPI.UpdatePlayerStatistics(request,onLeaderboardSend,onLeaderboardError);
        }
    }

    // Playfab Events

    public void onClientRankGet(GetLeaderboardAroundPlayerResult result){
        print("got client rank data");
        clientLeaderboardData = result;
        if (onRankGet != null){
            onRankGet();
            onRankGet = null;
        }
    }

    public void onLeaderboardGet(GetLeaderboardResult result){
        print("got leaderboard data");
        leaderboardData = result;
        if (onLeaderGet != null){
            onLeaderGet();
            onLeaderGet = null;
        }
    }

    public void onLeaderboardSend(UpdatePlayerStatisticsResult result){
        print("Score was sent to playfab leaderboard");
    }

    public void onClientRankError(PlayFabError error){
        print("Cannot get client rank");
        print(error.ErrorMessage);
        if (onRankGet != null){
            onRankGet();
            onRankGet = null;
        }
    }

    public void onLeaderboardError(PlayFabError error){
        print("Cannot connect to playfab leaderboard");
        print(error.ErrorMessage);
        if (onLeaderGet != null){
            onLeaderGet();
            onLeaderGet = null;
        }
    }

    public void onDataSend(UpdateUserDataResult result){
        print("Data was sent to playfab");
    }

    public void onDataReceive(GetUserDataResult result){
        print("Got player data");
        if (result != null && result.Data != null){
            if (result.Data.ContainsKey("Temp_Data")){
                tempDataClass tempData = JsonConvert.DeserializeObject<tempDataClass>(result.Data["Temp_Data"].Value);
                persistDataClass permData = JsonConvert.DeserializeObject<persistDataClass>(result.Data["Perm_Data"].Value);

                if (tempData != null){
                    currentTempData = tempData;
                }

                if (permData != null){
                    currentPersistData = permData;
                    masterVolume = permData.masterVolume;
                    //mobileControls = permData.mobileControls;
                    musicVolume = permData.musicVolume;
                    gameVolume = permData.gameVolume;
                    particleFX = permData.particleFX;
                    statHighscore = permData.statHighscore;
                    statKillCount = permData.statKillCount;
                    statPerkCount = permData.statPerkCount;
                    statRoomCount = permData.statRoomCount;
                    statRunCount = permData.statRunCount;
                    statWinCount = permData.statWinCount;
                }
            }
        }

        if (onDataGet != null){
            onDataGet();
            onDataGet = null;
        }
    }

    public void onDataError(PlayFabError error){
        print("could not save/get data");
        print(error.ErrorMessage);

        if (onDataGet != null){
            onDataGet();
            onDataGet = null;
        }
    }

    // Setup data module

    private void Start() {
        // Get UI management script
        if (uiManager != null){
            uiUpdate = uiManager.GetComponent<UIManager>();
        }
        
        //resetTempData();
        DontDestroyOnLoad(gameObject);
    }

    // runs at the start of the game
    private void Awake() {
        dataInstance = this;
    }
}
