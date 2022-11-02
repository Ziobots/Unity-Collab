/*******************************************************************************
* Name : UIManager.cs
* Section Description : This code handles any UI elements that reflect the character data.
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 10/27/22  0.10                 DS              Made the thing
* 11/02/22  0.10                 DS              added blur + changed ui visual
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

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

    // Blur Obj
    public GameObject blurObj;
    private PostProcessVolume postVolume;
    private DepthOfField blurField;

    // Pause Menu Functions
    public void pauseGame(){
        gamePaused = true;

        pausePanel.SetActive(true);
        Time.timeScale = 0f;

        // Update Mouse
        mouseCursor cursorData = cursorObj.GetComponent<mouseCursor>();
        cursorData.reticleActive = false;
        cursorData.smoothMovement = true;
        cursorData.overwriteMovement = false;
        cursorData.updateHover(false);
    }

    // un-Pause the game
    public void resumeGame(){
        // Update Mouse
        mouseCursor cursorData = cursorObj.GetComponent<mouseCursor>();
        cursorData.reticleActive = true;
        cursorData.updateHover(false);

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

            postVolume = blurObj.GetComponent<PostProcessVolume>();
            if (postVolume){
                postVolume.profile.TryGetSettings(out blurField);
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

        // blur effect
        if (blurObj != null){
            float setBlur = 10;
            if (gamePaused){
                setBlur = 0f;
                if (!blurObj.activeSelf){
                    blurField.focusDistance.value = 10f;
                    blurObj.SetActive(true);
                }
            }else{
                setBlur = 10f;
                if (blurObj.activeSelf){
                    if (blurField.focusDistance.value >= 9f){
                        blurObj.SetActive(false);
                    }
                }
            }

            if (blurObj.activeSelf){
                blurField.focusDistance.value = Mathf.Lerp(blurField.focusDistance.value,setBlur,Time.fixedDeltaTime * 5f);
            }
        }
    }
}
