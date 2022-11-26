/*******************************************************************************
* Name : spiral.cs
* Section Description : spiral enemy.
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 10/28/22  0.10                 DS              Made the thing
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
