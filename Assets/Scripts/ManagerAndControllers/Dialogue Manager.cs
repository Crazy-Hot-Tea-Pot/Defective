using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;    
    public Dialogue currentDialogue;

    public CallScreen CallScreen
    {
        get
        {
            return callScreen;
        }
        private set
        {
            callScreen = value;
        }
    }
    private CallScreen callScreen;    

    private Queue<DialogueLine> dialogueQueue = new Queue<DialogueLine>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    void Start()
    {
        GameManager.Instance.OnSceneChange += SceneChange;
    }

    public void StartDialogue(Dialogue dialogue)
    {
        if (dialogue == null || dialogue.lines.Count == 0)
            return;

        currentDialogue = dialogue;
        dialogueQueue.Clear();

        foreach (DialogueLine line in dialogue.lines)
        {
            dialogueQueue.Enqueue(line);
        }

        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>().SwitchCamera(CameraController.CameraState.Call);

        // Reset CallScreen before starting
        CallScreen.ClearDialogue();

        ShowNextLine();
    }

    /// <summary>
    /// Displays the next line of dialogue.
    /// </summary>
    public void ShowNextLine()
    {
        if (dialogueQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueLine line = dialogueQueue.Dequeue();

        // Set speaker name and image through CallScreen properties
        CallScreen.SpeakerName = line.isPlayerSpeaking ? "Player" : line.speakerName;
        CallScreen.SpeakerImage = line.speakerImage != null ? line.speakerImage.texture : null;

        // Send text to CallScreen for handling
        CallScreen.NextText(line.dialogueText, line.revealByLetter, line.textSpeed, line.timeBetweenLines);
    }

    /// <summary>
    /// Ends the dialogue sequence.
    /// </summary>
    private void EndDialogue()
    {
        CallScreen.ClearDialogue();

        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>().SwitchCamera(CameraController.CameraState.Default);
    }
    private void SceneChange(Levels newLevel)
    {
        switch (newLevel)
        {
            case Levels.Title:
            case Levels.Loading:
            case Levels.Settings:
            case Levels.Credits:
                break;
            default:
                CallScreen = GameObject.Find("CallScreen").GetComponent<CallScreen>();
                    break;
        }
    }
    void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        if (GameManager.Instance != null)
        {            
            GameManager.Instance.OnSceneChange -= SceneChange;
        }
    }
}
