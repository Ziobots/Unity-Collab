using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private float horizontal;
    private float veritcal;

    public float speed = 8f;
    public Rigid

    private bool facingRight = true;
    private bool flipDebounce = true;

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
    }
}
