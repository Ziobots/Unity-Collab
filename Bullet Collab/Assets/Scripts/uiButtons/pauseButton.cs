/*******************************************************************************
* Name : UIManager.cs
* Section Description : This code handles any UI elements that reflect the character data.
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 10/27/22  0.10                 DS              Made the thing
* 11/02/22  0.20                 DS              added blur + changed ui visual
* 11/04/22  0.20                 DS              saving + playfab
* 11/16/22  0.40                 DS              added settings menu
* 11/26/22  0.40                 DS              removed blur, was crashing on mobile
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
    public GameObject gamePanel;
    public RectTransform screenObj;
    public GameObject popupUI;
    public GameObject transitioner;  
    public GameObject playMenu;
    public GameObject mainMenu;
    public GameObject gameMenu;
    public GameObject settingsMenu;

    // Pause Variables
    public bool pauseActive = false;
    public bool gamePaused = false;
    public GameObject cursorObj;
    public GameObject playerObj;

    public GameObject uiManager;
    [HideInInspector] public UIManager uiUpdate;

    // Base Data Stuff
    public GameObject dataManager;
    [HideInInspector] public sharedData dataInfo;

    // Game Data Stuff
    public GameObject gameManager;
    [HideInInspector] public gameLoader gameInfo;

    // Blur Obj
    public GameObject blurObj;
    private PostProcessVolume postVolume;
    private DepthOfField blurField;

    // Pause Menu Functions
    public void pauseGame(){
        if (settingsMenu && settingsMenu.activeSelf){
            return;
        }
        
        if (gamePaused){
            return;
        }

        setupMenu();

        pauseActive = true;

        // Get data management script
        if (dataManager != null){
            dataInfo = dataManager.GetComponent<sharedData>();
        }

        gamePaused = true;
        pausePanel.SetActive(true);
        
        // load the perk viewer
        pausePanel.transform.Find("perkPanel").GetComponent<perkView>().loadPerkViewer(1);

        if (dataInfo != null){
            pausePanel.transform.Find("room").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = "" + (dataInfo.currentRoom + 1) + " : Room";
            pausePanel.transform.Find("seed").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = "" + dataInfo.seed + " : seed";
        }

        // set the blur
        //blurField.focusDistance.value = 0f;
        //blurObj.SetActive(true);

        // hide the main game ui
        if (gamePanel != null){
            gamePanel.GetComponent<CanvasGroup>().alpha = 0f;
        }

        Time.timeScale = 0f;

        // remove info popup for interacting
        if (popupUI != null){
            popupUI.GetComponent<infoPopup>().hidePopup(true);
        }

        if (playerObj != null){
            playerObj.GetComponent<interactPlayer>().currentObj = null;
        }


        // Update Mouse
        mouseCursor cursorData = cursorObj.GetComponent<mouseCursor>();
        cursorData.reticleActive = false;
        cursorData.smoothMovement = true;
        cursorData.overwriteMovement = false;
        cursorData.updateHover(false);
    }

    // un-Pause the game
    public void resumeGame(){
        if (settingsMenu && settingsMenu.activeSelf){
            return;
        }

        if (!pauseActive){
            return;
        }

        if (!gamePaused){
            return;
        }


        setupMenu();
        // Update Mouse
        pauseActive = false;

        mouseCursor cursorData = cursorObj.GetComponent<mouseCursor>();
        cursorData.reticleActive = true;
        cursorData.updateHover(false);

        gamePaused = false;
        pausePanel.SetActive(false);
        //blurField.focusDistance.value = 10f;
        //blurObj.SetActive(true);

        if (gamePanel != null){
            gamePanel.GetComponent<CanvasGroup>().alpha = 1f;
        }

        Time.timeScale = 1f;
    }

    public void settingsButton(){
        if (settingsMenu && pauseActive){
            pauseActive = false;
            transitioner.GetComponent<fadeTransition>().startFade(delegate{
                pauseActive = true;
                settingsMenu.SetActive(true);
                settingsMenu.GetComponent<settingSetup>().loadMenu();
            },true);
        }
    }

    // return to the main menu
    public void returnHome(){
        if (!pauseActive){
            return;
        }

        setupMenu();
        pauseActive = false;

        // save and quit
        if (dataInfo != null){
            // get time spent clearing room
            if (gameInfo.roomStartTime != 0){
                float roomTime = Time.time - gameInfo.roomStartTime;
                dataInfo.elapsedTime += roomTime;
                dataInfo.totalScore += (int)(Mathf.Clamp(120 - roomTime,0,120) * 1.5f);
                gameInfo.roomStartTime = 0;
            }

            dataInfo.saveTemporaryData(null);
        }

        print("GO HOME ");

        transitioner.GetComponent<fadeTransition>().startFade(delegate{
            pauseActive = true;
            resumeGame();
            print("DO ACTIVES");
            mainMenu.SetActive(false);
            playMenu.SetActive(true);
            playMenu.GetComponent<playscreenSetup>().loadMenu();
            gameMenu.SetActive(false);
        },false);
    }

    private void setupMenu() {
        // Get UI management script
        if (uiManager != null){
            uiUpdate = uiManager.GetComponent<UIManager>();
        }

        // Get Level management script
        if (gameManager != null){
            gameInfo = gameManager.GetComponent<gameLoader>();
        }

        //postVolume = blurObj.GetComponent<PostProcessVolume>();
        //if (postVolume){
        //    postVolume.profile.TryGetSettings(out blurField);
        //}
    }

    private void Start(){
        setupMenu();
    }
}
