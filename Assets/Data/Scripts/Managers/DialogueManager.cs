using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Ink.Runtime;
using System;
using UnityEngine.UI;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using UnityEngine.TextCore.Text;


public class DialogueManager : MonoBehaviour
{
    public GameObject dialogueBox;

    public TextMeshProUGUI dialogueText;

    public TextMeshProUGUI nameText;

    public Image portraitImage;

    public static DialogueManager Instance;

    public UnityEngine.TextAsset currentDialogue;

    public Story currentStory;

    public bool isDialogueActive;

    private string speakingCharacter;

    private string updatedText;

    public static Action InvokeOnDialogueEnter;

    public static Action InvokeOnDialogueExit;

    private Dictionary<string, Action<string>> tagHandlers;

    public int tagsToHandle;

    private Coroutine typingCoroutine;

    private AsyncOperationHandle<UnityEngine.TextAsset>? currentHandle;

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
            { "animateAndMove", HandleMoveWithAnimation},
            { "song", HandleSongTag },
        };
    }

    private void OnDestroy()
    {
        DialogueControls.InvokeContinueDialogue -= ContinueDialogue;
    }

    public void ContinueDialogue()
    {

        if (tagsToHandle > 0)
        {
            return;
        }

        if (!isDialogueActive)
        {
            return;
        }

        if (currentStory.canContinue)
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }

            HandleLine(currentStory.Continue());
            HandleTags(currentStory.currentTags);
        }
        else
        {
            ExitDialogueMode();
        }
    }

    public void EnterDialogueModeWithoutCollision(UnityEngine.TextAsset textAsset)
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

        Player.Instance.ChangeCameraPriority();

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
        UpdateDialogue(updatedText);
    }

    public void HandleTags(List<string> currentTags)
    {
        tagsToHandle = currentTags.Count;

        foreach (string tag in currentTags)
        {
            string tagKey;
            string tagValue;

            string[] tagSplit = tag.Split(':');

            if (tagSplit.Length == 1)
            {
                tagKey = tagSplit[0].Trim();
                tagValue = "Default";
            }
            else
            {
                tagKey = tagSplit[0].Trim();
                tagValue = tagSplit[1].Trim();
            }

            if (tagHandlers.ContainsKey(tagKey))
            {
                tagHandlers[tagKey](tagValue);
            }
        }
    }

    private void HandleMoveWithAnimation(string value)
    {
        string[] split = value.Split('_');

        if (split.Length < 5)
        {
            return;
        }

        if (!float.TryParse(split[0], out float x))
        {
            return;
        }
        if (!float.TryParse(split[1], out float y))
        {
            return;
        }
        if (!float.TryParse(split[2], out float time))
        {
            return;
        }

        string characterName = split[3];

        string animationName = split[4];

        switch (characterName)
        {
            case "Judas":
                StartCoroutine(Player.Instance.MoveTo(x, y, time));
                Player.Instance.PlayAnimation(animationName);
                break;
            case "EvilWizard":
                StartCoroutine(Boss.Instance.MoveTo(x, y, time));
                Boss.Instance.PlayAnimation(animationName);
                break;
            case "DemonLord":
                Boss2.Instance.MoveTo(x, y, time);
                Boss2.Instance.PlayAnimation(animationName);
                break;
            default:
                break;
        }
    }

    private void HandleAnimationTag(string value)
    {

        string[] split = value.Split("_");

        string characterName = split[0];

        string animationName = split[1];

        Debug.Log("Character: " + characterName);

        Debug.Log("Animation: " + animationName);

        switch (characterName)
        {
            case "Judas":
                Player.Instance.PlayAnimation(animationName);
                tagsToHandle--;
                break;
            case "EvilWizard":
                Boss.Instance.PlayAnimation(animationName);
                tagsToHandle--;
                break;
            case "DemonLord":
                Boss2.Instance.PlayAnimation(animationName);
                tagsToHandle--;
                break;
            default:
                Debug.LogWarning("Unknown animation tag: " + value);
                break;
        }
    }

    private void HandleSongTag(string value)
    {
        SongManager.Instance.PlaySongFromAddressable(value);
    }

    public void StartUI()
    {
        dialogueBox.SetActive(true);

        dialogueText.text = "";

        nameText.text = "";
    }

    public void EndUI()
    {
        dialogueBox.SetActive(false);
    }

    public void UpdateDialogue(string text)
    {
        typingCoroutine = StartCoroutine(TypeTextEffect(text));
    }

    private IEnumerator TypeTextEffect(string text)
    {
        dialogueText.text = "";

        int charIndex = 0;
        foreach (char c in text.ToCharArray())
        {
            dialogueText.text += c;

            if (charIndex % 2 == 0)
            {
                SFXManager.Instance.PlayTypingSFX();
            }

            charIndex++;
            yield return new WaitForSeconds(0.05f);
        }

    }

    public void CheckSpeakerName(string name)
    {
        switch (name)
        {
            case "Judas":
                nameText.text = "Judas";

                Player.Instance.ChangeCameraPriority();
                portraitImage.sprite = Player.Instance.sprite;
                break;
            case "Evil Wizard":
                nameText.text = "Evil Wizard";
                Boss.Instance.ChangeCameraPriority();
                portraitImage.sprite = Boss.Instance.sprite;
                break;
            case "Demon Lord":
                nameText.text = "Demon Lord";
                portraitImage.sprite = null;
                Boss2.Instance.ChangeCameraPriority();
                break;
            default:
                nameText.text = name;
                break;
        }
    }

    public IEnumerator StartDialogueByAdr3ss(string address)
    {

        if (currentHandle.HasValue && currentHandle.Value.IsValid())
        {
            Debug.Log("[SongManager] Releasing previous handle");
            Addressables.Release(currentHandle.Value);
        }

        currentHandle = Addressables.LoadAssetAsync<UnityEngine.TextAsset>(address);

        if (!currentHandle.HasValue)
        {
            Debug.LogError("[SongManager] currentHandle is null after LoadAssetAsync!");
            yield break;
        }

        yield return currentHandle.Value;
        
        EnterDialogueModeWithoutCollision(currentHandle.Value.Result);

    }
}