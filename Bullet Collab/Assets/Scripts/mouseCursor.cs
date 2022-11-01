/*******************************************************************************
* Name : mouseCursor.cs
* Section Description : This code handles the mouse reticle.
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 10/27/22  0.10                 DS              Made the thing
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class mouseCursor : MonoBehaviour
{
    // Menu Objs
    public RectTransform screenObj;

    // cursor Variables
    public bool isHovering = false;
    public bool reticleActive = true;
    public bool smoothMovement = false;
    public bool overwriteMovement = false;

    public float cursorRotation = 0;

    public Vector2 movementPosition = new Vector2();
    [HideInInspector] public Vector2 mousePosition;

    public void cursorHover(){
        isHovering = true;
        gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("hoverCursor");
        gameObject.GetComponent<Image>().color = new Color32(255,255,255,255);
    }

    public void cursorStopHover(){
        isHovering = false;

        if (reticleActive){
            gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Reticle");
            gameObject.GetComponent<Image>().color = new Color32(253,106,106,175);
        }else{
            gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("cursor");
            gameObject.GetComponent<Image>().color = new Color32(255,255,255,255);
        }
    }

    public void updateHover(bool hovering){
        if (hovering){
            cursorHover();
        }else{
            cursorStopHover();
        }
    }

    // Update is called once per frame
    void Update(){
        // Check for mouse cursor
        if (gameObject != null){
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (overwriteMovement){
                mousePosition = movementPosition;
            }else{
                cursorRotation = 0;
            }

            Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(mousePosition);
            Vector2 screenPosition = new Vector2(
            ((ViewportPosition.x*screenObj.sizeDelta.x)-(screenObj.sizeDelta.x*0.5f)),
            ((ViewportPosition.y*screenObj.sizeDelta.y)-(screenObj.sizeDelta.y*0.5f)));
            
            //set the position of cursor from the world point
            RectTransform cursorRect = gameObject.GetComponent<RectTransform>();
            if (cursorRect){
                Vector2 currentPosition = cursorRect.anchoredPosition;
                float cursorDistance = Vector2.Distance(currentPosition,screenPosition);
                float alpha = Mathf.Clamp(Time.fixedDeltaTime * 10f * (cursorDistance/15f),0,1);

                // end smooth mouse
                if (!smoothMovement || cursorDistance <= 5f){
                    smoothMovement = false;
                    alpha = 1;
                }

                print("move cursor");

                Quaternion setRotationEuler = Quaternion.Euler(0, 0, cursorRotation);
                cursorRect.rotation = Quaternion.Lerp(cursorRect.rotation, setRotationEuler, Time.fixedDeltaTime * 10f);
                cursorRect.anchoredPosition = screenPosition;//Vector2.Lerp(currentPosition,screenPosition,alpha);
            }
        }
    }
}
