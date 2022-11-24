/*******************************************************************************
* Name : bulletSystem.cs
* Section Description : This handles bullet generation, movement, and collision.
* -------------------------------
* - HISTORY OF CHANGES -
* ------------------------------- 
* Date		Software Version	Initials		Description
* 10/24/22  0.10                 DS              Made the thing
* 11/08/22  0.80                 DS              fixed bullet bouncing
* 11/09/22  0.90                 DS              fixed bullet damage 
* 11/12/22  0.90                 DS              optimizations? 
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

    // Game Data Stuff
    public GameObject gameManager;
    [HideInInspector] public gameLoader gameInfo;

    // Editable Variables
    public GameObject bulletOwner;
    public float bulletWeight = 1f;
    public float bulletSpeed = 1f;
    public float bulletSize = 0.1f;
    public float bulletDamage = 1f;
    public int bulletBounces = 0;
    public int bulletPierce = 0;
    public bool enemyBullet = false;
    public List<string> perkIDList;

    // Base Variables
    public Rigidbody2D rb;
    public Collider2D myCollider;
    public visualFx hitPrefab;
    public Transform bulletFolder;
    public Transform debriFolder;

    // Bullet Local Variabls
    private float createTime;
    public float lifeTime = 20f;
    private bool damageOwner = false;
    private bool firstFrame = true;
    private bool bulletSetup = false;
    private float deflectTime = 0;
    private Vector2 lastPosition;
    private Dictionary<GameObject, float> hitList = new Dictionary<GameObject, float>();


    // Sprite Variables
    public Sprite defaultSprite;
    public Sprite enemySprite;
    public Sprite customSprite = null;

    public void setupBullet() {
        createTime = Time.time;

        transform.localScale = new Vector3(bulletSize,bulletSize,1);
                
        // Get data management script
        if (dataManager != null){
            dataInfo = dataManager.GetComponent<sharedData>();
            perkCommands = dataManager.GetComponent<perkModule>();
        }

        if (gameManager != null){
            gameInfo = gameManager.GetComponent<gameLoader>();
        }

        if (customSprite != null){
            gameObject.GetComponent<SpriteRenderer>().sprite = customSprite;
        }else if (enemyBullet && enemySprite){
            gameObject.GetComponent<SpriteRenderer>().sprite = enemySprite;
        }else{
            gameObject.GetComponent<SpriteRenderer>().sprite = defaultSprite;
        }
        
        lastPosition = transform.position;
        bulletSetup = true;
    }

    private void hitEffect(){
        if (dataInfo.particleFX){
            visualFx newHitVFX = Instantiate(hitPrefab,new Vector3(transform.position.x,transform.position.y,-0.1f),transform.rotation,debriFolder);
            if (newHitVFX != null){
                newHitVFX.lifeTime = 0f;
                newHitVFX.killAnimation = true;
                newHitVFX.animSpeed = 2f;

                float hitScale = Mathf.Clamp(transform.localScale.x * 10f,2f,1000f);
                newHitVFX.transform.localScale = new Vector3(hitScale,hitScale,1);
                newHitVFX.setupVFX();
            }
        }
    }

    public void bulletHitEvent(Collider2D hit){
        if (hit != null && hit.gameObject != null){
            if (hitList.ContainsKey(hit.gameObject) && Time.time - hitList[hit.gameObject] <= 0.1){
                return;
            }

            // Check if hit obj can take damage
            Entity hitObj = hit.gameObject.GetComponent<Entity>();
            Dictionary<string, GameObject> editList = new Dictionary<string, GameObject>();
            editList.Add("Owner", bulletOwner);
            editList.Add("Bullet", gameObject);

            editList.Add("GameManager",gameManager);
            editList.Add("DataManager",dataManager);

            if (hitObj != null){
                bulletPierce -= 1;
                hitList[hit.gameObject] = Time.time;

                if (bulletOwner != hit.gameObject){
                    hitObj.damagedBy = bulletOwner;
                }

                editList.Add("Target", hit.gameObject);
                hitObj.takeDamage(bulletDamage);

                // Do knockback based on force
                if (hitObj.weight > 0f){
                    Rigidbody2D rb = hit.gameObject.GetComponent<Rigidbody2D>();
                    if (rb != null){
                        rb.velocity = gameObject.transform.right.normalized * (((Mathf.Sqrt(bulletSpeed) * 10f ) * bulletWeight) / hitObj.weight);
                    }
                }
            }

            // Apply an on hit modifiers
            if (perkCommands != null){
                perkCommands.applyPerk(perkIDList,"Hit",editList);
                
                // apply any changes to the data
                if (bulletOwner){
                    dataInfo.updateEntityData(bulletOwner);
                }
            }
        }

        hitEffect();
    }

    public void removeBullet(Collider2D hit) {
        if (myCollider.enabled){
            myCollider.enabled = false;

            bulletHitEvent(hit);

            if (hit != null && bulletPierce >= 1){
                return;
            }

            Destroy(gameObject);
        }
    }

    public void bounceBullet(Collider2D otherCollider,Vector2 hitNormal){
        if (bulletBounces > 0){
            // Get the new direction of the bullet
            bulletBounces -= 1;
            damageOwner = true;

            transform.right = Vector2.Reflect(transform.right.normalized,hitNormal.normalized);//contact.normal);

            // fix the velocity
            rb.velocity = transform.right * Mathf.Clamp(bulletSpeed,0,25f) * Time.fixedDeltaTime * 100f;

            //hitEffect();
            bulletHitEvent(otherCollider);

            // Check for any bounce modifiers
            if (perkCommands != null && gameObject != null){
                Dictionary<string, GameObject> editList = new Dictionary<string, GameObject>();
                editList.Add("Owner", bulletOwner);
                editList.Add("Bullet", gameObject);
                editList.Add("GameManager",gameManager);
                editList.Add("DataManager",dataManager);
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
                RaycastHit2D contact = Physics2D.CircleCast(origin,radius,direction,distance,LayerMask.GetMask("Obstacle","EntityCollide"),0f);
                if (contact.collider){
                    // Move bullet to safest point if it hit a wall
                    if (contact.collider.gameObject.layer != LayerMask.NameToLayer("EntityCollide")){
                        transform.position = origin + (direction * (contact.distance));
                    }

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
        float alpha = Time.fixedDeltaTime * 20f;
        transform.localScale = Vector3.Lerp(transform.localScale,new Vector3(bulletSize,bulletSize,1),alpha);
        gameObject.GetComponent<TrailRenderer>().startWidth = bulletSize * 2.3f;
        gameObject.GetComponent<TrailRenderer>().enabled = dataInfo.particleFX;

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
            editList.Add("GameManager",gameManager);
            editList.Add("DataManager",dataManager);
            perkCommands.applyPerk(perkIDList,"Update_Bullet",editList);
        }


        moveCheck();
        lastPosition = transform.position;
    }

    // When the Bullet overlaps an object with a Collider2D
    private void OnTriggerEnter2D(Collider2D otherCollider) {
        checkCollider(otherCollider,new Vector2(0,0));
    }

    public Vector2 rotateVector2(Vector2 baseVector, float angle){
        float newAngle = Mathf.Atan2(baseVector.y, baseVector.x) + angle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(newAngle), Mathf.Sin(newAngle));
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
                Entity entityData = otherCollider.gameObject.GetComponent<Entity>();
                if (entityData != null){// && entityData.currentHealth > 0
                    if (bulletOwner != null && otherCollider.gameObject){
                        if (otherCollider.gameObject == bulletOwner && !damageOwner){
                            return;
                        }
                    }

                    if (otherCollider.gameObject != null && entityData.deflectBullets){
                        if (Time.time - deflectTime >= 0.1f){
                            Vector2 deflectNormal = ((Vector2)lastPosition - (Vector2)otherCollider.gameObject.transform.position).normalized;
                            bulletBounces += 1;
                            deflectTime = Time.time;

                            //deflectNormal = rotateVector2(deflectNormal,90f);
                            bounceBullet(otherCollider,-deflectNormal);
                            return;
                        }else{
                            return;
                        }
                    }
                }else{
                    return;
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
