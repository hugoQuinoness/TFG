using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public static Boss Instance;

    public Cinemachine.CinemachineVirtualCamera virtualCamera;

    private Animator animator;

    private WizardMovement wizardMovement;

    public Sprite sprite;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        animator = GetComponent<Animator>();

        virtualCamera = GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>();

        wizardMovement = GetComponent<WizardMovement>();

        DialogueManager.InvokeOnDialogueEnter += OnEnterDialogueMode;

        DialogueManager.InvokeOnDialogueExit += OnExitDialogueMode;

    }

    public void OnDestroy()
    {
        DialogueManager.InvokeOnDialogueEnter -= OnEnterDialogueMode;

        DialogueManager.InvokeOnDialogueExit -= OnExitDialogueMode;
    }

    public void OnEnterDialogueMode()
    {
        wizardMovement.enabled = false;
    }

    public void OnExitDialogueMode()
    {
        wizardMovement.enabled = true;
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
        animator.Play(animation);
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
