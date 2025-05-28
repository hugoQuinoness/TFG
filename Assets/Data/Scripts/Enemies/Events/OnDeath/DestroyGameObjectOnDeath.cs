using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyGameObjectOnDeath : MonoBehaviour
{

    public GameObject gameObjectToDestroy;

    void OnDestroy()
    {
        Destroy(gameObjectToDestroy);    
    }
}
