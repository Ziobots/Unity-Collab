/*******************************************************************************
* Name : leaderSetup.cs
* Section Description : This code handles the leaderboard UI.
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 11/16/22  0.10                 DS              Made the thing
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class leaderSetup : MonoBehaviour
{
    // Base Data Stuff
    public GameObject dataManager;
    [HideInInspector] public sharedData dataInfo;
    
    // leaderboard variables
    public GameObject statHolder;
    public GameObject leaderHolder;

    private void setStatValue(string statName, string statValue){
        Transform statObj = statHolder.transform.Find(statName);
        if (statObj != null && statObj.gameObject != null){
            statObj.Find("valueField").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = statValue;
        }
    }

    public void setupMenu(){
        // Get data management script
        if (dataManager != null){
            dataInfo = dataManager.GetComponent<sharedData>();
        }
    }

    public void loadLeaderboard(){
        if (leaderHolder != null){
            
        }
    }

    public void loadMenu(){
        setupMenu();
        
        // load in the player stats
        if (dataInfo != null){
            setStatValue("stat_Score","" + dataInfo.statHighscore);
            setStatValue("stat_Run","" + dataInfo.statRunCount);
            setStatValue("stat_Win","" + dataInfo.statWinCount);
            setStatValue("stat_Enemy","" + dataInfo.statKillCount);
            setStatValue("stat_Perk","" + dataInfo.statPerkCount);
            setStatValue("stat_Room","" + dataInfo.statRoomCount);
        }

        // load in the leaderboard
        loadLeaderboard();
    }

    public void unloadMenu(){
        
    }
}
