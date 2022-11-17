/*******************************************************************************
* Name : Breakable.cs
* Section Description : This is a subclass for stuff that can be hit by bullets.
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 11/10/22  0.10                 DS              Made the thing
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : Entity
{
    // hit variables
    public bool spinHit = false;
    private bool canHit = true;

    [HideInInspector] public Vector2 spawnPosition;
    public bool moveToSpawn = false;

    // remove unwanted functions
    public override void reloadGun(){}
    public override bool fireBullets(){
        return false;
    }

    // spin effect
    private void spinRotation(float value){
        Quaternion setRotationEuler = Quaternion.Euler(0f, value, 0f);
        transform.rotation = setRotationEuler;
    }

    private void spinFinished(){
        spinRotation(0f);
        canHit = true;
    }

    private void spinAnimation(){
        spinRotation(0);

        if (currentHealth <= 0){
            LeanTween.value(gameObject,0f,270f,1f).setEaseOutQuad().setOnUpdate(spinRotation).setOnComplete(destroyObj);
        }else{ 
            LeanTween.value(gameObject,0f,360f,1.3f).setEaseOutBack().setOnUpdate(spinRotation).setOnComplete(spinFinished);
        }
    }

    public override void damageEffect(){
        if (spinHit){
            spinAnimation();
        }

        base.damageEffect();
    }

    public override void takeDamage(float amount){
        if (amount > 0 && currentHealth > 0 && canHit){
            amount = 1;

            currentHealth -= amount;
            if (hurtNoise != null){
                hurtNoise.PlayOneShot(hurtNoise.clip,hurtNoise.volume  * dataInfo.gameVolume * dataInfo.masterVolume);
            }

            damageEffect();
        }
        
        if (currentHealth <= 0 && !spinHit){
            destroyObj();
        }
    }

    public virtual void destroyObj(){
        
        Destroy(gameObject);
    }

    public override void Start(){
        base.Start();

        spawnPosition = transform.position;
    }

    public override void FixedUpdate(){
        if (rb && moveToSpawn){
            rb.velocity = rb.velocity * 0.98f;

            float alpha = Time.fixedDeltaTime * 10f;
            transform.position = Vector2.Lerp(transform.position,spawnPosition,alpha);
        }
    }
}
