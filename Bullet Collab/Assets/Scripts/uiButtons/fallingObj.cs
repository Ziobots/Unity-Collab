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

    public bool swtichAxis = false;

    // Update is called once per frame
    void FixedUpdate(){
        if (Time.timeScale <= 0){
            return;
        }

        if (parentRect != null){
            gameObject.GetComponent<RectTransform>().rotation = gameObject.GetComponent<RectTransform>().rotation * Quaternion.Euler(0f, 0f, rotationSpeed * 108f * Time.fixedDeltaTime);
            if (swtichAxis){
                gameObject.GetComponent<RectTransform>().localPosition = gameObject.GetComponent<RectTransform>().localPosition + new Vector3(fallSpeed * 108f * Time.fixedDeltaTime,0f,0f);
                if (gameObject.GetComponent<RectTransform>().localPosition.x >= parentRect.rect.width/2){
                    Destroy(gameObject);
                }
            }else{
                gameObject.GetComponent<RectTransform>().localPosition = gameObject.GetComponent<RectTransform>().localPosition + new Vector3(0f,fallSpeed * 108f * Time.fixedDeltaTime,0f);
                if (gameObject.GetComponent<RectTransform>().localPosition.y >= parentRect.rect.height/2){
                    Destroy(gameObject);
                }
            }
        }
    }
}
