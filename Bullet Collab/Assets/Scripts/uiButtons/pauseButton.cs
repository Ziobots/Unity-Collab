/*******************************************************************************
* Name : UIManager.cs
* Section Description : This code handles any UI elements that reflect the character data.
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 10/27/22  0.10                 DS              Made the thing
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class pauseButton : MonoBehaviour
{
    // Pause Menu Object
    public GameObject pausePanel;
    public RectTransform screenObj;

    // Pause Variables
    private bool gamePaused = false;
    public GameObject cursorObj;

    public GameObject uiManager;
    [HideInInspector] public UIManager uiUpdate;

    public GameObject levelManager;
    [HideInInspector] public levelLoader levelUpdate;

    // Pause Menu Functions
    public void pauseGame(){
        cursorObj.GetComponent<mouseCursor>().reticleActive = false;
        cursorObj.GetComponent<mouseCursor>().updateHover(cursorObj.GetComponent<mouseCursor>().isHovering);

        gamePaused = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    // un-Pause the game
    public void resumeGame(){
        cursorObj.GetComponent<mouseCursor>().reticleActive = true;
        cursorObj.GetComponent<mouseCursor>().updateHover(false);

        gamePaused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    // return to the main menu
    public void returnHome(string sceneID){
        resumeGame();
        if (levelUpdate != null){
            levelUpdate.LoadScene(sceneID);
        }
    }

    private void Start() {
        // Get UI management script
        if (uiManager != null){
            uiUpdate = uiManager.GetComponent<UIManager>();
        }

        // Get Level management script
        if (levelManager != null){
            levelUpdate = levelManager.GetComponent<levelLoader>();
        }
    }

    private void Update() {
        // check for esc key
        if (Input.GetKeyDown("escape")){
            if (gamePaused){
                resumeGame();
            }else{
                pauseGame();
            }
        }
    }
}
