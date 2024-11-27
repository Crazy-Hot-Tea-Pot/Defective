using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GameData;
using static UnityEditor.Progress;

public class TitleController : MonoBehaviour
{
    [Header("Buttons")]
    public Button PlayButton;
    public Button ResumeButton;
    public Button OptionsButton;
    public Button QuitButton;

    [Header("Game Status Info")]
    public TextMeshProUGUI VersionText;

    [Header("Starter Gear")]
    public List<Gear> StartingInventory;

    private GameData latestSave = null;
    // Start is called before the first frame update
    void Start()
    {
        VersionText.SetText("Version: " + Application.version);

        // Add listeners to buttons
        PlayButton.onClick.AddListener(StartGame);
        ResumeButton.onClick.AddListener(ResumeGame);
        OptionsButton.onClick.AddListener(OpenOptions);
        QuitButton.onClick.AddListener(Quit);

        CheckForSaveData();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// Plays Sound for when mouse over Button.
    /// 0 references because from inspector.
    /// </summary>
    public void PlayButtonSound()
    {
        SoundManager.PlayFXSound(SoundFX.MenuSelectionSound);
    }
    /// <summary>
    /// Start a new game.
    /// Set Default player Stats
    /// </summary>
   private void StartGame()
   {
        GameData startData = new GameData();

        startData.SaveName = "Beginning";
        startData.Level = Levels.Level1;
        startData.MaxHealth = 50;
        startData.Health = 50;
        startData.Scraps = 100;
        startData.TimeStamp = DateTime.Now;

        // Load default chips from Resources
        NewChip punch = Resources.Load<NewChip>("Scriptables/Chips/Punch");
        NewChip guard = Resources.Load<NewChip>("Scriptables/Chips/Guard");
        NewChip motivation = Resources.Load<NewChip>("Scriptables/Chips/Motivation");
        NewChip kickstart = Resources.Load<NewChip>("Scriptables/Chips/Kickstart");

        // Adds gear to list.
        
            foreach (var gear in StartingInventory)
            {
                ItemData itemData = new ItemData();
                itemData.GearName = gear.itemName;
            itemData.isEquipped = gear.IsEquipted;
                itemData.AmountOfAbilities = gear.AbilityList.Count;

                foreach (var ability in gear.AbilityList)
                {
                    AbilityData abilityData = new AbilityData();
                    abilityData.AbilityName = ability.abilityName;
                    abilityData.IsUpgraded = ability.isUpgraded;
                    itemData.ListOfAbilities.Add(abilityData);
                }

                startData.Gears.Add(itemData);
            }


        // Check if chips were loaded successfully
        if (punch == null) 
            Debug.LogWarning("Punch chip not found in Resources.");
        if (guard == null)
            Debug.LogWarning("Guard chip not found in Resources.");
        if (motivation == null) 
            Debug.LogWarning("Motivation chip not found in Resources.");
        if (kickstart == null) 
            Debug.LogWarning("Kickstart chip not found in Resources.");

        if (punch != null)
        {
            startData.Chips.Add(new ChipData { Name = punch.chipName });
            startData.Chips.Add(new ChipData { Name = punch.chipName });
            startData.Chips.Add(new ChipData { Name = punch.chipName });
        }
        if (guard != null)
        {
            startData.Chips.Add(new ChipData { Name = guard.chipName });
            startData.Chips.Add(new ChipData { Name = guard.chipName });
            startData.Chips.Add(new ChipData { Name = guard.chipName });
        }

        if (motivation != null) 
            startData.Chips.Add(new ChipData { Name = motivation.chipName });

        if (kickstart != null) 
            startData.Chips.Add(new ChipData { Name = kickstart.chipName });
        
        DataManager.Instance.CurrentGameData=startData;

        DataManager.Instance.Save(startData.SaveName);

        GameManager.Instance.RequestScene(Levels.Level1);

    }
    /// <summary>
    /// Logic for resuming the game.
    /// </summary>
    private void ResumeGame()
    {
        // Load the save data into the game
        DataManager.Instance.LoadData(latestSave.SaveName);

        // Request the scene from GameManager
        GameManager.Instance.RequestScene(latestSave.Level);
    }
    private void OpenOptions()
    {

    }
    /// <summary>
    /// Quit Game
    /// </summary>
    private void Quit()
    {
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
}
