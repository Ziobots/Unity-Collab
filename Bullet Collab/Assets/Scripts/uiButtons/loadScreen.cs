/*******************************************************************************
* Name : loadScreen.cs
* Section Description : This code handles the load screen.
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 11/14/22  0.10                 DS              Made the thing
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class loadScreen : MonoBehaviour
{
    // Update is called once per frame
    void Update(){
        GameObject icon = transform.Find("Holder").Find("icon").gameObject;
        GameObject label = transform.Find("Holder").Find("textField").gameObject;
        if (icon != null && label != null){
            icon.GetComponent<RectTransform>().rotation = Quaternion.Euler(0,(Time.time * 120f) % 360f,0);
            icon.GetComponent<RectTransform>().pivot = new Vector2(0.5f,Mathf.Sin(Time.time * 5f) * 0.1f + 0.5f);
            label.GetComponent<RectTransform>().pivot = new Vector2(0.5f,Mathf.Sin(Time.time * 5f) * 0.1f + 0.5f);
        }
    }
}
