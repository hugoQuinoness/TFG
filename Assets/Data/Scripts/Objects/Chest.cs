using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    private Animator animator;

    private bool isOpen = false;

    public bool isPlayerInRange;

    public GiveObject giveObject;

    public AudioClip openSound;

    private AudioSource audioSource;

    void Start()
    {
        animator = GetComponent<Animator>();
        EventManager.InteractEvent += OnInteract;
        giveObject = GetComponent<GiveObject>();
        audioSource = GetComponent<AudioSource>();
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

        if (isPlayerInRange)
        {
            animator.Play("Open");
            isOpen = true;
            audioSource.PlayOneShot(openSound);
        }
        else
        {
            return;
        }

        if (giveObject != null)
        {
            giveObject.GiveObjectToPlayer();
        }
    }
}
