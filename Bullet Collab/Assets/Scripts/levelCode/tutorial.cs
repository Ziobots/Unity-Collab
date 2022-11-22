/*******************************************************************************
* Name : tutorial.cs
* Section Description : This code handles the tutorial popups.
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 11/21/22  0.10                 DS              Made the thing
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class tutorial : levelData
{
    public GameObject tutorialFrame;
    [TextArea] public string tutorialText = "";

    // tween functions
    private void spawnRotation(float value){
        Quaternion setRotationEuler = Quaternion.Euler(0f, value, 0f);
        tutorialFrame.GetComponent<RectTransform>().rotation = setRotationEuler;
    }

    public override void loadLevel(){
        base.loadLevel();

    }

    public override void onLevelClear(){
        base.onLevelClear();
        spawnRotation(0);
        LeanTween.value(gameObject,0f,90f,.2f).setEaseOutQuad().setOnUpdate(spawnRotation).setOnComplete(delegate(){
            tutorialFrame.transform.Find("words").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = tutorialText;
            LeanTween.value(gameObject,-90f,0f,.3f).setEaseOutBack().setOnUpdate(spawnRotation).setOnComplete(delegate(){
                spawnRotation(0);
            });
        });
    }
}
