using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyHitbox : MonoBehaviour
{

    public int enemyDamage;
    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject.name);
        Debug.Log(collision.gameObject.tag);
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                Debug.Log("Player hit by enemy!");
                playerHealth.TakeDamage(enemyDamage);
            }
        }
    }
}
