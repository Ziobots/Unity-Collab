/*******************************************************************************
* Name : fadeTransition.cs
* Section Description : This code handles the error popup used for playfab stuff
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 11/05/22  0.10                 DS              Made the thing
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class errorPopup : MonoBehaviour
{
    // error variables
    public string currentMessage = "";
    private float tweenTime = 0.1f;
    private float displayDuration = 5f;
    private float displayTime = 0f;
    public bool errorVisible = false;

    // positions
    private Vector3 spawnPosition = new Vector3(0f,-50f,0f);
    private Vector3 startPosition = new Vector3(0f,50f,0f);

    // spike variables
    private float spikeTime = 0f;
    private bool spikeBool = false;

    // tween functions
    private void setAnchoredPosition(Vector3 value){
        gameObject.GetComponent<RectTransform>().anchoredPosition = value;
    }

    private void setPanelAlpha(float value){
        gameObject.GetComponent<CanvasGroup>().alpha = value;
    }

    // hide the error popup
    public void hideError(){
        if (errorVisible){
            currentMessage = "";
            displayTime = 0f;
            LeanTween.cancel(gameObject);
            LeanTween.value(gameObject,startPosition,spawnPosition,tweenTime).setIgnoreTimeScale(true).setEaseInBack().setOnUpdateVector3(setAnchoredPosition);
            LeanTween.value(gameObject,1f,0f,tweenTime).setIgnoreTimeScale(true).setEaseOutQuad().setOnUpdate(setPanelAlpha);
            errorVisible = false;
        }
    }

    // make background same size as text width
    private void scaleTextPopup(){
        // change background size to text width
        float width = transform.Find("textLabel").gameObject.GetComponent<TMPro.TextMeshProUGUI>().preferredWidth * 1.2f;
        int spikeNum = (int)Mathf.Ceil(width/120f);

        transform.Find("background").gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2((spikeNum * 120f) + 50f,75f);

        // fix spikes, spikes can only be done in increments of 120 units
        Vector2 spikeSize = new Vector2(spikeNum * 120,45);
        transform.Find("background").Find("spikeTop").gameObject.GetComponent<RectTransform>().sizeDelta = spikeSize;
        transform.Find("background").Find("spikeBottom").gameObject.GetComponent<RectTransform>().sizeDelta = spikeSize;
    }

    // change the error message
    public void displayError(string errorMessage,Color32 backgroundColor){
        // tween the fade
        LeanTween.cancel(gameObject);

        // just bump if already visible
        if (errorVisible){
            Vector3 bump = startPosition + new Vector3(0f,5f,0f);
            LeanTween.value(gameObject,bump,startPosition,tweenTime).setIgnoreTimeScale(true).setEaseOutBack().setOnUpdateVector3(setAnchoredPosition);
            LeanTween.value(gameObject,0f,1f,tweenTime).setIgnoreTimeScale(true).setEaseOutQuad().setOnUpdate(setPanelAlpha);
        }else{
            LeanTween.value(gameObject,spawnPosition,startPosition,tweenTime).setIgnoreTimeScale(true).setEaseOutBack().setOnUpdateVector3(setAnchoredPosition);
            LeanTween.value(gameObject,0f,1f,tweenTime).setIgnoreTimeScale(true).setEaseOutQuad().setOnUpdate(setPanelAlpha);
        }

        // change the text and find the size
        transform.Find("textLabel").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = errorMessage.ToUpper();
        scaleTextPopup();

        // change the color
        transform.Find("background").Find("spikeTop").gameObject.GetComponent<Image>().color = backgroundColor;
        transform.Find("background").Find("spikeBottom").gameObject.GetComponent<Image>().color = backgroundColor;
        transform.Find("background").gameObject.GetComponent<Image>().color = backgroundColor;

        currentMessage = errorMessage;
        displayTime = Time.fixedTime;
        errorVisible = true;
    }

    // this didnt look good so it isnt used
    private void spikeAnimation(){
        if (Time.fixedTime - spikeTime >= 0.25f){
            spikeTime = Time.fixedTime;
            spikeBool = !spikeBool;

            if (spikeBool){
                transform.Find("background").Find("spikeTop").gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("errorSpikes2");
                transform.Find("background").Find("spikeBottom").gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("errorSpikesB_1");
            }else{
                transform.Find("background").Find("spikeTop").gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("errorSpikes2_1");
                transform.Find("background").Find("spikeBottom").gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("errorSpikesB");
            }
        }
    }

    // Start is called before the first frame update
    void Start(){
        errorVisible = true;
        hideError();
    }

    // Update is called once per frame
    void Update(){
        if (errorVisible){
            scaleTextPopup();
            //spikeAnimation();
            
            if (Time.fixedTime - displayTime >= displayDuration){
                hideError();
            }
        }
    }
}
