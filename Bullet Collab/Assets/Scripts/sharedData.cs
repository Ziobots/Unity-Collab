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
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sharedData : MonoBehaviour
{
    // reference for use in other scripts
    public sharedData dataInstance;

    // User Information
    public string userName;
    public int userID; 

    // Persistant Data
    public int runCount;
    public int winCount;
    public float maxScore;
    public float minTime; // in seconds

    // Temporary Data
    public string currentSceneID;
    public int currenthealth;
    public int maxHealth;
    public float currency;
    public List<string> perkIDList = new List<string>();

    // Reset the Run Data
    public void resetTempData() {
        maxHealth = 6;
        currenthealth = maxHealth;
        currency = 0;
        perkIDList = new List<string>();
    }

    private void Start() {
        resetTempData();
        DontDestroyOnLoad(gameObject);
    }

    // runs at the start of the game
    private void Awake() {
        dataInstance = this;
    }
}
