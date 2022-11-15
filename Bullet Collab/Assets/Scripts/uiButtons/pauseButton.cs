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
    public GameObject gamePanel;
    public RectTransform screenObj;
    public GameObject popupUI;
    public GameObject transitioner;  
    public GameObject playMenu;
    public GameObject mainMenu;
    public GameObject gameMenu;

    // Pause Variables
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
        setupMenu();

        // Get data management script
        if (dataManager != null){
            dataInfo = dataManager.GetComponent<sharedData>();
        }

        gamePaused = true;
        pausePanel.SetActive(true);
        
        // load the perk viewer
        pausePanel.transform.Find("perkPanel").GetComponent<perkView>().loadPerkViewer(1);

        // set the blur
        blurField.focusDistance.value = 0f;
        blurObj.SetActive(true);

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
        setupMenu();
        // Update Mouse
        mouseCursor cursorData = cursorObj.GetComponent<mouseCursor>();
        cursorData.reticleActive = true;
        cursorData.updateHover(false);

        gamePaused = false;
        pausePanel.SetActive(false);
        blurField.focusDistance.value = 10f;
        blurObj.SetActive(true);

        if (gamePanel != null){
            gamePanel.GetComponent<CanvasGroup>().alpha = 1f;
        }

        Time.timeScale = 1f;
    }

    // return to the main menu
    public void returnHome(){
        setupMenu();

        // save and quit
        if (dataInfo != null){
            dataInfo.currentTempData = dataInfo.getTemporaryJSON();
            dataInfo.saveTemporaryData(null);
        }

        print("GO HOME");

        transitioner.GetComponent<fadeTransition>().startFade(delegate{
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

        postVolume = blurObj.GetComponent<PostProcessVolume>();
        if (postVolume){
            postVolume.profile.TryGetSettings(out blurField);
        }
    }

    private void Start(){
        setupMenu();
    }
}
