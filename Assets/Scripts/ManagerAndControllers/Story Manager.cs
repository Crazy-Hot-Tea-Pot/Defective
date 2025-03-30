using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GameData;

public class StoryManager : MonoBehaviour
{
    public static StoryManager Instance
    {
        get
        {
            return instance;
        }
        private set
        {
            instance = value;
        }
    }

    public Story CurrentStory
    {
        get
        {
            return currentStory;
        }
        private set
        {
            currentStory = value;
        }
    }

    public LevelDefinition CurrentLevel
    {
        get
        {
            return currentLevel;
        }
        private set
        {
            currentLevel = value;
        }
    }
    public bool StoryCompleted
    {
        get
        {
            return storyComplete;
        }
        private set
        {
            storyComplete = value;
        }
    }

    public IReadOnlyList<Story> AllStories
    {
        get
        {
            return allStories;
        }
    } 

    private List<Story> allStories = new();
    private static StoryManager instance;
    private Story currentStory;
    private LevelDefinition currentLevel;
    private bool storyComplete = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAllStories();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.OnSceneChange += SceneChange;
    }
    /// <summary>
    /// Loads a story from resources and sets the correct level based on saved progress.
    /// </summary>
    /// <param name="story"></param>
    public void LoadStory(Story story)
    {        
        CurrentStory = Instantiate(story);
        CurrentLevel = CurrentStory.levels[0];
    }
    /// <summary>
    /// Returns the currently active level definition.
    /// Useful for referencing which enemies to spawn or quest gating.
    /// </summary>
    public LevelDefinition GetCurrentLevel()
    {
        return currentLevel;
    }
    /// <summary>
    /// Loads the saved story progress from DataManager, sets currentStory & currentLevel accordingly.
    /// </summary>
    public void LoadStoryProgress()
    {
        if (AllStories.Count == 0) {
            Debug.LogWarning("No Stories loaded. Will attempt to load again.");
            LoadAllStories();
        }

        StoryProgress StoryProgress = DataManager.Instance.CurrentGameData.storyProgress;

        if (!string.IsNullOrEmpty(StoryProgress.storyName))
        {
            Story loadingStory = AllStories.FirstOrDefault(s => s.storyName == StoryProgress.storyName);

            if (loadingStory == null)
            {
                Debug.LogError($"Story '{StoryProgress.storyName}' not found in loaded stories.");
                return;
            }


            //New isntance to avoid modifying the original sriptableObject
            CurrentStory = Instantiate(loadingStory);

            //Restore `isCompleted` states
            foreach (var savedLevel in StoryProgress.levels)
            {
                LevelDefinition matchingLevel = CurrentStory.levels.FirstOrDefault(l => l.uniqueLevelID == savedLevel.uniqueLevelID);
                if (matchingLevel != null)
                {
                    matchingLevel.isCompleted = savedLevel.isCompleted;
                }
            }

            //Fix: Use `FindLevelRecursive()` to find `currentLevelUniqueID`
            CurrentLevel = FindLevelRecursive(CurrentStory.levels[0], StoryProgress.currentLevelUniqueID);

            if (CurrentLevel == null)
            {
                Debug.LogWarning($"Could not determine last played level. Defaulting to first level.");
                CurrentLevel = CurrentStory.levels[0];
            }

            StoryCompleted = StoryProgress.isStoryComplete;
        }
        else
        {
            Debug.LogWarning("No story name found in saved progress.");
            StoryCompleted = false;
        }
    }
    /// <summary>
    /// Saves current progress to DataManager’s GameData.
    /// Usually called right after changing levels or completing objectives.
    /// </summary>
    public void SaveStoryProgress()
    {
        DataManager.Instance.CurrentGameData.storyProgress.storyName = currentStory ? currentStory.storyName : "";
        DataManager.Instance.CurrentGameData.storyProgress.currentLevelUniqueID = currentLevel?.uniqueLevelID ?? "";
        DataManager.Instance.CurrentGameData.storyProgress.isStoryComplete = storyComplete;

        DataManager.Instance.CurrentGameData.storyProgress.levels = new List<SavedLevelData>();

        foreach (var level in CurrentStory.levels)
        {
            DataManager.Instance.CurrentGameData.storyProgress.levels.Add(new SavedLevelData
            {
                uniqueLevelID = level.uniqueLevelID,
                levelID = level.levelID,
                isCompleted = level.isCompleted
            });
        }

        DataManager.Instance.CurrentGameData.storyProgress.currentLevel = currentLevel != null ? currentLevel.levelID : 0;
    }
    /// <summary>
    /// Update the story with next level the player choose.
    /// </summary>
    /// <param name="nextLevel">Level Enum</param>
    public void SetNextLevel(Levels nextLevel)
    {

        if (CurrentStory == null)
        {
            Debug.LogError("StoryManager: No current story loaded!");
            return;
        }

        // Mark the current level as completed before switching
        if (CurrentLevel != null)
        {
            CurrentLevel.isCompleted = true;
        }

        LevelDefinition nextLevelDef = CurrentLevel.NextLevels.Find(level => level.levelID == nextLevel);

        if (nextLevelDef != null)
        {
            CurrentLevel = nextLevelDef;
            Debug.Log($"StoryManager: Current Level set to {CurrentLevel.levelID}");            

            if (CurrentLevel.NextLevels == null || CurrentLevel.NextLevels.Count == 0)
            {
                StoryCompleted = true;  
                Debug.Log("StoryManager: Story complete!");
                SaveStoryProgress();
            }
        }
        else
        {
            Debug.LogWarning($"StoryManager: Level {nextLevel} not found in the story! Check story setup.");
        }
    }
    private void SceneChange(Levels newLevel)
    {
        switch (newLevel)
        {
            case Levels.Title:
            case Levels.Settings:
            case Levels.Credits:
            case Levels.Loading:
            case Levels.Win:
                break;
            default:

                GameObject[] tempDoors = GameObject.FindGameObjectsWithTag("Exit");

                if (tempDoors.Length == 0)
                {
                    Debug.LogWarning("No exits found in the scene.");
                    return;
                }

                if (CurrentLevel.NextLevels.Count == 0)
                {
                    Debug.LogWarning("No next levels assigned for this level.");
                    return;
                }

                // Ensure we are not assigning more exits than next levels available
                for (int i = 0; i < tempDoors.Length; i++)
                {
                    SceneChange sceneChange = tempDoors[i].GetComponent<SceneChange>();

                    if (sceneChange == null)
                    {
                        Debug.LogError($"Exit {tempDoors[i].name} is missing a SceneChange component!");
                        continue;
                    }

                    // Assign next levels in order, looping if necessary
                    Levels assignedLevel = CurrentLevel.NextLevels[i % CurrentLevel.NextLevels.Count].levelID;
                    sceneChange.SetNextLevel(assignedLevel);

                }

                //Handle terminal Activation
                GameObject upgradeComputer = GameObject.Find("Upgrade Computer");

                if (upgradeComputer != null)
                {
                    if (Random.Range(0, 100) < CurrentLevel.terminalSpawnChance)
                        upgradeComputer.SetActive(true);
                    else
                    {
                        Debug.Log("With a terminal spawn chance of " + CurrentLevel.terminalSpawnChance + " upgradeComputer failed to spawn.");
                        upgradeComputer.SetActive(false);
                    }
                }
                else
                    Debug.LogWarning("Can not find Upgrade Computer in scene.");
                break;
        }
    }
    private void LoadAllStories()
    {
        Story[] loadedStories = Resources.LoadAll<Story>("Scriptables/Stories");
        if (loadedStories.Length == 0)
        {
            Debug.LogWarning("No stories found in Resources/Scriptables/Stories.");
        }
        allStories = loadedStories.ToList();
        Debug.Log($"Loaded {allStories.Count} stories from Resources.");
    }
    /// <summary>
    /// Recursively searches for a level in the story by its levelID.
    /// </summary>
    /// <param name="current"></param>
    /// <param name="targetLevel"></param>
    /// <returns></returns>
    private LevelDefinition FindLevelRecursive(LevelDefinition current, string targetUniqueID)
    {
        if (current.uniqueLevelID == targetUniqueID)
            return current;

        foreach (var nextLevel in current.NextLevels)
        {
            LevelDefinition foundLevel = FindLevelRecursive(nextLevel, targetUniqueID);
            if (foundLevel != null)
                return foundLevel;
        }
        return null;
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
