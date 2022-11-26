/*******************************************************************************
* Name : playscreenSetup.cs
* Section Description : This code handles playscreen ui.
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 11/04/22  0.10                 DS              Made the thing + playfab
* 11/05/22  0.20                 MG              Helped work on the playscreen
* 11/06/22  0.30                 DS              added some graphics
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
    public GameObject settingsMenu;

    // transition obj
    public GameObject transitioner;   
    private EventSystem system;
    private bool playMenuActive = false;

    // Player Variables
    public GameObject playerObj;
    private string userName;

    // splash icons
    public GameObject splashHead;
    public GameObject splashHand;
    public GameObject backPanel;
    public GameObject fallObjPrefab;
    public GameObject logo;
    public float fallTime = 0;

    public void quitButton(){
        if (dataInfo.loggedIn){
            dataInfo.canDoSave = true;
            dataInfo.saveTemporaryData(dataInfo.currentTempData);
        }

        Application.Quit();
    }

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

    public void settingsButton(){
        if (settingsMenu){
            playMenuActive = false;
            transitioner.GetComponent<fadeTransition>().startFade(delegate{
                playMenuActive = true;
                settingsMenu.SetActive(true);
                settingsMenu.GetComponent<settingSetup>().loadMenu();
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
        gameInfo.switchMusic(gameInfo.musicMenu,0.5f);

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
             if (dataInfo.currentTempData != null && dataInfo.currentTempData.room >= 0){
                buttonText = "Continue Run";
             }

             continueButton.transform.Find("Holder").Find("textField").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = buttonText;
        }

        playMenuActive = true;
    }

    public void spawnFallingPerk(){
        if (backPanel != null && backPanel.transform.Find("fallingIcon")){
            GameObject fallObj = Instantiate(fallObjPrefab,backPanel.transform.Find("fallingIcon"));
            if (fallObj != null){
                RectTransform backRect = backPanel.transform.Find("fallingIcon").GetComponent<RectTransform>();
                float yPosition = -backRect.rect.height/2;
                Vector3 spawnPosition = new Vector3(Random.Range(-backRect.rect.width/2*0.8f,backRect.rect.width/2*0.8f),yPosition,0);
                Quaternion spawnRotation = Quaternion.Euler(0f, 0f, Random.Range(0,360));

                fallObj.transform.localPosition = spawnPosition;
                fallObj.transform.rotation = spawnRotation;

                int perkSeed = Mathf.Abs((int)System.DateTime.Now.Ticks);
                perkData chosenPerk = gameObject.GetComponent<perkModule>().getRandomPerk(perkSeed,null,null);
                if (chosenPerk){
                    fallObj.GetComponent<Image>().sprite = chosenPerk.perkIcon;
                }

                fallObj.GetComponent<fallingObj>().fallSpeed = Random.Range(5,9) * 0.5f;
                fallObj.GetComponent<fallingObj>().rotationSpeed = Random.Range(-5,5) * 0.5f;
                if (fallObj.GetComponent<fallingObj>().rotationSpeed == 0){
                    fallObj.GetComponent<fallingObj>().rotationSpeed = 1;
                }

                fallObj.GetComponent<fallingObj>().parentRect = backRect;
            }
        }
    }

    private void Update() {
        float angle = Mathf.Sin(Time.time * 3f) * 3f;
        float offset = Mathf.Sin(Time.time * 5f) * 0.02f;
        Quaternion setRotationEuler = Quaternion.Euler(0f, 0f, angle);
        
        if (splashHand != null && splashHead != null){

            splashHead.GetComponent<RectTransform>().pivot = new Vector2(0.5f,0.5f + offset);
            splashHead.GetComponent<RectTransform>().rotation = setRotationEuler;

            setRotationEuler = Quaternion.Euler(0f, 0f, -angle * 0.5f);
            splashHand.GetComponent<RectTransform>().pivot = new Vector2(0.5f,0.5f + offset);
            splashHand.GetComponent<RectTransform>().rotation = setRotationEuler;

            splashHead.transform.Find("foot1").gameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f,0.5f + (offset * 0.5f));
            splashHead.transform.Find("foot2").gameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f,0.5f - (offset * 0.5f));
        }

        if (backPanel != null && backPanel.transform.Find("background")){
            RawImage scrollBack =  backPanel.transform.Find("background").gameObject.GetComponent<RawImage>();
            if (Time.timeScale > 0){
                scrollBack.uvRect = new Rect(scrollBack.uvRect.position + new Vector2(0,-4.20f) * Time.deltaTime,scrollBack.uvRect.size);
            }
        }

        if (Time.time >= fallTime){
            fallTime = Time.time + Random.Range(70,90)/50;
            spawnFallingPerk();
        }

        if (logo != null){
            offset = Mathf.Sin(Time.time * 5f) * 0.05f;
            angle = Mathf.Sin(Time.time * 3f) * 3f;

            if (offset < 0){
                offset *= 1.2f;
            }else{
                offset *= 0.5f;
            }

            setRotationEuler = Quaternion.Euler(0f, 0f, angle);
            logo.GetComponent<RectTransform>().rotation = setRotationEuler;
            logo.GetComponent<RectTransform>().pivot = new Vector2(0.5f,0.5f + (offset * 0.5f));
        }
    }
}
