/*******************************************************************************
* Name : startScreen.cs
* Section Description : This code handles the start screen.
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 11/18/22  0.10                 DS              Made the thing
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class startScreen : MonoBehaviour
{
    // start variables
    private GameObject enterMenu;
    private bool startVisible = true;
    private float startTime = 0;
    private float gameLoadTime = 0;
    private bool anyKeyPressed = false;

    // transition obj
    public GameObject transitioner; 
    public GameObject loginScreen;

    // Start is called before the first frame update
    private void Start(){
        // get event sytem
        gameLoadTime = Time.fixedTime;
    }

    // Update is called once per frame
    void Update(){
        enterMenu = gameObject;

        // do main menu stuff
        if (enterMenu && enterMenu.activeSelf){
            // animate start game text
            float blipTime = startVisible ? 1f : 0.2f;
            if (Time.fixedTime - startTime >= blipTime){
                startVisible = !startVisible;
                startTime = Time.fixedTime;

                float startAlpha = !startVisible ? 1f : 0f;
                float endAlpha = !startVisible ? 0f : 1f;

                //enterMenu.transform.Find("title").gameObject.SetActive(startVisible);
                LeanTween.value(enterMenu.transform.Find("title").gameObject,startAlpha,endAlpha,0.1f).setIgnoreTimeScale(true).setEaseLinear().setOnUpdate(delegate(float value){
                    enterMenu.transform.Find("title").gameObject.GetComponent<CanvasGroup>().alpha = value;
                });
            }

            // check for any key press
            if (Time.fixedTime - gameLoadTime >= 1f){
                if (Input.anyKey && !anyKeyPressed){
                    anyKeyPressed = true;
                    transitioner.GetComponent<fadeTransition>().startFade(delegate{
                        gameObject.SetActive(false);
                        loginScreen.GetComponent<loginSetup>().loadMenu();
                    },true);
                }
            }

            float offset = Mathf.Sin(Time.time * 5f) * 0.05f;
            float angle = Mathf.Sin(Time.time * 3f) * 3f;

            if (offset < 0){
                offset *= 1.2f;
            }else{
                offset *= 0.5f;
            }

            Quaternion setRotationEuler = Quaternion.Euler(0f, 0f, angle);
            enterMenu.transform.Find("logo").gameObject.GetComponent<RectTransform>().rotation = setRotationEuler;
            enterMenu.transform.Find("logo").gameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f,0.5f + (offset * 0.5f));
        }
    }
}
