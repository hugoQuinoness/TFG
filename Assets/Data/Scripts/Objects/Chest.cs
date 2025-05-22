using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    private Animator animator;

    private bool isOpen = false;

    private bool isPlayerInRange;

    public GiveObject giveObject;

    void Start()
    {
        animator = GetComponent<Animator>();
        EventManager.InteractEvent += OnInteract;
        giveObject = GetComponent<GiveObject>();
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
        if (isOpen)
        {
            return;
        }

        if (giveObject != null)
        {
            giveObject.GiveObjectToPlayer();
        }

        if (isPlayerInRange)
        {
            animator.Play("Open");
            isOpen = true;
        }
    }
}
