/*******************************************************************************
* Name : settingSetup.cs
* Section Description : This code handles the settings menu.
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 11/16/22  0.10                 DS              Made the thing
* 11/17/22  0.20                 MG              Helped work on the settings menu
* 11/17/22  0.30                 DS              ui stuff
* 11/19/22  0.40                 DS              mobile support
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class settingSetup : MonoBehaviour
{
    // Base Data Stuff
    public GameObject dataManager;
    [HideInInspector] public sharedData dataInfo;

    // ui stuff
    public GameObject transitioner; 

    // menu variables
    public bool menuActive = false;
    public GameObject statPanel;
    public GameObject mobilePanel;

    // sounds
    public AudioMixer masterMixer;

    // setting functions

    public void update_MasterVolume(){
        float value = statPanel.transform.Find("stat_MasterVolume").Find("Slider").gameObject.GetComponent<Slider>().value;
        masterMixer.SetFloat("mixer_Master",Mathf.Log10(value) * 20);
        if (dataInfo != null){
            dataInfo.masterVolume = value;
        }
    }

    public void update_MusicVolume(){
        float value = statPanel.transform.Find("stat_MusicVolume").Find("Slider").gameObject.GetComponent<Slider>().value;
        masterMixer.SetFloat("mixer_Music",Mathf.Log10(value) * 20);
        if (dataInfo != null){
            dataInfo.musicVolume = value;
        }
    }

    public void update_GameVolume(){
        float value = statPanel.transform.Find("stat_SFXVolume").Find("Slider").gameObject.GetComponent<Slider>().value;
        masterMixer.SetFloat("mixer_Sound",Mathf.Log10(value) * 20);
        if (dataInfo != null){
            dataInfo.gameVolume = value;
        }
    }

    public void update_MobileControl(){
        if (dataInfo != null){
            dataInfo.mobileControls = statPanel.transform.Find("stat_Mobile").Find("Toggle").gameObject.GetComponent<Toggle>().isOn;
            mobilePanel.SetActive(dataInfo.mobileControls);
        }
    }

    public void update_Particles(){
        if (dataInfo != null){
            dataInfo.particleFX = statPanel.transform.Find("stat_Particles").Find("Toggle").gameObject.GetComponent<Toggle>().isOn;
        }
    }

    public void setupMenu(){
        // Get data management script
        if (dataManager != null){
            dataInfo = dataManager.GetComponent<sharedData>();
        }
    }

    // set the stats to the default values
    public void resetButton(){
        setupMenu();

        persistDataClass resetValues = new persistDataClass();
        dataInfo.masterVolume = resetValues.masterVolume;
        dataInfo.mobileControls = resetValues.mobileControls;
        dataInfo.musicVolume = resetValues.musicVolume;
        dataInfo.gameVolume = resetValues.gameVolume;
        dataInfo.particleFX = resetValues.particleFX;
        loadMenu();
    }

    // return to previous menu
    public void backButton(){
        if (menuActive){
            menuActive = false;
            transitioner.GetComponent<fadeTransition>().startFade(delegate{
                unloadMenu();
            },false);
        }
    }

    // load in the saved settings values
    public void loadFields(){
        setupMenu();

        if (dataInfo != null && statPanel != null){
            // Volume Fields
            statPanel.transform.Find("stat_MasterVolume").Find("Slider").gameObject.GetComponent<Slider>().value = dataInfo.masterVolume;
            statPanel.transform.Find("stat_MusicVolume").Find("Slider").gameObject.GetComponent<Slider>().value = dataInfo.musicVolume;
            statPanel.transform.Find("stat_SFXVolume").Find("Slider").gameObject.GetComponent<Slider>().value = dataInfo.gameVolume;

            // Bool Fields
            statPanel.transform.Find("stat_Mobile").Find("Toggle").gameObject.GetComponent<Toggle>().isOn = dataInfo.mobileControls;
            statPanel.transform.Find("stat_Particles").Find("Toggle").gameObject.GetComponent<Toggle>().isOn = dataInfo.particleFX;
        }
    }

    public void loadMenu(){
        setupMenu();
        
        // load in the player settings
        loadFields();

        menuActive = true;
    }

    public void unloadMenu(){
        gameObject.SetActive(false);
    }

    private void Start() {
        setupMenu();
    }

    private void Update() {
        if (menuActive){
            if (Input.GetKeyDown("escape")){
                backButton();
            }

            if (Input.GetKeyDown("space")){
                resetButton();
            }
        }
    }
}
