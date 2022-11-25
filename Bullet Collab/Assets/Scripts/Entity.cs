/*******************************************************************************
* Name : Entity.cs
* Section Description : This is a superclass for bullet detection purposes. Player, Enemy (and by extension Boss), 
* and possibly destructable obstacles will be subclasses.
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 10/24/22  0.10                 DS              Made the thing
* 11/03/22  0.20                 DS              updated health stuff
* 11/07/22  0.70                 DS              added enemy stuff and knockback
* 11/19/22  0.70                 DS              moved push code here
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    // Base Data Stuff
    public GameObject dataManager;
    [HideInInspector] public sharedData dataInfo;
    [HideInInspector] public perkModule perkCommands;

    // Game Data Stuff
    public GameObject gameManager;
    [HideInInspector] public gameLoader gameInfo;

    // Local Variables
    public float currentHealth = 5;
    public float maxHealth = 5;
    [HideInInspector] public float damageAmount = 0;
    public int currency = 0;
    public float weight = 5f;
    public float meleeDamage = 1f;

    // Stat Variables
    [HideInInspector] public GameObject damagedBy;
    [HideInInspector] public bool tookDamage;

    // UI Stuff 
    public GameObject uiManager;
    [HideInInspector] public UIManager uiUpdate;

    // Movement Variables
    [HideInInspector] public Vector2 movement;
    public float walkSpeed = 6f;
    public Rigidbody2D rb;
    [HideInInspector] public bool facingRight = true;
    public float lastSeeTime = 0;


    // Bullet Variables
    public bulletSystem bulletPrefab;
    public List<Transform> launchPoints = new List<Transform>();
    public Transform bulletFolder;
    public Transform debriFolder;

    // Temporary Bullet
    public int maxAmmo;
    public int fireCount = 1;
    public int currentAmmo;
    public float reloadTime;
    public float bulletTime;
    public float bulletSpread;
    public float reloadDelay = 0f;
    public bool automaticGun = false;
    public bool reloadingGun = false;
    public bool deflectBullets = false;
    public bool shootingGun = false;
    // time vars
    [HideInInspector] public float reloadStartTime = 0;
    [HideInInspector] public float delayStartTime = 0;
    [HideInInspector] public float reloadFinishTime = 0;

    // Sound Stuff
    public AudioSource gunNoise;
    public AudioSource reloadNoise;
    public AudioSource hurtNoise;
    public AudioSource damageNoise;
    public GameObject currentCamera;

    // Visual Variables
    public Color spriteColor = new Color(255,255,255,255);

    // Upgrade Variables
    public List<string> perkIDList;

    public void setCurrentAmmo(float value){
        currentAmmo = (int) value;

        if (dataInfo != null){
            dataInfo.updateEntityData(gameObject);
        }

        if (uiUpdate != null && gameObject.tag == "Player"){
            uiUpdate.updateGameUI();
        }
    }

    // reload gun function
    public virtual void reloadGun(){
        reloadStartTime = Time.time;
        delayStartTime = 0;
        reloadingGun = true;
        shootingGun = false;

        // Check for any reload modifiers
        Dictionary<string, GameObject> editList = new Dictionary<string, GameObject>();
        editList.Add("Owner", gameObject);
        editList.Add("GameManager",gameManager);
        editList.Add("DataManager",dataManager);
        perkCommands.applyPerk(perkIDList,"Reload",editList);

        if (reloadNoise != null){
            reloadNoise.PlayOneShot(reloadNoise.clip,reloadNoise.volume);//  * dataInfo.gameVolume * dataInfo.masterVolume
        }

        // add to the ammo one by one over time
        LeanTween.value(gameObject,(float)currentAmmo,(float)maxAmmo,reloadTime).setEaseLinear().setOnUpdate(setCurrentAmmo).setOnComplete(finishedReload);

        if (dataInfo != null){
            dataInfo.updateEntityData(gameObject);
        }

        // update the ui
        if (uiUpdate != null && gameObject.tag == "Player"){
            uiUpdate.updateGameUI();
        }
    }

    // This function exits for enemies with unique bullet phases
    public virtual void finishedReload(){
        currentAmmo = maxAmmo;
        reloadFinishTime = Time.time;
        reloadingGun = false;
    }

    // This function exits for enemies with unique bullet phases
    public virtual void bulletFired(){}

    // This function exists for enemies with unique bullets, that cant be achieved with upgrades
    public virtual void localEditBullet(bulletSystem bulletObj){}

    // Entity will fire Bullets
    public virtual bool fireBullets(){
        if (currentHealth <= 0){
            return false;
        }

        // make sure game is running
        if (Time.timeScale <= 0){
            return false;
        }

        // check if entity is  in bullet cooldown
        if (Time.time - delayStartTime < bulletTime){
            return false;
        }

        // check if entity is reloading
        if (Time.time - reloadStartTime < reloadTime || reloadingGun){
            return false;
        }

        // check for reload delay - this is only works for enemies
        if (Time.time - reloadFinishTime < reloadDelay){
            return false;
        }

        // check if entity has ammo
        if (currentAmmo <= 0){
            reloadGun();
            return false;
        }

        // entity passed all checks, can fire
        delayStartTime = Time.time;
        shootingGun = true;

        if (gunNoise != null){
            gunNoise.PlayOneShot(gunNoise.clip,gunNoise.volume);//  * dataInfo.gameVolume * dataInfo.masterVolume
        }

        foreach(Transform point in launchPoints){
            for (int i = 1; i <= fireCount; i++){
                if (currentAmmo > 0){
                    currentAmmo -= 1;
                    // calculate bullet spread
                    Quaternion spreadQuaternion = Quaternion.Euler(0, 0, 0);
                    if (bulletSpread > 0){
                        bulletSpread = Mathf.Clamp(bulletSpread,0,50);
                        float angle = Random.Range(-bulletSpread * 1000,bulletSpread * 1000) / 1000;
                        spreadQuaternion = Quaternion.Euler(0, 0, angle);
                    }

                    bulletSystem newBullet = Instantiate(bulletPrefab,point.position,point.rotation * spreadQuaternion,bulletFolder);
                    if (newBullet != null){
                        if (gameObject.tag == "Enemy"){
                            newBullet.enemyBullet = true;
                        }

                        List<string> copyPerkList = new List<string>(perkIDList);
                        // set the default bullet stats
                        newBullet.dataManager = dataManager;
                        newBullet.gameManager = gameManager;
                        newBullet.bulletOwner = gameObject;
                        newBullet.bulletSpeed = 5f;
                        newBullet.bulletSize = 0.11f;
                        newBullet.bulletBounces = 0;
                        newBullet.perkIDList = copyPerkList;
                        newBullet.bulletFolder = bulletFolder;
                        newBullet.debriFolder = debriFolder;

                        // finish setting up the bullet
                        localEditBullet(newBullet);
                        newBullet.setupBullet();

                        // Check for any onFire modifiers
                        Dictionary<string, GameObject> editList = new Dictionary<string, GameObject>();
                        editList.Add("Owner", gameObject);
                        editList.Add("Bullet", newBullet.gameObject);
                        editList.Add("GameManager",gameManager);
                        editList.Add("DataManager",dataManager);
                        perkCommands.applyPerk(perkIDList,"Shoot",editList);
                    }
                }
            }
        }

        // the delay after firing the last bullet shouldnt be as long
        if (currentAmmo <= 0){
            delayStartTime = Time.time - bulletTime + 0.25f;
        }

        bulletFired();

        if (dataInfo != null){
            dataInfo.updateEntityData(gameObject);
        }

        if (uiUpdate != null && gameObject.tag == "Player"){
            uiUpdate.updateGameUI();
        }

        return true;
    }

    // bullets will call this when they hit
    public virtual void takeDamage(float amount){
        if (amount > 0 && currentHealth > 0){
            currentHealth -= amount;

            // Check for any bounce modifiers 
            Dictionary<string, GameObject> editList = new Dictionary<string, GameObject>();
            editList.Add("Owner", gameObject);
            editList.Add("GameManager",gameManager);
            editList.Add("DataManager",dataManager);
            damageAmount = amount;
            perkCommands.applyPerk(perkIDList,"Damaged",editList);

            // check if the entity has died
            if (currentHealth <= 0){
                damageOnDeath();
                perkCommands.applyPerk(perkIDList,"Killed",editList);
                if (hurtNoise != null){
                    hurtNoise.PlayOneShot(hurtNoise.clip,hurtNoise.volume);// * dataInfo.gameVolume * dataInfo.masterVolume
                }
            }else{
                if (damageNoise != null){
                    damageNoise.PlayOneShot(damageNoise.clip,damageNoise.volume);// * dataInfo.gameVolume * dataInfo.masterVolume
                }
            }

            if (dataInfo != null){
                dataInfo.updateEntityData(gameObject);
            }

            damageEffect();
        }
    }

    // rotate vector by x degrees
    public Vector2 rotateVector2(Vector2 baseVector, float angle){
        float newAngle = Mathf.Atan2(baseVector.y, baseVector.x) + angle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(newAngle), Mathf.Sin(newAngle));
    }

    public virtual void onPushObj(Entity pushInfo){

    }

    // get pushed by nearby objects
    public virtual void pushNearby(){
        if (currentHealth <= 0){
            return;
        }

        Collider2D myCollider = gameObject.GetComponent<Collider2D>();
        List<Collider2D> entityList = new List<Collider2D>();

        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(LayerMask.GetMask("EntityCollide"));
        filter.useLayerMask = true;

        // get any entities that are overlapping
        Physics2D.OverlapCollider(myCollider,filter,entityList);

        // go through each object and push if too close
        foreach(Collider2D collider in entityList){
            GameObject entityObj = collider.gameObject;
            if (entityObj){
                Entity entityData = entityObj.GetComponent<Entity>();
                Collider2D otherCollider = gameObject.GetComponent<Collider2D>();
                if (entityData && (entityData.currentHealth > 0 || entityObj.tag == "Player") && otherCollider){
                    Vector2 direction = ((Vector2)entityObj.transform.position - (Vector2)transform.position).normalized;
                    Vector2 randomDir = rotateVector2(direction.normalized,Random.Range(-900,900)/10).normalized;

                    // this only happens if they are right on top of eachother
                    if (direction.magnitude <= 0){
                        direction = rotateVector2(new Vector2(0f,1f),Random.Range(-360,360)).normalized * 3f;
                    }

                    // calculate push based on weights of two objects
                    Vector2 pushForce = (direction * -entityData.weight) + (direction * (weight * 0.7f));
                    if (weight * 0.7f > entityData.weight && entityData.weight > 0){
                        pushForce = direction * -entityData.weight * 0.1f;
                    }

                    if (entityData.weight <= 0){
                        pushForce = direction * ((entityData.weight - 2) * 0.5f);
                    }

                    // Player should take damage when bumped
                    if (entityObj.tag != gameObject.tag && meleeDamage > 0 && !entityData.perkIDList.Contains("noMelee")){
                        entityData.takeDamage(meleeDamage);
                    }else{
                        pushForce = pushForce + (randomDir * 0.1f);
                    }

                    onPushObj(entityData);

                    // Apply the force
                    rb.velocity = rb.velocity + (pushForce * 0.9f);
                }
            }
        } 
    }

    public IEnumerator doWait(float waitTime, System.Action onComplete){
        yield return new WaitForSeconds(waitTime);
        // run on complete
        onComplete();
    }

    public virtual void damageEffect(){
        if (gameObject){
            Transform bodyObj = gameObject.transform.Find("body");
            if (gameObject.GetComponent<SpriteRenderer>() && bodyObj == null){
                bodyObj = gameObject.transform;
            }

            if (bodyObj && bodyObj.gameObject){
                bodyObj.gameObject.GetComponent<SpriteRenderer>().material = Resources.Load("Materials/damaged") as Material;
                bodyObj.gameObject.GetComponent<SpriteRenderer>().color = new Color32(253,106,106,255);
                tookDamage = true;
                StartCoroutine(doWait(0.05f,delegate{
                    if (bodyObj && bodyObj.gameObject){
                        bodyObj.gameObject.GetComponent<SpriteRenderer>().material = Resources.Load("Materials/default") as Material;
                        bodyObj.gameObject.GetComponent<SpriteRenderer>().color = spriteColor;
                        tookDamage = false;
                    }
                }));
            }
        }
    }

    public virtual void damageOnDeath(){

    }

    public virtual void setupEntity(){
        Start();
    }

    public virtual void Start() {
        // Get data management script
        if (dataManager != null){
            dataInfo = dataManager.GetComponent<sharedData>();
            perkCommands = dataManager.GetComponent<perkModule>();
        }

        // get game management script
        if (gameManager != null){
            gameInfo = gameManager.GetComponent<gameLoader>();
        }

        // Get UI management script
        if (uiManager != null){
            uiUpdate = uiManager.GetComponent<UIManager>();
        }

        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    // Fixed Update is called every physics step
    public virtual void FixedUpdate() {
        if (perkCommands != null && currentHealth > 0){
            // Check for any entity lifetime modifiers
            Dictionary<string, GameObject> editList = new Dictionary<string, GameObject>();
            editList.Add("Owner", gameObject);
            editList.Add("GameManager",gameManager);
            editList.Add("DataManager",dataManager);
            perkCommands.applyPerk(perkIDList,"Update_Entity",editList);

            pushNearby();
        }
    }
}
