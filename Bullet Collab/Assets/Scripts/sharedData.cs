/*******************************************************************************
* Name : sharedData.cs
* Section Description : This is where player data will be stored. Scoreboard Database 
* will reference this for login, name, score, etc.
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 10/23/22  0.10                 DS              Made the thing
* 10/26/22  0.11                 DS              Added Health to connect to healthbar
* 11/04/22  0.11                 DS              Added saving system + json
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PlayFab;
using PlayFab.ClientModels;

using Newtonsoft.Json;

// use this to store default values
public class tempDataClass{
    public List<string> perkIDList = new List<string>();
    public float currenthealth = 6;
    public int currency = 0;

    public tempDataClass(){

    }

    public tempDataClass(List<string> perkIDS,float setHealth, int setMoney){
        this.perkIDList = perkIDS;
        this.currenthealth = setHealth;
        this.currency = setMoney;
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

    // Persistant Data
    public int runCount;
    public int winCount;
    public float maxScore;
    public float minTime; // in seconds

    // Temporary Data
    public string currentSceneID;
    public float currenthealth;
    public float maxHealth;
    public int currency;
    public List<string> perkIDList = new List<string>();
    // Temporary Bullet - mainly for ui
    public int maxAmmo;
    public int currentAmmo;
    public float reloadTime;
    public float bulletTime;
    public float bulletSpread;
    public float bulletDamage;
    // time vars
    [HideInInspector] public float reloadStartTime = 0;
    [HideInInspector] public float delayStartTime = 0;


    // Reset the Run Data
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
                entityInfo.reloadTime = 0.7f;
                entityInfo.bulletTime = 0.25f;
                entityInfo.bulletSpread = 2;
                entityInfo.bulletDamage = 1;
                entityInfo.automaticGun = false;
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
                    }
                }

                // overwrite the health with their old health
                entityInfo.currentHealth = Mathf.Clamp(overwriteData.currenthealth,1,entityInfo.maxHealth);
            }
                
            // apply any changes to the data
            updateEntityData(playerObj);
            if (uiUpdate != null){
                uiUpdate.updateGameUI();
            }
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
                currentAmmo = entityInfo.currentAmmo;
                reloadTime = entityInfo.reloadTime;
                bulletTime = entityInfo.bulletTime;
                bulletSpread = entityInfo.bulletSpread;
                bulletDamage = entityInfo.bulletDamage;
                reloadStartTime = entityInfo.reloadStartTime;
                delayStartTime = entityInfo.delayStartTime;
            }
        }
    }

    public tempDataClass getTemporaryJSON(){
        tempDataClass tempData = new tempDataClass(perkIDList,currenthealth,currency);
        return tempData;
    }

    public void getTemporaryData(){
        if (loggedIn){
            PlayFabClientAPI.GetUserData(new GetUserDataRequest(), onDataReceive, onDataError);
        }
    }

    public void saveTemporaryData(){
        if (loggedIn){
            var request = new UpdateUserDataRequest{
                Data = new Dictionary<string, string>{
                    {"Temp_Data", JsonConvert.SerializeObject(getTemporaryJSON())}
                }
            };

            PlayFabClientAPI.UpdateUserData(request,onDataSend,onDataError);
        }
    }

    // Playfab Events

    public void onDataSend(UpdateUserDataResult result){
        print("Data was sent to playfab");
    }

    public void onDataReceive(GetUserDataResult result){
        print("Got player data");
        if (result != null && result.Data != null){
            if (result.Data.ContainsKey("Temp_Data")){
                tempDataClass tempData = JsonConvert.DeserializeObject<tempDataClass>(result.Data["Temp_Data"].Value);
                if (tempData != null){
                    overwriteEntity(playerObj,tempData);
                }
            }
        }
    }

    public void onDataError(PlayFabError error){

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
