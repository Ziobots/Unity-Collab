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
    public bool hitGrow = false;

    [HideInInspector] public Vector2 spawnPosition;
    [HideInInspector] public Vector3 baseScale;
    public bool moveToSpawn = false;
    private float noiseTime = 0;

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

    // grow effect
    public void growSprite(Vector3 value){
        transform.localScale = value;
    }

    private void growAnimation(){
        growSprite(baseScale);

        if (currentCamera){
            if (damageNoise != null && Time.time - noiseTime >= .1f){
                noiseTime = Time.time;
                damageNoise.PlayOneShot(damageNoise.clip,damageNoise.volume);
            }
        }

        LeanTween.cancel(gameObject);
        Vector3 growScale = new Vector3(baseScale.x*1.2f,baseScale.y*1.2f,baseScale.z*1);
        LeanTween.value(gameObject,baseScale,growScale,0.07f).setEaseOutQuad().setOnUpdateVector3(growSprite).setOnComplete(delegate(){
            LeanTween.value(gameObject,growScale,baseScale,0.2f).setEaseOutBack().setOnUpdateVector3(growSprite).setOnComplete(delegate(){
                growSprite(baseScale);
                canHit = true;
            });
        });
    }

    private void spinAnimation(){
        spinRotation(0);

        if (currentCamera){
            if (damageNoise != null && Time.time - noiseTime >= .1f){
                noiseTime = Time.time;
                damageNoise.PlayOneShot(damageNoise.clip,damageNoise.volume);
            }
        }

        LeanTween.cancel(gameObject);
        if (currentHealth <= 0){
            LeanTween.value(gameObject,0f,270f,1f).setEaseOutQuad().setOnUpdate(spinRotation).setOnComplete(destroyObj);
        }else{ 
            LeanTween.value(gameObject,0f,360f,1.3f).setEaseOutBack().setOnUpdate(spinRotation).setOnComplete(delegate(){
                spinRotation(0f);
                canHit = true;
            });
        }
    }

    public override void damageEffect(){
        if (spinHit){
            spinAnimation();
        }

        if (hitGrow){
            growAnimation();
        }

        base.damageEffect();
    }

    public override void onPushObj(Entity pushInfo){
        base.onPushObj(pushInfo);
        takeDamage(1);
    }

    public override void takeDamage(float amount){
        if (amount > 0 && currentHealth > 0 && canHit){
            base.takeDamage(1);

            if (hurtNoise != null){
                //hurtNoise.PlayOneShot(hurtNoise.clip,hurtNoise.volume);// * dataInfo.gameVolume * dataInfo.masterVolume
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
        baseScale = transform.localScale;
    }

    public override void FixedUpdate(){
        if (rb && moveToSpawn && Time.timeScale > 0){

            float alpha = Time.fixedDeltaTime * 12f;
            transform.position = Vector2.Lerp(transform.position,spawnPosition,alpha);
            rb.velocity = Vector3.Lerp(rb.velocity, new Vector3(0,0,0),alpha);
        }

        pushNearby();
    }
}
