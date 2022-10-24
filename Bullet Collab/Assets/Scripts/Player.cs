// Make sure to add that thing the teacher wants saying who edit what and when (check slides)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Movement Variables
    public Vector2 movement;
    public float walkSpeed = 6f;
    public Rigidbody2D rb;

    // Mouse Variables
    public Vector2 mousePosition;
    public Transform arrow;
    public Vector2 arrowDirection = new Vector2(0,0);

    private bool facingRight = true;
    //private bool flipDebounce = tdrue;

    // Update is called once per frame
    void Update()
    {
        // Arrow Movement
        if (arrow != null){
            arrowDirection = (mousePosition - (Vector2)arrow.position).normalized;

            Vector2 arrowDir = arrowDirection * 1.2f;
            Vector2 arrowPos = (Vector2)transform.position + arrowDir;//(mousePosition.normalized);
            float distance = Vector2.Distance(transform.position,mousePosition);
            
            if (distance <= 2f){
                arrowPos = (Vector2)transform.position - ((arrowDir * 0.5f) * (1.5f - distance));// - (mousePosition.normalized);
            }

            arrow.position = Vector2.Lerp(arrow.position,arrowPos,Time.fixedDeltaTime * 1f);
            arrow.right = Vector2.Lerp(arrow.right,arrowDir,Time.fixedDeltaTime * 1f);
            arrow.GetComponent<SpriteRenderer>().flipY = !facingRight;
        }

        // Update Variables
        facingRight = (bool)(arrowDirection.x > 0);

        // Movement Input Here
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Mouse Direction Here
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Flip Effect
        Quaternion setRotationEuler = Quaternion.Euler(0,facingRight ? 0f : 180f,0);
        transform.rotation = Quaternion.Lerp(transform.rotation,setRotationEuler,Time.fixedDeltaTime * 0.8f);
    }

    // Fixed Update is called every physics step
    void FixedUpdate() {
        // Handle Movement Here 

        Vector3 moveDirection = (movement.normalized * walkSpeed);
        rb.velocity = Vector3.Lerp(rb.velocity,moveDirection,Time.fixedDeltaTime * 10f);
    }
}
