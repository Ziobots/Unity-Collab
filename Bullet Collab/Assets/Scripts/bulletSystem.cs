using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletSystem : MonoBehaviour
{   
    // Editable Variables
    public float bulletSpeed = 1f;
    public float bulletSize = 0.5f;

    // Movement Variables
    public Rigidbody2D rb;

    private void Awake() {

    }

    void FixedUpdate() {
        rb.velocity = transform.right * bulletSpeed * Time.fixedDeltaTime * 100f;
    }

    private void OnCollisionEnter2D(Collision2D other) {
        Destroy(gameObject);
    }
}
