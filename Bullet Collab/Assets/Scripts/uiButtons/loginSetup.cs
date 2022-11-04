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
    // login variables
    public TMPro.TMP_InputField emailField;
    public TMPro.TMP_InputField passwordField;

    // Create account variables
    public TMPro.TMP_InputField createEmailField;
    public TMPro.TMP_InputField createNameField;
    public TMPro.TMP_InputField createPasswordField;

    // main stuff
    public GameObject createMenu;
    public GameObject loginMenu;

    public Button submitButton;
    private EventSystem system;

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
        print("SKIP LOGIN");

        loginMenu.SetActive(false);
        createMenu.SetActive(false);

    }

    public void backButton(){
        if (createMenu != null && loginMenu != null){
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
    }

    private void onRegisterSuccess(RegisterPlayFabUserResult result){
        print("register success");
        loginButton();
    }

    private void onPasswordEmail(SendAccountRecoveryEmailResult result){
        print("Sent email");
    }

    // Start is called before the first frame update
    void Start(){
        system = EventSystem.current;
    }

    // Update is called once per frame
    void Update(){
        if (system.currentSelectedGameObject){
            inputSetup buttonSetup = system.currentSelectedGameObject.GetComponent<inputSetup>();
            if (buttonSetup){
                if (Input.GetKeyDown(KeyCode.Tab) && buttonSetup.tabSelect != null){
                    buttonSetup.onDeSelected();

                    buttonSetup.tabSelect.Select();

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
