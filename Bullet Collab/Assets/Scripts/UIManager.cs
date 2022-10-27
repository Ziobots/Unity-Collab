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

        for (int i = 1; i < heartMax; i++){
            // Health is split into wholes and halves, maybe temporary hearts
            int baseIndex = i - (1 - i % 2);
            GameObject newHeart = GameObject.Find(healthBar.name + "/heart_" + baseIndex);
            if (newHeart == null){
                newHeart = Instantiate(heartPrefab,healthBar.transform);
            }

            // Setup the heart
            if (newHeart != null){
                newHeart.name = "heart_" + baseIndex;
                if (i > dataInfo.maxHealth){
                    // temporary heart?
                }else if (i <= dataInfo.currenthealth){
                    if (i % 2 == 0){
                        newHeart.GetComponent<Image>().sprite = Resources.Load<Sprite>("Hearts_0");
                    }else{
                        newHeart.GetComponent<Image>().sprite = Resources.Load<Sprite>("Hearts_1");
                    }
                }else{
                    newHeart.GetComponent<Image>().sprite = Resources.Load<Sprite>("Hearts_2");
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
