using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Video;
using Random = UnityEngine.Random;

public class TitleController : MonoBehaviour
{
    [Header("Title Animation Stuff")]
    public Animator animator;
    public VideoPlayer videoPlayer;
    public GameObject Screen;
    public TextMeshProUGUI TitleText;

    [Header("Title Effect Settings")]
    [Tooltip("Switch between Glitch and Scramble effects.")]
    public bool useScrambleEffect = false;

    [Range(0.0f, 1.0f)]
    [Tooltip("How often the effect happens (increase this to slow down the effect)")]
    public float effectInterval = 0.5f;

    [Range(0.00f, 1.00f)]
    [Tooltip("How long each effect lasts (increase this to make effects last longer)")]
    public float effectDuration = 0.15f;

    [Range(0f, 10f)]
    [Tooltip("How much the text shakes (reduce this for subtle movement)")]
    public float positionJitter = 1f;

    [Header("Buttons")]
    public GameObject ButtonPanel;
    public Button PlayButton;
    public Button ResumeButton;
    public Button OptionsButton;
    public Button QuitButton;

    [Header("Button Sounds")]
    public SoundFX ButtonSelectSound;
    public SoundFX ButtonClickSound;

    [Header("Game Status Info")]
    public TextMeshProUGUI VersionText;

    private GameData latestSave = null;

    [Header("Starting stuff")]
    public Story CurrentStory;
    public int MaxHealth;
    public int StartingScrap;

    // Input System reference
    private PlayerInputActions playerInput;

    // Skip video action
    private InputAction skipAction;

    private string originalText;
    void OnEnable()
    {
        skipAction = playerInput.Player.SkipAnimation;
        skipAction.Enable();
        skipAction.performed += SkipVideo;
    }
    void Awake()
    {
        // Get PlayerInput component
        playerInput = new PlayerInputActions();
    }
    // Start is called before the first frame update
    void Start()
    {
        VersionText.SetText("Version: " + Application.version);

        // Add button click listeners
        PlayButton.onClick.AddListener(PlayButtonClickSound);
        PlayButton.onClick.AddListener(() => StartCoroutine(StartGame()));
        ResumeButton.onClick.AddListener(PlayButtonClickSound);
        ResumeButton.onClick.AddListener(() => StartCoroutine(ResumeGame()));
        OptionsButton.onClick.AddListener(PlayButtonClickSound);
        OptionsButton.onClick.AddListener(() => StartCoroutine(OpenOptions()));
        QuitButton.onClick.AddListener(PlayButtonClickSound);
        QuitButton.onClick.AddListener(() => StartCoroutine(Quit()));

        // Add OnSelect listeners dynamically
        AddOnSelectListener(PlayButton);
        AddOnSelectListener(ResumeButton);
        AddOnSelectListener(OptionsButton);
        AddOnSelectListener(QuitButton);        

        videoPlayer.loopPointReached += OnVideoEnd;

        if (skipAction != null)
        {
            // Listen for input
            skipAction.performed += SkipVideo;
        }


        originalText=TitleText.text;
    }

    /// <summary>
    /// Plays Sound for when mouse over Button.
    /// 0 references because from inspector.
    /// </summary>
    public void PlaySelectButtonSound()
    {
        SoundManager.PlayFXSound(ButtonSelectSound);
    }
    public void PlayButtonClickSound()
    {
        SoundManager.PlayFXSound(ButtonClickSound);
    }

    public void Credits()
    {
        GameManager.Instance.RequestScene(Levels.Credits);
    }

    public void StartEffect()
    {
        StopCoroutine(useScrambleEffect ? ScrambleEffect() : GlitchEffect());
        StartCoroutine(useScrambleEffect ? ScrambleEffect() : GlitchEffect());
    }
    /// <summary>
    /// Adds OnSelect listener to play button sound.
    /// </summary>
    private void AddOnSelectListener(Button button)
    {
        // Get or add an EventTrigger component
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>() ??
                               button.gameObject.AddComponent<EventTrigger>();

        // Add Select event
        EventTrigger.Entry selectEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.Select
        };
        selectEntry.callback.AddListener((eventData) => PlaySelectButtonSound());
        trigger.triggers.Add(selectEntry);

