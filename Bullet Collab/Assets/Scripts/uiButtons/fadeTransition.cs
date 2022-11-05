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
    private float fadeTime = 5f;
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

    // do a fade transition
    public void startFade(){
        print("START FADE");
        transitioning = true;
        LeanTween.cancel(gameObject);
        LeanTween.value(gameObject,new Vector2(0f,0.5f),new Vector2(1f,0.5f),fadeTime).setIgnoreTimeScale(true).setEaseLinear().setOnUpdateVector2(setPivot).setOnComplete(hideFade);
        Transform fade = gameObject.transform.Find("fade");
        if (fade){
            fade.gameObject.SetActive(true);
        }

        // wait until screen is fully covered to continue thread
        float startTime = Time.fixedTime;
        while (Time.fixedTime - startTime < fadeTime/2f){}
    }

    // hide this at the start
    void Start(){
        Transform fade = gameObject.transform.Find("fade");
        if (fade){
            fade.gameObject.SetActive(false);
        }
    }
}
