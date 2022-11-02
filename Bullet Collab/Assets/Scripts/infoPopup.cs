/*******************************************************************************
* Name : infoPopup.cs
* Section Description : 
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 11/2/22  0.10                 DS              Made the thing
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class infoPopup : MonoBehaviour
{
    // popup variables
    public bool popupVisible = true;
    private Dictionary<string, string> currentData;
    private float startHeight = 120f;

    // function for checking if there are any differences in the new and current data
    private bool compare(Dictionary<string, string> A, Dictionary<string, string> B){
        foreach (KeyValuePair<string, string> field in A){
            if (!B.ContainsKey(field.Key)){
                return false;
            }else if (B[field.Key] != field.Value){
                return false;
            }
        }

        return true;
    }

    private bool sameDictionary(Dictionary<string, string> newData){
        if (currentData != null && newData != null){
            return compare(newData,currentData) || compare(currentData,newData);
        }

        return false;
    }

    // overwrite the popup with the new data
    public Dictionary<string, bool> setBoxInfo(Dictionary<string, string> showData){
        Dictionary<string, bool> showUI = new Dictionary<string, bool>();
        bool showBox = false;
        bool showContext = false;
        bool showCost = false;

        if (showData.ContainsKey("Title")){
            showBox = true;
        }else{
            showData.Add("Title","Information");
        }

        if (showData.ContainsKey("Description")){
            showBox = true;
        }else{
            showBox = false;
            showData.Add("Description","Description goes here.");
        }

        if (showData.ContainsKey("Context")){
            showContext = true;
            if (showData.ContainsKey("Cost") && (int.Parse(showData["Cost"]) > 0)){
                showCost = true;
                showData["Context"] = "Buy " + showData["Cost"];
            }
        }else{
            showData.Add("Context","Interact");
        }

        transform.Find("Info").gameObject.transform.Find("perkName").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = showData["Title"];
        transform.Find("Info").gameObject.transform.Find("perkDesc").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = showData["Description"];
        transform.Find("Purchase").gameObject.transform.Find("context").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = showData["Context"].ToUpper();

        showUI.Add("Box", showBox);
        showUI.Add("Context", showContext);
        showUI.Add("Cost", showCost);

        return showUI;
    }

    // equation for the bounce effect when shown
    private float easeOutBack(float alpha){
        float c1 = 3f;
        float c3 = c1 + 1f;

        return 1 + c3 * Mathf.Pow(alpha - 1f, 3f) + c1 * Mathf.Pow(alpha - 1f,2);
    }

    // handles all the visibility stuff
    private IEnumerator fadeUI(bool visible,float duration){
        float progress = 0f;
        float endVisible = visible ? 1f : 0f;

        // get initial values for the lerp
        float startOpacity = transform.gameObject.GetComponent<CanvasGroup>().alpha;

        // gotta use StartCoroutine so use while loop
        while (progress < duration && visible == popupVisible){
            progress += Time.deltaTime;
            float alpha = progress / duration;
            float yOffset = easeOutBack(alpha);
            if (!visible){
                yOffset = easeOutBack(1f - alpha);
            }

            transform.gameObject.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(startOpacity,endVisible,alpha);
            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0,(startHeight - 30f) + (yOffset * 30f),0);

            // wait until next frame to run
            yield return null;
        }

        if (!visible && !popupVisible){
            hidePopup(true);
        }
    }

    // show the popup
    public void showPopup(Dictionary<string, string> showData) {
        if (!popupVisible || !sameDictionary(showData)){
            currentData = showData;

            // hide the ui before showing so we can change the values
            //hidePopup(true);
            popupVisible = true;

            // show the new info
            Dictionary<string, bool> showUI = setBoxInfo(showData);
            transform.Find("Info").gameObject.SetActive(showUI["Box"]);
            transform.Find("Purchase").gameObject.SetActive(showUI["Context"]);

            //show the ui
            gameObject.SetActive(true);
            StartCoroutine(fadeUI(true,0.2f));
        }
    }

    // hide the popup
    public void hidePopup(bool instantHide) {
        if (popupVisible){
            popupVisible = false;

            if (!instantHide){
                StartCoroutine(fadeUI(false,0.15f));
            }else{
                // hide and reset values
                gameObject.SetActive(false);
                transform.Find("Info").gameObject.SetActive(false);
                transform.Find("Purchase").gameObject.SetActive(false);
                transform.gameObject.GetComponent<CanvasGroup>().alpha = 0f;
                gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0,startHeight,0);
            }

        }
    }

    // gotta do this because it will look weird the first time it shows up
    private void Start() {
        popupVisible = true;
        hidePopup(true);
    }
}
