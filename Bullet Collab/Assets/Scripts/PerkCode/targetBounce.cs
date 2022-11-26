/*******************************************************************************
* Name : targetBounce.cs
* Section Description : perk
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 11/24/22  0.10                 DS              Made the thing
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Perk/targetBounce")]
public class targetBounce : perkData
{
    public int addBounce = 1;
    public float addReload = 0.2f;
    public float damageMultiple = 0.85f;
    public float speedMultiple = 1.2f;
    public float bounceRange = 15f;

    public override void addedEvent(Dictionary<string, GameObject> objDictionary,int Count,bool initialize) {
        Entity entityStats = getEntityStats(objDictionary);

        if (entityStats){
            // Add the player stats
            entityStats.reloadTime += addReload;
        }
    }

    public override void shootEvent(Dictionary<string, GameObject> objDictionary,int Count,bool initialize) {
        if (objDictionary.ContainsKey("Bullet")){
            // Add the Bounces
            bulletSystem bulletInfo = getBulletStats(objDictionary);
            if (bulletInfo){
                bulletInfo.bulletBounces += addBounce;
                bulletInfo.bulletDamage *= damageMultiple;
            }
        }
    }

    public override void bounceEvent(Dictionary<string, GameObject> objDictionary,int Count,bool initialize) {
        bulletSystem bulletInfo = getBulletStats(objDictionary);
        if (bulletInfo != null && initialize){
            Entity[] targetChoices = FindObjectsOfType<Entity>();
            GameObject bulletObj = objDictionary["Bullet"];
            if (bulletObj == null){
                return;
            }

            RaycastHit2D closestHit = new RaycastHit2D();
            List<RaycastHit2D> contactList = new List<RaycastHit2D>();

            foreach (Entity targetInfo in targetChoices){
                GameObject target = targetInfo.gameObject;
                if (target != null && targetInfo.currentHealth > 0){
                    if (target.GetComponent<Player>() || target.GetComponent<Enemy>()){
                        Vector2 direction = ((Vector2)target.transform.position - (Vector2)bulletObj.transform.position).normalized;
                        Vector2 origin = (Vector2)bulletObj.transform.position - direction;
                        float distance = bounceRange * Count;
                        if (target == bulletInfo.bulletOwner){
                            distance -= 5f;
                        }

                        RaycastHit2D[] contacts = Physics2D.RaycastAll(origin,direction,distance,LayerMask.GetMask("EntityCollide","Obstacle"));
                        
                        foreach(RaycastHit2D contact in contacts){
                            if (contact.collider.gameObject == target){
                                contactList.Add(contact);
                            }
                        }
                    }
                }
            }

            foreach(RaycastHit2D contact in contactList){
                if (!closestHit.collider || Vector3.Distance(contact.point,bulletObj.transform.position) < Vector3.Distance(closestHit.point,bulletObj.transform.position)){
                    if (contact.collider.gameObject != null){
                        closestHit = contact;
                    }
                }
            }

            if (closestHit && closestHit.collider && closestHit.collider.gameObject != null){
                Vector2 direction = ((Vector2)closestHit.collider.gameObject.transform.position - (Vector2)bulletObj.transform.position).normalized;
                bulletObj.transform.right = direction.normalized;
                bulletInfo.bulletSpeed *= speedMultiple;
            }
        }
    }
}
