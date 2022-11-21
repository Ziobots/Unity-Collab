using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gunAnimatorCode : MonoBehaviour
{
    public GameObject playerObj;

    public void shootingFinished()
    {
        playerObj.GetComponent<Entity>().shootingGun = false;
    }

}
