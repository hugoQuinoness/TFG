using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;

    private DialogueControls dialogueControls;

    private PlayerControler playerControler;

    private CinemachineVirtualCamera virtualCamera;

    private Animator animator;


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

        dialogueControls = GetComponent<DialogueControls>();

        playerControler = GetComponent<PlayerControler>();

        virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();

        animator = GetComponent<Animator>();

        DialogueManager.InvokeOnDialogueEnter += OnEnterDialogueMode;

        DialogueManager.InvokeOnDialogueExit += OnExitDialogueMode;
    }

    private void OnDestroy()
    {
        
        DialogueManager.InvokeOnDialogueEnter -= OnEnterDialogueMode;

        DialogueManager.InvokeOnDialogueExit -= OnExitDialogueMode;
    }


    private void OnEnterDialogueMode()
    {
        Debug.Log("Entering dialogue mode");
        dialogueControls.enabled = true;
        playerControler.enabled = false;
    }

    private void OnExitDialogueMode()
    {
        Debug.Log("Exiting dialogue mode");
        dialogueControls.enabled = false;
        playerControler.enabled = true;
    }

    public void ChangeCameraPriority()
    {
        if(CMCameraManager.Instance.currentCinemachineVCamera == null)
        {
            CMCameraManager.Instance.currentCinemachineVCamera = virtualCamera;
        }

        CMCameraManager.Instance.ChangeVcamPriority(virtualCamera);
    }

    public void PlayAnimation(string animation)
    {
        animator.Play(animation);
    }
}
