/*******************************************************************************
* Name : spiral.cs
* Section Description : This is a superclass for bullet detection purposes. Player, Enemy (and by extension Boss), 
* and possibly destructable obstacles will be subclasses.
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 10/24/22  0.10                 DS              Made the thing
* 11/03/22  0.20                 DS              updated health stuff
* 11/07/22  0.70                 DS              added enemy stuff and knockback
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spiral : Enemy
{
    private bool spinning = false;

    public override void bulletFired(){
        base.bulletFired();
        defaultFace = "eyes_Dizzy";
        spinning = true;
        flipSprite = false;

    }

    public override void reloadGun(){
        defaultFace = "eyes_Normal";
        spinning = false;
        base.reloadGun();
    }

    public override bool fireGunCheck(){
        return spinning;
    }

    public override Vector2 getLookDirection(){
        if (reloadingGun || !spinning){
            return base.getLookDirection();
        }

        if (currentTarget != null && currentTarget.transform && currentHealth > 0){
            lookDirection = rotateVector2(lookDirection,5f);
        }

        return lookDirection;
    }
}
