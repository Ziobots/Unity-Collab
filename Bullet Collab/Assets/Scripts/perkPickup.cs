/*******************************************************************************
* Name : perkPickup.cs
* Section Description : 
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 10/23/22  0.10                 DS              Made the thing
* 10/31/22  0.10                 DS              made perks you can pickup
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class perkPickup : MonoBehaviour
{
    // Base Data Stuff
    public GameObject dataManager;
    [HideInInspector] public sharedData dataInfo;
    [HideInInspector] public perkModule perkCommands;

    // perk obj variables
    public string perkID;
    public int cost = 0;
    public List<GameObject> perkObjList; // List of perk objs to be destroyed when this perk is picked up
    public visualFx collectPrefab;
    public Transform addFolder;

    private void collectEffect(){
        visualFx collectVFX = Instantiate(collectPrefab,new Vector3(transform.position.x,transform.position.y,-0.1f),transform.rotation,addFolder);
        if (collectVFX != null){
            collectVFX.lifeTime = 0f;
            collectVFX.killAnimation = true;
            collectVFX.animSpeed = 2.5f;

            float hitScale = transform.localScale.x;
            collectVFX.transform.localScale = new Vector3(hitScale,hitScale,1);
            collectVFX.setupVFX();
        }
    }

    public void setupPickup(){
        // Get data management script
        if (dataManager != null){
            dataInfo = dataManager.GetComponent<sharedData>();
            perkCommands = dataManager.GetComponent<perkModule>();
        }

        if (perkCommands != null){
            perkData perk = perkCommands.getPerk(perkID);
            if (perk != null){
                gameObject.GetComponent<SpriteRenderer>().sprite = perk.perkIcon;
            }
        }
    }

    private void Start() {
        setupPickup();
    }

    public void onPickup(){
        // probably remove this later
        setupPickup();

        if (perkCommands != null && dataInfo != null){
            // Disabled Collider
            gameObject.GetComponent<Collider2D>().enabled = false;

            // add perk to ID list
            dataInfo.perkIDList.Add(perkID);

            // do collect effect
            collectEffect();

            // if perk was connected to other perks remove those since this was chosen
            foreach (GameObject perkObj in perkObjList){
                // do destroy effect
                Destroy(perkObj);
            }

            // remove this perk obj
            if (gameObject != null){
                Destroy(gameObject);
            }
        }
    }
}
