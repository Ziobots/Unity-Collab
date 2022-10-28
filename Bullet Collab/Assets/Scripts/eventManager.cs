/*******************************************************************************
* Name : Enemy.cs
* Section Description : This code exists to prevent the event system from disappearing during a scene change.
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 10/27/22  0.10                 DS              Made the thing
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eventManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start(){
        DontDestroyOnLoad(gameObject);
    }
}
