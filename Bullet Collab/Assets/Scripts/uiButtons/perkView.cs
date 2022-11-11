/*******************************************************************************
* Name : perkView.cs
* Section Description : This code handles the perk viewer in the pause menu.
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 11/02/22  0.10                 DS              Made the thing
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class perkView : MonoBehaviour
{
    // Base Data Stuff
    public GameObject dataManager;
    [HideInInspector] public sharedData dataInfo;

    public GameObject uiManager;
    [HideInInspector] public UIManager uiUpdate;

    // Prefabs
    public GameObject dotPrefab;

    // perk view variables
    public int currentPerkIndex = 1;
    public GameObject cursorObj;

    public void loadCountPanel(int perkIndex){
        Transform countPanel = transform.Find("perkCount");

        // remove all old points
         foreach (Transform child in countPanel) {
            GameObject.Destroy(child.gameObject);
        }

        // get the short perk id List
        List<string> shortPerkList = viewerShortList();

        // make new dots if more than one
        if (dotPrefab != null && shortPerkList.Count > 1){
            // make dot for each perk 
            for (int i = 1; i <= shortPerkList.Count; i++){
                GameObject newDot = Instantiate(dotPrefab,new Vector2(),new Quaternion(),countPanel);
                int myIndex = i;

                if (newDot != null){
                    newDot.GetComponent<buttonHover>().cursorObj = cursorObj;

                    // create button code
                    newDot.GetComponent<Button>().onClick.AddListener(delegate{
                        loadPerkViewer(myIndex);
                    });

                    // set color for current index
                    newDot.transform.Find("Holder").GetComponent<RectTransform>().sizeDelta = new Vector2(40f,40f);
                    newDot.transform.Find("Holder").GetComponent<Image>().color = (i == perkIndex) ? new Color32(253,255,255,255) : new Color32(123,123,123,255);
                }
            }
        }
    }

    public void loadPerkViewer(int perkIndex){
        // get dataInfo just in case
        if (dataManager != null){
            dataInfo = dataManager.GetComponent<sharedData>();
        }

        // fix the current index just in case;
        currentPerkIndex = perkIndex;
        switchPage(0);

        // get the short list
        List<string> shortPerkList = viewerShortList();
        Dictionary<string, int> perkCounts = gameObject.GetComponent<perkModule>().countPerks(dataInfo.perkIDList);

        //get the perk to display
        string indexPerkID = shortPerkList[perkIndex - 1];
        perkData perk = gameObject.GetComponent<perkModule>().getPerk(indexPerkID);

        // get number collected
        int perkStack = 0;
        if (perkCounts.ContainsKey(indexPerkID)){
            perkStack = perkCounts[indexPerkID];
        }

        // update the dots
        loadCountPanel(perkIndex);

        // set the new info
        transform.Find("perkName").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = perk.perkName;
        transform.Find("perkDesc").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = perk.perkDescription;
        transform.Find("perkIcon").gameObject.GetComponent<Image>().sprite = perk.perkIcon;

        // get the count string
        string countString = "" + perkStack;
        if (perkStack < 10){
            countString = " " + perkStack;
        }

        // set count tag
        if (perkStack > 1){
            transform.Find("countTag").gameObject.transform.Find("count").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = countString;
        }

        // hide arrows if no next
        transform.Find("forward").gameObject.SetActive(shortPerkList.Count > 1);
        transform.Find("backward").gameObject.SetActive(shortPerkList.Count > 1);

        viewerAnimation(perkStack > 1);
    }

    // scale functions for tweening
    private void scaleCount(Vector3 value){
        transform.Find("countTag").gameObject.GetComponent<RectTransform>().localScale = value;
    }

    private void scaleIcon(Vector3 value){
        transform.Find("perkIcon").gameObject.GetComponent<RectTransform>().localScale = value;
    }

    // hide count on tween complete
    private void countHide(){
        transform.Find("countTag").gameObject.SetActive(false);
    }

    private void viewerAnimation(bool showCount){
        LeanTween.cancel(gameObject);
        LeanTween.moveLocal(gameObject,new Vector3(-210f,10f,0),0.1f).setIgnoreTimeScale(true).setEaseOutQuad();
        LeanTween.moveLocal(gameObject,new Vector3(-210f,0f,0),0.1f).setIgnoreTimeScale(true).setEaseOutQuad().setDelay(0.1f);

        GameObject iconTag = transform.Find("perkIcon").gameObject;
        Vector3 growScale = new Vector3(1.1f,1.1f,1f);
        LeanTween.cancel(iconTag);
        LeanTween.value(iconTag,new Vector3(1f,1f,1f),growScale,0.05f).setIgnoreTimeScale(true).setEaseOutQuad().setOnUpdateVector3(scaleIcon);
        LeanTween.value(iconTag,growScale,new Vector3(1f,1f,1f),0.1f).setIgnoreTimeScale(true).setEaseOutQuad().setOnUpdateVector3(scaleIcon).setDelay(0.05f);

        GameObject countTagObj = transform.Find("countTag").gameObject;
        if (showCount){
            // Get the Values
            Vector3 currentScale = new Vector3(0.5f,0.5f,1f);
            growScale = new Vector3(1.4f,1.4f,1f);
            countTagObj.GetComponent<RectTransform>().localScale = currentScale;
            countTagObj.SetActive(true);

            // Tween Values
            LeanTween.cancel(countTagObj);
            LeanTween.value(countTagObj,currentScale,growScale,0.05f).setIgnoreTimeScale(true).setEaseOutQuad().setOnUpdateVector3(scaleCount);
            LeanTween.value(countTagObj,growScale,new Vector3(1f,1f,1f),0.1f).setIgnoreTimeScale(true).setEaseOutQuad().setOnUpdateVector3(scaleCount).setDelay(0.05f);
        }else if (countTagObj.activeSelf){
            LeanTween.cancel(countTagObj);
            LeanTween.value(countTagObj,countTagObj.GetComponent<RectTransform>().localScale,new Vector3(0.5f,0.5f,1f),0.05f).setIgnoreTimeScale(true).setEaseInBack()
                .setOnUpdateVector3(scaleCount).setOnComplete(countHide);
        }
    }

    public List<string> viewerShortList(){
        List<string> shortPerkList = new List<string>();
        shortPerkList.Add("noPerk_Display");
        if (dataInfo != null && dataInfo.perkIDList.Count > 0){
            List<string> shortCheck = gameObject.GetComponent<perkModule>().shortenList(dataInfo.perkIDList);
            if (shortCheck.Count > 0){
                shortPerkList = shortCheck;
            }
        }

        return shortPerkList;
    }

    public void switchPage(int direction){
        // get the short list
        List<string> shortPerkList = viewerShortList();
        currentPerkIndex += direction;

        // fix the page number
        if (currentPerkIndex > shortPerkList.Count){
            currentPerkIndex = 1;
        }else if (currentPerkIndex < 1){
            currentPerkIndex = shortPerkList.Count;
        }
    }

    public void arrowPress(int direction){
        switchPage(direction);
        loadPerkViewer(currentPerkIndex);

        Transform arrow = direction > 0 ? transform.Find("forward") : transform.Find("backward");
        if (arrow != null & direction != 0){
            LeanTween.cancel(arrow.gameObject);
            RectTransform rect = arrow.gameObject.GetComponent<RectTransform>();
            rect.LeanMove(new Vector3(-10f * direction,0f,0),0.1f).setIgnoreTimeScale(true).setEaseOutQuad();
            rect.LeanMove(new Vector3(-20f * direction,0f,0),0.1f).setIgnoreTimeScale(true).setEaseOutQuad().setDelay(0.1f);
        }
    }

    private void Start() {
        // Get UI management script
        if (uiManager != null){
            uiUpdate = uiManager.GetComponent<UIManager>();
        }

        // Get data management script
        if (dataManager != null){
            dataInfo = dataManager.GetComponent<sharedData>();
        }

        loadPerkViewer(1);
    }
}
