/*******************************************************************************
* Name : Entity.cs
* Section Description : This is a superclass for bullet detection purposes. Player, Enemy (and by extension Boss), 
* and possibly destructable obstacles will be subclasses.
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 10/24/22  0.10                 DS              Made the thing
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
    public float reloadTime;
    public float bulletTime;
    // time vars
    [HideInInspector] public float reloadStartTime = 0;
    [HideInInspector] public float delayStartTime = 0;

    // Upgrade Variables
    public List<string> perkIDList;

    // Entity will fire Bullets
    public virtual void fireBullets(){
        if (Time.timeScale <= 0){
            return;
        }

        foreach(Transform point in launchPoints){
            bulletSystem newBullet = Instantiate(bulletPrefab,point.position,point.rotation,bulletFolder);
            if (newBullet != null){
                newBullet.dataManager = dataManager;
                newBullet.bulletOwner = gameObject;
                newBullet.bulletSpeed = 5f;
                newBullet.bulletSize = 0.11f;
                newBullet.bulletBounces = 5;
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
