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
* 11/09/22  0.40                 DS              bumping
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Enemy : Entity
{
    // Targeting Variables
    public GameObject currentTarget;
    public float shootDistance = 5f;
    public Vector2 lookDirection = new Vector2(1f,0f);
    public float turnSpeed = 10f;

    // Pathfinding Variables
    public Vector2 lastTargetPosition;
    private float updatePathDistance = 4f; // if target moves more than x units from last position, update path
    public float wayPointDistance = 1f;
    [HideInInspector] private List<Vector2> currentPath = null;
    [HideInInspector] private int currentWaypoint = 0;
    [HideInInspector] private int visibleWaypoint = -1;
    private Seeker seekObj;

    // Visuals
    public visualFx killPrefab;
    public string defaultFace = "eyes_Normal";
    public string currentFace = "eyes_Normal";
    public float faceSwapTime = 0;

    public bool checkVisibility(GameObject target, float circleRadius){
        bool canSee = false;

        if (target != null && gameObject){
            Vector2 direction = ((Vector2)target.transform.position - (Vector2)transform.position).normalized;
            Vector2 origin = (Vector2)transform.position - direction;
            float distance = Vector2.Distance(origin, target.transform.position) + 5f;

            if (distance > 0){
                List<RaycastHit2D> contactList = new List<RaycastHit2D>();
                RaycastHit2D[] contacts = Physics2D.RaycastAll(origin,direction,distance,LayerMask.GetMask("EntityCollide","Obstacle"));
                    foreach(RaycastHit2D contact in contacts){
                        contactList.Add(contact);
                    } 
                
                if (circleRadius != 0f){
                    float radius = gameObject.GetComponent<CircleCollider2D>().radius * 1.1f;
                    if (circleRadius > 0){
                        radius = circleRadius;
                    }

                    RaycastHit2D[] addList = Physics2D.CircleCastAll(origin,radius,direction,distance,LayerMask.GetMask("EntityCollide","Obstacle"));
                    foreach(RaycastHit2D contact in addList){
                        contactList.Add(contact);
                    } 
                }

                RaycastHit2D closestHit = new RaycastHit2D();

                foreach(RaycastHit2D contact in contactList){
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
        if (currentHealth <= 0){
            currentTarget = null;
            return null;
        }

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
            if (damagedBy.tag != gameObject.tag){
                returnTarget = damagedBy;
            }
        }else{
            damagedBy = null;
        }

        // if no targets find a new one
        if (currentTarget == null){
            foreach (GameObject choice in targetChoices){
                if (choice != null && choice.transform){
                    Entity entityData = choice.GetComponent<Entity>();
                    if (entityData && entityData.currentHealth > 0){
                        if (checkVisibility(choice,0)){
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
        if (currentTarget != null && currentTarget.transform && currentHealth > 0){
            if (checkVisibility(currentTarget,0)){
                lookDirection = ((Vector2)transform.position - (Vector2)currentTarget.transform.position).normalized;
            }else if (movement.magnitude > 0){
                lookDirection = -rb.velocity;
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

            currentWaypoint = 1;
            visibleWaypoint = -1;

            // optimize the path so its not snapped to grid
            float radius = gameObject.GetComponent<CircleCollider2D>().radius * 0.5f;
            List<Vector2> newPath = new List<Vector2>();
            Vector2 lastPoint = new Vector2(0,0);

            for (int i = 0; i < pathGen.vectorPath.Count; i++){
                Vector2 fixPoint = pathGen.vectorPath[i];

                if (i > 0 && i < pathGen.vectorPath.Count - 1){
                    Vector2 direction = ((Vector2)pathGen.vectorPath[i + 1] - (Vector2)lastPoint).normalized;
                    Vector2 origin = lastPoint - direction * 1.5f;
                    float distance = Vector2.Distance(origin, pathGen.vectorPath[i + 1]) + 1f;

                    RaycastHit2D[] addList = Physics2D.RaycastAll(origin,direction,distance,LayerMask.GetMask("Obstacle"));
                    bool turnPoint = false;
                    
                    foreach(RaycastHit2D contact in addList){
                        if (contact && contact.collider){
                            turnPoint = true;
                            break;
                        }
                    } 

                    if (!turnPoint){
                        continue;
                    }
                }

                lastPoint = fixPoint;
                newPath.Add(fixPoint);
            }

            currentPath = newPath;

            if (currentTarget != null){
                lastTargetPosition = currentTarget.transform.position;
            }
        }
    }

    public virtual void movePattern(){
        if (currentHealth <= 0){
            rb.velocity = rb.velocity * 0.95f;
            return;
        }
        
        if (currentTarget != null){
            float distance = Vector2.Distance((Vector2)transform.position, (Vector2)currentTarget.transform.position);

            bool goStraight = false;
            if (checkVisibility(currentTarget,-1)){
                goStraight = true;
                if (visibleWaypoint <= -1 && currentWaypoint > 0){
                    visibleWaypoint = currentWaypoint;
                }

                if (currentPath == null){
                    goStraight = true;
                }else if (visibleWaypoint > 0){
                    if (currentWaypoint >= currentPath.Count || currentWaypoint >= visibleWaypoint + 5){
                        goStraight = true;
                    }
                }
            }

            // check if enemy can see target
            if (goStraight){
                lastTargetPosition = currentTarget.transform.position;
                currentPath = null;

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
                if (currentPath == null || currentWaypoint >= currentPath.Count){
                    movement = new Vector2(0,0);
                }else if (currentPath != null){
                    movement = (((Vector2) currentPath[currentWaypoint]) - ((Vector2) transform.position)).normalized;

                    // check if enemy should move to next point
                    float pointDistance = Vector2.Distance(transform.position,currentPath[currentWaypoint]);
                    if (pointDistance < wayPointDistance){
                        currentWaypoint++;
                    }
                }
            }
        }else{
            currentPath = null;
            movement = new Vector2(0,0);
        }

        Vector3 moveDirection = (movement.normalized * walkSpeed);
        rb.velocity = Vector3.Lerp(rb.velocity,moveDirection,Time.fixedDeltaTime * 2f);
    }

    public virtual void pushNearby(){
        if (currentHealth <= 0){
            return;
        }

        CircleCollider2D myCollider = gameObject.GetComponent<CircleCollider2D>();
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
                if (entityData && entityData.currentHealth > 0 && otherCollider){
                    Vector2 direction = ((Vector2)entityObj.transform.position - (Vector2)transform.position).normalized;

                    // this only happens if they are right on top of eachother
                    if (direction.magnitude <= 0){
                        direction = new Vector2(Random.Range(100,100)/200,Random.Range(100,100)/200);
                    }

                    // calculate push based on weights of two objects
                    Vector2 pushForce = (direction * -entityData.weight) + (direction * (gameObject.GetComponent<Entity>().weight * 0.7f));
                    if (gameObject.GetComponent<Entity>().weight * 0.7f > entityData.weight){
                        pushForce = direction * -entityData.weight * 0.1f;
                    }

                    if (entityData.weight <= 0){
                        pushForce = -direction * 2;
                    }

                    // Player should take damage when bumped
                    if (entityObj.tag == "Player"){
                        entityData.takeDamage(1);
                    }

                    // Apply the force
                    rb.velocity = rb.velocity + (pushForce * 0.9f);
                }
            }
        } 
    }

    // base enemy bullet stats
    public override void localEditBullet(bulletSystem bulletObj){
        base.localEditBullet(bulletObj);
        bulletObj.bulletSpeed = 3f;
        bulletObj.bulletSize = 0.14f;
        bulletObj.bulletBounces = 0;
    }

    // for on death
    private void fadeEyesAlpha(float value){
        if (transform.Find("eyes")){
            transform.Find("eyes").gameObject.GetComponent<SpriteRenderer>().color = new Color(1f,1f,1f,value);
        }
    }

    private bool killed = false;
    public override void takeDamage(int amount){
        base.takeDamage(amount);
        if (currentHealth <= 0 && !killed){
            killed = true;

            visualFx killVFX = Instantiate(killPrefab,new Vector3(transform.position.x,transform.position.y,-0.1f),new Quaternion(),gameObject.transform);
            if (killVFX != null){
                currentFace = "eyes_Shock";
                faceSwapTime = Time.time;

                killVFX.lifeTime = 0f;
                killVFX.killAnimation = true;
                killVFX.animSpeed = 1.3f;
                killVFX.destroyObj = gameObject;
                killVFX.gameObject.GetComponent<SpriteRenderer>().color = transform.Find("body").gameObject.GetComponent<SpriteRenderer>().color;
                transform.Find("body").gameObject.SetActive(false);
                LeanTween.value(transform.Find("eyes").gameObject,1f,0f,.3f).setEaseLinear().setOnUpdate(fadeEyesAlpha).setDelay(0.2f);

                float radius = gameObject.GetComponent<CircleCollider2D>().radius * 3f;
                killVFX.transform.localScale = new Vector3(radius,radius,1);
                killVFX.setupVFX();
            }
        }else{
            currentFace = "eyes_Hurt";
            faceSwapTime = Time.time;
        }
    }

    public virtual void shootGun(){
        if (currentTarget != null && currentHealth > 0){
            if (checkVisibility(currentTarget,1.5f)){
                // check angle between target and the way the enemy is facing
                Vector2 targetDirection = ((Vector2)currentTarget.transform.position - (Vector2)transform.position).normalized;
                Vector2 myDirection = -transform.Find("body").right.normalized;

                if (Vector2.Dot(targetDirection,myDirection) <= 0.5){
                    return;
                }

                if (fireBullets()){
                    currentFace = defaultFace;
                    faceSwapTime = 0;
                    
                    rb.velocity = rb.velocity * 0.8f;
                }
            }
        }
    }

    // for facial animations
    private string setFace = "";
    public virtual void faceCheck(){
        // should the face return to the default
        if (currentFace != defaultFace && Time.time - faceSwapTime >= .25){
            if (currentHealth <= 0){
                currentFace = "eyes_Hurt";
            }else{
                currentFace = defaultFace;
            }
        }

        // just so we dont load the sprite every frame
        if (transform.Find("eyes") && setFace != currentFace){
            setFace = currentFace;
            transform.Find("eyes").gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(currentFace);
        }
    }

    public override void Start() {
        base.Start();
        seekObj = gameObject.GetComponent<Seeker>();
    }

    // Fixed Update is called every physics step
    void FixedUpdate() {
        currentTarget = updateTarget();

        shootGun();
        movePattern();
        rotateEnemy();
        pushNearby();
        faceCheck();
    }
}
