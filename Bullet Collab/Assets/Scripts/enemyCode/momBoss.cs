/*******************************************************************************
* Name : momBoss.cs
* Section Description : momBoss
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 11/25/22  0.10                 DS              Made the thing
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class momBoss : Enemy
{
    private bool shooting = false;
    private Vector2 shootDirection;
    private bool secondPhase = false;
    private float fireTime = 0;
    private float meleeSpawnTime = 0;
    private int meleeSpawnCount = 0;
    public GameObject spawnPoint;

    public override void bulletFired(){
        base.bulletFired();
        fireTime = Time.time;
    }

    public override Vector2 getLookDirection(){
        if (!secondPhase){
            return base.getLookDirection();
        }

        lookDirection = rotateVector2(lookDirection,2.5f);

        return lookDirection;
    }

    public override void takeDamage(float amount){
        amount = Mathf.Sqrt(amount);

        base.takeDamage(amount);
    }

    public override bool fireGunCheck(){
        return shooting || (Time.time - fireTime >= reloadTime * 3f) || checkVisibility(currentTarget,0f);
    }

    public override void localEditBullet(bulletSystem bulletObj){
        base.localEditBullet(bulletObj);

        bulletObj.bulletSpeed *= 1f;
        bulletObj.bulletDamage *= 0.7f;
        bulletObj.bulletSize *= 1.1f;

        if (secondPhase){

        }

        // hide prevention - just spawn a bunch of homing bouncing bullets
        if (Time.time - lastSeeTime >= 20f){
            bulletObj.bulletBounces = 15;
            bulletObj.bulletSpeed *= 1.1f;
            bulletObj.perkIDList.Add("remoteBullet");
            deflectBullets = true;
        }
    }

    public override void movePattern(){
        movement = ((Vector2)spawnPosition - (Vector2)transform.position).normalized;
    }

    public override void reloadGun(){
        shooting = false;

        if (currentHealth <= maxHealth * 0.6f && !secondPhase){
            secondPhase = true;
            reloadTime += 1f;
            bulletTime *= 0.7f;
            turnSpeed += 2f;
        }

        meleeSpawnCount = secondPhase ? 4 : 2;
        meleeSpawnTime = Time.time;

        base.reloadGun();
    }

    private void spawnMelee(){
        gameInfo.createEnemy("melee",spawnPoint,null,delegate(Entity entityInfo){
            if (entityInfo.gameObject.GetComponent<Enemy>()){
                entityInfo.gameObject.GetComponent<Enemy>().skipSpawnAnimation = true;
                entityInfo.weight = weight * 0.7f;

                if (secondPhase){
                    entityInfo.maxHealth *= 1.5f;
                    entityInfo.walkSpeed *= 0.85f;
                }

                if (Time.time - lastSeeTime >= 20f){
                    entityInfo.walkSpeed *= 2f;
                    entityInfo.maxHealth += 10;
                }
            }
        });
    }

    public override void damageOnDeath(){
        base.damageOnDeath();
        for (int i = 0; i < 5; i++){
            spawnMelee();
        }

    }

    public override void FixedUpdate(){
        if (currentHealth > 0 && Time.time - meleeSpawnTime >= 0.3f && meleeSpawnCount > 0){
            meleeSpawnCount--;
            meleeSpawnTime = Time.time;
            spawnMelee();
        }

        if (Time.time - lastSeeTime >= 20f){
            meleeSpawnCount = 10;
        }

        base.FixedUpdate();
    }
}
