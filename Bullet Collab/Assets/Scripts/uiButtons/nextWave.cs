/*******************************************************************************
* Name : nextWave.cs
* Section Description : This code handles the next wave button.
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 11/11/22  0.10                 DS              Made the thing
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nextWave : MonoBehaviour
{
    // continue variables
    private float fadeTime = 0.45f;
    public bool buttonActive = false;
    public bool transitioning = false;
    private Vector2 startPivot = new Vector2(1f,0.5f);
    private Vector2 endPivot = new Vector2(0.5f,0.5f);
    private bool nextVisible = false;

    public System.Action onClick;

    // for tweening
    private void setPivot(Vector2 value){
        gameObject.GetComponent<RectTransform>().pivot = value;
    }

    public void showComplete(){
        buttonActive = true;
        transitioning = false;
    }
    
    public void hideButton(){
        if (!nextVisible){
            return;
        }

        nextVisible = false;
        buttonActive = false;
        transitioning = false;
        onClick = null;

        LeanTween.cancel(gameObject);
        LeanTween.value(gameObject,endPivot,startPivot,0.1f).setIgnoreTimeScale(true).setEaseOutQuad().setOnUpdateVector2(setPivot).setOnComplete(delegate(){
            if (!nextVisible){
                gameObject.SetActive(false);
            }
        });
    }
        
    public IEnumerator doWait(float waitTime, System.Action waitDone){
        yield return new WaitForSeconds(waitTime);
        // run on complete
        waitDone();
    }
        
    public void showButton(System.Action onComplete){
        if (transitioning){
            return;
        }

        if (nextVisible){
            return;
        }

        nextVisible = true;
        transitioning = true;
        buttonActive = false;

        gameObject.GetComponent<buttonHover>().canHover = false;

        // tween the fade
        LeanTween.cancel(gameObject);
        LeanTween.value(gameObject,startPivot,endPivot,fadeTime).setIgnoreTimeScale(true).setEaseOutBack().setOnUpdateVector2(setPivot).setOnComplete(showComplete);
        
        // activate it
        gameObject.SetActive(true);
        onClick = onComplete;

        StartCoroutine(doWait(fadeTime,delegate{
            if (nextVisible){
                gameObject.GetComponent<buttonHover>().canHover = true;
                showComplete();
            }
        }));
    }

    public void onWaveClick(){
        print("NEXT WAVE CLICKED: " + buttonActive + " _ " + onClick);
        if (onClick != null && buttonActive){
            buttonActive = false;
            onClick();
        }
    }

    private void Update() {
        if (Input.GetKeyDown("space") && buttonActive && Time.timeScale > 0){
            onWaveClick();
        }
    }
}
