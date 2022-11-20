/*******************************************************************************
* Name : rainBoss.cs
* Section Description : test boss
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 11/20/22  0.10                 DS              Made the thing
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rainBoss : Enemy
{
    private bool spinning = false;

    public override void bulletFired(){
        base.bulletFired();
        walkSpeed = 1;
        defaultFace = "eyes_Hurt";
        spinning = true;
        flipSprite = false;

    }

    public override void takeDamage(float amount){
        if (amount > 6){
            amount = 6;
        }

        base.takeDamage(amount);
    }

    public override void reloadGun(){
        walkSpeed = 3.5f;
        defaultFace = "eyes_Normal";
        spinning = false;
        base.reloadGun();
    }

    public override bool fireGunCheck(){
        return spinning;
    }
}
