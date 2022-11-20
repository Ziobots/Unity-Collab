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
    private bool shooting = false;
    private Vector2 shootDirection;
    private bool secondPhase = false;
    private float fireTime = 0;

    public override void bulletFired(){
        base.bulletFired();
        walkSpeed = 1;
        defaultFace = "eyes_Hurt";
        if (!shooting){
            fireTime = Time.time;
            shootDirection = lookDirection;
        }
        shooting = true;
        flipSprite = false;

    }

    public override Vector2 getLookDirection(){
        if (reloadingGun || !shooting || !secondPhase){
            return base.getLookDirection();
        }

        float angle = Mathf.Sin(Time.time * 8f) * 50f;
        lookDirection = rotateVector2(shootDirection,angle);

        return lookDirection;
    }

    public override void takeDamage(float amount){
        amount = Mathf.Sqrt(amount);

        if (amount > 5){
            //amount = 5;
        }

        base.takeDamage(amount);
    }

    public override bool fireGunCheck(){
        return shooting || (Time.time - fireTime >= reloadTime * 3f) || checkVisibility(currentTarget,0f);
    }

    public override void localEditBullet(bulletSystem bulletObj){
        base.localEditBullet(bulletObj);

        if (secondPhase){
            bulletObj.bulletSpeed *= 1.1f;
            bulletObj.bulletDamage *= 1.5f;
            bulletObj.bulletSize *= 1.1f;
        }

        // hide prevention - just spawn a bunch of homing bouncing bullets
        if (Time.time - lastSeeTime >= 20f){
            bulletObj.bulletBounces = 15;
            bulletObj.bulletSpeed *= 1.1f;
            bulletObj.perkIDList.Add("remoteBullet");
        }
    }

    public override void reloadGun(){
        walkSpeed = secondPhase ? 2f : 3.5f;
        defaultFace = "eyes_Normal";
        shooting = false;

        if (currentHealth <= maxHealth * 0.6f && !secondPhase){
            secondPhase = true;
            fireCount -= 1;
            reloadTime *= 0.8f;
            bulletTime *= 0.7f;
            turnSpeed += 10f;
        }

        base.reloadGun();
    }
}
