/*******************************************************************************
* Name : CameraBehavior.cs
* Section Description : This code handles the camera.
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 10/26/22  0.10                 DS              Made the thing
* 10/26/22  0.20                 DS              added instant step
* 10/26/22  0.30                 DS              added death zoom
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    // Camera Variables
    public GameObject followObject;
    public bool factorMouse = true;
    public float cameraZoom = 5;
    public float extraZoom = 0;
    public Vector2 cameraPosition = new Vector2(0,0);
    public GameObject cursorObj;
    
    // zoom variables
    public float zoomSpeed = 1f;
    public bool instantJump = false;

    // Mouse Variables
    [HideInInspector] public Vector2 mousePosition;
    private float mouseDistance = 0;
    public Vector2 mouseDirection;

    // mobile var
    public Joystick aimStick;

    private void Start() {
        DontDestroyOnLoad(gameObject);
        Cursor.visible = false;
    }

    // Camera Default Values
    public void resetCamera() {
        cameraZoom = 5;
        extraZoom = 0;
    }

    // Late Update is called after the normal Update - important because of input stuff
    private void FixedUpdate() {
        // Move the reticle
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (cursorObj != null){
            mousePosition = Camera.main.ScreenToWorldPoint(cursorObj.GetComponent<RectTransform>().position);
        }

        // Check if the Camera is following an object
        if (followObject != null){
            Vector2 followPosition = followObject.transform.position;
            if (aimStick.Direction.magnitude > 0){
                mousePosition = (Vector2)followPosition + aimStick.Direction * 10f;
            }

            // Check if Camera should factor the Mouse Position when finding the new Position
            if (factorMouse){

                mouseDistance = Vector2.Distance(followPosition,mousePosition);
                mouseDistance = Mathf.Clamp(mouseDistance,0f,13f);
                extraZoom = mouseDistance / 20f;

                mouseDirection = (mousePosition - (Vector2)followPosition).normalized;

                cameraPosition = followPosition + (mouseDirection * (mouseDistance/3));
            }else{// if no mouse factor then just focus on the object
                cameraPosition = followPosition;
            }
        }

        // Calculate the Zoom Level, Lerp for smooth transition
        float alpha = instantJump ? 1f : Time.fixedDeltaTime * zoomSpeed;
        Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize,cameraZoom + extraZoom,alpha);

        // Calculate the New Position, Lerp for smooth transition
        Vector3 setPosition = new Vector3(cameraPosition.x,cameraPosition.y,-10);
        alpha = instantJump ? 1f : Time.fixedDeltaTime * 5f;
        transform.position = Vector3.Lerp(transform.position,setPosition,alpha);

        if (instantJump){
            instantJump = false;
        }
    }
}
