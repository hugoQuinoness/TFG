using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;

    private CinemachineVirtualCamera virtualCamera;

    private Animator animator;

    public Sprite sprite;

    public EventManager eventManagerPrefab;

    public List<ObjectTemplate> uniqueItems;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();

        animator = GetComponent<Animator>();

        uniqueItems = new List<ObjectTemplate>();
    }

    private void Start()
    {
        if (EventManager.Instance == null)
        {
            Debug.Log("EventManager not found, instantiating a new one.");
            Instantiate(eventManagerPrefab);
        }
        
        foreach (ObjectTemplate item in uniqueItems)
        {
            Debug.Log($"Unique item: {item.name} with ID: {item.id}");
        }
    }

    public void ChangeCameraPriority()
    {
        if (CMCameraManager.Instance.currentCinemachineVCamera == null)
        {
            CMCameraManager.Instance.currentCinemachineVCamera = virtualCamera;
        }

        CMCameraManager.Instance.ChangeVcamPriority(virtualCamera);
    }

    public void PlayAnimation(string animation)
    {
        if (animation == "Attack")
        {
            animator.SetTrigger("Attack");
        }
        else
        {
            animator.Play(animation);
        }
    }

    public IEnumerator MoveTo(float x, float y, float duration)
    {
        Vector2 targetPosition = new Vector2(x, y);
        float durationF = duration;
        float elapsedTime = 0f;
        Vector2 startingPosition = transform.position;

        while (elapsedTime < duration)
        {
            transform.position = Vector2.Lerp(startingPosition, targetPosition, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
        DialogueManager.Instance.tagsToHandle--;
    }
    

}
