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

public class loginSetup : MonoBehaviour
{
    // login variables
    public TMPro.TMP_InputField emailField;
    public TMPro.TMP_InputField passwordField;
    
    public Button submitButton;
    private EventSystem system;

    // button functions
    public void loginButton(){

    }

    public void forgotPasswordButton(){

    }

    public void createAccountButton(){

    }

    public void guestButton(){

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
