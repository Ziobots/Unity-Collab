/*******************************************************************************
* Name : fadeTransition.cs
* Section Description : This code handles the fade transition between scenes
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

public class fadeTransition : MonoBehaviour
{
    private float fadeTime = 0.4f;
    public bool transitioning = false;

    // for tweening
    private void setPivot(Vector2 value){
        Transform fade = gameObject.transform.Find("fade");
        if (fade){
            fade.gameObject.GetComponent<RectTransform>().pivot = value;
        }
    }

    // for when the tween is completed
    public void hideFade(){
        Transform fade = gameObject.transform.Find("fade");
        if (fade){
            fade.gameObject.SetActive(false);
        }

        transitioning = false;
    }

    // wait function
    private IEnumerator doWait(System.Action onComplete){
        yield return new WaitForSecondsRealtime(fadeTime/2);
        // run on complete
        onComplete();
    }

    private void halfFade(System.Action onComplete,bool rightToLeft){
        Vector2 startPivot = new Vector2(0f,0.5f);
        Vector2 endPivot = new Vector2(1f,0.5f);
        if (!rightToLeft){
            startPivot = new Vector2(1f,0.5f);
            endPivot = new Vector2(0f,0.5f);
        }

        onComplete();

        LeanTween.value(gameObject,new Vector2(0.5f,0.5f),endPivot,fadeTime/2).setIgnoreTimeScale(true).setEaseLinear().setOnUpdateVector2(setPivot).setOnComplete(hideFade);
    }

    // do a fade transition
    public void startFade(System.Action onComplete,bool rightToLeft){
        transitioning = true;

        // set the direction
        Vector2 startPivot = new Vector2(0f,0.5f);
        Vector2 endPivot = new Vector2(1f,0.5f);
        if (!rightToLeft){
            startPivot = new Vector2(1f,0.5f);
            endPivot = new Vector2(0f,0.5f);
        }

        // tween the fade
        LeanTween.cancel(gameObject);
        setPivot(startPivot);
        Transform fade = gameObject.transform.Find("fade");
        if (fade){
            fade.gameObject.SetActive(true);
        }
        
        LeanTween.value(gameObject,startPivot,new Vector2(0.5f,0.5f),fadeTime/2).setIgnoreTimeScale(true).setEaseLinear().setOnUpdateVector2(setPivot).setOnComplete(delegate(){
            halfFade(onComplete,rightToLeft);
        });
    }

    // hide this at the start
    void Start(){
        Transform fade = gameObject.transform.Find("fade");
        if (fade){
            fade.gameObject.SetActive(false);
        }
    }
}
