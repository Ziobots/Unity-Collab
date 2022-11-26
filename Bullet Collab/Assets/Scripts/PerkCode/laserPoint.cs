/*******************************************************************************
* Name : laserPoint.cs
* Section Description : laserPoint
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 11/23/22  0.10                 DS              Made the thing
*******************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Perk/laserPoint")]
public class laserPoint : perkData
{
    public float spreadMultiple = 0.5f;
    public float speedMultiple = 1.5f;
    private Material defaultMaterial = null;

    public override void addedEvent(Dictionary<string, GameObject> objDictionary, int Count, bool initialize){
        Entity entityInfo = getEntityStats(objDictionary);
        if (entityInfo){
            entityInfo.bulletSpread *= spreadMultiple;
        }
    }

    public override void shootEvent(Dictionary<string, GameObject> objDictionary,int Count,bool initialize) {
        bulletSystem bulletStats = getBulletStats(objDictionary);

        if (bulletStats){
            // Add the stats
            bulletStats.bulletSpeed *= speedMultiple;
        }
    }

    public override void updateEntity(Dictionary<string, GameObject> objDictionary, int Count, bool initialize){
        if (Time.timeScale <= 0){
            return;
        }

        Entity entityInfo = getEntityStats(objDictionary);
        if (entityInfo != null && initialize){
            if (entityInfo.launchPoints != null){
                foreach (Transform launchPoint in entityInfo.launchPoints){
                    if (launchPoint.gameObject){
                        LineRenderer laser = launchPoint.gameObject.GetComponent<LineRenderer>();
                        if (!laser){
                            if (defaultMaterial == null){
                                defaultMaterial = Resources.Load<Material>("Materials/default");
                            }

                            laser = launchPoint.gameObject.AddComponent<LineRenderer>();
                            laser.material = defaultMaterial;
                            laser.startWidth = 0.1f;
                            laser.endWidth = 0.1f;
                        }

                        laser.enabled = entityInfo.currentHealth > 0;

                        float alpha = entityInfo.reloadingGun ? 0f : 220f;
                        float lerpAlpha = Mathf.Lerp(laser.startColor.a * 255,alpha,15f * Time.deltaTime);
                        laser.startColor = new Color32(253,106,106,(byte)lerpAlpha);
                        laser.endColor = new Color32(253,106,106,(byte)lerpAlpha);

                        if (laser && laser.enabled){
                            Vector3 startPosition = launchPoint.position;
                            Vector2 startDirection = launchPoint.right.normalized;
                            Vector3 zOffset = new Vector3(0,0,-5f);
                            List<Vector3> positionList = new List<Vector3>();
                            positionList.Add(startPosition + zOffset);

                            for (int i = 0; i < Count; i++){
                                RaycastHit2D contact = Physics2D.Raycast(startPosition,startDirection,100f,LayerMask.GetMask("Obstacle","EntityCollide"),0f);
                                if (contact.collider){
                                    startPosition = startPosition + (Vector3)(startDirection * contact.distance);
                                    startDirection = Vector2.Reflect(startDirection.normalized,contact.normal.normalized);
                                    positionList.Add(startPosition + zOffset);
                                    if (contact.collider.gameObject){
                                        Entity hitInfo = contact.collider.gameObject.GetComponent<Entity>();
                                        if (hitInfo && !hitInfo.deflectBullets){
                                            break;
                                        }
                                    }
                                }else{
                                    positionList.Add(startPosition + ((Vector3)startDirection * 100f) + zOffset);
                                    break;
                                }
                            }

                            laser.positionCount = positionList.Count;
                            laser.SetPositions(positionList.ToArray());
                        }
                    }
                }
            }
        }
    }
}
