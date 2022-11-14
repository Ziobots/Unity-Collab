/*******************************************************************************
* Name : endScreen.cs
* Section Description : This code handles the end screen after you die.
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 11/14/22  0.10                 DS              Made the thing
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class endScreen : MonoBehaviour
{
    // Base Data Stuff
    public GameObject dataManager;
    [HideInInspector] public sharedData dataInfo;

    // ui variables
    public GameObject statHolder;
    
    private void setStatValue(string statName, string statValue){
        Transform statObj = statHolder.transform.Find(statName);
        if (statObj && statObj.gameObject != null){
            statObj.Find("value_Field").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = statValue;
        }
    }

    // load this menu
    public void loadMenu(){
        setupMenu();

        if (dataInfo != null && statHolder != null){
            setStatValue("stat_Time","0:00:00");
            setStatValue("stat_Enemy","" + dataInfo.enemiesKilled);
            setStatValue("stat_Perk","" + dataInfo.perkIDList.Count);
            setStatValue("stat_Room","" + (dataInfo.currentRoom));
            setStatValue("stat_Score","" + dataInfo.totalScore);
        }

        gameObject.SetActive(true);
    }

    public void setupMenu(){
        // Get data management script
        if (dataManager != null){
            dataInfo = dataManager.GetComponent<sharedData>();
        }
    }

    private void Start() {
        setupMenu();
    }
}
