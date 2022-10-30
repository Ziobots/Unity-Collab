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
    // Base Data Stuff
    public GameObject dataManager;
    [HideInInspector] public sharedData dataInfo;
    [HideInInspector] public perkModule perkCommands;

    // Editable Variables
    public GameObject bulletOwner;
    public float bulletSpeed = 1f;
    public float bulletSize = 0.5f;
    public int bulletDamage = 1;
    public int bulletBounces = 0;
    public List<string> perkIDList;

    // Base Variables
    public Rigidbody2D rb;
    public Collider2D myCollider;

    // Bullet Local Variabls
    private float createTime;
    private float lifeTime = 20f;
    private bool damageOwner = false;
    private bool firstFrame = true;

    public void setupBullet() {
        createTime = Time.time;
                
        // Get data management script
        if (dataManager != null){
            dataInfo = dataManager.GetComponent<sharedData>();
            perkCommands = dataManager.GetComponent<perkModule>();
        }
    }

    private void removeBullet(Collider2D hit) {
        if (myCollider.enabled){
            myCollider.enabled = false;

            if (hit != null && hit.gameObject != null){
                // Check if hit obj can take damage
                Entity hitObj = hit.gameObject.GetComponent<Entity>();
                Dictionary<string, GameObject> editList = new Dictionary<string, GameObject>();
                editList.Add("Owner", bulletOwner);
                editList.Add("Bullet", gameObject);

                if (hitObj != null){
                    editList.Add("Target", hit.gameObject);
                    hitObj.takeDamage((int)bulletDamage);
                }

                // Apply an on hit modifiers
                if (perkCommands != null){
                    perkCommands.applyPerk(perkIDList,"Hit",editList);
                }
            }

            Destroy(gameObject);
        }
    }

    public void bounceBullet(Collider2D otherCollider){
        // Find the surface normal of the hit object
        Vector2 origin = transform.position - transform.right.normalized;
        RaycastHit2D contact = Physics2D.Raycast(origin,transform.right.normalized,2f,LayerMask.GetMask("Default"));

        if (contact){
            if (bulletBounces > 0){
                // Get the new direction of the bullet
                bulletBounces -= 1;
                damageOwner = true;
                transform.right = Vector2.Reflect(transform.right,contact.normal);

                // Check for any bounce modifiers
                if (perkCommands != null && gameObject != null){
                    Dictionary<string, GameObject> editList = new Dictionary<string, GameObject>();
                    editList.Add("Owner", bulletOwner);
                    editList.Add("Bullet", gameObject);
                    perkCommands.applyPerk(perkIDList,"Bounce",editList);
                }
            }else{
                removeBullet(otherCollider);
            }
        }
    }

    void FixedUpdate() {
        // move the bullet
        rb.velocity = transform.right * Mathf.Clamp(bulletSpeed,0,25f) * Time.fixedDeltaTime * 100f;

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

        if (perkCommands != null){
            // Check for any bullet lifetime modifiers
            Dictionary<string, GameObject> editList = new Dictionary<string, GameObject>();
            editList.Add("Owner", bulletOwner);
            editList.Add("Bullet", gameObject);
            perkCommands.applyPerk(perkIDList,"Update_Bullet",editList);
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
            bounceBullet(otherCollider);
        }
    }
}
