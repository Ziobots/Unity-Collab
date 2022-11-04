/*******************************************************************************
* Name : UIManager.cs
* Section Description : This code handles any UI elements that reflect the character data.
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 10/27/22  0.10                 DS              Made the thing
* 11/03/22  0.15                 DS              added bullet stuff
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{    
    // Base Data Stuff
    public GameObject dataManager;
    [HideInInspector] public sharedData dataInfo;

    // UI Objects
    public GameObject healthBar;
    public GameObject heartPrefab;

    public GameObject bulletBar;
    public GameObject bulletUIPrefab;

    // Update the health bar visual
    public void updateHealth(){
        // Shield Check?
        int heartMax = (int) dataInfo.maxHealth;
        if ((int) dataInfo.currenthealth > (int) heartMax){
            heartMax = (int) dataInfo.currenthealth;
        }

        for (int i = 2; i <= (int) heartMax; i += 2){
            // Health is split into wholes and halves, maybe temporary hearts
            GameObject newHeart = GameObject.Find(healthBar.name + "/heart_" + i);

            if (newHeart == null){
                newHeart = Instantiate(heartPrefab,healthBar.transform);
            }

            // Setup the heart
            if (newHeart != null){
                newHeart.name = "heart_" + i;
                if (i <= (dataInfo.currenthealth + 1)){
                    if (i > dataInfo.currenthealth){
                        newHeart.GetComponent<Image>().sprite = Resources.LoadAll<Sprite>("Hearts")[1];
                    }else{
                        newHeart.GetComponent<Image>().sprite = Resources.LoadAll<Sprite>("Hearts")[0];
                    }
                }else{
                    newHeart.GetComponent<Image>().sprite = Resources.LoadAll<Sprite>("Hearts")[2];
                }
            }
        }
    }

    // bullet ui stuff
    //private int lastMax = -1;
    private int lastCurrent = -1;

    private void setTextSize(float value){
        GameObject textLabel = bulletBar.transform.Find("reSize").Find("ammoCount").gameObject;
        if (textLabel){
            textLabel.GetComponent<TMPro.TextMeshProUGUI>().fontSize = value;
        }
    }

    public void updateBullet(){
        // create the bullets UI
        for (int i = 1; i <= dataInfo.maxAmmo; i += 1){
            Transform newBulletUI = bulletBar.transform.Find("bulletObj").Find("bulletUI_" + i);

            if (newBulletUI == null){
                newBulletUI = Instantiate(bulletUIPrefab,bulletBar.transform.Find("bulletObj")).transform;
            }

            // Setup the heart 
            if (newBulletUI != null){
                bulletUI bulletUIInfo = newBulletUI.gameObject.GetComponent<bulletUI>();
                bulletUIInfo.ammoIndex = i;
                newBulletUI.gameObject.name = "bulletUI_" + i;
                newBulletUI.transform.Find("bullet").GetComponent<RectTransform>().sizeDelta = new Vector2(22f,22f);

                if (i > dataInfo.currentAmmo){
                    bulletUIInfo.hideBullet();
                }else if(!bulletUIInfo.bulletVisible){
                    bulletUIInfo.showBullet();
                }
            }
        }

        // remove bullets that dont exist
        foreach (Transform child in bulletBar.transform.Find("bulletObj")){
            if (child != null && child.gameObject){
                bulletUI uiInfo = child.gameObject.GetComponent<bulletUI>();
                if (uiInfo && (uiInfo.ammoIndex > dataInfo.maxAmmo || uiInfo.ammoIndex <= 0)){
                    Destroy(child.gameObject);
                }
            }
        }

        // old ver
        //float width = 22f * Mathf.Clamp(dataInfo.maxAmmo,0f,10f);
        //bulletBar.GetComponent<RectTransform>().sizeDelta = new Vector2(width,60f);

        // set the text
        string ammoString = "" + dataInfo.currentAmmo + "<size=35><color=#828282D7>/" + dataInfo.maxAmmo + "</color></size>";
        GameObject textLabel = bulletBar.transform.Find("reSize").Find("ammoCount").gameObject;
        textLabel.GetComponent<TMPro.TextMeshProUGUI>().text = ammoString;

        // bump tween for text
        if (lastCurrent != dataInfo.currentAmmo){
            lastCurrent = dataInfo.currentAmmo;
            
            LeanTween.cancel(textLabel);
            LeanTween.value(textLabel,60f,65f,0.1f).setIgnoreTimeScale(true).setEaseOutQuad().setOnUpdate(setTextSize);
            LeanTween.value(textLabel,65f,60f,0.1f).setIgnoreTimeScale(true).setEaseOutQuad().setOnUpdate(setTextSize).setDelay(0.1f);
        }
    }

    // function for updating ui
    public void updateGameUI(){
        updateHealth();
        updateBullet();
    }

    private void Update() {
        if (bulletBar != null){
            float radialAlpha = 0;
            if (Time.time - dataInfo.reloadStartTime < dataInfo.reloadTime){
                radialAlpha = (Time.time - dataInfo.reloadStartTime) / dataInfo.reloadTime;
            }else if (Time.time - dataInfo.delayStartTime < dataInfo.bulletTime){
                radialAlpha = (Time.time - dataInfo.delayStartTime) / dataInfo.bulletTime;
            }else{
                radialAlpha = 1;
            }

            Image radialImage = bulletBar.transform.Find("reSize").Find("reload").Find("radial").gameObject.GetComponent<Image>();
            radialImage.fillAmount = Mathf.Lerp(radialImage.fillAmount,radialAlpha,0.8f);
        }
    }

    // Start of Game update UI
    private void Start() {
        // Keep UI between Scenes
        DontDestroyOnLoad(gameObject);

        // Get data management script
        if (dataManager != null){
            dataInfo = dataManager.GetComponent<sharedData>();
        }

        updateGameUI();
    }

}
