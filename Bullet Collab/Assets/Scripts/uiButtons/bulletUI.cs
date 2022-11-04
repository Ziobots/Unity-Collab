/*******************************************************************************
* Name : bulletUI.cs
* Section Description : just meant for the ammo bar ammo ui thing, holding variables.
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 11/03/22  0.10                 DS              Made the thing
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletUI : MonoBehaviour
{
    public int ammoIndex;
    public bool bulletVisible = false;

    public void showBullet(){
        bulletVisible = true;
        gameObject.transform.Find("bullet").gameObject.SetActive(true);
    }

    public void hideBullet(){
        bulletVisible = false;
        gameObject.transform.Find("bullet").gameObject.SetActive(false);
    }
}
