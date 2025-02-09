using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static GameData;
public class DataManager : MonoBehaviour
{    
    public static DataManager Instance
    {
        get;
        private set;
    }

    /// <summary>
    /// currentGameData for current game session.
    /// </summary>
    public GameData CurrentGameData
    {
        get
        {
            return currentGameData;
        }
        set
        {
            currentGameData = value;
        }
    }

    [SerializeField]
    private string saveDirectory;


    [SerializeField]
    private GameData currentGameData;

    void Awake()
    {
        // Check if another instance of the SettingsManager exists
        if (Instance == null)
        {
            Instance = this;

            // Keep this object between scenes
            DontDestroyOnLoad(gameObject);

            //Save Directory
            saveDirectory = Path.Combine(Application.dataPath, "GameData");

            if (!Directory.Exists(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
            }          
        }
        else
        {
            Destroy(gameObject);  // Destroy duplicates
        }    
        
        CurrentGameData = new GameData();
    }

    #region Saving

    /// <summary>
    /// Saves all CurrentGameData.
    /// Make sure to update the CurrentGameData first.
    /// </summary>
    public void Save(string saveName)
    {
        string saveFilePath = Path.Combine(saveDirectory, $"{saveName}.json");

        CurrentGameData.SaveName = saveName;
        CurrentGameData.TimeStamp = DateTime.Now;
        CurrentGameData.UpdateTimeStamp();

        // Save Chips
        CurrentGameData.Chips.Clear();

        foreach (var chip in ChipManager.Instance.PlayerDeck)
        {
            GameData.ChipData chipData = new()
            {
                Name = chip.chipName,
                IsUpgraded = chip.IsUpgraded,
                DisableCounter = chip.DisableCounter
            };

            CurrentGameData.Chips.Add(chipData);
        }

        //Save Gear
        CurrentGameData.Gear.Clear();

        foreach (var gear in GearManager.Instance.PlayerCurrentGear) {
            GearData gearData = new()
            {
                GearName = gear.itemName,
                IsEquipped = gear.IsEquipped,
                IsPlayerOwned = gear.IsPlayerOwned,
                Teir=gear.ItemTeir.ToString()
            };

            CurrentGameData.Gear.Add(gearData);
        }

        StoryManager.Instance.SaveStoryProgress();

        string json = JsonUtility.ToJson(CurrentGameData, true);
        File.WriteAllText(saveFilePath, json);

        Debug.Log($"Game saved successfully: {saveFilePath}");

    }

    /// <summary>
    /// AutoSave.
    /// Get all current auto-saves.
    /// Sort auto-saves by number (not timestamp).
    /// Find the next auto-save number (fill gaps or cycle).
    /// Get all used auto-save numbers.
    /// Find the lowest unused number, or cycle to the oldest save.
    /// Delete the oldest save to maintain the limit.
    /// then finally save the data.
    /// </summary>
    public void AutoSave()
    {
        List<GameData> allSaves = GetAllSaves();
        List<GameData> autoSaves = allSaves.FindAll(save => save.SaveName.StartsWith("AutoSave"));

        autoSaves.Sort((a, b) =>
            int.Parse(a.SaveName.Replace("AutoSave", "")) - int.Parse(b.SaveName.Replace("AutoSave", ""))
        );

        int maxAutoSaves = SettingsManager.Instance.DataSettings.MaxAutoSave;

        // Start with 1 by default
        int nextAutoSaveNumber = 1;
        if (autoSaves.Count > 0)
        {
            var usedNumbers = autoSaves.Select(save => int.Parse(save.SaveName.Replace("AutoSave", ""))).ToList();

            if (usedNumbers.Count < maxAutoSaves)
            {
                nextAutoSaveNumber = Enumerable.Range(1, maxAutoSaves).Except(usedNumbers).First();
            }
            else
            {
                nextAutoSaveNumber = int.Parse(autoSaves[0].SaveName.Replace("AutoSave", ""));
                DeleteSave(autoSaves[0].SaveName);
                autoSaves.RemoveAt(0);
            }
        }

        string saveName = $"AutoSave{nextAutoSaveNumber}";

        // Save the data
        Save(saveName);
    }
    #endregion

    #region Loading
    /// <summary>
    /// Load Player stats
    /// Re-link Chips by loading from Resources
    /// Re-link Gears(havn't been done yet)
    /// </summary>
    /// <param name="saveName"></param>
    public void LoadData(string saveName)
    {
        string saveFilePath = Path.Combine(saveDirectory, $"{saveName}.json");

        if (!File.Exists(saveFilePath))
        {
            Debug.LogWarning($"Save file not found: {saveName}");
            return;
        }

        string json = File.ReadAllText(saveFilePath);
        GameData loadedData = JsonUtility.FromJson<GameData>(json);

        // Parse the string back into a DateTime object
        loadedData.ParseTimeStamp();
        CurrentGameData = loadedData;

        //Load Player stats
        CurrentGameData.Health = loadedData.Health;
        CurrentGameData.MaxHealth = loadedData.MaxHealth;
        CurrentGameData.Scraps = loadedData.Scraps;


        // Load Chips
        ChipManager.Instance.PlayerDeck.Clear();

        foreach (var chipData in loadedData.Chips)
        {
            var matchedChip = ChipManager.Instance.AllChips.Find(chip => chip.chipName == chipData.Name);
            if (matchedChip != null)
            {
                // Clone the chip
                NewChip chipInstance = Instantiate(matchedChip);

                matchedChip.IsUpgraded = chipData.IsUpgraded;
                matchedChip.DisableCounter = chipData.DisableCounter;

                ChipManager.Instance.PlayerDeck.Add(matchedChip);
            }
            else
            {
                Debug.LogWarning($"Chip {chipData.Name} not found in AllChips.");
            }
        }

        // Load Gears
        GearManager.Instance.PlayerCurrentGear.Clear();

        foreach (var gearData in loadedData.Gear)
        {
            var matchedItem = GearManager.Instance.AllGear.Find(item => item.itemName == gearData.GearName);
            if (matchedItem != null)
            {
                // Clone the gear
                Item itemInstance = Instantiate(matchedItem);

                matchedItem.IsPlayerOwned = gearData.IsPlayerOwned;
                matchedItem.IsEquipped = gearData.IsEquipped;

                //Restore Teir
                if(Enum.TryParse(gearData.Teir, out Item.Teir loadedTier))
                    matchedItem.ItemTeir=loadedTier;

                GearManager.Instance.PlayerCurrentGear.Add(matchedItem);
            }
            else
            {
                Debug.LogWarning($"Gear {gearData.GearName} not found in AllGears.");
            }
        }

        Debug.Log("Game loaded successfully.");
    }
    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <param name="saveName"></param>
    /// <returns></returns>
    public GameData RetrieveGameData(string saveName)
    {
        string saveFilePath = Path.Combine(saveDirectory, $"{saveName}.json");
        if (!File.Exists(saveFilePath))
        {
            Debug.LogWarning($"Save file not found: {saveName}");
            return null;
        }

        string json = File.ReadAllText(saveFilePath);
        GameData gameData = JsonUtility.FromJson<GameData>(json);
        Debug.Log($"Game loaded successfully: {saveFilePath}");
        return gameData;
    }

    /// <summary>
    /// Gets all the saves the Player has made.
    /// </summary>
    /// <returns></returns>
    public List<GameData> GetAllSaves()
    {
        List<GameData> saves = new List<GameData>();

        if (!Directory.Exists(saveDirectory)) 
            return saves;

        string[] saveFiles = Directory.GetFiles(saveDirectory, "*.json");

        foreach (string filePath in saveFiles)
        {
            string json = File.ReadAllText(filePath);
            GameData gameData = JsonUtility.FromJson<GameData>(json);
            saves.Add(gameData);
        }

        return saves;
    }

    /// <summary>
    /// Delete Save Data.
    /// </summary>
    /// <param name="saveName">Must be exact name of the Save</param>
    /// <returns></returns>
    public bool DeleteSave(string saveName)
    {
        string savePath = Path.Combine(saveDirectory, $"{saveName}.json");
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log($"Deleted save: {saveName}");
            return true;
        }
        Debug.LogWarning($"Attempted to delete a save that does not exist: {saveName}");
        return false;
    }
}
