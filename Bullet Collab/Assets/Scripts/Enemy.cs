/*******************************************************************************
* Name : Enemy.cs
* Section Description : This code handles basic enemy behavior which will inhertied by unique enemy classes.
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 10/26/22  0.10                 DS              Made the thing
* 11/07/22  0.20                 DS              started AI
* 11/08/22  0.30                 DS              pathfinding
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Pathfinding;

public class Enemy : Entity
{
    // Targeting Variables
    public GameObject currentTarget;
    public float shootDistance = 5f;
    public Vector2 lookDirection = new Vector2(1f,0f);
    public float turnSpeed = 10f;

    // Pathfinding Variables
    public Vector2 lastTargetPosition;
    private float updatePathDistance = 8f; // if target moves more than x units from last position, update path
    public float wayPointDistance = 1f;
    private Path currentPath;
    private int currentWaypoint = 0;
    private Seeker seekObj;

    public bool checkVisibility(GameObject target, bool circle){
        bool canSee = false;

        if (target != null && gameObject){
            Vector2 direction = ((Vector2)target.transform.position - (Vector2)transform.position).normalized;
            Vector2 origin = (Vector2)transform.position - direction;
            float distance = Vector2.Distance(origin, target.transform.position) + 5f;

            if (distance > 0){
                RaycastHit2D[] contacts = Physics2D.RaycastAll(origin,direction,distance,LayerMask.GetMask("EntityCollide","Obstacle"));
                if (circle){
                    float radius = gameObject.GetComponent<CircleCollider2D>().radius * 1.5f;
                    contacts = Physics2D.CircleCastAll(origin,radius,direction,distance,LayerMask.GetMask("EntityCollide","Obstacle"));
                }

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
            }else{
                returnTarget = currentTarget;
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
                        if (checkVisibility(choice,false)){
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
            if (checkVisibility(currentTarget,true)){
                lookDirection = ((Vector2)transform.position - (Vector2)currentTarget.transform.position).normalized;
            }else if (movement.magnitude > 0){
                lookDirection = -movement;
            }
        }

        // Rotate Base
        float turnAlpha = Mathf.Clamp(Time.fixedDeltaTime * turnSpeed,0f,1f);
        transform.Find("body").right = Vector2.Lerp(transform.Find("body").right,lookDirection,turnAlpha);
        facingRight = (bool)(lookDirection.x > 0);

        // Gun Flip Direction
        if (transform.rotation.eulerAngles.y == 180){// this part is to fix some weird rotation rounding error
            transform.Find("body").gameObject.GetComponent<SpriteRenderer>().flipY = facingRight;
        }else{
            transform.Find("body").gameObject.GetComponent<SpriteRenderer>().flipY = !facingRight;
        }

        // Flip Effect
        Quaternion setRotationEuler = Quaternion.Euler(0, facingRight ? 0f : 180f, 0);
        transform.Find("eyes").rotation = Quaternion.Lerp(transform.Find("eyes").rotation, setRotationEuler, Time.fixedDeltaTime * 10f);
    }

    private void pathGenerated(Path pathGen){
        if (!pathGen.error){
            currentPath = pathGen;
            currentWaypoint = 0;

            if (currentTarget != null){
                lastTargetPosition = currentTarget.transform.position;
            }
        }
    }

    public virtual void movePattern(){
        if (currentTarget != null){
            float distance = Vector2.Distance((Vector2)transform.position, (Vector2)currentTarget.transform.position);

            // check if enemy can see target
            if (checkVisibility(currentTarget,true)){
                lastTargetPosition = currentTarget.transform.position;

                // check if enemy is in shooting range
                if (distance <= shootDistance){
                    movement = new Vector2(0,0);
                }else{
                    // no need to pathfind if the enemy can see the target, straight line path
                    movement = ((Vector2)currentTarget.transform.position - (Vector2)transform.position).normalized;
                }
            }else if(seekObj){
                // calculate new path to last target position
                if (seekObj.IsDone()){
                    float updateDistance = Vector2.Distance(lastTargetPosition,currentTarget.transform.position);
                    if (currentPath == null || updateDistance > updatePathDistance){
                        seekObj.StartPath(transform.position,currentTarget.transform.position,pathGenerated);
                    }
                }

                // check if reached end of path
                if (currentPath == null || currentWaypoint >= currentPath.vectorPath.Count){
                    movement = new Vector2(0,0);
                }else if (currentPath != null){
                    movement = (((Vector2) currentPath.vectorPath[currentWaypoint]) - ((Vector2) transform.position)).normalized;

                    // check if enemy should move to next point
                    float pointDistance = Vector2.Distance(transform.position,currentPath.vectorPath[currentWaypoint]);
                    if (pointDistance < wayPointDistance){
                        currentWaypoint++;
                    }
                }
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

    public override void Start() {
        base.Start();
        seekObj = gameObject.GetComponent<Seeker>();
    }

    // Update is called once per frame
    private void Update() {
        //if(Time.time - attackTime >= 1f){
        //    attackTime = Time.time;
            //fireBullets();
        //}
    }

    // Fixed Update is called every physics step
    void FixedUpdate() {
        currentTarget = updateTarget();

        if (currentTarget != null){
            if (checkVisibility(currentTarget,false)){
                if (fireBullets()){
                    rb.velocity = rb.velocity * 0.5f;
                }
            }
        }

        movePattern();
        rotateEnemy();
    }
}
