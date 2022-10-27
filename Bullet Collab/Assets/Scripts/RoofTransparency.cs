/*******************************************************************************
* Name : RoofTransparency.cs
* Section Description : This code handles tiles the player may pass under. When 
* the player passes/collides with this section, the tilemap will become translucent
* so the player character can still be seen. Notably doesn't activate when enemies
* pass underneath, so can act as an ambush/hiding place.
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 10/27/22  0.10                 KJ              Made the thing
*******************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoofTransparency : MonoBehaviour
{
    public Tilemaps;
    public float bulletSpeed = 1f;

    private void OnTriggerStay2D(Collider2D otherCollider)
    {
        //if there is a collision between this boxCollider and a player object
        if (otherCollider.gameObject.tag == "Player")
        {
            UnityEngine.Debug.Log("Change alpha to 0.3f");
        }
    }
}
