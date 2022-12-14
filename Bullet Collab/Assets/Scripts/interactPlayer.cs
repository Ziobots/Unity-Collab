/*******************************************************************************
* Name : interactPlayer.cs
* Section Description : interaction system that finds nearby objects
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 10/31/22  0.10                 DS              made the thing
* 11/02/22  0.20                 DS              added popup functionality
* 11/03/22  0.30                 DS              hide when player dies
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
        bool canSearch = true;
        if (Time.timeScale <= 0){
            return;
        }

        if (gameObject & gameObject.GetComponent<Entity>()){
            if (gameObject.GetComponent<Entity>().currentHealth <= 0){
                canSearch = false;
            }
        }

        Collider2D interactObj = findObject();
        if (interactObj != null && interactObj.gameObject != null && canSearch){
            // apply highlight effect to obj
            if (currentObj != interactObj){
                applyNearbyVFX(interactObj);
                currentObj = interactObj;
            }

            // check for player input
            if (interactPressed()){
                buttonInteract();
            }
        }else if (currentObj || popupUI.GetComponent<infoPopup>().popupVisible){
            applyNearbyVFX(null);
            currentObj = null;
        }
    }

    public void buttonInteract(){
        if (currentObj != null){
            // check if obj has pickup
            perkPickup pickupdData = currentObj.gameObject.GetComponent<perkPickup>();
            if (pickupdData && pickupdData.interactActive){
                pickupdData.onPickup(gameObject);
                currentObj = null;
                applyNearbyVFX(null);
            }else{
                print("add pickup functionality to obj");
            }
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
        if (newObj && newObj.gameObject != null){
            showPopup = true;
            if (newObj.gameObject.GetComponent<perkPickup>()){
                newObj.gameObject.GetComponent<perkPickup>().playerNearby = true;
            }
        }

        if (popupUI != null){
            infoPopup popupData = popupUI.GetComponent<infoPopup>();
            if (showPopup){
                Dictionary<string, string> showData = new Dictionary<string, string>();
                perkPickup pickupData = newObj.gameObject.GetComponent<perkPickup>();

                if (pickupData){ // check if the object is a perk
                    perkModule perkMod = newObj.gameObject.GetComponent<perkModule>();
                    perkData perk = perkMod.getPerk(pickupData.perkID);
                    // get the perk data
                    if (perk != null){
                        showData.Add("Title", perk.perkName);
                        showData.Add("Description", perk.perkDescription);
                        showData.Add("Cost", "" + pickupData.cost);
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
                perkPickup pickupData = interactObj.gameObject.GetComponent<perkPickup>();
                bool isActive = true;

                if (pickupData){
                    isActive = pickupData.interactActive;
                }

                if (isActive){
                    closestObj = interactObj;
                }
            }
        }

        // to prevent spam from objects getting in range and out of range too quickly
        if (currentObj != null && currentObj.gameObject != null){
            // check current active
            perkPickup pickupData = currentObj.gameObject.GetComponent<perkPickup>();
            bool currentActive = true;
            if (pickupData){
                currentActive = pickupData.interactActive;
            }
            
            if (currentActive){
                float myDistance = Vector2.Distance(currentObj.gameObject.transform.position,transform.position);
                if (closestObj != null && closestObj != currentObj){
                    float otherDistance = Vector2.Distance(closestObj.gameObject.transform.position,transform.position);
                    // if too close dont switch
                    if (Mathf.Abs(myDistance - otherDistance) <= 1f){
                        closestObj = currentObj;
                    }
                }else if (myDistance <= detectRadius + 2f){
                    closestObj = currentObj;
                }
            }
        }

        // return the closest interactable object
        return closestObj;
    }

    private bool interactPressed(){
        return Input.GetKeyDown(KeyCode.E);
    }
}
