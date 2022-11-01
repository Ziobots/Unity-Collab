using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class interactPlayer : MonoBehaviour
{
    // Interaction Variables 
    private const float detectRadius = 2f;
    public LayerMask interactLayer;
    [HideInInspector] public Collider2D currentObj = null;

    // Cursor Variables
    public GameObject cursorObj;

    // Update is called once per frame
    void Update() {
        if (Time.timeScale <= 0){
            return;
        }

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
                    interactObj.gameObject.GetComponent<perkPickup>().onPickup();
                    currentObj = null;
                    applyNearbyVFX(null);
                }else{
                    print("add pickup functionality to obj");
                }
            }
        }else if (currentObj){
            print("no nearby obj");
            applyNearbyVFX(null);
            currentObj = null;
        }
    }

    private void applyNearbyVFX(Collider2D newObj){
        bool moveCursor = false;
        if (currentObj != null && newObj != currentObj){
            moveCursor = false;
        }

        if (newObj != null){
            moveCursor = true;
        }

        if (cursorObj != null){
            mouseCursor cursorData = cursorObj.GetComponent<mouseCursor>();
            if (moveCursor){
                cursorData.movementPosition = (Vector2)newObj.transform.position + new Vector2(0f,-1f + (Mathf.Sin(Time.time * 10) * 0.25f));
                cursorData.smoothMovement = true;
                cursorData.cursorRotation = 0;
                cursorData.smoothMovement = true;
                cursorData.overwriteMovement = true;
                cursorData.updateHover(true);
            }else{
                cursorData.smoothMovement = true;
                cursorData.overwriteMovement = false;
                cursorData.updateHover(false);
            }
        }
    }

    public Collider2D findObject(){
        Collider2D interactObj = Physics2D.OverlapCircle(transform.position,detectRadius,interactLayer);
        return interactObj;
    }

    private bool interactPressed(){
        return Input.GetButtonDown("Fire1");
    }
}
