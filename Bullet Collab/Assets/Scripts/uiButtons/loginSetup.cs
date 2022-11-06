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

public class loginSetup : MonoBehaviour
{
    // Base Data Stuff
    public GameObject dataManager;
    [HideInInspector] public sharedData dataInfo;

    // login variables
    public TMPro.TMP_InputField emailField;
    public TMPro.TMP_InputField passwordField;

    // Create account variables
    public TMPro.TMP_InputField createEmailField;
    public TMPro.TMP_InputField createNameField;
    public TMPro.TMP_InputField createPasswordField;

    // main stuff
    public GameObject mainMenu;
    public GameObject gameMenu;
    public GameObject createMenu;
    public GameObject loginMenu;

    // start game variables
    public GameObject enterMenu;
    private bool startVisible = true;
    private float startTime = 0;
    private float gameLoadTime = 0;
    private bool anyKeyPressed = false;

    // error variables
    public GameObject errorMenu;
    private Color32 errorColor = new Color32(253,106,106,255);

    // transition obj
    public GameObject transitioner;   

    public Button submitButton;
    private EventSystem system;
    public GameObject cursorObj;

    // Player Variables
    public GameObject playerObj;
    private string userName;

    // button functions
    public void loginButton(){
        if (emailField.text == ""){
            errorMenu.GetComponent<errorPopup>().displayError("Please enter a valid email address.",errorColor);
            return;
        }

        if (passwordField.text == ""){
            errorMenu.GetComponent<errorPopup>().displayError("Please enter a password.",errorColor);
            return;
        }else if (passwordField.text.Length < 6){
            errorMenu.GetComponent<errorPopup>().displayError("Passwords must have a minimum 6 characters.",errorColor);
            return;
        }

        var request = new LoginWithEmailAddressRequest {
            Email = emailField.text,
            Password = passwordField.text
        };

        PlayFabClientAPI.LoginWithEmailAddress(request,onLoginSuccess,onPlayfabError);
    }

    public void forgotPasswordButton(){
        if (emailField.text == ""){
            errorMenu.GetComponent<errorPopup>().displayError("Please enter a valid email address.",errorColor);
            return;
        }

        var request = new SendAccountRecoveryEmailRequest {
            Email = emailField.text,
            TitleId = "1853B"
        };

        PlayFabClientAPI.SendAccountRecoveryEmail(request,onPasswordEmail,onPlayfabError);
    }

    // to access the account creation menu
    public void c_A_B(){
        errorMenu.GetComponent<errorPopup>().hideError();
        loginMenu.SetActive(false);
        createMenu.SetActive(true);
    }

    public void createAccountButton(){
        if (createMenu != null && loginMenu != null){
            errorColor = new Color32(253,106,106,255);
            transitioner.GetComponent<fadeTransition>().startFade(c_A_B,true);
        }
    }

    // to make the account
    public void newAccountButton(){
        if (createEmailField.text == ""){
            errorMenu.GetComponent<errorPopup>().displayError("Please enter a valid email address.",errorColor);
            return;
        }

        if (createNameField.text == ""){
            errorMenu.GetComponent<errorPopup>().displayError("Please enter a valid user name.",errorColor);
            return;
        }

        if (createPasswordField.text == ""){
            errorMenu.GetComponent<errorPopup>().displayError("Please enter a password.",errorColor);
            return;
        }else if (createPasswordField.text.Length < 6){
            errorMenu.GetComponent<errorPopup>().displayError("Passwords must have a minimum 6 characters.",errorColor);
            return;
        }

        var request = new RegisterPlayFabUserRequest {
            Email = createEmailField.text,
            Password = createPasswordField.text,
            Username = createNameField.text,
            RequireBothUsernameAndEmail = true
        };

        emailField.text = createEmailField.text;
        passwordField.text = createPasswordField.text;

        PlayFabClientAPI.RegisterPlayFabUser(request,onRegisterSuccess,onPlayfabError);
    }


    public void guestButton(){
        closeMenu();
    }

    public void b_B(){
        errorMenu.GetComponent<errorPopup>().hideError();
        loginMenu.SetActive(true);
        createMenu.SetActive(false);
    }

    public void backButton(){
        if (createMenu != null && loginMenu != null){
            errorColor = new Color32(247,192,74,255);
            transitioner.GetComponent<fadeTransition>().startFade(b_B,false);
        }
    }

    // Playfab events

