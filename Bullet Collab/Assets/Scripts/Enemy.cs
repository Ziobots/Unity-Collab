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

    public bool checkVisibility(GameObject target){
        bool canSee = false;

        print("CHECK VIS" + target.name);
        if (target != null && gameObject){
            Vector2 direction = ((Vector2)transform.position - (Vector2)target.transform.position).normalized;
            Vector2 origin = transform.position;
            float distance = Vector2.Distance(origin, target.transform.position) + 2f;
            if (distance > 0){
                RaycastHit2D[] contacts = Physics2D.RaycastAll(origin,direction,distance,LayerMask.GetMask("EntityCollide","Default"),0f);
                foreach(RaycastHit2D contact in contacts){
                    if (contact.collider && contact.collider.gameObject != gameObject){
                        print(contact.collider);
                        canSee = canSee || contact.collider.gameObject == target;
                    }
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
        rb.velocity = Vector3.Lerp(rb.velocity,moveDirection,Time.fixedDeltaTime * 10f);
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
                fireBullets();
            }
        }

        movePattern();
    }
}
