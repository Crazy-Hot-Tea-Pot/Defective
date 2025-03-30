using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

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
    private PlayerInputActions playerInput;

    [Header("Sounds")]
    public SoundFX SpeakerSound;
    public SoundFX PlayerSpeakingSound;
    public SoundFX MessageRecieveSound;

    void OnEnable()
    {
        playerInput.Player.Enable();
    }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        playerInput = new PlayerInputActions();
        playerInput.Player.SkipDialogue.performed += ctx => SkipDialogue();
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
        CallScreen.gameObject.SetActive(true);

        CallScreen.ClearDialogue();

        ShowNextLine();
    }

    /// <summary>
    /// Displays the next line of dialogue.
    /// </summary>
    public void ShowNextLine()
    {
        SoundManager.StopLoopingFXSound(SpeakerSound);
        SoundManager.StopLoopingFXSound(PlayerSpeakingSound);
        
        if (dialogueQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueLine line = dialogueQueue.Dequeue();

        if (line.isPlayerSpeaking)
        {
            // If the player is speaking, use PlayerSpeak() instead of CallScreen
            GameObject.Find("Player").GetComponent<PlayerController>().CharacterSpeak(
                line.dialogueText, line.revealByLetter, line.textSpeed, line.timeBetweenLines, true
            );

            SoundManager.PlayFXSound(PlayerSpeakingSound,true,false);
        }
        else
        {
            SoundManager.PlayFXSound(MessageRecieveSound);

            // If an NPC is speaking, show it on CallScreen
            CallScreen.SpeakerName = line.speakerName;
            CallScreen.SpeakerImage = line.speakerImage != null ? line.speakerImage.texture : null;
            CallScreen.NextText(line.dialogueText, line.revealByLetter, line.textSpeed, line.timeBetweenLines);

            SoundManager.PlayFXSound(SpeakerSound, true, false);
        }
    }

    /// <summary>
    /// Ends the dialogue sequence.
    /// </summary>
    private void EndDialogue()
    {
        CallScreen.ClearDialogue();
        CallScreen.gameObject.SetActive(false);

        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>().SwitchCamera(CameraController.CameraState.Default);

        GameManager.Instance.UpdateGameMode(GameManager.GameMode.Roaming);
    }

    /// <summary>
    /// Skips all remaining dialogue and ends immediately
    /// </summary>
    private void SkipDialogue()
    {
        if (GameManager.Instance.CurrentGameMode == GameManager.GameMode.Dialogue)
        {
            if (currentDialogue != null)
            {
                dialogueQueue.Clear();
                EndDialogue();
            }
        }
    }

    private void SceneChange(Levels newLevel)
    {
        switch (newLevel)
        {
            case Levels.Title:
            case Levels.Loading:
            case Levels.Settings:
            case Levels.Credits:
            case Levels.Win:
                break;
            default:
                CallScreen = GameObject.Find("Player").GetComponent<PlayerController>().CallScreen.GetComponent<CallScreen>();
                    break;
        }
    }
    void OnDisable()
    {
        playerInput.Player.Disable();    
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
