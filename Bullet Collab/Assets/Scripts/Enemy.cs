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
* 11/09/22  0.40                 DS              bumping + damage effects
* 11/10/22  0.50                 DS              spawn in animation
* 11/10/22  0.60                 DS              enemies patrol the area instead of standing still when no target
* 11/10/22  0.70                 DS              added stats stuff
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum EnemyType {None,Mob,StrongMob,UniqueMob,MiniBoss,Boss};

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
    public GameObject levelObj = null;
    public GameObject pathTarget = null;

    // Visuals
    public visualFx killPrefab;
    public string defaultFace = "eyes_Normal";
    public string currentFace = "eyes_Normal";
    public float faceSwapTime = 0;

    // Enemy Variables
    public bool checkAngle = true;
    public bool flipSprite = true;
    public EnemyType myType = EnemyType.None;
    public int roomSpawnMinimum = 0;

    // Spawn Visuals
    public bool Loaded = false;

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
            editList.Add("GameManager",gameManager);
            editList.Add("DataManager",dataManager);
            perkCommands.applyPerk(perkIDList,"Target_Change",editList);

            // We can use the edit list to send back information as well
            if (editList.ContainsKey("NEW_Target")){
                currentTarget = editList["NEW_Target"];
            }
        }

        return returnTarget;
    }

    // figure out which way the enemy is supposed to be facing
    public virtual Vector2 getLookDirection(){
        if (currentTarget != null && currentTarget.transform && currentHealth > 0){
            if (checkVisibility(currentTarget,0)){
                lookDirection = ((Vector2)transform.position - (Vector2)currentTarget.transform.position).normalized;
            }else if (movement.magnitude > 0){
                lookDirection = -rb.velocity;//-movement.normalized;// -rb.velocity;
            }
        }else if (movement.magnitude > 0){
            lookDirection = -rb.velocity;// -movement.normalized;
        }

        return lookDirection;
    }

    public virtual void rotateEnemy(){
        // get the look direction
        lookDirection = getLookDirection();

        // Rotate Base
        float alpha = Time.fixedDeltaTime * turnSpeed * 0.5f;
        float turnAlpha = Mathf.Clamp(alpha,0f,1f);
        transform.Find("body").right = Vector2.Lerp(transform.Find("body").right,lookDirection,turnAlpha);
        facingRight = (bool)(lookDirection.x > 0);

        bool facingDirection = facingRight;
        if (!flipSprite){
            facingDirection = true;
        }

        // Gun Flip Direction
        if (transform.Find("body").rotation.eulerAngles.y == 180){// this part is to fix some weird rotation rounding error
            transform.Find("body").gameObject.GetComponent<SpriteRenderer>().flipY = facingDirection;
        }else{
            transform.Find("body").gameObject.GetComponent<SpriteRenderer>().flipY = !facingDirection;
        }

        // Flip Effect
        Quaternion setRotationEuler = Quaternion.Euler(0, facingRight ? 0f : 180f, 0);
        alpha = Time.fixedDeltaTime * 10f;
        transform.Find("eyes").rotation = Quaternion.Lerp(transform.Find("eyes").rotation, setRotationEuler, alpha);
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
                pathTarget = null;
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
            }else if(seekObj && currentTarget != null){
                pathTarget = currentTarget;
            }
        }else{
            // do patrol
            if (pathTarget == null && levelObj && levelObj.transform.Find("spawnPoints")){
                Transform spawnPoints = levelObj.transform.Find("spawnPoints");
                if (spawnPoints.childCount > 0){
                    pathTarget = spawnPoints.GetChild(Random.Range(0,spawnPoints.childCount)).gameObject;
                    lastTargetPosition = pathTarget.transform.position;
                    //currentPath = null;
                    //movement = new Vector2(0,0);
                }
            }
        }

        if (pathTarget != null && pathTarget.transform){
            // calculate new path to last target position
            if (seekObj.IsDone()){
                float updateDistance = Vector2.Distance(lastTargetPosition,pathTarget.transform.position);
                if (currentPath == null || updateDistance > updatePathDistance){
                    //currentPath = null;
                    seekObj.StartPath(transform.position,pathTarget.transform.position,pathGenerated);
                }
            }

            // check if reached end of path
            if (currentPath == null || currentWaypoint >= currentPath.Count){
                // only end if not calc new path
                if (seekObj.IsDone()){
                    pathTarget = null;
                    currentPath = null;
                    movement = new Vector2(0,0);
                }
            }else if (currentPath != null){
                movement = (((Vector2) currentPath[currentWaypoint]) - ((Vector2) transform.position)).normalized;

                // check if enemy should move to next point
                float pointDistance = Vector2.Distance(transform.position,currentPath[currentWaypoint]);
                if (pointDistance < wayPointDistance){
                    currentWaypoint++;
                }
            }
        }

        Vector3 moveDirection = (movement.normalized * walkSpeed);
        float alpha = Time.fixedDeltaTime * 2f;
        rb.velocity = Vector3.Lerp(rb.velocity,moveDirection,alpha);
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
    public override void takeDamage(float amount){
        if (!Loaded){
            return;
        }

        base.takeDamage(amount);
        
        if (currentHealth <= 0 && !killed){
            killed = true;

            if (damagedBy != null){
                int healthMoney = (int)(Mathf.Ceil((int)Mathf.Sqrt(maxHealth) * 1.5f) + 1);
                int addMoney = (int)Mathf.Ceil(Random.Range(healthMoney * 0.5f,healthMoney * Mathf.Sqrt(dataInfo.currentRoom) * 0.8f));

                // give the killer money, this works for enemies too
                Entity killerInfo = damagedBy.GetComponent<Entity>();
                if (killerInfo != null){
                    killerInfo.currency += (currency + addMoney);
                }

                // add player stats
                if (damagedBy.tag == "Player"){
                    if (dataInfo != null){
                        dataInfo.enemiesKilled++;
                        dataInfo.totalScore += (int)(maxHealth * 10);
                        if (uiUpdate != null){
                            uiUpdate.updateGameUI();
                        }
                    }
                }
            }

            visualFx killVFX = Instantiate(killPrefab,new Vector3(transform.position.x,transform.position.y,-0.1f),new Quaternion(),gameObject.transform);
            if (killVFX != null){
                currentFace = "eyes_Shock";
                faceSwapTime = Time.time;

                killVFX.lifeTime = 0f;
                killVFX.killAnimation = true;
                killVFX.animSpeed = 1.3f;
                killVFX.destroyObj = gameObject;
                killVFX.gameObject.GetComponent<SpriteRenderer>().color = spriteColor;
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

    // for enemies with unique fire patterns
    public virtual bool fireGunCheck(){
        return false;
    }

    public virtual void shootGun(){
        if (currentTarget != null && currentHealth > 0){
            if (checkVisibility(currentTarget,1f) || fireGunCheck()){
                // check angle between target and the way the enemy is facing
                Vector2 targetDirection = ((Vector2)currentTarget.transform.position - (Vector2)transform.position).normalized;
                Vector2 myDirection = -transform.Find("body").right.normalized;

                if (Vector2.Dot(targetDirection,myDirection) <= 0.5 && checkAngle){
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

    // tween functions
    private void spawnRotation(float value){
        Quaternion setRotationEuler = Quaternion.Euler(0f, value, 0f);
        transform.rotation = setRotationEuler;
    }

    private void spawnFinished(){
        spawnRotation(0f);
        Loaded = true;
    }

    private void spawnAnimation(){
        spawnRotation(90);
        LeanTween.value(gameObject,270f,720f,.7f).setEaseOutBack().setOnUpdate(spawnRotation).setOnComplete(spawnFinished);
    }

    public override void setupEntity(){
        Loaded = false;

        base.setupEntity();

        if (currentCamera != null){
            hurtNoise = currentCamera.transform.Find("SoundAssets").Find("hurt").gameObject.GetComponent<AudioSource>();
            gunNoise = currentCamera.transform.Find("SoundAssets").Find("enemyFire").gameObject.GetComponent<AudioSource>();
        }

        spawnAnimation();
    }

    public override void Start() {
        base.Start();
        seekObj = gameObject.GetComponent<Seeker>();
    }

    // Fixed Update is called every physics step
    public override void FixedUpdate() {
        if (!Loaded){
            return;
        }

        base.FixedUpdate();

        currentTarget = updateTarget();

        shootGun();
        movePattern();
        rotateEnemy();
        faceCheck();
    }
}
