/*******************************************************************************
* Name : keepLoad.cs
* Section Description : If a GameObject does not have a script but should continue to exist between areas just add this script
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 10/27/22  0.10                 DS              Made the thing
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class keepLoad : MonoBehaviour
{
    // Start is called before the first frame update
    void Start() {
        // Keep between Scenes
        DontDestroyOnLoad(gameObject);
    }
}
