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

    public override void takeDamage(int amount){
        // Player can only take damage every so often
        if (Time.time - hitTime >= iFrames){
            hitTime = Time.time;

            // Players should only take 1 damage when hit
            amount = 1;

            // Run Damage Visual
            base.takeDamage(amount);

            // Update Data
            dataInfo.currenthealth -= amount;
            uiUpdate.updateHealth(); 
        }
    }

    // Carry Player between scenes
    public override void Start() {
        // Keep Player between Scenes
        DontDestroyOnLoad(gameObject);

        perkIDList = new List<string>();
        perkIDList.Add("bounceDmg");
        perkIDList.Add("bounceDmg");
        perkIDList.Add("bounceDmg");
        perkIDList.Add("bounceDmg");
        perkIDList.Add("bounceDmg");
        //perkIDList.Add("remoteBullet");

        // Entity Setup
        base.Start();
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

        // Bullet Fire
        if (Input.GetButtonDown("Fire1")) {
            // Check if mouse is hovering button
            if (cursorObj == null || !cursorObj.GetComponent<mouseCursor>().isHovering){
                // check if player is too close to wall
                Vector2 origin = playerRig.position;
                RaycastHit2D contact = Physics2D.Raycast(origin,arrowDirection.normalized,armDistance * 1.1f,LayerMask.GetMask("Default"));
                if (!contact){
                    attackTime = Time.time;
                    fireBullets();
                }
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
