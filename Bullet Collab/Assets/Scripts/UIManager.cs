/*******************************************************************************
* Name : UIManager.cs
* Section Description : This code handles any UI elements that reflect the character data.
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 10/27/22  0.10                 DS              Made the thing
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{    
    // Base Data Stuff
    public GameObject dataManager;
    [HideInInspector] public sharedData dataInfo;

    // UI Objects
    public GameObject healthBar;
    public GameObject heartPrefab;

    // Update the health bar visual
    public void updateHealth(){
        // Shield Check?
        int heartMax = dataInfo.maxHealth;
        if (dataInfo.currenthealth > heartMax){
            heartMax = dataInfo.currenthealth;
        }

        for (int i = 2; i <= heartMax; i += 2){
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

    // Start of Game update UI
    private void Start() {
        // Get data management script
        if (dataManager != null){
            dataInfo = dataManager.GetComponent<sharedData>();
        }

        updateHealth();
    }


}
