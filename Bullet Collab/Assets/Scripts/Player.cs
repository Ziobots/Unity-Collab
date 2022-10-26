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

    private void moveGun() {
        if (arrow != null) {
            arrowDirection = (mousePosition - (Vector2)arrow.position).normalized;

            Vector2 arrowDir = arrowDirection * 1.2f;
            Vector2 arrowPos = (Vector2)transform.position + arrowDir;//(mousePosition.normalized);
            float distance = Vector2.Distance(transform.position,mousePosition);
            
            // Check if gun is too close to Player
            if (distance <= 2f){
                arrowPos = (Vector2)transform.position - ((arrowDir * 0.5f) * (1.5f - distance));// - (mousePosition.normalized);
            }

            // Set Gun Position
            arrow.position = Vector2.Lerp(arrow.position,arrowPos,Time.fixedDeltaTime * 30f);
            // Rotate Gun
            arrow.right = Vector2.Lerp(arrow.right,arrowDir,Time.fixedDeltaTime * 10f);
            
            // Gun Flip Direction
            if (arrow.transform.rotation.eulerAngles.y == 180){// this part is to fix some weird rotation rounding error
                arrow.GetComponent<SpriteRenderer>().flipY = facingRight;
            }else{
                arrow.GetComponent<SpriteRenderer>().flipY = !facingRight;
            }
        }
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
            fireBullets();
        }
    }

    // Fixed Update is called every physics step
    void FixedUpdate() {
        // Arrow Movement
        moveGun();
        // Flip Effect
        Quaternion setRotationEuler = Quaternion.Euler(0, facingRight ? 0f : 180f, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, setRotationEuler, Time.fixedDeltaTime * 10f);
        // Smooth Movement 
        Vector3 moveDirection = (movement.normalized * walkSpeed);
        rb.velocity = Vector3.Lerp(rb.velocity,moveDirection,Time.fixedDeltaTime * 10f);
    }
}