    private void onPlayfabError(PlayFabError error){
        print(error.ErrorMessage);
        errorMenu.GetComponent<errorPopup>().displayError(error.ErrorMessage,errorColor);
    }

    private void onLoginSuccess(LoginResult result){
        print("login success");

        // update the shared data
        dataInfo.sessionTicket = result.SessionTicket;
        dataInfo.userID = result.PlayFabId;
        dataInfo.loggedIn = true;

        closeMenu();
    }

    private void onRegisterSuccess(RegisterPlayFabUserResult result){
        print("register success");
        loginButton();
    }

    private void onPasswordEmail(SendAccountRecoveryEmailResult result){
        print("Sent email");
    }

    public void c_M(){
        errorMenu.GetComponent<errorPopup>().hideError();

        // enable the player controller
        if (playerObj != null){
            playerObj.SetActive(true);
        }

        // remove input data
        createEmailField.text = "";
        createPasswordField.text = "";
        createNameField.text = "";

        emailField.text = "";
        passwordField.text = "";

        // enable reticle
        mouseCursor cursorData = cursorObj.GetComponent<mouseCursor>();
        cursorData.reticleActive = true;
        cursorData.updateHover(false);

        // show game menu
        loginMenu.SetActive(false);
        createMenu.SetActive(false);
        mainMenu.SetActive(false);
        gameMenu.SetActive(true);
        enterMenu.SetActive(false);

        // continue the last run --------- MOVE THIS LATER TO THE CONTINUE GAME BUTTON
        dataInfo.getTemporaryData();
    }

    public void closeMenu(){
        errorColor = new Color32(253,106,106,255);
        transitioner.GetComponent<fadeTransition>().startFade(c_M,true);
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
        loginMenu.SetActive(true);
        createMenu.SetActive(false);
        mainMenu.SetActive(true);
        gameMenu.SetActive(false);
        enterMenu.SetActive(false);

        errorColor = new Color32(247,192,74,255);
    }

    public void loadEnter(){
        // get event sytem
        system = EventSystem.current;

        // hide other menus
        createMenu.SetActive(false);
        gameMenu.SetActive(false);
        enterMenu.SetActive(true);
        loginMenu.SetActive(true);
        mainMenu.SetActive(true);

        // disable reticle
        mouseCursor cursorData = cursorObj.GetComponent<mouseCursor>();
        cursorData.reticleActive = false;
        cursorData.updateHover(false);

        // disable the player controller
        if (playerObj != null){
            playerObj.SetActive(false);
        }
    }

    // Start is called before the first frame update
    private bool gameStarted = false;
    private void Start(){
        // get event sytem
        system = EventSystem.current;
        gameLoadTime = Time.fixedTime;

        loadEnter();
        gameStarted = true;
    }
    
    // Update is called once per frame
    private void Update(){
        if (!gameStarted){
            return;
        }

        if (system.currentSelectedGameObject){
            inputSetup buttonSetup = system.currentSelectedGameObject.GetComponent<inputSetup>();
            if (buttonSetup){
                if (Input.GetKeyDown(KeyCode.Tab) && buttonSetup.tabSelect != null && buttonSetup.tabSelect.GetComponent<Selectable>()){
                    buttonSetup.onDeSelected();

                    buttonSetup.tabSelect.GetComponent<Selectable>().Select();

                    buttonSetup = system.currentSelectedGameObject.GetComponent<inputSetup>();
                    buttonSetup.onSelected();
                }    
            }
        }

        // check if enter pressed
        if (loginMenu.activeSelf){
            if (emailField && passwordField && submitButton){
                if (Input.GetKeyDown(KeyCode.Return)){
                    if (emailField.text != "" && passwordField.text != ""){
                        submitButton.onClick.Invoke();
                    }
                }
            }
        }

        // do main menu stuff
        if (enterMenu && enterMenu.activeSelf){
            // animate start game text
            float blipTime = startVisible ? 1f : 0.1f;
            if (Time.fixedTime - startTime >= blipTime){
                startVisible = !startVisible;
                startTime = Time.fixedTime;
                enterMenu.transform.Find("title").gameObject.SetActive(startVisible);
            }

            // check for any key press
            if (Time.fixedTime - gameLoadTime >= 0.5f){
                if (Input.anyKey && !anyKeyPressed){
                    anyKeyPressed = true;
                    transitioner.GetComponent<fadeTransition>().startFade(loadMenu,true);
                }
            }
        }
    }
    
}
