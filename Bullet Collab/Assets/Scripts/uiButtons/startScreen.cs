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
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class startScreen : MonoBehaviour
{
    // start variables
    private GameObject enterMenu;
    private bool startVisible = true;
    private float startTime = 0;
    private float gameLoadTime = 0;
    private bool anyKeyPressed = false;
    private float enemySpinTime = 0;
    public GameObject fallObjPrefab;

    // transition obj
    public GameObject transitioner; 
    public GameObject loginScreen;

    // Sound Stuff
    public AudioSource startNoise;

    // Start is called before the first frame update
    private void Start(){
        // get event sytem
        gameLoadTime = Time.fixedTime;
    }

    // enemy tween functions
    private void spawnRotation(float value){
        Quaternion setRotationEuler = Quaternion.Euler(0f, value, 0f);
        enterMenu.transform.Find("backHolder").Find("enemy").gameObject.GetComponent<RectTransform>().rotation = setRotationEuler;
    }

    private void spawnFinished(){
        spawnRotation(0f);
    }

    private void spawnAnimation(){
        spawnRotation(0);
        LeanTween.cancel(enterMenu.transform.Find("backHolder").Find("enemy").gameObject);
        LeanTween.value(enterMenu.transform.Find("backHolder").Find("enemy").gameObject,0f,360f * -2f,1.5f).setEaseOutBack().setOnUpdate(spawnRotation).setOnComplete(spawnFinished);
    }

    public Rect GetScreenCoordinates(RectTransform baseRect){
        var worldCorners = new Vector3[4];
        baseRect.GetWorldCorners(worldCorners);
        return new Rect(worldCorners[0].x,worldCorners[0].y,worldCorners[2].x - worldCorners[0].x,worldCorners[2].y - worldCorners[0].y);
    }

    public void shootGun(){
        RectTransform point = enterMenu.transform.Find("backHolder").Find("point").gameObject.GetComponent<RectTransform>();
        GameObject fallObj = Instantiate(fallObjPrefab,enterMenu.transform.Find("backHolder"));
        if (fallObj != null){
            RectTransform backRect = enterMenu.transform.Find("backHolder").GetComponent<RectTransform>();
            Vector3 spawnPosition = point.anchoredPosition;
            fallObj.transform.localPosition = spawnPosition;

            fallObj.GetComponent<RectTransform>().localScale = new Vector2(0.5f,0.5f);
            fallObj.GetComponent<Image>().sprite = Resources.Load<Sprite>("theBullet 1");
            fallObj.GetComponent<fallingObj>().swtichAxis = true;
            fallObj.GetComponent<fallingObj>().fallSpeed = 9 * 0.5f;
            fallObj.GetComponent<fallingObj>().rotationSpeed = 0;
            fallObj.GetComponent<fallingObj>().parentRect = backRect;

            GameObject gun = enterMenu.transform.Find("backHolder").Find("hero").Find("gun").Find("pistol").gameObject;
            LeanTween.cancel(gun);
            LeanTween.value(gun,0.5f,0.45f,0.1f).setEaseOutQuad().setOnUpdate(delegate(float value){
                gun.GetComponent<RectTransform>().pivot = new Vector2(value,0.5f);
            }).setIgnoreTimeScale(true);

            LeanTween.value(gun,0.45f,0.5f,0.1f).setEaseOutQuad().setOnUpdate(delegate(float value){
                gun.GetComponent<RectTransform>().pivot = new Vector2(value,0.5f);
            }).setDelay(0.1f).setIgnoreTimeScale(true);
        }
    }

    private IEnumerator doWait(System.Action onComplete,float waitTime){
        yield return new WaitForSecondsRealtime(waitTime);
        // run on complete
        onComplete();
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
    
                LeanTween.cancel(enterMenu.transform.Find("title").gameObject);
                LeanTween.value(enterMenu.transform.Find("title").gameObject,startAlpha,endAlpha,0.1f).setIgnoreTimeScale(true).setEaseLinear().setOnUpdate(delegate(float value){
                    enterMenu.transform.Find("title").gameObject.GetComponent<CanvasGroup>().alpha = value;
                });
            }

            // check for any key press
            if (Time.fixedTime - gameLoadTime >= 0.5f){
                if (Input.anyKey && !anyKeyPressed){
                    if (startNoise != null){
                        startNoise.PlayOneShot(startNoise.clip,startNoise.volume);
                    }

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

            setRotationEuler = Quaternion.Euler(0f, 0f, -angle * 0.2f);
            enterMenu.transform.Find("backHolder").gameObject.GetComponent<RectTransform>().rotation = setRotationEuler;
            enterMenu.transform.Find("backHolder").gameObject.GetComponent<RectTransform>().localScale = new Vector3(1 - offset * 0.5f,1 - offset * 0.5f,1);

            setRotationEuler = Quaternion.Euler(0f, 0f, angle * 5f);
            offset = Mathf.Sin(Time.time * 3.2f) * 0.02f;

            enterMenu.transform.Find("backHolder").Find("hero").Find("gun").gameObject.GetComponent<RectTransform>().rotation = setRotationEuler;
            enterMenu.transform.Find("backHolder").Find("enemy").gameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f,0.5f + (offset));

            if (Time.time - enemySpinTime >= ((Mathf.PI / 3f) * 3)){
                enemySpinTime = Time.time;
                shootGun();
                StartCoroutine(doWait(delegate{
                    spawnAnimation();
                },0.6f));
            }
        }
    }
}
