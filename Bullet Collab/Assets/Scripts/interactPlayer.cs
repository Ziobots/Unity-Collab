/*******************************************************************************
* Name : interactPlayer.cs
* Section Description : interaction system that finds nearby objects
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 10/31/22  0.10                 DS              made the thing
* 11/02/22  0.10                 DS              added popup functionality
*******************************************************************************/

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

    // Popup UI
    public GameObject popupUI;

    // Update is called once per frame
    void Update() {
        if (Time.timeScale <= 0){
            return;
        }

        Collider2D interactObj = findObject();
        if (interactObj != null && interactObj.gameObject != null){
            // apply highlight effect to obj
            if (currentObj != interactObj){
                applyNearbyVFX(interactObj);
                currentObj = interactObj;
            }

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
            applyNearbyVFX(null);
            currentObj = null;
        }
    }

    private void applyNearbyVFX(Collider2D newObj){
        bool showPopup = false;

        // remove nearby from last interactable obj
        if (currentObj != null && newObj != currentObj){
            if (currentObj.gameObject.GetComponent<perkPickup>()){
                currentObj.gameObject.GetComponent<perkPickup>().playerNearby = false;
            }
        }

        // add nearby to interactable obj
        if (newObj != null){
            showPopup = true;
            if (newObj.gameObject.GetComponent<perkPickup>()){
                newObj.gameObject.GetComponent<perkPickup>().playerNearby = true;
            }
        }

        if (popupUI != null){
            infoPopup popupData = popupUI.GetComponent<infoPopup>();
            if (showPopup){
                Dictionary<string, string> showData = new Dictionary<string, string>();
                perkPickup isPerkObj = newObj.gameObject.GetComponent<perkPickup>();

                if (isPerkObj){ // check if the object is a perk
                    perkModule perkMod = newObj.gameObject.GetComponent<perkModule>();
                    perkData perk = perkMod.getPerk(isPerkObj.perkID);
                    // get the perk data
                    if (perk != null){
                        showData.Add("Title", perk.perkName);
                        showData.Add("Description", perk.perkDescription);
                        showData.Add("Cost", "" + isPerkObj.cost);
                        showData.Add("Context", "Pickup");
                    }
                }else{
                    print("some other type of interactable object");
                }

                // show the popup
                popupData.showPopup(showData);
            }else{
                // hide the popup
                popupData.hidePopup(false);
            }
        }
    }

    public Collider2D findObject(){
        Collider2D[] overlapResults = Physics2D.OverlapCircleAll(transform.position,detectRadius,interactLayer);
        Collider2D closestObj = null;

        foreach (Collider2D interactObj in overlapResults){
            float myDistance = Vector2.Distance(interactObj.gameObject.transform.position,transform.position);
            // compare distance between current obj and closest, if no closest then skip
            if (!closestObj || myDistance < Vector2.Distance(closestObj.gameObject.transform.position,transform.position)){
                closestObj = interactObj;
            }
        }

        // to prevent spam from objects getting in range and out of range too quickly
        if (closestObj == null && currentObj != null && currentObj.gameObject != null){
            float myDistance = Vector2.Distance(currentObj.gameObject.transform.position,transform.position);
            if (myDistance <= detectRadius + 2f){
                closestObj = currentObj;
            }
        }

        // return the closest interactable object
        return closestObj;
    }

    private bool interactPressed(){
        return Input.GetKeyDown(KeyCode.E);
    }
}
