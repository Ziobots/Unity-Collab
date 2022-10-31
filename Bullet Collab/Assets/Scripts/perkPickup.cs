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

    public void onPickup(){
        if (perkCommands != null && dataInfo != null){
            // add perk to ID list
            dataInfo.perkIDList.Add(perkID);

            foreach (GameObject perkObj in perkObjList){
                // do destroy effect
                Destroy(perkObj);
            }
        }
    }
}
