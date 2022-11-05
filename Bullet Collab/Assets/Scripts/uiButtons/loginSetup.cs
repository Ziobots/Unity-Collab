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
        var request = new LoginWithEmailAddressRequest {
            Email = emailField.text,
            Password = passwordField.text
        };

        PlayFabClientAPI.LoginWithEmailAddress(request,onLoginSuccess,onPlayfabError);
    }

    public void forgotPasswordButton(){
        var request = new SendAccountRecoveryEmailRequest {
            Email = emailField.text,
            TitleId = "1853B"
        };

        PlayFabClientAPI.SendAccountRecoveryEmail(request,onPasswordEmail,onPlayfabError);
    }

    // to access the account creation menu
    public void createAccountButton(){
        if (createMenu != null && loginMenu != null){
            transitioner.GetComponent<fadeTransition>().startFade();
            loginMenu.SetActive(false);
            createMenu.SetActive(true);
        }
    }

    // to make the account
    public void newAccountButton(){
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

    public void backButton(){
        if (createMenu != null && loginMenu != null){
            transitioner.GetComponent<fadeTransition>().startFade();
            loginMenu.SetActive(true);
            createMenu.SetActive(false);
        }
    }

    // Playfab events

    private void onPlayfabError(PlayFabError error){
        print(error.ErrorMessage);
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

    public void closeMenu(){
        transitioner.GetComponent<fadeTransition>().startFade();

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

        // continue the last run --------- MOVE THIS LATER TO THE CONTINUE GAME BUTTON
        dataInfo.getTemporaryData();
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
    }

    // Start is called before the first frame update
    void Start(){
        loadMenu();
    }
    
    // Update is called once per frame
    void Update(){
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

        if (emailField && passwordField && submitButton){
            if (Input.GetKeyDown(KeyCode.Return)){
                if (emailField.text != "" && passwordField.text != ""){
                    submitButton.onClick.Invoke();
                }
            }
        }
    }
    
}
