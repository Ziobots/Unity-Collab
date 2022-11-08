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
    public float bulletSize = 0.1f;
    public float bulletDamage = 1f;
    public int bulletBounces = 0;
    public List<string> perkIDList;

    // Base Variables
    public Rigidbody2D rb;
    public Collider2D myCollider;
    public visualFx hitPrefab;
    public Transform bulletFolder;

    // Bullet Local Variabls
    private float createTime;
    private float lifeTime = 20f;
    private bool damageOwner = false;
    private bool firstFrame = true;
    private bool bulletSetup = false;

    private Vector2 lastPosition;

    public void setupBullet() {
        createTime = Time.time;

        transform.localScale = new Vector3(bulletSize,bulletSize,1);
                
        // Get data management script
        if (dataManager != null){
            dataInfo = dataManager.GetComponent<sharedData>();
            perkCommands = dataManager.GetComponent<perkModule>();
        }

        lastPosition = transform.position;
        bulletSetup = true;
    }

    private void hitEffect(){
        visualFx newHitVFX = Instantiate(hitPrefab,new Vector3(transform.position.x,transform.position.y,-0.1f),transform.rotation,bulletFolder);
        if (newHitVFX != null){
            newHitVFX.lifeTime = 0f;
            newHitVFX.killAnimation = true;
            newHitVFX.animSpeed = 2f;

            float hitScale = Mathf.Clamp(transform.localScale.x * 10f,2f,1000f);
            newHitVFX.transform.localScale = new Vector3(hitScale,hitScale,1);
            newHitVFX.setupVFX();
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
                    if (bulletOwner != hit.gameObject){
                        hitObj.damagedBy = bulletOwner;
                    }

                    editList.Add("Target", hit.gameObject);
                    hitObj.takeDamage((int)bulletDamage);

                    // Do knockback based on force
                    if (hitObj.weight != 0f){
                        Rigidbody2D rb = hit.gameObject.GetComponent<Rigidbody2D>();
                        if (rb != null){
                            rb.velocity = gameObject.transform.right.normalized * ((bulletSpeed * 5f) / hitObj.weight);
                        }
                    }
                }

                // Apply an on hit modifiers
                if (perkCommands != null){
                    perkCommands.applyPerk(perkIDList,"Hit",editList);
                }
            }

            hitEffect();
            Destroy(gameObject);
        }
    }

    public void bounceBullet(Collider2D otherCollider,Vector2 hitNormal){
        if (bulletBounces > 0){
            // Get the new direction of the bullet
            bulletBounces -= 1;
            damageOwner = true;
            transform.right = Vector2.Reflect(transform.right,hitNormal);//contact.normal);
            hitEffect();

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

    void moveCheck(){
        if (lastPosition != null){
            Vector2 direction = ((Vector2)transform.position - (Vector2)lastPosition).normalized;
            Vector2 origin = lastPosition;
            float distance = Vector2.Distance(transform.position, origin);
            if (distance > 0){
                float radius = transform.localScale.x * 1.45f;
                RaycastHit2D contact = Physics2D.CircleCast(origin,radius,direction,distance,LayerMask.GetMask("Default"),0f);
                if (contact.collider){
                    // Move bullet to safest point
                    transform.position = origin + (direction * (contact.distance));
                    checkCollider(contact.collider,contact.normal);
                }
            }
        }
    }

    void FixedUpdate() {
        if (!bulletSetup){
            return;
        }

        // first frame collision doesnt work so skip it
        if (firstFrame){
            firstFrame = false;
            return;
        }

        // move the bullet
        rb.velocity = transform.right * Mathf.Clamp(bulletSpeed,0,25f) * Time.fixedDeltaTime * 100f;

        // scale the bullet
        transform.localScale = Vector3.Lerp(transform.localScale,new Vector3(bulletSize,bulletSize,1),Time.fixedDeltaTime * 20f);
        gameObject.GetComponent<TrailRenderer>().startWidth = bulletSize * 2.3f;

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


        moveCheck();
        lastPosition = transform.position;
    }

    // When the Bullet overlaps an object with a Collider2D
    private void OnTriggerEnter2D(Collider2D otherCollider) {
        checkCollider(otherCollider,new Vector2(0,0));
    }

    private void checkCollider(Collider2D otherCollider,Vector2 hitNormal) {
        bool entityHit = true;

        // Check to ignore bullet owner
        if (otherCollider != null){
            // Check for collision type
            if (otherCollider.gameObject.tag == "Bullet"){
                return;
            }else if (otherCollider.gameObject.tag == "Interactable"){
                return;
            }else if (otherCollider.gameObject.tag == "Level"){
                entityHit = false;

                // Skip collsion vs raycast
                if (hitNormal.magnitude <= 0f){
                    return;
                }
            }else{
                if (bulletOwner != null && otherCollider.gameObject){
                    if (otherCollider.gameObject == bulletOwner && !damageOwner){
                        return;
                    }
                }
            }
        }

        // check for bounces or hit entity
        if (bulletBounces <= 0 || entityHit){
            removeBullet(otherCollider);
        }else if (otherCollider != null && hitNormal.magnitude > 0f){
            bounceBullet(otherCollider,hitNormal);
        }
    }
}
