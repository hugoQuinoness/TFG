using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class DialogueControls : MonoBehaviour
{

    private PlayerInput input;

    public static Action InvokeContinueDialogue;

    public void Awake()
    {
        input = EventSystem.current.GetComponent<PlayerInput>();

        input.actions["ContinueDialogue"].performed += InvokeContinueDialogueFunc;
    }

    public void OnDestroy()
    {
        if (input != null && input.actions != null)
        {
            input.actions["ContinueDialogue"].performed -= InvokeContinueDialogueFunc;
        }
    }


    private void InvokeContinueDialogueFunc(InputAction.CallbackContext context)
    {
        InvokeContinueDialogue?.Invoke();
    }
}
