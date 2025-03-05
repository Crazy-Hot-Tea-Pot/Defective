using System;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance
    {
        get;
        private set;
    }
    public bool Debugging = false;

    public enum GameMode
    {
        None,
        Title,
        Pause,
        Interacting,
        Roaming,
        Combat,
        CombatLoot,
        Settings,
        GameOver,
        Credits,
        Loading
    }

    public GameMode CurrentGameMode
    {
        get
        {
            return currentGameMode;
        }
        private set
        {
            currentGameMode = value;

            // Trigger the event to notify subscribers
            OnGameModeChanged?.Invoke();
        }
    }

    // Events for other Managers
    public event Action OnStartCombat;
    public event Action OnEndCombat;
    public event Action<Levels> OnSceneChange;
    public event Action OnGameModeChanged;
    
    /// <summary>
    /// Scene to load.
    /// </summary>
    public Levels TargetScene
    {
        get
        {
            return targetScene;
        }
        private set
        {
            targetScene = value;
        }
    }

    public Levels CurrentLevel
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
    public Levels PreviousScene
    {
        get;
        private set;
    }

    [SerializeField]
    private GameMode currentGameMode;

    [SerializeField]
    private Levels currentLevel;

    private Levels targetScene;

    void Awake()
    {
        // Check if another instance of the GameManager exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Keep this object between scenes

            // Subscribe to the sceneLoaded event
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);  // Destroy duplicates
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }
    void Initialize()
    {

    }
    /// <summary>
    /// A method that can be used to transition into combat when out of combat
    /// </summary>
    public void StartCombat()
    {       

        CurrentGameMode = GameMode.Combat;

        // Trigger the event for other Managers
        OnStartCombat?.Invoke();
    }

    /// <summary>
    /// A method to transition out of combat
    /// </summary>
    public void EndCombat()
    {       

        // Trigger the event for other Managers
        OnEndCombat?.Invoke();
    }

    /// <summary>
    /// Update Gamemode Externally.
    /// </summary>
    /// <param name="gameMode"></param>
    public void UpdateGameMode(GameMode gameMode)
    {
        CurrentGameMode = gameMode;
    }

    /// <summary>
    /// Initate change level.
    /// </summary>
    /// <param name="level"></param>
    public void RequestScene(Levels level)
    {
        switch (CurrentLevel)
        {
            case Levels.Title:
            case Levels.Loading:
            case Levels.Credits:
                break;
            default:
                DataManager.Instance.CurrentGameData.Level = level;                

                //Get Player
                PlayerController player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

                //Send Data to DataManager
                DataManager.Instance.CurrentGameData.Health = player.Health;
                DataManager.Instance.CurrentGameData.MaxHealth = player.MaxHealth;
                DataManager.Instance.CurrentGameData.Scraps = player.Scrap;



                //Do a Auto Save
                DataManager.Instance.AutoSave();

                break;
        }
        
        TargetScene = level;
        SceneManager.LoadScene(Levels.Loading.ToString());        
    }

    public void GameOver()
    {
        CurrentGameMode = GameMode.GameOver;
    }

    /// <summary>
    /// Load Data after level loads
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="mode"></param>
    private void OnSceneLoaded(Scene scene,LoadSceneMode mode)
    {
        //Debug.Log($"Scene loaded: {scene.name}");
        // Update the CurrentLevel based on the loaded level
        if (System.Enum.TryParse(scene.name, out Levels loadedScene))
        {
            if(CurrentLevel!=Levels.Loading && CurrentLevel != Levels.Credits)
                // Track the previous level
                PreviousScene = CurrentLevel;            

            CurrentLevel = loadedScene;

            switch (loadedScene)
            {
                case Levels.Title:
                    CurrentGameMode = GameMode.Title;
                    break;
                case Levels.Loading:
                    CurrentGameMode = GameMode.Loading;
                    break;
                case Levels.Settings:
                    CurrentGameMode = GameMode.Settings;
                    break;
                case Levels.Credits:
                    CurrentGameMode = GameMode.Credits;
                    break;
                default:
                    CurrentGameMode = GameMode.Roaming;

                    try
                    {
                        if (PreviousScene == Levels.Title)
                            GameObject.FindGameObjectWithTag("Entrance").GetComponent<SceneChange>().SetNextLevel(CurrentLevel);
                        else
                            GameObject.FindGameObjectWithTag("Entrance").GetComponent<SceneChange>().SetNextLevel(PreviousScene);
                    }
                    catch
                    {
                        Debug.LogWarning("No Entrance in scene.");
                    }
                    break;
            }
            
            OnSceneChange?.Invoke(CurrentLevel);
        }        
    }
}