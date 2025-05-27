using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetectionCollider : MonoBehaviour
{
    
    public IEnemy enemy;

    public GameObject enemyObject;

    private void Awake()
    {
        enemy = enemyObject.GetComponent<IEnemy>();
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            enemy.OnPlayerDetected();
        }
    }
}
