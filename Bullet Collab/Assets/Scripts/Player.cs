// Make sure to add that thing the teacher wants saying who edit what and when (check slides)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Movement Variables
    public Vector2 movement;
    public float walkSpeed = 8f;
    public Rigidbody2D rb;

    // Mouse Variables
    public Vector2 mousePosition;
    public Transform arrow;

    //private bool facingRight = true;
    //private bool flipDebounce = tdrue;

    // Update is called once per frame
    void Update()
    {
        // Movement Input Here
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Mouse Direction Here
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Player Visuals
        if (arrow != null){
            arrow.up = (mousePosition - (Vector2)transform.position).normalized;
        }
    }

    // Fixed Update is called every physics step
    void FixedUpdate() {
        // Handle Movement Here 

        rb.MovePosition(rb.position + movement.normalized * walkSpeed * Time.fixedDeltaTime);
    }
}
