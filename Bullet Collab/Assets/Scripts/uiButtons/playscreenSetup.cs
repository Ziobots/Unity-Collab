/*******************************************************************************
* Name : loginSetup.cs
* Section Description : This code handles input fields.
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 11/04/22  0.10                 DS              Made the thing
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using PlayFab;
using PlayFab.ClientModels;

public class playscreenSetup : MonoBehaviour
{
    // Base Data Stuff
    public GameObject dataManager;
    [HideInInspector] public sharedData dataInfo;

    // Game Data Stuff
    public GameObject gameManager;
    [HideInInspector] public gameLoader gameInfo;

    // main stuff
    public GameObject mainMenu;
    public GameObject gameMenu;
    public GameObject pauseMenu;
    public GameObject cursorObj;
    public GameObject errorMenu;
    public GameObject continueButton;
    public GameObject leaderboardMenu;

    // transition obj
    public GameObject transitioner;   
    private EventSystem system;
    private bool playMenuActive = false;

    // Player Variables
    public GameObject playerObj;
    private string userName;

    public void newgameButton(){
        if (playMenuActive){
            setupMenu();
            playMenuActive = false;
            transitioner.GetComponent<fadeTransition>().startFade(delegate{
                unloadMenu();
                gameInfo.newGameTransition();
            },false);
        }
    }

    public void leaderboardButton(){
        if (playMenuActive){
            setupMenu();
            playMenuActive = false;
            transitioner.GetComponent<fadeTransition>().startFade(delegate{
                unloadMenu();
                leaderboardMenu.SetActive(true);
                leaderboardMenu.GetComponent<leaderSetup>().loadMenu();
            },true);
        }
    }

    public void unloadMenu(){
        errorMenu.GetComponent<errorPopup>().hideError();
        gameObject.SetActive(false);

        // remove any created instances and hide any objects that were created
    }

    public void setupMenu(){
        // get event sytem
        system = EventSystem.current;

        // Get data management script
        if (dataManager != null){
            dataInfo = dataManager.GetComponent<sharedData>();
        }

        // get game management script
        if (gameManager != null){
            gameInfo = gameManager.GetComponent<gameLoader>();
        }
    }

    public void loadMenu(){
        setupMenu();

        // wipe workspace
        gameInfo.clearGameObj();

        // disable the player controller
        if (playerObj != null){
            playerObj.SetActive(false);
        }

        // disable reticle
        mouseCursor cursorData = cursorObj.GetComponent<mouseCursor>();
        cursorData.reticleActive = false;
        cursorData.updateHover(false);

        // show login menu
        mainMenu.SetActive(true);
        gameMenu.SetActive(false);
        pauseMenu.SetActive(false);

        // disable the player controller
        if (playerObj != null){
            playerObj.SetActive(false);
        }

        // check for previous run
        if (continueButton != null){
            string buttonText = "Start Run";
             if (dataInfo.currentTempData.room >= 1){
                buttonText = "Continue Run";
             }

             continueButton.transform.Find("Holder").Find("textField").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = buttonText;
        }

        playMenuActive = true;
    }
}
