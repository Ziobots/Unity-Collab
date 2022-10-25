/*******************************************************************************
* Name : sharedData.cs
* Section Description : This is where player data will be stored. Scoreboard Database 
* will reference this for login, name, score, etc.
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 10/23/22  0.10                 DS              Made the thing
*******************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sharedData : MonoBehaviour
{
    // reference for use in other scripts
    public static sharedData dataInstance;

    // User Information
    public static string userName;
    public static int userID; 

    // Persistant Data
    public static int runCount;
    public static int winCount;
    public static float maxScore;
    public static float minTime; // in seconds

    // Temporary Data
    public static float currency;
    public List<string> perkIDList = new List<string>();

    // runs at the start of the game
    private void Awake() {
        dataInstance = this;
    }
}
