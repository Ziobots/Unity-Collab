// Make sure to add that thing the teacher wants saying who edit what and when (check slides)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Vector2 movement;

    public float speed = 8f;
    public Rigidbody2D rb;

    //private bool facingRight = true;
    //private bool flipDebounce = true;

    // Update is called once per frame
    void Update()
    {
        // Handle Input Here
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate() {
        // Handle Movement Here

        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
    }
}
