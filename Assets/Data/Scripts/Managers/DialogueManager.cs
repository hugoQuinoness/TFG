using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Ink.Runtime;

public class DialogueManager : MonoBehaviour
{
    public GameObject dialogueBox;

    public TextMeshProUGUI dialogueText;

    public TextMeshProUGUI nameText;

    public Image portraitImage;

    public static DialogueManager Instance;

    public TextAsset currentDialogue;

    public Story currentStory;

    private string speakingCharacter;

    private string updatedText;

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
    }

    public void EnterDialogueModeWithoutCollision(TextAsset textAsset)
    {
        StartUI();

        currentDialogue = textAsset;

        currentStory = new Story(currentDialogue.text);


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



    }

    public void StartUI()
    {
        dialogueBox.SetActive(true);

        dialogueText.text = "";

        nameText.text = "";

        portraitImage.sprite = null;
    }

    public void EndUI()
    {
        dialogueBox.SetActive(false);
    }

    public void UpdateDialogue(string text)
    {
        dialogueText.text = text;
    }

    public void CheckSpeakerName(string name)
    {
        switch(name)
        {
            case "Player":
                nameText.text = "Player";
                break;
            case "Demon King":
                nameText.text = "Demon King";
                break;
            case "Evil Wizard":
                nameText.text = "Evil Wizard";
                break;
            default:
                nameText.text = name;
                break;
        }
    }
}