        // Add PointerEnter event
        EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        pointerEnterEntry.callback.AddListener((eventData) => PlaySelectButtonSound());
        trigger.triggers.Add(pointerEnterEntry);
    }

    /// <summary>
    /// Start a new game.
    /// Set Default Player Stats
    /// </summary>
    private IEnumerator StartGame()
    {

        // Wait for the duration of the sound (or a short delay)
        yield return new WaitForSeconds(1f);

        GameData startData = new GameData();

        startData.SaveName = "Beginning";

        startData.MaxHealth=MaxHealth;
        startData.Health = MaxHealth;
        startData.Scraps=StartingScrap;
        startData.storyProgress.storyName = CurrentStory.storyName;

        startData.storyProgress.currentLevel = CurrentStory.levels[0].levelID;

        StoryManager.Instance.LoadStory(CurrentStory);


        // Adds gear to list.

        foreach (Item gear in GearManager.Instance.StartingGear)
        {
            Item gearInstance = Instantiate(gear);
            GearManager.Instance.Acquire(gearInstance);
            GearManager.Instance.EquipGear(gearInstance); 
        }

        // Add Starting Chips
        foreach(NewChip newChip in ChipManager.Instance.StartingChips)
        {
            NewChip chipInstance = Instantiate(newChip);

            ChipManager.Instance.AddNewChipToDeck(chipInstance);
        }
        
        DataManager.Instance.CurrentGameData=startData;

        DataManager.Instance.Save(startData.SaveName);

        // Request the scene from StoryManager (instead of latestSave)
        GameManager.Instance.RequestScene(StoryManager.Instance.CurrentLevel.levelID);
        //GameManager.Instance.RequestScene(Levels.Tutorial);

    }
    /// <summary>
    /// Logic for resuming the game.
    /// </summary>
    private IEnumerator ResumeGame()
    {
        // Wait for the duration of the sound (or a short delay)
        yield return new WaitForSeconds(1f);

        // Load the save data into the game
        DataManager.Instance.LoadData(latestSave.SaveName);

        StoryManager.Instance.LoadStoryProgress();

        // Request the scene from GameManager
        // Request the scene from StoryManager (instead of latestSave)
        GameManager.Instance.RequestScene(StoryManager.Instance.CurrentLevel.levelID);
        //GameManager.Instance.RequestScene(latestSave.storyProgress.currentLevel);
    }
    private IEnumerator OpenOptions()
    {
        // Wait for the duration of the sound (or a short delay)
        yield return new WaitForSeconds(1f);
    }
    /// <summary>
    /// Quit Game
    /// </summary>
    private IEnumerator Quit()
    {
        // Wait for the duration of the sound (or a short delay)
        yield return new WaitForSeconds(1f);

        Application.Quit();
    }
    /// <summary>
    /// Get all saves from DataManager
    /// Enable or disable the ResumeButton based on save availability
    /// Is Called by Animator.
    /// </summary>
    private void CheckForSaveData()
    {    
        List<GameData> allSaves = DataManager.Instance.GetAllSaves();

        if (allSaves != null && allSaves.Count > 0)
        {
            // Find the latest save by timestamp
            foreach (var save in allSaves)
            {
                if (latestSave == null || save.TimeStamp > latestSave.TimeStamp)
                    latestSave = save;
            }

            // Check if the story is complete
            if (latestSave != null && latestSave.storyProgress.isStoryComplete)
            {
                Debug.Log("Story is complete. Disabling Resume button.");
                ResumeButton.interactable = false;
            }
            else
            {
                ResumeButton.interactable = true;
            }
        }     
        else
            ResumeButton.interactable = false;

        Debug.Log("Checked for save data");
    }    
    private void OnVideoEnd(VideoPlayer source)
    {
        animator.SetTrigger("VideoFinish");
    }
    /// <summary>
    /// Stop the video immediately
    /// Move to the next animation
    /// </summary>
    /// <param name="context"></param>
    private void SkipVideo(InputAction.CallbackContext context)
    {        
        videoPlayer.Stop();
        animator.SetTrigger("VideoFinish");
    }


    /// <summary>
    /// 20% chance to glitch each character.
    /// Replace with random ASCII character.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private string GlitchText(string input)
    {
        char[] chars = input.ToCharArray();
        for (int i = 0; i < chars.Length; i++)
        {
            if (Random.value > 0.8f)
            {
                chars[i] = (char)UnityEngine.Random.Range(33, 126);
            }
        }
        return new string(chars);
    }

    private IEnumerator ScrambleEffect()
    {
        char[] scrambleChars = "0123456789ABCDEF@#$%&*<>?/\\|".ToCharArray();

        while (true)
        {
            yield return new WaitForSeconds(effectInterval);
            char[] chars = originalText.ToCharArray();

            for (int i = 0; i < chars.Length; i++)
            {
                if (!char.IsWhiteSpace(chars[i]))
                {
                    char originalChar = chars[i];
                    for (int j = 0; j < 5; j++)
                    {
                        chars[i] = scrambleChars[Random.Range(0, scrambleChars.Length)];
                        TitleText.text = new string(chars);
                        yield return new WaitForSeconds(effectDuration / 5f);
                    }
                    chars[i] = originalChar;
                    TitleText.text = new string(chars);
                }
            }

            Vector3 randomOffset = new Vector3(Random.Range(-positionJitter, positionJitter), Random.Range(-positionJitter, positionJitter), 0);
            TitleText.rectTransform.anchoredPosition += new Vector2(randomOffset.x, randomOffset.y);
            yield return new WaitForSeconds(effectDuration);
            TitleText.rectTransform.anchoredPosition -= new Vector2(randomOffset.x, randomOffset.y);
        }
    }
    private IEnumerator GlitchEffect()
    {
        while (true) // Runs infinitely until title screen changes
        {
            yield return new WaitForSeconds(effectInterval);

            string glitchedText = GlitchText(originalText);           
            Vector3 randomOffset = new Vector3(Random.Range(-positionJitter, positionJitter), Random.Range(-positionJitter, positionJitter), 0);

            TitleText.text = glitchedText;
            // Convert to Vector2
            Vector2 offset2D = new Vector2(randomOffset.x, randomOffset.y);
            TitleText.rectTransform.anchoredPosition += offset2D;

            yield return new WaitForSeconds(effectDuration);

            TitleText.text = originalText;
            TitleText.rectTransform.anchoredPosition -= offset2D;
        }
    }
    void OnDestroy()
    {
        PlayButton.onClick.RemoveAllListeners();
        ResumeButton.onClick.RemoveAllListeners();
        OptionsButton.onClick.RemoveAllListeners();
        QuitButton.onClick.RemoveAllListeners();
        skipAction.Disable();
    }
}
