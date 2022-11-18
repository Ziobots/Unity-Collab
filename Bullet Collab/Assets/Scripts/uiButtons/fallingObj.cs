/*******************************************************************************
* Name : fallingObj.cs
* Section Description : for the perks that fall in the background on the playscreen
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 11/18/22  0.10                 DS              Made the thing
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fallingObj : MonoBehaviour
{
    public float fallSpeed = 0;
    public float rotationSpeed = 0;
    public RectTransform parentRect;

    // Update is called once per frame
    void Update(){
        if (parentRect != null){
            gameObject.GetComponent<RectTransform>().rotation = gameObject.GetComponent<RectTransform>().rotation * Quaternion.Euler(0f, 0f, rotationSpeed * 8f * Time.fixedDeltaTime);
            gameObject.GetComponent<RectTransform>().localPosition = gameObject.GetComponent<RectTransform>().localPosition - new Vector3(0f,fallSpeed * 8f * Time.fixedDeltaTime,0f);
            if (gameObject.GetComponent<RectTransform>().localPosition.y <= -parentRect.rect.height/2){
                Destroy(gameObject);
            }
        }
    }
}
