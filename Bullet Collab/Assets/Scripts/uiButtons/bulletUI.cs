/*******************************************************************************
* Name : bulletUI.cs
* Section Description : just meant for the ammo bar ammo ui thing, holding variables.
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 11/03/22  0.10                 DS              Made the thing
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class bulletUI : MonoBehaviour
{
    // ui variables
    public int ammoIndex;
    public bool bulletVisible = true;
    private float hidePivot = 1.5f;

    // tween functions
    private void setPivot(Vector2 value){
        Transform bullet =  gameObject.transform.Find("bullet");
        if (bullet){
            bullet.gameObject.GetComponent<RectTransform>().pivot = value;
        }
    }

    private void setTransparency(float alpha){
        Transform bullet =  gameObject.transform.Find("bullet");
        if (bullet){
            bullet.gameObject.GetComponent<CanvasGroup>().alpha = alpha;
        }
    }

    public void showBullet(){
        if (!bulletVisible){
            Transform bullet =  gameObject.transform.Find("bullet");
            bullet.gameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f,hidePivot);
            bullet.gameObject.GetComponent<Image>().color = new Color32(247,192,74,255);
            bullet.gameObject.GetComponent<CanvasGroup>().alpha = 1f;

            // tween stuff
            LeanTween.cancel(gameObject);
            LeanTween.value(gameObject,new Vector2(0.5f,hidePivot),new Vector2(0.5f,0.5f),0.1f).setIgnoreTimeScale(true).setEaseOutQuad().setOnUpdateVector2(setPivot);
        }
        
        bulletVisible = true;
    }

    public void hideBullet(){
        if (bulletVisible){
            Transform bullet =  gameObject.transform.Find("bullet");
            bullet.gameObject.GetComponent<Image>().color = new Color32(255,255,255,255);

            LeanTween.cancel(gameObject);
            LeanTween.value(gameObject,1f,0f,0.1f).setIgnoreTimeScale(true).setEaseLinear().setOnUpdate(setTransparency).setDelay(0.02f);
        }
        
        bulletVisible = false;
    }
}
