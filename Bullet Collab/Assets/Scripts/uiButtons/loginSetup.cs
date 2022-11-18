/*******************************************************************************
* Name : loginSetup.cs
* Section Description : This code handles input fields.
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 11/04/22  0.10                 DS              Made the thing
* 11/16/22  0.40                 DS              login as guest creates a temp account
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
    public GameObject pauseMenu;
    public GameObject createMenu;
    public GameObject loginMenu;
    public GameObject playMenu;
    public GameObject loadUIScreen;

    // loadbuttons
    public GameObject loginUIButton;
    public GameObject signupUIButton;

    // start game variables
    public GameObject enterMenu;
    private float gameLoadTime = 0;

    // error variables
    public GameObject errorMenu; 
    private Color32 errorColor = new Color32(253,106,106,255);

    // transition obj
    public GameObject transitioner; 
    private bool loginActive = false;  
    private bool closeActive = true;

    public Button submitButton;
    private EventSystem system;
    public GameObject cursorObj;

    // Player Variables
    public GameObject playerObj;
    private string userName;

    // button functions
    public void loginButton(){
        if (!loginActive || dataInfo.loggedIn){
            return;
        }

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

        loginActive = false;
        setButtonVisual(false);
        PlayFabClientAPI.LoginWithEmailAddress(request,onLoginSuccess,onPlayfabError);
    }

    public void forgotPasswordButton(){
        if (!loginActive){
            return;
        }

        if (emailField.text == ""){
            errorMenu.GetComponent<errorPopup>().displayError("Please enter a valid email address.",errorColor);
            return;
        }

        var request = new SendAccountRecoveryEmailRequest {
            Email = emailField.text,
            TitleId = "1853B"
        };

        loginActive = false;
        PlayFabClientAPI.SendAccountRecoveryEmail(request,onPasswordEmail,onPlayfabError);
    }

    public void setButtonVisual(bool active){
        if (loginUIButton != null && signupUIButton != null){
            if (active){
                loginUIButton.transform.Find("Holder").Find("textField").gameObject.SetActive(true);
                signupUIButton.transform.Find("Holder").Find("textField").gameObject.SetActive(true);
                loginUIButton.transform.Find("Holder").Find("loader").gameObject.SetActive(false);
                signupUIButton.transform.Find("Holder").Find("loader").gameObject.SetActive(false);
                loginUIButton.transform.Find("Holder").Find("background").gameObject.GetComponent<Image>().color = new Color32(253,106,106,255);
                signupUIButton.transform.Find("Holder").Find("background").gameObject.GetComponent<Image>().color = new Color32(247,192,74,255);
            }else{
                loginUIButton.transform.Find("Holder").Find("textField").gameObject.SetActive(false);
                signupUIButton.transform.Find("Holder").Find("textField").gameObject.SetActive(false);
                loginUIButton.transform.Find("Holder").Find("loader").gameObject.SetActive(true);
                signupUIButton.transform.Find("Holder").Find("loader").gameObject.SetActive(true);
                loginUIButton.transform.Find("Holder").Find("background").gameObject.GetComponent<Image>().color = new Color32(152,152,152,255);
                signupUIButton.transform.Find("Holder").Find("background").gameObject.GetComponent<Image>().color = new Color32(152,152,152,255);
            }
        }
    }

    // to access the account creation menu
    public void c_A_B(){
        errorMenu.GetComponent<errorPopup>().hideError();
        loginMenu.SetActive(false);
        createMenu.SetActive(true);
    }

    public void createAccountButton(){
        if (createMenu != null && loginMenu != null){
            if (!loginActive){
                return;
            }

            errorColor = new Color32(253,106,106,255);
            transitioner.GetComponent<fadeTransition>().startFade(c_A_B,true);
        }
    }

    // to make the account
    public void newAccountButton(){
        if (!loginActive){
            return;
        }
        
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
            DisplayName = createNameField.text,
            RequireBothUsernameAndEmail = true
        };

        emailField.text = createEmailField.text;
        passwordField.text = createPasswordField.text;
        loginActive = false;
        setButtonVisual(false);

        PlayFabClientAPI.RegisterPlayFabUser(request,onRegisterSuccess,onPlayfabError);
    }


    public void guestButton(){
        if (!loginActive){
            return;
        }

        // create a temporary login so that we can still call api for the leaderboard, etc
        if (!dataInfo.loggedIn){
            print("Login as guest");

            var request = new LoginWithCustomIDRequest {
                CustomId = SystemInfo.deviceUniqueIdentifier,
                CreateAccount = true
            };

            PlayFabClientAPI.LoginWithCustomID(request,onGuestSuccess,onGuestError);
        }

        closeMenu();
    }

    public void b_B(){
        errorMenu.GetComponent<errorPopup>().hideError();
        loginMenu.SetActive(true);
        loginActive = true;
        createMenu.SetActive(false);
    }

    public void backButton(){
        if (createMenu != null && loginMenu != null){
            if (!loginActive){
                return;
            }
            
            loginActive = false;
            errorColor = new Color32(247,192,74,255);
            transitioner.GetComponent<fadeTransition>().startFade(b_B,false);
        }
    }

    // Playfab events

    private void onPlayfabError(PlayFabError error){
        print(error.ErrorMessage);
        errorMenu.GetComponent<errorPopup>().displayError(error.ErrorMessage,errorColor);
        loginActive = true;
        setButtonVisual(true);
    }

    private void onGuestError(PlayFabError error){
        print(error.ErrorMessage);
    }

    private void onGuestSuccess(LoginResult result){
        print("Guest login successful");
        dataInfo.connectedToPlayfab = true;
    }

    private void onLoginSuccess(LoginResult result){
        if (!dataInfo.loggedIn){
            // update the shared data
            dataInfo.sessionTicket = result.SessionTicket;
            dataInfo.userID = result.PlayFabId;
            dataInfo.loggedIn = true;
            loginActive = false;
            dataInfo.connectedToPlayfab = true;
            setButtonVisual(true);

            closeMenu();
        }
    }

    private void onRegisterSuccess(RegisterPlayFabUserResult result){
        print("register success");
        loginActive = true;
        loginButton();
    }

    private void onPasswordEmail(SendAccountRecoveryEmailResult result){
        loginActive = true;
        print("Sent email");
        errorMenu.GetComponent<errorPopup>().displayError("Playfab has sent you an email to reset your password.",errorColor);
    }

    private IEnumerator doWait(System.Action onComplete,float waitTime){
        yield return new WaitForSecondsRealtime(waitTime);
        // run on complete
        onComplete();
    }

    public void gotCurrentRunData(){
        errorMenu.GetComponent<errorPopup>().hideError();

        // show game menu
        loadUIScreen.SetActive(false);
        loginMenu.SetActive(false);
        createMenu.SetActive(false);
        mainMenu.SetActive(false);
        gameMenu.SetActive(false);
        pauseMenu.SetActive(false);
        playMenu.SetActive(true);
        enterMenu.SetActive(false);

        playMenu.GetComponent<playscreenSetup>().loadMenu();
    }

    public void c_M(){
        setButtonVisual(true);

        // remove input data
        createEmailField.text = "";
        createPasswordField.text = "";
        createNameField.text = "";

        emailField.text = "";
        passwordField.text = "";

        if (dataInfo.loggedIn){
            // show load screen
            loadUIScreen.SetActive(true);

            // get the information from the last run
            bool alreadyGot = false;
            dataInfo.onDataGet = delegate{
                if (alreadyGot){
                    return;
                }

                alreadyGot = true;
                dataInfo.onDataGet = null;
                StartCoroutine(doWait(delegate{
                    transitioner.GetComponent<fadeTransition>().startFade(delegate{
                        gotCurrentRunData();
                    },true);
                },1f));
            };

            // get the user information
            StartCoroutine(doWait(delegate{
                dataInfo.getTemporaryData();
            },0.5f));
        }else{// gotta wait for temp login
            // show load screen
            loadUIScreen.SetActive(true);

            StartCoroutine(doWait(delegate{
                transitioner.GetComponent<fadeTransition>().startFade(delegate{
                    gotCurrentRunData();
                },true);
            },1f));
        }
    }

    public void closeMenu(){
        if (!closeActive){
            return;
        }

        closeActive = false;
        loginActive = false;
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
        pauseMenu.SetActive(false);
        enterMenu.SetActive(false);

        errorColor = new Color32(247,192,74,255);
        loginActive = true;
    }

    public void loadEnter(){
        // get event sytem
        system = EventSystem.current;

        // hide other menus
        createMenu.SetActive(false);
        gameMenu.SetActive(false);
        pauseMenu.SetActive(false);
        enterMenu.SetActive(true);
        loginMenu.SetActive(true);
        mainMenu.SetActive(true);
        loadUIScreen.SetActive(false);

        // disable reticle
        mouseCursor cursorData = cursorObj.GetComponent<mouseCursor>();
        cursorData.reticleActive = false;
        cursorData.updateHover(false);

        // disable the player controller
        if (playerObj != null){
            playerObj.SetActive(false);
        }

        loginActive = true;
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
    }
    
}
