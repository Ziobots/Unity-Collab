/*******************************************************************************
* Name : enemyList.cs
* Section Description : Used to mark which enemies can be created at certain spawn points.
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 11/09/22  0.10                 DS              Made the thing
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyList : MonoBehaviour
{
    public List<string> enemySpawns = new List<string>();
}
