/*******************************************************************************
* Name : settingSetup.cs
* Section Description : This code handles the settings menu.
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 11/16/22  0.10                 DS              Made the thing
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class settingSetup : MonoBehaviour
{
    // Base Data Stuff
    public GameObject dataManager;
    [HideInInspector] public sharedData dataInfo;

    // ui stuff
    public GameObject transitioner; 

    // menu variables
    public bool menuActive = false;

    public void setupMenu(){
        // Get data management script
        if (dataManager != null){
            dataInfo = dataManager.GetComponent<sharedData>();
        }
    }

    public void resetButton(){
        
    }

    public void backButton(){
        if (menuActive){
            menuActive = false;
            transitioner.GetComponent<fadeTransition>().startFade(delegate{
                unloadMenu();
            },false);
        }
    }

    public void loadMenu(){
        setupMenu();
        
        // load in the player settings
        if (dataInfo != null){

        }

        menuActive = true;
    }

    public void unloadMenu(){
        gameObject.SetActive(false);
    }
}
