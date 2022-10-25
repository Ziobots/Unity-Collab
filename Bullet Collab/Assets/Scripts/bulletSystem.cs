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

    // Bullet Local Variabls
    private float createTime;
    private float lifeTime = 20f;

    private void Awake() {
        createTime = Time.time;
    }

    private void removeBullet(Collision2D hit) {
        print(hit);
        Destroy(gameObject);
    }

    void FixedUpdate() {
        rb.velocity = transform.right * bulletSpeed * Time.fixedDeltaTime * 100f;
        if (Time.time - createTime >= lifeTime){
            removeBullet(null);
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        removeBullet(other);
    }
}
