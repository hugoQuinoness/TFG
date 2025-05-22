using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeakIcon : MonoBehaviour
{
    SpriteRenderer spriteRenderer;

    public float floatSpeed = 1f;
    public float floatAmplitude = 0.1f;
    private Vector3 initialPosition;

    public void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        initialPosition = transform.position;
        DialogueManager.InvokeOnDialogueEnter += OnEnterDialogueMode;
        DialogueManager.InvokeOnDialogueExit += OnExitDialogueMode;
    }

    private void Update()
    {
        // Floating animation
        float newX = initialPosition.x + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        float newY = initialPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.position = new Vector3(newX, newY, initialPosition.z);
    }

    public void OnDestroy()
    {
        DialogueManager.InvokeOnDialogueEnter -= OnEnterDialogueMode;
        DialogueManager.InvokeOnDialogueExit -= OnExitDialogueMode;
    }

    void OnEnterDialogueMode()
    {
        spriteRenderer.enabled = false;
    }

    void OnExitDialogueMode()
    {
        spriteRenderer.enabled = true;
    }


}
