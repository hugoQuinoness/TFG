using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;


    public PlayerInput playerInput;

    public static event Action ContinueDialogueEvent;
    public static event Action MoveEvent;

    public static event Action AttackEvent;

    public static event Action RunEvent;

    public static event Action RunCanceledEvent;

    public static event Action PauseEvent;

    public static event Action InteractEvent;

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

        playerInput = EventSystem.current.GetComponent<PlayerInput>();

        playerInput.actions["Move"].canceled += InvokeMoveEvent;
        playerInput.actions["Attack"].performed += InvokeAttackEvent;
        playerInput.actions["Run"].performed += InvokeRunEvent;
        playerInput.actions["Run"].canceled += InvokeRunCanceledEvent;
        playerInput.actions["ContinueDialogue"].performed += InvokeContinueDialogueEvent;
        playerInput.actions["Pause"].performed += InvokePauseEvent;
        playerInput.actions["Interact"].performed += InvokeInteractEvent;
    }

    private void OnDestroy()
    {
        playerInput.actions["Move"].canceled -= InvokeMoveEvent;
        playerInput.actions["Attack"].performed -= InvokeAttackEvent;
        playerInput.actions["Run"].performed -= InvokeRunEvent;
        playerInput.actions["Run"].canceled -= InvokeRunCanceledEvent;
        playerInput.actions["ContinueDialogue"].performed -= InvokeContinueDialogueEvent;
        playerInput.actions["Pause"].performed -= InvokePauseEvent;
        playerInput.actions["Interact"].performed -= InvokeInteractEvent;
    }

    private void InvokeContinueDialogueEvent(InputAction.CallbackContext context)
    {
        ContinueDialogueEvent?.Invoke();
    }

    private void InvokeMoveEvent(InputAction.CallbackContext context)
    {
        MoveEvent?.Invoke();
    }

    private void InvokeAttackEvent(InputAction.CallbackContext context)
    {
        AttackEvent?.Invoke();
    }

    private void InvokeRunEvent(InputAction.CallbackContext context)
    {
        RunEvent?.Invoke();
    }

    private void InvokeRunCanceledEvent(InputAction.CallbackContext context)
    {
        RunCanceledEvent?.Invoke();
    }

    private void InvokePauseEvent(InputAction.CallbackContext context)
    {
        PauseEvent?.Invoke();
    }

    private void InvokeInteractEvent(InputAction.CallbackContext context)
    {
        InteractEvent?.Invoke();
    }


}
