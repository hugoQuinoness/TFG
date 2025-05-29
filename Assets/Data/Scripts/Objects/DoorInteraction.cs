using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorInteraction : MonoBehaviour
{
    public bool isPlayerInRange = false;

    public string sceneToLoad;

    public int requiredKeyId;

    public GameObject DoorVisual;

    private Animator animator;

    public bool interacted = false;


    private void Awake()
    {
        animator = DoorVisual.GetComponent<Animator>();
    }


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
        if (!isPlayerInRange)
        {
            return;
        }

        if (interacted)
        {
            return;
        }

        bool hasKey = Player.Instance.uniqueItems.Exists(item => item.id == requiredKeyId);

        if (hasKey)
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            StartCoroutine(LockedDoorCoroutine());
        }
    }

    private IEnumerator LockedDoorCoroutine()
    {
        interacted = true;
        animator.SetTrigger("LockedDoor");
        yield return new WaitForSeconds(1f);
        interacted = false;
    }
}
