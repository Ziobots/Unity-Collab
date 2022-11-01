using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class interactPlayer : MonoBehaviour
{
    // Interaction Variables
    private const float detectRadius = 1f;
    public LayerMask interactLayer;
    [HideInInspector] public Collider2D currentObj = null;

    // Update is called once per frame
    void Update() {
        Collider2D interactObj = findObject();
        if (interactObj != null && interactObj.gameObject != null){
            // apply highlight effect to obj
            applyNearbyVFX(interactObj);
            currentObj = interactObj;

            // check for player input
            if (interactPressed()){
                print("Interact with Obj");

                // check if obj has pickup
                if (interactObj.gameObject.GetComponent<perkPickup>()){
                    print("pickup perk");
                    interactObj.gameObject.GetComponent<perkPickup>().onPickup();
                }else{
                    print("add pickup functionality to obj");
                }
            }
        }
    }

    private void applyNearbyVFX(Collider2D newObj){
        if (currentObj != null){
            // remove current Effect
        }

        // apply to newObj
    }

    public Collider2D findObject(){
        Collider2D interactObj = Physics2D.OverlapCircle(transform.position,detectRadius,interactLayer);
        return interactObj;
    }

    private bool interactPressed(){
        return Input.GetKeyDown(KeyCode.E);
    }
}
