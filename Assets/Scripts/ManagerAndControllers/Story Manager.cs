using UnityEngine;

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
            if (currentLevel == null)
            {
                Debug.LogError("StoryManager: Current level is null!");
            }
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
    /// Loads a story from resources and sets teh current level
    /// </summary>
    /// <param name="storyName"></param>
    public void LoadStory(string storyName)
    {
        Debug.LogError("Nothing in here, Jayce you forgot me!");
    }
    public void LoadStory(Story story)
    {
        CurrentStory = story;
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
        var sp = DataManager.Instance.CurrentGameData.storyProgress;
        if (!string.IsNullOrEmpty(sp.storyName))
        {
            LoadStory(sp.storyName);
        }
        // 
        // Then find the matching LevelDefinition in currentStory 
        // and set currentLevel to it, if it exists.
    }

    /// <summary>
    /// Saves current progress to DataManager’s GameData.
    /// Usually called right after changing levels or completing objectives.
    /// </summary>
    public void SaveStoryProgress()
    {
        DataManager.Instance.CurrentGameData.storyProgress.storyName = currentStory ? currentStory.storyName : "";
        DataManager.Instance.CurrentGameData.storyProgress.currentLevel = currentLevel?.levelID ?? Levels.Title;
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
        else
        {
            CurrentLevel.isCompleted = true;
        }

        LevelDefinition nextLevelDef = CurrentLevel.NextLevels.Find(level => level.levelID == nextLevel);

        if (nextLevelDef != null)
        {
            CurrentLevel = nextLevelDef;
            Debug.Log($"StoryManager: Current Level set to {CurrentLevel.levelID}");
        }
        else
        {
            Debug.LogError($"StoryManager: Level {nextLevel} not found in the story! Check story setup.");
        }
    }


    private void SceneChange(Levels newLevel)
    {
        switch (newLevel)
        {
            case Levels.Title:
            case Levels.Credits:
            case Levels.Loading:
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
    void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnSceneChange -= SceneChange;
        }
    }
}
