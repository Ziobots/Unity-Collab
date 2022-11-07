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

    // Local Variables
    public float currentHealth = 5;
    public float maxHealth = 5;
    [HideInInspector] public int damageAmount = 0;
    public int currency = 0;

    // Stat Variables
    [HideInInspector] public GameObject damagedBy;

    // UI Stuff 
    public GameObject uiManager;
    [HideInInspector] public UIManager uiUpdate;

    // Movement Variables
    [HideInInspector] public Vector2 movement;
    public float walkSpeed = 6f;
    public Rigidbody2D rb;
    [HideInInspector] public bool facingRight = true;

    // Bullet Variables
    public bulletSystem bulletPrefab;
    public List<Transform> launchPoints = new List<Transform>();
    public Transform bulletFolder;
    // Temporary Bullet
    public int maxAmmo;
    public int currentAmmo;
    public float reloadTime;
    public float bulletTime;
    public float bulletSpread;
    public bool automaticGun = false;
    // time vars
    [HideInInspector] public float reloadStartTime = 0;
    [HideInInspector] public float delayStartTime = 0;

    // Sound Stuff
    public AudioSource gunNoise;
    public AudioSource reloadNoise;

    // Upgrade Variables
    public List<string> perkIDList;

    private void setCurrentAmmo(float value){
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

        // Check for any reload modifiers
        Dictionary<string, GameObject> editList = new Dictionary<string, GameObject>();
        editList.Add("Owner", gameObject);
        perkCommands.applyPerk(perkIDList,"Reload",editList);

        if (reloadNoise != null){
            reloadNoise.PlayOneShot(reloadNoise.clip,0.2f);
        }

        // add to the ammo one by one over time
        LeanTween.value(gameObject,(float)currentAmmo,(float)maxAmmo,reloadTime).setEaseLinear().setOnUpdate(setCurrentAmmo);

        if (dataInfo != null){
            dataInfo.updateEntityData(gameObject);
        }

        // update the ui
        if (uiUpdate != null && gameObject.tag == "Player"){
            uiUpdate.updateGameUI();
        }
    }

    // Entity will fire Bullets
    public virtual void fireBullets(){
        // make sure game is running
        if (Time.timeScale <= 0){
            return;
        }

        // check if entity is  in bullet cooldown
        if (Time.time - delayStartTime < bulletTime){
            return;
        }

        // check if entity is reloading
        if (Time.time - reloadStartTime < reloadTime){
            return;
        }

        // check if entity has ammo
        if (currentAmmo <= 0){
            reloadGun();
            return;
        }

        // entity passed all checks, can fire
        delayStartTime = Time.time;
        currentAmmo -= 1;

        // the delay after firing the last bullet shouldnt be as long
        if (currentAmmo <= 0){
            delayStartTime = Time.time - bulletTime + 0.25f;
        }

        if (gunNoise != null){
            gunNoise.PlayOneShot(gunNoise.clip,0.3f);
        }

        foreach(Transform point in launchPoints){
            // calculate bullet spread
            Quaternion spreadQuaternion = Quaternion.Euler(0, 0, 0);
            if (bulletSpread > 0){
                bulletSpread = Mathf.Clamp(bulletSpread,0,50);
                float angle = Random.Range(-bulletSpread * 1000,bulletSpread * 1000) / 1000;
                spreadQuaternion = Quaternion.Euler(0, 0, angle);
            }

            bulletSystem newBullet = Instantiate(bulletPrefab,point.position,point.rotation * spreadQuaternion,bulletFolder);
            if (newBullet != null){
                newBullet.dataManager = dataManager;
                newBullet.bulletOwner = gameObject;
                newBullet.bulletSpeed = 5f;
                newBullet.bulletSize = 0.11f;
                newBullet.bulletBounces = 0;
                newBullet.perkIDList = perkIDList;
                newBullet.bulletFolder = bulletFolder;
                newBullet.setupBullet();

                // Check for any onFire modifiers
                Dictionary<string, GameObject> editList = new Dictionary<string, GameObject>();
                editList.Add("Owner", gameObject);
                editList.Add("Bullet", newBullet.gameObject);
                perkCommands.applyPerk(perkIDList,"Shoot",editList);
            }
        }

        if (dataInfo != null){
            dataInfo.updateEntityData(gameObject);
        }

        if (uiUpdate != null && gameObject.tag == "Player"){
            uiUpdate.updateGameUI();
        }
    }

    public virtual void Start() {
        // Get data management script
        if (dataManager != null){
            dataInfo = dataManager.GetComponent<sharedData>();
            perkCommands = dataManager.GetComponent<perkModule>();
        }

        // Get UI management script
        if (uiManager != null){
            uiUpdate = uiManager.GetComponent<UIManager>();
        }
    }

    // Update is called once per frame
    void Update() {

    }

    // Fixed Update is called every physics step
    void FixedUpdate() {
        if (perkCommands != null){
            // Check for any entity lifetime modifiers
            Dictionary<string, GameObject> editList = new Dictionary<string, GameObject>();
            editList.Add("Owner", gameObject);
            perkCommands.applyPerk(perkIDList,"Update_Entity",editList);
        }
    }

    // bullets will call this when they hit
    public virtual void takeDamage(int amount){
        if (amount > 0){
            currentHealth -= amount;

            // Check for any bounce modifiers
            Dictionary<string, GameObject> editList = new Dictionary<string, GameObject>();
            editList.Add("Owner", gameObject);
            damageAmount = amount;
            perkCommands.applyPerk(perkIDList,"Damaged",editList);

            // check if the entity has died
            if (currentHealth <= 0){
                perkCommands.applyPerk(perkIDList,"Killed",editList);
            }
        }
    }
}
