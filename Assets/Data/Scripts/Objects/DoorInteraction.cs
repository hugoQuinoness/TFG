using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteraction : MonoBehaviour
{
    public bool isPlayerInRange = false;


    void Start()
    {
        EventManager.InteractEvent += OnInteract;
    }

    void OnDestroy()
    {
        EventManager.InteractEvent -= OnInteract;        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }

    void OnInteract()
    {
        if (!isPlayerInRange) return;

        bool hasKey = Player.Instance.uniqueItems.Exists(item => item.id == 1);

        if (hasKey)
        {
            Debug.Log("Door opened!");
        }
        else
        {
            Debug.Log("You need a key to open the door.");
        }
    }
}
