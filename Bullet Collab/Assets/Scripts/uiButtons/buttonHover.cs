/*******************************************************************************
* Name : buttonHover.cs
* Section Description : This code handles buttons being hovered over.
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 10/27/22  0.10                 DS              Made the thing
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class buttonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject cursorObj;
    public Vector2 pivotHover = new Vector2(0.5f,0.5f);
    public float rotationHover = 0f;

    public bool hovering = false;
    private Transform holderUI;

    // for tweening
    private void setPivot(Vector2 value){
        holderUI = gameObject.transform.Find("Holder");
        if (holderUI){
            holderUI.gameObject.GetComponent<RectTransform>().pivot = value;
        }
    }

    private void setRotation(float value){
        holderUI = gameObject.transform.Find("Holder");
        if (holderUI){
            Quaternion setRotationEuler = Quaternion.Euler(0, 0, value);
            holderUI.gameObject.GetComponent<RectTransform>().rotation = setRotationEuler;
        }
    }

    // cursor hovers over button

    public void OnPointerEnter(PointerEventData pointerEventData){
        if (cursorObj != null){
            cursorObj.GetComponent<mouseCursor>().updateHover(true);

            // tween animations
            if (!hovering){
                Vector2 currentPivot = pivotHover;
                float currentRotation = rotationHover;
                holderUI = gameObject.transform.Find("Holder");
                if (holderUI){
                    currentPivot = holderUI.gameObject.GetComponent<RectTransform>().pivot;
                    currentRotation = holderUI.gameObject.GetComponent<RectTransform>().rotation.z;
                }

                LeanTween.cancel(gameObject);
                LeanTween.value(gameObject,currentPivot,pivotHover,0.1f).setIgnoreTimeScale(true).setEaseOutQuad().setOnUpdateVector2(setPivot);
                LeanTween.value(gameObject,currentRotation,rotationHover,0.25f).setIgnoreTimeScale(true).setEaseOutQuad().setOnUpdate(setRotation);
            }

        }
            
        hovering = true;
    }

    // cursor stops hovering over button
    public void OnPointerExit(PointerEventData pointerEventData){
        cursorObj.GetComponent<mouseCursor>().updateHover(false);

        if (hovering){
            // get current pivot
            Vector2 currentPivot = pivotHover;
            float currentRotation = rotationHover;
            holderUI = gameObject.transform.Find("Holder");
            if (holderUI){
                currentPivot = holderUI.gameObject.GetComponent<RectTransform>().pivot;
                currentRotation = holderUI.gameObject.GetComponent<RectTransform>().rotation.z;
            }

            // tween animations
            LeanTween.cancel(gameObject);
            LeanTween.value(gameObject,currentPivot,new Vector2(0.5f,0.5f),0.1f).setIgnoreTimeScale(true).setEaseOutQuad().setOnUpdateVector2(setPivot);
            LeanTween.value(gameObject,currentRotation,0,0.25f).setIgnoreTimeScale(true).setEaseOutQuad().setOnUpdate(setRotation);
        }

        hovering = false;
    }

    private void OnDisable() {
        // check if button is no longer visible
        if (!gameObject.activeInHierarchy || !gameObject.activeSelf){
            OnPointerExit(null);
        }
    }
}
