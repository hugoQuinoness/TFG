using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Ink.Runtime;
using System;

public class DialogueManager : MonoBehaviour
{
    public GameObject dialogueBox;

    public TextMeshProUGUI dialogueText;

    public TextMeshProUGUI nameText;

    public Image portraitImage;

    public static DialogueManager Instance;

    public TextAsset currentDialogue;

    public Story currentStory;

    public bool isDialogueActive;

    private string speakingCharacter;

    private string updatedText;

    public static Action InvokeOnDialogueEnter;

    public static Action InvokeOnDialogueExit;


    private Dictionary<string, Action<string>> tagHandlers;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        DialogueControls.InvokeContinueDialogue += ContinueDialogue;

        tagHandlers = new Dictionary<string, Action<string>>
        {
            { "animation", HandleAnimationTag },
        };
    }

    private void Start()
    {
        EnterDialogueModeWithoutCollision(currentDialogue);
    }

    private void OnDestroy()
    {
        DialogueControls.InvokeContinueDialogue -= ContinueDialogue;
    }

    public void ContinueDialogue()
    {

        if (currentStory.canContinue)
        {
            HandleLine(currentStory.Continue());
            HandleTags(currentStory.currentTags);
        }
        else
        {
            ExitDialogueMode();
        }
    }

    public void EnterDialogueModeWithoutCollision(TextAsset textAsset)
    {
        StartUI();

        InvokeOnDialogueEnter?.Invoke();

        isDialogueActive = true;

        currentDialogue = textAsset;

        currentStory = new Story(currentDialogue.text);

        if (currentStory.canContinue)
        {
            string line = currentStory.Continue();
            HandleLine(line);
            HandleTags(currentStory.currentTags);
        }
    }

    public void ExitDialogueMode()
    {
        EndUI();

        isDialogueActive = false;

        currentDialogue = null;

        InvokeOnDialogueExit?.Invoke();
    }

    public void HandleLine(string line)
    {
        line = line.Trim();

        int colonIndex = line.IndexOf(":");

        if (colonIndex > 0 && colonIndex < line.Length - 1)
        {
            speakingCharacter = line.Substring(0, colonIndex).Trim();

            updatedText = line.Substring(colonIndex + 1).Trim();
        }
        else
        {
            speakingCharacter = "";
            updatedText = line;
        }

        CheckSpeakerName(speakingCharacter);
        dialogueText.text = updatedText;
    }

    public void HandleTags(List<string> currentTags)
    {
        foreach (string tag in currentTags)
        {
            string tagKey;
            string tagValue;

            string[] tagSplit = tag.Split(':');

            if(tagSplit.Length == 1)
            {
                tagKey = tagSplit[0].Trim();
                tagValue = "Default";
            }
            else
            {
                tagKey = tagSplit[0].Trim();
                tagValue = tagSplit[1].Trim();
            }

            if(string.IsNullOrEmpty(tagKey) || string.IsNullOrEmpty(tagValue))
            {
                Debug.LogWarning("Tag key or value is empty.");
            }

            if(tagHandlers.ContainsKey(tagKey))
            {
                tagHandlers[tagKey](tagValue);
            }
        }
    }

    public void StartUI()
    {
        dialogueBox.SetActive(true);

        dialogueText.text = "";

        nameText.text = "";

        //portraitImage.sprite = null;
    }

    public void EndUI()
    {
        dialogueBox.SetActive(false);
    }

    public void UpdateDialogue(string text)
    {
        dialogueText.text = text;
    }

    private void HandleAnimationTag(string value)
    {

        string[] split = value.Split("-");

        string characterName = split[0];

        string animationName = split[1];

        Debug.Log("Character: " + characterName);

        Debug.Log("Animation: " + animationName);

        switch (characterName)
        {
            case "Judas":
                Player.Instance.PlayAnimation(animationName);
                break;
            case "EvilWizard":
                Boss.Instance.PlayAnimation(animationName);
                break;
            case "DemonLord":
                Boss2.Instance.PlayAnimation(animationName);
                break;
            default:
                Debug.LogWarning("Unknown animation tag: " + value);
                break;
        }
    }

    public void CheckSpeakerName(string name)
    {
        switch (name)
        {
            case "Judas":
                nameText.text = "Judas";
                Player.Instance.ChangeCameraPriority();
                break;
            case "Evil Wizard":
                nameText.text = "Evil Wizard";
                Boss.Instance.ChangeCameraPriority();
                break;
            case "Demon Lord":
                nameText.text = "Demon Lord";
                Boss2.Instance.ChangeCameraPriority();
                break;
            default:
                nameText.text = name;
                break;
        }
    }

    

    
}