/*******************************************************************************
* Name : bulletSystem.cs
* Section Description : This handles bullet generation, movement, and collision.
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 10/24/22  0.10                 DS              Made the thing
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletSystem : MonoBehaviour
{   
    // Editable Variables
    public GameObject bulletOwner;
    public float bulletSpeed = 1f;
    public float bulletSize = 0.5f;
    public int bulletDamage = 1;
    public int bulletBounces = 0;

    // Base Variables
    public Rigidbody2D rb;
    public Collider2D myCollider;

    // Bullet Local Variabls
    private float createTime;
    private float lifeTime = 20f;
    private bool damageOwner = false;
    private bool firstFrame = true;

    private void Awake() {
        createTime = Time.time;
    }

    private void removeBullet(Collider2D hit) {
        if (myCollider.enabled){
            myCollider.enabled = false;

            if (hit != null && hit.gameObject != null){
                Entity hitObj = hit.gameObject.GetComponent<Entity>();
                if (hitObj != null){
                    hitObj.takeDamage((int)bulletDamage);
                }
            }

            Destroy(gameObject);
        }
    }

    public void bounceBullet(){
        Vector2 origin = transform.position - transform.right.normalized;
        RaycastHit2D contact = Physics2D.Raycast(origin,transform.right.normalized,2f,LayerMask.GetMask("Default"));

        if (contact){
            if (bulletBounces > 0){
                bulletBounces -= 1;
                damageOwner = true;
                transform.right = Vector2.Reflect(transform.right,contact.normal);
            }else{
                removeBullet(null);
            }
        }
    }

    void FixedUpdate() {
        // move the bullet
        rb.velocity = transform.right * bulletSpeed * Time.fixedDeltaTime * 100f;

        // first frame collision doesnt work so skip it
        if (firstFrame){
            firstFrame = false;
            return;
        }

        // remove the bullet after certain amount of time
        if (Time.time - createTime >= lifeTime){
            removeBullet(null);
            return;
        }

        // Bullet should not be able to damage owner if it spawned on top of them
        if ((!damageOwner) && (bulletOwner != null) && (myCollider != null)){
            Collider2D ownerCollider = bulletOwner.GetComponent<Collider2D>();
            if (ownerCollider && !myCollider.IsTouching(ownerCollider)){
                if (Time.time - createTime > 0.25f){
                    damageOwner = true;
                }
            }
        }
    }

    private Collider2D stuckCollider;
    private float stuckTime = 0;
    // Check if bullet is inside of something it shouldnt be
    private void OnTriggerStay2D(Collider2D otherCollider) {
        if (otherCollider != stuckCollider){
            stuckCollider = otherCollider;
            stuckTime = Time.time;
        }

        if (Time.time - stuckTime > .05f){
            removeBullet(null); 
        }
    }

    // When the Bullet overlaps an object with a Collider2D
    private void OnTriggerEnter2D(Collider2D otherCollider) {
        bool entityHit = true;

        // Check to ignore bullet owner
        if (otherCollider != null){
            // Check for collision type
            if (otherCollider.gameObject.tag == "Bullet"){
                return;
            }else if (otherCollider.gameObject.tag == "Level"){
                entityHit = false;
            }else{
                if (bulletOwner != null && otherCollider.gameObject){
                    if (otherCollider.gameObject == bulletOwner && !damageOwner){
                        return;
                    }
                }
            }
        }

        // do on hit effect


        if (bulletBounces <= 0 || entityHit){
            removeBullet(otherCollider);
        }else if (otherCollider != null){
            bounceBullet();
        }
    }
}
