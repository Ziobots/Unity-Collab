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
    public Tilemap Foreground;

    // If the player touches the area, make the tilemap translucent
    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if (otherCollider.gameObject.tag == "Player")
        {
            Foreground.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        }
    }

    // If the player leaves the area, make the tilemap normal again
    private void OnTriggerExit2D(Collider2D otherCollider) {
        if (otherCollider.gameObject.tag == "Player") 
        {
            Foreground.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

        }
    }
}
