using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TriggerDialogue : MonoBehaviour
{
    public TextAsset dialogue;
    private bool isPlayerInRange;
    private PlayerInput playerInput;

    void Start()
    {   
        EventManager.InteractEvent += OnInteract;
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
        if (isPlayerInRange)
        {
            DialogueManager.Instance.EnterDialogueModeWithoutCollision(dialogue);
        }
    }
}
