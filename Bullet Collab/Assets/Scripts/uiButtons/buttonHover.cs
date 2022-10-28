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

    // cursor hovers over button
    public void OnPointerEnter(PointerEventData pointerEventData){
        if (cursorObj != null){
            cursorObj.GetComponent<mouseCursor>().updateHover(true);
        }
    }

    // cursor stops hovering over button
    public void OnPointerExit(PointerEventData pointerEventData){
        cursorObj.GetComponent<mouseCursor>().updateHover(false);
    }
}
