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
    private float fadeTime = 0.25f;
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
        yield return new WaitForSeconds(fadeTime/2);
        // run on complete
        onComplete();
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
        LeanTween.value(gameObject,new Vector2(0f,0.5f),new Vector2(1f,0.5f),fadeTime).setIgnoreTimeScale(true).setEaseLinear().setOnUpdateVector2(setPivot).setOnComplete(hideFade);
        
        // activate it
        Transform fade = gameObject.transform.Find("fade");
        if (fade){
            fade.gameObject.SetActive(true);
        }

        // wait until screen is fully covered to continue thread
        StartCoroutine(doWait(onComplete));
    }

    // hide this at the start
    void Start(){
        Transform fade = gameObject.transform.Find("fade");
        if (fade){
            fade.gameObject.SetActive(false);
        }
    }
}
