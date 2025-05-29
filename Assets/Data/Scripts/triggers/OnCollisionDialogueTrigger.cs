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
            StartCoroutine(StartDialogueAndDestroy());
        }
    }


    private IEnumerator StartDialogueAndDestroy()
    {
        yield return DialogueManager.Instance.StartDialogueByAdress(dialogueAddress);

        if (destroyOnTrigger)
        {
            Destroy(gameObject);
        }
    }

}
