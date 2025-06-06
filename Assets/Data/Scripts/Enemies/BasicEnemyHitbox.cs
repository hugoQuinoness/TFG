using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyHitbox : MonoBehaviour
{

    public int enemyDamage;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(enemyDamage);
            }
        }
    }
}
