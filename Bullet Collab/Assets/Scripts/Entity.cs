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
    public float health = 5;

    // UI Stuff
    public GameObject uiManager;
    [HideInInspector] public UIManager uiUpdate;

    // Movement Variables
    [HideInInspector] public Vector2 movement;
    public float walkSpeed = 6f;
    public Rigidbody2D rb;
    [HideInInspector] public bool facingRight = true;

    // Bullet Variables
    [HideInInspector] public float attackTime = 0;
    public bulletSystem bulletPrefab;
    public List<Transform> launchPoints = new List<Transform>();
    public Transform bulletFolder;

    // Entity will fire Bullets
    public virtual void fireBullets(){
        foreach(Transform point in launchPoints){
            bulletSystem newBullet = Instantiate(bulletPrefab,point.position,point.rotation,bulletFolder);
            if (newBullet != null){
                newBullet.bulletOwner = gameObject;
                newBullet.bulletBounces = 5;
            }
        }
    }

    public virtual void Start() {
        // Get data management script
        if (dataManager != null){
            dataInfo = dataManager.GetComponent<sharedData>();
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

    }

    // bullets will call this when they hit
    public virtual void takeDamage(int amount){
        if (amount > 0){
            health -= amount;

            if (rb != null){
                //rb.velocity = new Vector3(0,0,0);
            }
        }
    }
}
