/*******************************************************************************
* Name : fadeTransition.cs
* Section Description : This code handles the error popup used for playfab stuff
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 11/20/22  0.10                 DS              Made the thing
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class bossHP : MonoBehaviour
{
    // bar variables
    public Entity currentEntity;
    public bool hpVisible = false;
    private float tweenTime = 0.1f;
    public float currentHeath = -1f;
    public float currentMaxHealth = -1f;

    // positions
    private Vector3 spawnPosition = new Vector3(0f,-50f,0f);
    private Vector3 startPosition = new Vector3(0f,50f,0f);

    // tween functions
    private void setAnchoredPosition(Vector3 value){
        gameObject.GetComponent<RectTransform>().anchoredPosition = value;
    }

    private void setHealthSize(float value){
        gameObject.GetComponent<Slider>().value = value;
    }

    private void setPanelAlpha(float value){
        gameObject.GetComponent<CanvasGroup>().alpha = value;
    }

    // hide the error popup
    public void hideHealthBar(){
        if (hpVisible){
            currentEntity = null;
            LeanTween.cancel(gameObject);
            LeanTween.value(gameObject,startPosition,spawnPosition,tweenTime).setIgnoreTimeScale(true).setEaseInBack().setOnUpdateVector3(setAnchoredPosition);
            LeanTween.value(gameObject,1f,0f,tweenTime).setIgnoreTimeScale(true).setEaseOutQuad().setOnUpdate(setPanelAlpha);
            hpVisible = false;
        }
    }

    // update the size of the health bar
    public void updateHealthBar(bool instant){
        if (currentEntity != null && hpVisible){
            if (currentHeath != currentEntity.currentHealth){
                currentHeath = currentEntity.currentHealth;
                currentMaxHealth = currentEntity.maxHealth;


                GameObject healthBar = gameObject.transform.Find("bar").gameObject;
                GameObject backBar = gameObject.transform.Find("background").gameObject;
                float percent = Mathf.Clamp(currentHeath/currentMaxHealth,0,1);
                float startSize = gameObject.GetComponent<Slider>().value;
                
                // tween the fade
                LeanTween.cancel(healthBar);
                if (instant){
                    setHealthSize(percent);
                }else{
                    LeanTween.value(gameObject,startSize,percent,0.1f).setEaseOutQuad().setOnUpdate(setHealthSize);
                }
            }
        }
    }

    // change the error message
    public void displayHealthBar(Entity setEntity){
        // tween the fade
        LeanTween.cancel(gameObject);

        // just bump if already visible
        if (hpVisible){
            Vector3 bump = startPosition + new Vector3(0f,5f,0f);
            LeanTween.value(gameObject,bump,startPosition,tweenTime).setIgnoreTimeScale(true).setEaseOutBack().setOnUpdateVector3(setAnchoredPosition);
        }else{
            LeanTween.value(gameObject,spawnPosition,startPosition,tweenTime).setIgnoreTimeScale(true).setEaseOutBack().setOnUpdateVector3(setAnchoredPosition);
            LeanTween.value(gameObject,0f,1f,tweenTime).setIgnoreTimeScale(true).setEaseOutQuad().setOnUpdate(setPanelAlpha);
        }

        // set the entity info
        currentHeath = -1;
        currentMaxHealth = setEntity.maxHealth;
        currentEntity = setEntity;

        hpVisible = true;
        updateHealthBar(true);
    }

    // Update is called once per frame
    void Update(){
        if (hpVisible){
            updateHealthBar(false);
        }
    }
}
