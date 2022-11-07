/*******************************************************************************
* Name : Player.cs
* Section Description : This code handles player inputs, movement, and weapon position. It uses Unity's input detectors.
* Update activates every frame and fixed update activates separately of that so that the physics should function the same
* no matter the specs of the user's system. 
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 10/24/22  0.10                 DS              Made the thing
* 10/25/22  0.11                 KJ              Moved gun and flip to Fixed, changed Lerp alpha values to 10
* 11/02/22  0.15                 DS              used ref for perkidlist 
* 11/03/22  0.20                 DS              updated health stuff, removed ref for perk list
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    // Mouse Variables
    [HideInInspector] public Vector2 mousePosition;
    [HideInInspector] public Vector2 arrowDirection = new Vector2(0,0);
    public Transform arrow;
    public GameObject cursorObj;

    // Rig Variables
    public Transform playerRig;
    public float armDistance = 0;

    // Local Variables
    public float hitTime = 0f;
    public float iFrames = 0.5f;

    private void moveGun() {
        if (arrow != null) {
            arrowDirection = (mousePosition - (Vector2)arrow.position).normalized;

            Vector2 arrowDir = arrowDirection * 1.2f;
            Vector2 arrowPos = (Vector2)playerRig.position + arrowDir;//(mousePosition.normalized);
            float distance = Vector2.Distance(playerRig.position,mousePosition);
            
            // Check if gun is too close to Player
            if (distance <= 2f){
                arrowPos = (Vector2)playerRig.position - ((arrowDir * 0.5f) * (1.5f - distance));// - (mousePosition.normalized);
            }

            // Set Gun Position
            arrow.position = Vector2.Lerp(arrow.position,arrowPos,Time.fixedDeltaTime * 30f);

            // Rotate Gun
            arrow.right = Vector2.Lerp(arrow.right,arrowDir,Time.fixedDeltaTime * 10f);
            
            // Set Arm Distance
            armDistance = Vector2.Distance(playerRig.position,arrow.position + (arrow.right.normalized * 1f));

            // Gun Flip Direction
            if (arrow.transform.rotation.eulerAngles.y == 180){// this part is to fix some weird rotation rounding error
                arrow.GetComponent<SpriteRenderer>().flipY = facingRight;
            }else{
                arrow.GetComponent<SpriteRenderer>().flipY = !facingRight;
            }
        }
    }

    // overwrite the take damage function so its only 1 damage - also check for gameover here
    public override void takeDamage(int amount){
        // Player can only take damage every so often
        if (Time.time - hitTime >= iFrames){
            hitTime = Time.time;

            // Run Damage Visual - only 1 damage for players
            base.takeDamage(1);

            // Update Data
            dataInfo.updateEntityData(gameObject);
            uiUpdate.updateGameUI(); 
        }
    }

    // Carry Player between scenes
    public override void Start() {
        // Entity Setup
        base.Start();

        // Keep Player between Scenes
        DontDestroyOnLoad(gameObject);

        perkIDList.Add("bounceDmg");
        perkIDList.Add("bounceDmg");
        perkIDList.Add("perkLottery");
        perkIDList.Add("perkLottery");
        perkIDList.Add("perkLottery");
        //dataInfo.perkIDList.Add("perkLottery");
        //perkIDList.Add("remoteBullet");

        // apply any changes to the data
        dataInfo.updateEntityData(gameObject);
        uiUpdate.updateGameUI();
    }

    // Update is called once per frame
    void Update() {
        // Update Variables
        facingRight = (bool)(arrowDirection.x > 0);

        // Movement Input Here
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Mouse Direction Here
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Check for mouse down
        bool isMouseDown = Input.GetButtonDown("Fire1");
        if (automaticGun){
            isMouseDown = Input.GetMouseButton(0);
        }

        // fire bullet
        if (isMouseDown) {
            // Check if mouse is hovering button
            if (cursorObj == null || !cursorObj.GetComponent<mouseCursor>().isHovering){
                // check if player is too close to wall
                Vector2 origin = playerRig.position;
                RaycastHit2D contact = Physics2D.Raycast(origin,arrowDirection.normalized,armDistance * 1.1f,LayerMask.GetMask("Default"));
                if (!contact){
                    fireBullets();
                }
            }
        }

        if (Input.GetKeyDown("r")){
            // check to see if they, just fired bullet, have max ammo, are currently reloading
            if (Time.time - delayStartTime >= 0.25f && currentAmmo < maxAmmo && Time.time - reloadStartTime >= reloadTime){
                reloadGun();
            }
        }
    }

    // Fixed Update is called every physics step
    void FixedUpdate() {
        // Arrow Movement
        moveGun();
        // Flip Effect
        Quaternion setRotationEuler = Quaternion.Euler(0, facingRight ? 0f : 180f, 0);
        playerRig.rotation = Quaternion.Lerp(playerRig.rotation, setRotationEuler, Time.fixedDeltaTime * 10f);
        // Smooth Movement 
        Vector3 moveDirection = (movement.normalized * walkSpeed);
        rb.velocity = Vector3.Lerp(rb.velocity,moveDirection,Time.fixedDeltaTime * 10f);
    }
}
