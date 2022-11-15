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
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class endScreen : MonoBehaviour
{
    // Base Data Stuff
    public GameObject dataManager;
    [HideInInspector] public sharedData dataInfo;

    // Game Data Stuff
    public GameObject gameManager;
    [HideInInspector] public gameLoader gameInfo;

    // ui variables
    public GameObject statHolder;
    public GameObject popupUI;
    public GameObject cursorObj;
    public GameObject playerObj;
    public GameObject transitioner;  
    public GameObject playMenu;
    public GameObject mainMenu;
    public GameObject gameMenu;

    private bool optionMade = false;

    // Blur Obj
    public GameObject blurObj;
    private PostProcessVolume postVolume;
    private DepthOfField blurField;
    
    private void setStatValue(string statName, string statValue){
        Transform statObj = statHolder.transform.Find(statName);
        if (statObj != null && statObj.gameObject != null){
            statObj.Find("valueField").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = statValue;
        }
    }

    // for tweening
    private void setPivot(Vector2 value){
        gameObject.transform.Find("Holder").gameObject.GetComponent<RectTransform>().pivot = value;
    }

    // buttons
    public void replayButton(){
        if (gameInfo != null && !optionMade){
            optionMade = true;
            dataInfo.currentTempData = new tempDataClass();
            transitioner.GetComponent<fadeTransition>().startFade(delegate{
                unloadMenu();
                gameInfo.newGameTransition();
            },false);
        }
    }

    public void menuButton(){
        if (gameInfo != null && !optionMade){
            optionMade = true;

            transitioner.GetComponent<fadeTransition>().startFade(delegate{
                unloadMenu();
                mainMenu.SetActive(false);
                gameMenu.SetActive(false);
                playMenu.SetActive(true);
                playMenu.GetComponent<playscreenSetup>().loadMenu();
            },false);


        }
    }

    public void unloadMenu(){
        blurObj.SetActive(false);
    }

    public string padInteger(int padNumber){
        if (Mathf.Abs(padNumber) < 10){
            return "0" + Mathf.Abs(padNumber);
        }

        return "" + padNumber;
    }

    // get readable time
    public string getReadableTime(float duration){
        int hours = (int) Mathf.Floor(duration/60f/60f);
        duration -= hours * 60f * 60f;
        int minutes = (int) Mathf.Floor(duration/60f);
        duration -= minutes * 60f;
        return padInteger(hours) + ":" + padInteger(minutes) + ":" + padInteger((int)duration);
    }

    // load this menu
    public void loadMenu(){
        setupMenu();

        // set the stats from shared info module
        if (dataInfo != null && statHolder != null){
            setStatValue("stat_Time",getReadableTime(dataInfo.gameEndTime - dataInfo.gameStartTime));
            setStatValue("stat_Enemy","" + dataInfo.enemiesKilled);
            setStatValue("stat_Perk","" + dataInfo.perkIDList.Count);
            setStatValue("stat_Room","" + (dataInfo.currentRoom - 1));
            setStatValue("stat_Score","" + dataInfo.totalScore);
        }

        // set the blur
        blurField.focusDistance.value = 2f;
        blurObj.SetActive(true);

        // hide the main game ui
        if (gameMenu != null){
            gameMenu.SetActive(false);
        }

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

        gameObject.SetActive(true);
        Camera.current.GetComponent<CameraBehavior>().factorMouse = false;


        // tween the fade
        LeanTween.cancel(gameObject);
        LeanTween.value(gameObject,new Vector2(0.5f,1f),new Vector2(0.5f,0.5f),0.4f).setIgnoreTimeScale(true).setEaseOutBack().setOnUpdateVector2(setPivot);

        optionMade = false;
    }

    public void setupMenu(){
        // Get data management script
        if (dataManager != null){
            dataInfo = dataManager.GetComponent<sharedData>();
        }

        // get game management script
        if (gameManager != null){
            gameInfo = gameManager.GetComponent<gameLoader>();
        }

        // blur game
        postVolume = blurObj.GetComponent<PostProcessVolume>();
        if (postVolume){
            postVolume.profile.TryGetSettings(out blurField);
        }
    }

    private void Start() {
        setupMenu();
    }
}
