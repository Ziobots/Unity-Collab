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
        if (dataInfo != null && statPanel != null){
            // Volume Fields
            statPanel.transform.Find("stat_MasterVolume").Find("Slider").gameObject.GetComponent<Slider>().value = dataInfo.masterVolume;
            statPanel.transform.Find("stat_MusicVolume").Find("Slider").gameObject.GetComponent<Slider>().value = dataInfo.musicVolume;
            statPanel.transform.Find("stat_SFXVolume").Find("Slider").gameObject.GetComponent<Slider>().value = dataInfo.gameVolume;

            // Bool Fields
            statPanel.transform.Find("stat_Mobile").Find("Toggle").gameObject.GetComponent<Toggle>().isOn = dataInfo.mobileControls;
            statPanel.transform.Find("stat_Particles").Find("Toggle").gameObject.GetComponent<Toggle>().isOn = dataInfo.particleFX;
        }

        menuActive = true;
    }

    public void unloadMenu(){
        gameObject.SetActive(false);
    }
}
