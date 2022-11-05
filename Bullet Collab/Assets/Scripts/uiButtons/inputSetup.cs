/*******************************************************************************
* Name : loginSetup.cs
* Section Description : This code handles input fields.
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 11/04/22  0.10                 DS              Made the thing
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class inputSetup : MonoBehaviour
{
    public GameObject tabSelect;

    // Holder Variables
    public GameObject Holder;
    public Color32 defaultColor;
    public Color32 selectColor;


    public void onSelected(){
        if (Holder != null && Holder.transform.Find("background")){
            Holder.transform.Find("background").gameObject.GetComponent<Image>().color = selectColor;
        }
    }
    
    public void onDeSelected(){
        if (Holder != null && Holder.transform.Find("background")){
            Holder.transform.Find("background").gameObject.GetComponent<Image>().color = defaultColor;
        }
    }
}
