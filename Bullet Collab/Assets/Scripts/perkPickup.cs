/*******************************************************************************
* Name : perkPickup.cs
* Section Description : 
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 10/31/22  0.10                 DS              Made the thing
* 11/02/22  0.20                 DS              adding perks
* 11/20/22  0.30                 DS              shop stuff
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

    // Game Data Stuff
    public GameObject gameManager;
    [HideInInspector] public gameLoader gameInfo;

    // UI Stuff
    public GameObject uiManager;
    [HideInInspector] public UIManager uiUpdate;

    // perk obj variables
    public string perkID;
    public int cost = 0;
    public int count = 1;

    public List<GameObject> perkObjList; // List of perk objs to be destroyed when this perk is picked up 
    public visualFx collectPrefab;
    public Transform addFolder;
    private Vector3 basePosition;
    public TMPro.TextMeshProUGUI costField;

    public bool interactActive = false;
    [HideInInspector] public System.Action perkGet = null;

    // Interaction
    public bool playerNearby = false;

    // Sound Stuff
    public AudioSource collectNoise;
    public AudioSource errorNoise;

    private void collectEffect(){
        visualFx collectVFX = Instantiate(collectPrefab,new Vector3(transform.position.x,transform.position.y,-0.1f),new Quaternion(),addFolder);
        if (collectVFX != null){
            if (collectNoise != null){
                collectNoise.PlayOneShot(collectNoise.clip,collectNoise.volume);
            }

            collectVFX.lifeTime = 0f;
            collectVFX.killAnimation = true;
            collectVFX.animSpeed = 2.5f;

            float hitScale = transform.localScale.x;
            collectVFX.transform.localScale = new Vector3(hitScale,hitScale,1);
            collectVFX.setupVFX();
        }
    }

    // tween functions
    private void spawnRotation(float value){
        Quaternion setRotationEuler = Quaternion.Euler(0f, value, 0f);
        transform.rotation = setRotationEuler;
    }

    private void spawnFinished(){
        spawnRotation(0f);
        interactActive = true;
    }

    private void spawnAnimation(){
        spawnRotation(90);
        LeanTween.value(gameObject,270f,720f,.7f).setEaseOutBack().setOnUpdate(spawnRotation).setOnComplete(spawnFinished);
    }

    private void deletePerk(){
        spawnRotation(90);
        Destroy(gameObject);
    }

    public void removePerk(){
        interactActive = false;
        spawnRotation(0);
        LeanTween.value(gameObject,0f,90f,.3f).setEaseOutBack().setOnUpdate(spawnRotation).setOnComplete(deletePerk);
    }

    public void setupPickup(){
        // Get data management script 
        if (dataManager != null){
            dataInfo = dataManager.GetComponent<sharedData>();
            perkCommands = dataManager.GetComponent<perkModule>();
        }

        if (gameManager != null){
            gameInfo = dataManager.GetComponent<gameLoader>();
        }

        // Get UI management script
        if (uiManager != null){
            uiUpdate = uiManager.GetComponent<UIManager>();
        }

        if (perkCommands != null){
            perkData perk = perkCommands.getPerk(perkID);
            if (perk != null){
                gameObject.GetComponent<SpriteRenderer>().sprite = perk.perkIcon;
            }
        }

        basePosition = transform.position;
        costField.text = cost > 0 ? "<sprite index=0>" + cost : "";
        
        // do spawn animation
        spawnAnimation();
    }

    public void onPickup(GameObject entityObj){
        if (perkCommands != null && dataInfo != null){
            Entity entityInfo = entityObj.GetComponent<Entity>();
            if (entityInfo && entityInfo.currency >= cost){
                entityInfo.currency -= cost;
                // Disabled Collider
                gameObject.GetComponent<Collider2D>().enabled = false;

                // add perk to ID list
                perkData perk = perkCommands.getPerk(perkID);
                if (perk != null){
                    for (int i = 1; i <= count; i++){
                        // add the perk to the data
                        print("added perk " + perkID);
                        entityObj.GetComponent<Entity>().perkIDList.Add(perkID);

                        // create the dictionary for on add
                        Dictionary<string, GameObject> addList = new Dictionary<string, GameObject>();
                        addList.Add("Owner", entityObj);
                        addList.Add("PerkObj", gameObject);

                        // This event should only run here and data load, 3 parameter should always be true here?
                        perk.addedEvent(addList,perkCommands.countPerks(entityObj.GetComponent<Entity>().perkIDList)[perkID],true);

                        // This is for when the perk has a buy event
                        if (cost > 0){
                            perk.buyEvent(addList,perkCommands.countPerks(entityObj.GetComponent<Entity>().perkIDList)[perkID],true);
                        }

                        // fix any stats that are really bad
                        gameObject.GetComponent<perkModule>().fixEntity(entityObj.GetComponent<Entity>());

                        // apply any changes to the data
                        dataInfo.updateEntityData(entityObj);
                    }
                }

                // do collect effect
                collectEffect();

                Dictionary<string, GameObject> editList = new Dictionary<string, GameObject>();
                if (perkCommands != null){
                    editList.Add("Owner", entityObj);
                    editList.Add("PerkObj", gameObject);
                    editList.Add("GameManager",gameManager);
                    editList.Add("DataManager",dataManager);
                    perkCommands.applyPerk(dataInfo.perkIDList,"Perk_Collect",editList);

                    // apply any changes to the data
                    dataInfo.updateEntityData(entityObj);
                }

                // if perk was connected to other perks remove those since this was chosen
                if (!editList.ContainsKey("SKIP_DESTROY")){
                    foreach (GameObject perkObj in perkObjList){
                        if (perkObj != gameObject && perkObj != null){
                            // do destroy effect
                            perkObj.GetComponent<perkPickup>().removePerk();
                        }
                    }
                }

                if (perkGet != null){
                    perkGet();
                }

                if (uiUpdate){
                    uiUpdate.updateGameUI();
                }

                // remove this perk obj
                if (gameObject != null){
                    Destroy(gameObject);
                }

            }else{
                if (errorNoise != null){
                    errorNoise.PlayOneShot(errorNoise.clip,errorNoise.volume);
                }
            }
        }
    }

    private void Start() {
        setupPickup();
    }

    private void FixedUpdate() {
        if (gameObject && interactActive){
            if (Time.timeScale <= 0){
                return;
            }

            Vector3 setPosition = basePosition;
            float rotation = 0;
            
            // check if player nearby
            if (playerNearby){
                float offset = Mathf.Sin(Time.time * 5f) * 0.25f;
                if (offset < 0f){
                    offset *= 0.35f;
                }

                setPosition = basePosition + new Vector3(0f, offset, 0f);
                rotation = Mathf.Sin(Time.time * 2.5f) * 15f;
            }

            // set sticker hover position and rotation
            Quaternion setRotationEuler = Quaternion.Euler(0, 0, rotation);
            float alpha = Time.fixedDeltaTime * 10f;
            transform.rotation = Quaternion.Lerp(transform.rotation, setRotationEuler, alpha);
            transform.position = Vector3.Lerp(transform.position,setPosition,alpha);
        }
    }
}
