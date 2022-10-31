/*******************************************************************************
* Name : visualFx.cs
* Section Description : This handles any vfx.
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 10/30/22  0.10                 DS              Made the thing
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class visualFx : MonoBehaviour
{
    // Animation Variables
    private Animator anim;
    public bool killAnimation = false;
    public float animSpeed = 1f;

    // Vfx Variables
    [HideInInspector] public float createTime = -1;
    public float lifeTime;
    private bool didSetup = false;

    // Start is called before the first frame update
    public void setupVFX(){
        createTime = Time.time;

        // Animation components
        anim = GetComponent<Animator>();
        if (anim != null){
            anim.speed = animSpeed;
        }

        didSetup = true;
    }

    // Update is called once per frame
    void FixedUpdate(){
        if (didSetup && gameObject != null){
            if (Time.time - createTime >= lifeTime && lifeTime > 0){
                Destroy(gameObject);
            }
        }
    }

    public void endAnimationFrame(){
        if (killAnimation){
            Destroy(gameObject);
        }
    }
}
