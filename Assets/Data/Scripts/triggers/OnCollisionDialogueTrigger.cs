using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnCollisionDialogueTrigger : MonoBehaviour
{
    public string dialogueAddress;

    public bool destroyOnTrigger;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(DialogueManager.Instance.StartDialogueByAdress(dialogueAddress));
        }
    }


}
