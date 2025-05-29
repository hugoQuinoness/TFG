using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObjectOnDeath : MonoBehaviour
{
    public GameObject objectToSpawn;

    public Transform spawnPoint;


    [Header("Chest")]
    public bool isObjectAChest;

    public int objectId;
    public string objectName;

    void OnDestroy()
    {

        if(isObjectAChest)
        {
            objectToSpawn.GetComponent<GiveObject>().id = objectId;
            objectToSpawn.GetComponent<GiveObject>().objectName = objectName;
            Instantiate(objectToSpawn, spawnPoint.position, spawnPoint.rotation);
        }
        
    }
}
