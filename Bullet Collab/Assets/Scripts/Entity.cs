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
    public float health;

    // Movement Variables
    [HideInInspector] public Vector2 movement;
    public float walkSpeed = 6f;
    public Rigidbody2D rb;
    [HideInInspector] public bool facingRight = true;

        // Bullet Variables
    public bulletSystem bulletPrefab;
    public List<Transform> launchPoints = new List<Transform>();

    public Transform bulletFolder;

    // Entity will fire Bullets
    public void fireBullets(){
        foreach(Transform point in launchPoints){
            bulletSystem newBullet = Instantiate(bulletPrefab,point.position,point.rotation,bulletFolder);
            if (newBullet != null){
                newBullet.bulletOwner = this;
            }
        }
    }

    // Update is called once per frame
    void Update(){
        
    }

    // bullets will call this when they hit
    void takeDamage(int amount){

    }
}
