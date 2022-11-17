/*******************************************************************************
* Name : hurtUI.cs
* Section Description : This code handles the hurt effect, red flash thing
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 11/17/22  0.10                 DS              Made the thing
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class hurtUI : MonoBehaviour
{
    // tween function
    private void setScale(Vector3 scale){
        gameObject.GetComponent<RectTransform>().localScale = scale;
    }

    private void setAlpha(float alpha){
        gameObject.GetComponent<CanvasGroup>().alpha = alpha;
    }

    // for when the tween is completed
    public void hideFade(){
        gameObject.SetActive(false);
    }

    // do a fade transition
    public void startFlash(){
        // set the direction
        Vector2 startSize = new Vector3(3f,3f,1f);
        Vector2 endSize = new Vector2(1.5f,1.5f);

        float flashAlpha = 0.5f;
        float flashTime = 0.1f;
        float fadeTime = 0.5f;

        // tween the fade
        LeanTween.cancel(gameObject);

        setScale(startSize);
        setAlpha(0f);

        // do the tweens
        LeanTween.value(gameObject,0,flashAlpha,flashTime).setEaseLinear().setOnUpdate(setAlpha);
        LeanTween.value(gameObject,startSize,endSize,flashTime).setEaseOutQuad().setOnUpdateVector3(setScale).setOnComplete(delegate(){
            // now hide
            LeanTween.value(gameObject,flashAlpha,0,fadeTime).setEaseLinear().setOnUpdate(setAlpha);
            LeanTween.value(gameObject,endSize,startSize,fadeTime).setEaseLinear().setOnUpdateVector3(setScale).setOnComplete(hideFade);
        });


        gameObject.SetActive(true);
    }
}
