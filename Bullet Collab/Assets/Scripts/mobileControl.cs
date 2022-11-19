/*******************************************************************************
* Name : mobileControl.cs
* Section Description : This code handles touch detection and the mobile control scheme.
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 11/19/22  0.10                 DS              Made the thing
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mobileControl : MonoBehaviour
{
    public RectTransform movementRect;
    public RectTransform aimRect;

    // Update is called once per frame
    void Update(){
        //Touch movementTouch;
        //Touch aimTouch;

        for (int i = 0; i < Input.touchCount; i++){
            
        }
    }
}
