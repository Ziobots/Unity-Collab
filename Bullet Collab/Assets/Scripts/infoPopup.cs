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

    // show the popup
    public void showPopup(Dictionary<string, string> showData) {
        if (!popupVisible || !sameDictionary(showData)){
            hidePopup(true);

            // set data fields

            gameObject.SetActive(true);
            //tween to show alpha mayb size and pos

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

            }

            transform.Find("Info").gameObject.GetComponent<CanvasGroup>().alpha = 0;
            transform.Find("Purchase").gameObject.GetComponent<CanvasGroup>().alpha = 0;
            gameObject.SetActive(false);
        }
    }
}
