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

    // 

    //private bool facingRight = true;
    //private bool flipDebounce = tdrue;

    // Update is called once per frame
    void Update()
    {
        // Handle Input Here
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate() {
        // Handle Movement Here 

        rb.MovePosition(rb.position + movement.normalized * walkSpeed * Time.fixedDeltaTime);
    }
}
