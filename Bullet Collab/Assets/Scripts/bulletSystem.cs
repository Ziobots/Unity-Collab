using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletSystem : MonoBehaviour
{   
    // Editable Variables
    public float bulletSpeed = 1f;
    public float bulletSize = 0.5f;
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
                damageOwner = true;
            }
        }
    }

    // When the Bullet overlaps an object with a Collider2D
    private void OnTriggerEnter2D(Collider2D otherCollider) {
        // Check to ignore bullet owner
        if (otherCollider != null){
            if (bulletOwner != null){
                Collider2D ownerCollider = bulletOwner.GetComponent<Collider2D>();
                if (ownerCollider == otherCollider && !damageOwner){
                    return;
                }
            }
        }

        removeBullet(otherCollider);
    }
}
