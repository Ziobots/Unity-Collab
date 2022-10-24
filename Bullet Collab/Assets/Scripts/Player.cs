// Make sure to add that thing the teacher wants saying who edit what and when (check slides)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Movement Variables
    public Vector2 movement;
    public float walkSpeed = 1f;
    public Rigidbody2D rb;

    // Mouse Variables
    public Vector2 mousePosition;
    public Transform arrow;

    private bool facingRight = true;
    //private bool flipDebounce = tdrue;

    // Update is called once per frame
    void Update()
    {
        // Update Variables
        facingRight = (bool)(movement.x > 0);

        // Movement Input Here
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Mouse Direction Here
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Arrow
        if (arrow != null){
            Vector2 arrowDir = (mousePosition - (Vector2)arrow.position).normalized;
            Vector2 arrowPos = (Vector2)transform.position + arrowDir;//(mousePosition.normalized);
            float distance = Vector2.Distance(transform.position,mousePosition);
            
            if (distance <= 2f){
                arrowPos = (Vector2)transform.position - (arrowDir * (1.5f - distance));// - (mousePosition.normalized);
            }

            arrow.position = Vector2.Lerp(arrow.position,arrowPos,Time.fixedDeltaTime * 0.5f);
            arrow.right = Vector2.Lerp(arrow.right,arrowDir,Time.fixedDeltaTime * 1f);

            //arrow.GetComponent<SpriteRenderer>().flipY = (bool)(arrowDir.x < 0);
        }

        float yRot = Mathf.Lerp(transform.rotation.y,facingRight ? 180 : 0,Time.fixedDeltaTime * 0.5f);
        transform.Rotate(0,yRot,0);
    }

    // Fixed Update is called every physics step
    void FixedUpdate() {
        // Handle Movement Here 

        rb.MovePosition(rb.position + movement.normalized * walkSpeed * Time.fixedDeltaTime);
    }
}
