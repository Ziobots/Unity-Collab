/*******************************************************************************
* Name : levelLoader.cs
* Section Description : This script will handle using multiple scenes.
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 10/27/22  0.10                 DS              Made the thing
* 10/27/22  0.20                 DS              started integrating game manager
*******************************************************************************/ 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class levelLoader : MonoBehaviour
{
    // Base Data Stuff
    public GameObject dataManager;
    [HideInInspector] public sharedData dataInfo;

    // Game Folders to Clear on Scene Change
    public Transform bulletFolder;
    public Transform enemyFolder;

    // Player
    public Transform playerObj;

    // Clear Folders
    public void clearFolder(Transform folder){
        if (folder != null){
            foreach (Transform chlid in folder){
                GameObject.Destroy(chlid.gameObject);
            }
        }
    }

    public void LoadScene(string sceneID,System.Action onComplete) {
        // Update Player Data
        dataInfo.currentSceneID = sceneID;

        // Load the Scene from ID
        SceneManager.LoadScene(sceneID);

        // Reset Position of Player to 0,0,0
        if (playerObj != null){
            playerObj.position = new Vector2(0,0);
        }

        // Remove Bullets and Enemies
        clearFolder(bulletFolder);
        clearFolder(enemyFolder);

        // maybe do wait stuff
        if (onComplete != null){
            onComplete();
        }
    }

    private void Start() {
        // Get data management script
        if (dataManager != null){
            dataInfo = dataManager.GetComponent<sharedData>();
        }
    }
}
