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

    // main stuff
    public GameObject mainMenu;
    public GameObject gameMenu;
    public GameObject cursorObj;
    public GameObject errorMenu;

    // start game variables
    private bool startVisible = true;
    private float startTime = 0;
    private float gameLoadTime = 0;
    private bool anyKeyPressed = false;

    // transition obj
    public GameObject transitioner;   
    private EventSystem system;
    private bool playMenuActive = false;

    // Player Variables
    public GameObject playerObj;
    private string userName;

    public void newGame(){
        errorMenu.GetComponent<errorPopup>().hideError();
        unloadMenu();

        // enable the player controller
        if (playerObj != null){
            playerObj.SetActive(true);
        }

        // enable reticle
        mouseCursor cursorData = cursorObj.GetComponent<mouseCursor>();
        cursorData.reticleActive = true;
        cursorData.updateHover(false);

        // show game menu
        mainMenu.SetActive(false);
        gameMenu.SetActive(true);

        // continue the last run --------- MOVE THIS LATER TO THE CONTINUE GAME BUTTON
        dataInfo.getTemporaryData();
    }

    public void newgameButton(){
        if (playMenuActive){
            playMenuActive = false;
            transitioner.GetComponent<fadeTransition>().startFade(newGame,false);
        }
    }

    public void unloadMenu(){
        // remove any created instances and hide any objects that were created
    }

    public void loadMenu(){
        // get event sytem
        system = EventSystem.current;

        // Get data management script
        if (dataManager != null){
            dataInfo = dataManager.GetComponent<sharedData>();
        }

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

        // disable the player controller
        if (playerObj != null){
            playerObj.SetActive(false);
        }

        playMenuActive = true;
    }
}
