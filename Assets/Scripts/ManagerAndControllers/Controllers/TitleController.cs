using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TitleController : MonoBehaviour
{
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


        CheckForSaveData();

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
        //startData.Level = Levels.Tutorial;
        //startData.MaxHealth = 50;
        //startData.Health = 50;
        //startData.Scraps = 100;
        //startData.TimeStamp = DateTime.Now;

        //startData.storyProgress.storyName = "Tutorial";
        //startData.storyProgress.currentLevel = Levels.Tutorial;

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

        GameManager.Instance.RequestScene(Levels.Tutorial);

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

        // Request the scene from GameManager
        GameManager.Instance.RequestScene(latestSave.Level);
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
    /// </summary>
    private void CheckForSaveData()
    {    
        List<GameData> allSaves = DataManager.Instance.GetAllSaves();

        if (allSaves != null && allSaves.Count > 0)
            ResumeButton.interactable = true;        
        else
            ResumeButton.interactable = false;

        // Find the latest save by timestamp        
        foreach (var save in allSaves)
        {
            if (latestSave == null || save.TimeStamp > latestSave.TimeStamp)
                latestSave = save;
        }
    }
    [ContextMenu("Bring in Buttons")]
    private void BringInButtons()
    {
        ButtonPanel.GetComponent<Animator>().SetTrigger("BringInButtons");
    }

    void OnDestroy()
    {
       
    }
}
