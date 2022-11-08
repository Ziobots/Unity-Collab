/*******************************************************************************
* Name : Enemy.cs
* Section Description : This code handles basic enemy behavior which will inhertied by unique enemy classes.
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 10/26/22  0.10                 DS              Made the thing
* 11/07/22  0.20                 DS              started AI
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    // Targeting Variables
    [HideInInspector] public GameObject currentTarget;
    public float shootDistance = 5f;
    public Vector2 lookDirection = new Vector2(1f,0f);
    public float turnSpeed = 10f;

    public bool checkVisibility(GameObject target){
        bool canSee = false;

        if (target != null && gameObject){
            Vector2 direction = ((Vector2)target.transform.position - (Vector2)transform.position).normalized;
            Vector3 origin = transform.position;
            float distance = Vector2.Distance(origin, target.transform.position) + 2f;
            if (distance > 0){
                RaycastHit2D[] contacts = Physics2D.RaycastAll(origin,direction,distance,LayerMask.GetMask("EntityCollide","Default"));
                RaycastHit2D closestHit = new RaycastHit2D();

                foreach(RaycastHit2D contact in contacts){
                    if (!closestHit.collider || Vector3.Distance(contact.point,origin) < Vector3.Distance(closestHit.point,origin)){
                        if (contact.collider.gameObject != gameObject){
                            closestHit = contact;
                        }
                    }
                }

                if (closestHit && closestHit.collider && closestHit.collider.gameObject == target){
                    canSee = true;
                }
            }
        }

        return canSee;
    }

    // update the Target, is usually the player
    public virtual GameObject updateTarget(){
        GameObject[] targetChoices = GameObject.FindGameObjectsWithTag("Player");
        GameObject returnTarget = null;

        // check currentTarget
        if (currentTarget != null){
            Entity entityData = currentTarget.GetComponent<Entity>();
            if (entityData && entityData.currentHealth <= 0){
                // remove current target since it is dead
                currentTarget = null;
            }
        }

        // prioritize entities that have dealt damage to this
        if (damagedBy != null && damagedBy.GetComponent<Entity>().currentHealth > 0){
            returnTarget = damagedBy;
        }else{
            damagedBy = null;
        }

        // if no targets find a new one
        if (currentTarget == null){
            foreach (GameObject choice in targetChoices){
                if (choice != null && choice.transform){
                    Entity entityData = choice.GetComponent<Entity>();
                    if (entityData && entityData.currentHealth > 0){
                        if (checkVisibility(choice)){
                            if (!returnTarget || Vector3.Distance(returnTarget.transform.position,transform.position) <= Vector3.Distance(choice.transform.position,transform.position)){
                                returnTarget = choice;
                            }
                        }
                    }
                }
            }
        }

        // was the target changed
        if (currentTarget != returnTarget){
            currentTarget = returnTarget;

            // Check for Target Change modifiers
            Dictionary<string, GameObject> editList = new Dictionary<string, GameObject>();
            editList.Add("Owner", gameObject);
            editList.Add("Target", currentTarget);
            perkCommands.applyPerk(perkIDList,"Target_Change",editList);

            // We can use the edit list to send back information as well
            if (editList.ContainsKey("NEW_Target")){
                currentTarget = editList["NEW_Target"];
            }
        }

        return returnTarget;
    }

    public virtual void rotateEnemy(){
        if (currentTarget != null && currentTarget.transform){
            lookDirection = ((Vector2)transform.position - (Vector2)currentTarget.transform.position).normalized;
        }

        // Rotate Base
        float turnAlpha = Mathf.Clamp(Time.fixedDeltaTime * turnSpeed,0f,1f);
        transform.Find("body").right = Vector2.Lerp(transform.Find("body").right,lookDirection,turnAlpha);
        facingRight = (bool)(lookDirection.x > 0);

        // Gun Flip Direction
        if (transform.rotation.eulerAngles.y == 180){// this part is to fix some weird rotation rounding error
            transform.Find("body").gameObject.GetComponent<SpriteRenderer>().flipY = facingRight;
            transform.Find("eyes").gameObject.GetComponent<SpriteRenderer>().flipX = facingRight;
        }else{
            transform.Find("body").gameObject.GetComponent<SpriteRenderer>().flipY = !facingRight;
            transform.Find("eyes").gameObject.GetComponent<SpriteRenderer>().flipX = !facingRight;
        }
    }

    public virtual void movePattern(){
        if (currentTarget != null){
            float distance = Vector2.Distance((Vector2)transform.position, (Vector2)currentTarget.transform.position);
            if (checkVisibility(currentTarget)){
                if (distance <= shootDistance){
                    movement = new Vector2(0,0);
                }else{
                    movement = ((Vector2)currentTarget.transform.position - (Vector2)transform.position).normalized;
                }
            }else{
                // put pathfinding here if doing that?
                movement = ((Vector2)currentTarget.transform.position - (Vector2)transform.position).normalized;
            }
        }else{
            movement = new Vector2(0,0);
        }

        Vector3 moveDirection = (movement.normalized * walkSpeed);
        rb.velocity = Vector3.Lerp(rb.velocity,moveDirection,Time.fixedDeltaTime * 5f);
    }

    // base enemy bullet stats
    public override void localEditBullet(bulletSystem bulletObj){
        base.localEditBullet(bulletObj);
        bulletObj.bulletSpeed = 3f;
        bulletObj.bulletSize = 0.14f;
        bulletObj.bulletBounces = 0;
    }

    public override void takeDamage(int amount){
        base.takeDamage(amount);
        if (amount > 0){

        }
    }

    // Update is called once per frame
    void Update() {
        //if(Time.time - attackTime >= 1f){
        //    attackTime = Time.time;
            //fireBullets();
        //}
    }

    // Fixed Update is called every physics step
    void FixedUpdate() {
        currentTarget = updateTarget();

        if (currentTarget != null){
            if (checkVisibility(currentTarget)){
                if (fireBullets()){
                    rb.velocity = rb.velocity * 0.5f;
                }
            }
        }

        movePattern();
        rotateEnemy();
    }
}
