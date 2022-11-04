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

    // Base Data Stuff
    public GameObject dataManager;
    [HideInInspector] public sharedData dataInfo;

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
        gameObject.transform.Find("recharge").gameObject.SetActive(false);
    }

    public void cursorStopHover(){
        isHovering = false;

        if (reticleActive){
            gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("reticle2");
            gameObject.GetComponent<Image>().color = new Color32(0,0,0,118);
            gameObject.transform.Find("recharge").gameObject.SetActive(true);
        }else{
            gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("cursor");
            gameObject.GetComponent<Image>().color = new Color32(255,255,255,255);
            gameObject.transform.Find("recharge").gameObject.SetActive(false);
        }
    }

    public void updateHover(bool hovering){
        if (hovering){
            cursorHover();
        }else{
            cursorStopHover();
        }
    }

    private void reloadRadial(){
        // check if can do radial
        if (dataInfo && Time.timeScale > 0){
            float radialAlpha = 0;
            bool reloading = false;

            // check for which timer to display
            if (Time.time - dataInfo.reloadStartTime < dataInfo.reloadTime){
                reloading = true;
                radialAlpha = (Time.time - dataInfo.reloadStartTime) / dataInfo.reloadTime;
            }else if (Time.time - dataInfo.delayStartTime < dataInfo.bulletTime){
                radialAlpha = (Time.time - dataInfo.delayStartTime) / dataInfo.bulletTime;
            }else{
                radialAlpha = 1;
            }

            // radial image stuff
            Image radialImage = gameObject.transform.Find("recharge").gameObject.GetComponent<Image>();
            if ((radialAlpha >= 1 && dataInfo.currentAmmo > 0) || reloading){
                radialImage.color = Color32.Lerp(radialImage.color,new Color32(253,106,106,230),0.4f);
            }else{
                radialImage.color = Color32.Lerp(radialImage.color,new Color32(255,255,255,230),0.4f);
            }
            
            radialImage.fillAmount = radialAlpha;//Mathf.Lerp(radialImage.fillAmount,radialAlpha,radialAlpha <= 0.05f ? 1f : 0.9f);
        }
    }

    // start is called when loaded
    private void Start() {
        isHovering = true;

        // Get data management script
        if (dataManager != null){
            dataInfo = dataManager.GetComponent<sharedData>();
        }
        
        updateHover(false);
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
                
                Quaternion setRotationEuler = Quaternion.Euler(0, 0, cursorRotation);
                cursorRect.rotation = Quaternion.Lerp(cursorRect.rotation, setRotationEuler, Time.fixedDeltaTime * 10f);
                cursorRect.anchoredPosition = screenPosition;//Vector2.Lerp(currentPosition,screenPosition,alpha);
            }

            // display radial
            reloadRadial();
        }
    }
}
