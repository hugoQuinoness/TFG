using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Starter : MonoBehaviour
{

    public bool hasDialogueOnEnter;

    public TextAsset dialogue;

    void Start()
    {
        if (hasDialogueOnEnter)
        {
            DialogueManager.Instance.EnterDialogueModeWithoutCollision(dialogue);
        }
    }
}
