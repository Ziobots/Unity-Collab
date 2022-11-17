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
    private float fadeTime = 0.45f;
    public bool buttonActive = false;
    public bool transitioning = false;

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
        buttonActive = false;
        transitioning = false;
        gameObject.SetActive(false);
        onClick = null;
    }
        
    public void showButton(System.Action onComplete){
        if (transitioning){
            return;
        }

        transitioning = true;
        buttonActive = false;

        // set the direction
        Vector2 startPivot = new Vector2(1f,0.5f);
        Vector2 endPivot = new Vector2(0.5f,0.5f);

        // tween the fade
        LeanTween.cancel(gameObject);
        LeanTween.value(gameObject,startPivot,endPivot,fadeTime).setIgnoreTimeScale(true).setEaseOutBack().setOnUpdateVector2(setPivot).setOnComplete(showComplete);
        
        // activate it
        gameObject.SetActive(true);
        onClick = onComplete;
    }

    public void onWaveClick(){
        print("NEXT WAVE CLICKED: " + buttonActive + " _ " + onClick);
        if (onClick != null && buttonActive){
            buttonActive = false;
            onClick();
        }
    }
}
