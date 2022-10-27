/*******************************************************************************
* Name : bulletSystem.cs
* Section Description : This handles bullet generation, movement, and collision.
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 10/24/22  0.10                DS              Made the thing
* 10/27/22  0.11                KJ              Changed collision to ignore Foreground
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletSystem : MonoBehaviour
{   
    // Editable Variables
    public float bulletSpeed = 1f;
    public float bulletSize = 0.5f;
    public int bulletDamage = 1;
    public Entity bulletOwner;

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

    // When the Bullet overlaps an object with a Collider2D
    private void OnTriggerEnter2D(Collider2D otherCollider) {
        // Check to ignore bullet owner
        if (otherCollider != null)
        {
            if (bulletOwner != null)
            {
                Collider2D ownerCollider = bulletOwner.GetComponent<Collider2D>();
                if (ownerCollider == otherCollider && !damageOwner)// If collider that spawned bullet and target collider are the same, exit
                {
                    return;
                }
            }

            // If target collider is another bullet or the Foreground tilemap
            if (otherCollider.gameObject.tag == "Bullet" || otherCollider.gameObject.tag == "Foreground")
            {
                return;
            }
        }

        removeBullet(otherCollider);
    }
}
