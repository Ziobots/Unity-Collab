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
    public bool popupVisible = false;
    private Dictionary<string, string> currentData;

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
            if (showData.ContainsKey("Cost")){
                showCost = true;
                showData["Context"] = "Buy " + showData["Cost"];
            }
        }else{
            showData.Add("Context","Interact");
        }

        transform.Find("Info").gameObject.transform.Find("perkName").gameObject.GetComponent<TextMesh>().text = showData["Title"];
        transform.Find("Info").gameObject.transform.Find("perkDesc").gameObject.GetComponent<TextMesh>().text = showData["Description"];
        transform.Find("Purchase").gameObject.transform.Find("context").gameObject.GetComponent<TextMesh>().text = showData["Context"].ToUpper();

        showUI.Add("Box", showBox);
        showUI.Add("Context", showContext);
        showUI.Add("Cost", showCost);

        return showUI;
    }

    private float easeOutBack(float alpha){
        float c1 = 1.70158f;
        float c3 = c1 + 1f;

        return 1 + c3 * Mathf.Pow(alpha - 1f, 3f) + c1 * Mathf.Pow(alpha - 1f,2);
    }

    private IEnumerator fadeUI(bool visible,float duration){
        float progress = 0f;
        float endVisible = visible ? 1f : 0f;

        // get initial values for the lerp
        float startInfo = transform.Find("Info").gameObject.GetComponent<CanvasGroup>().alpha;
        float startPurchase = transform.Find("Purchase").gameObject.GetComponent<CanvasGroup>().alpha;
        float startHeight = 120f;

        while (progress < duration && visible == popupVisible){
            progress += Time.deltaTime;
            float alpha = progress / duration;
            float yOffset = easeOutBack(alpha);
            if (!visible){
                yOffset = easeOutBack(1f - alpha);
            }

            transform.Find("Info").gameObject.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(startInfo,endVisible,alpha);
            transform.Find("Purchase").gameObject.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(startPurchase,endVisible,alpha);
            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0,startHeight + yOffset,0);

            yield return null;
        }
    }

    // show the popup
    public void showPopup(Dictionary<string, string> showData) {
        if (!popupVisible || !sameDictionary(showData)){
            popupVisible = true;
            currentData = showData;

            // hide the ui before showing so we can change the values
            hidePopup(true);

            // show the new info
            Dictionary<string, bool> showUI = setBoxInfo(showData);

            //show the ui
            gameObject.SetActive(true);
            StartCoroutine(fadeUI(true,0.4f));
            print("Finished showing");
        }
        /*
        Dictionary<string, string> showData = new Dictionary<string, string>();
        editList.Add("Title", perk.Name);
        editList.Add("Description", perk.Desc);
        editList.Add("Cost", perkObj.Cost);
        editList.Add("Context", pickup / buy /etc);
        */
    }

    // hide the popup
    public void hidePopup(bool instantHide) {
        if (popupVisible){
            popupVisible = false;

            if (!instantHide){
                StartCoroutine(fadeUI(false,0.3f));
            }

            transform.Find("Info").gameObject.GetComponent<CanvasGroup>().alpha = 0;
            transform.Find("Purchase").gameObject.GetComponent<CanvasGroup>().alpha = 0;
            gameObject.SetActive(false);
        }
    }
}
