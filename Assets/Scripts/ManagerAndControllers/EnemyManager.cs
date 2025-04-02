using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using static GameEnums;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance
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

    /// <summary>
    /// Have to manually Update this for now.
    /// TODO add a generator that auto populates list of enemy types.
    /// </summary>
    public enum TypeOfEnemies
    {
        Looter,
        SecurityDrone,
        Maintenancebot,
        TicketVendor,
        Garbagebot,
        GangLeader
    }

    public List<GameObject> EnemiesInLevel;

    public List<GameObject> CombatEnemies
    {
        get
        {
            return combatEnemies;
        }
        private set
        {
            combatEnemies = value;
        }
    }

    private static EnemyManager instance;
    private List<GameObject> combatEnemies = new();
    [SerializeField] 
    private List<GameObject> enemyPrefabs;
    private Dictionary<TypeOfEnemies, GameObject> enemyPrefabDict = new();

    void Awake()
    {
        // Check if another instance of the GameManager exists
        if (Instance == null)
        {
            Instance = this;
            // Keep this object between scenes
            DontDestroyOnLoad(gameObject);

        }
        else
        {
            // Destroy duplicates
            Destroy(gameObject);
        }

        PopulateDictionary();
    }

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.OnStartCombat += StartCombat;
        GameManager.Instance.OnEndCombat += EndCombat;
        GameManager.Instance.OnSceneChange += SceneChange;
    }

    /// <summary>
    /// Add Enemy to Combat Enemies list.
    /// </summary>
    /// <param name="enemy"></param>
    public void AddCombatEnemy(GameObject enemy)
    {
        CombatEnemies.Add(enemy);
    }

    /// <summary>
    /// Get Enemy Object for Enemy Type
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public GameObject GetEnemyPrefab(TypeOfEnemies type)
    {
        return enemyPrefabDict.ContainsKey(type) ? enemyPrefabDict[type] : null;
    }

    /// <summary>
    /// Remove defeated enemy from game.
    /// </summary>
    /// <param name="enemy"></param>
    public void RemoveEnemy(GameObject enemy)
    {
        EnemiesInLevel.Remove(enemy);
        CombatEnemies.Remove(enemy);
        
        Destroy(enemy);
    }

    private void PopulateDictionary()
    {
        enemyPrefabDict.Clear();
        foreach (var prefab in enemyPrefabs)
        {
            if (prefab != null)
            {
                string normalizedPrefabName = prefab.name.Replace(" ", "").ToLower();
                foreach (TypeOfEnemies type in System.Enum.GetValues(typeof(TypeOfEnemies)))
                {
                    string normalizedEnumName = type.ToString().ToLower(); // Normalize enum name (lowercase)
                    if (normalizedPrefabName == normalizedEnumName)
                    {
                        enemyPrefabDict[type] = prefab;
                        break; // Stop looping once we find a match
                    }
                }
            }
        }

        //Debug.Log("Enemy Prefab Dictionary:");
        //foreach(var test in enemyPrefabDict)
        //{
        //    Debug.Log("Type: " + test.Key + ", Prefab:" + test.Value.name);
        //}
    }

    private void GetAllEnemiesInLevel()
    {
        CombatEnemies.Clear();
        EnemiesInLevel.Clear();
        EnemiesInLevel.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
    }

    /// <summary>
    /// Get Current Level data from Story Manager and spawn those enemies into the scene.
    /// </summary>
    private void SpawnEnemiesForLevel()
    {
        // 1. Get the current level from StoryManager
        LevelDefinition currentLevel = StoryManager.Instance.GetCurrentLevel();
        if (currentLevel == null)
        {
            Debug.LogWarning("No current level defined in StoryManager.");
            return;
        }

        // 2. Get all combat zones in the scene
        CombatZone[] combatZones = FindObjectsOfType<CombatZone>();
        if (combatZones.Length == 0)
        {
            Debug.LogWarning("No CombatZones found in the scene.");
            return;
        }

        List<EnemySpawn> enemySpawns = new List<EnemySpawn>(currentLevel.enemySpawns);
        if (enemySpawns.Count == 0)
        {
            Debug.LogWarning("No enemies are set to spawn for this level.");
            return;
        }                        
        //3. loop through each combat zone and directly use enemiesinzone
        foreach(CombatZone combatZone in combatZones)
        {
            foreach (var enemyData in combatZone.EnemiesInZone)
            {
                if (enemyData.enemyObject == null)
                {
                    Debug.LogWarning("Warning: Missing enemy object for " + enemyData.enemyType + " in " + combatZone.name);
                    continue;
                }

                GameObject enemyPrefab = GetEnemyPrefab(enemyData.enemyType);

                if (enemyPrefab == null)
                {
                    Debug.LogError("ERROR: No prefab found for " + enemyData.enemyType);
                    continue;
                }

                string enemyName = "Unable to fetch name";
                Enemy.EnemyDifficulty enemyDifficulty = Enemy.EnemyDifficulty.Medium;
                EnemySpawn matchingEnemy = enemySpawns.Find(e => e.enemyType == enemyData.enemyType);

                if(matchingEnemy != null)
                {
                    enemyName = matchingEnemy.enemyName;
                    enemyDifficulty = matchingEnemy.difficulty;
                    enemySpawns.Remove(matchingEnemy);
                }
                else
                {
                    Debug.LogWarning("Can't find enemy name");
                }

                //Spawn enemy
                GameObject enemy = Instantiate(enemyPrefab, enemyData.position, Quaternion.identity);
                Enemy enemyComponent = enemy.GetComponent<Enemy>();                
                enemy.SetActive(true);

                if (enemyComponent != null)
                {
                    enemyComponent.SetEnemyName(enemyName);
                    enemyComponent.Difficulty = (enemyDifficulty == Enemy.EnemyDifficulty.None) ? Enemy.EnemyDifficulty.Medium : enemyDifficulty;
                }
                else
                {
                    Debug.LogError("Spawned enemy is missing the Enemy component!");
                }
            }
        }
    }

    private void StartCombat()
    {

    }

    private void EndCombat()
    {
        CombatEnemies.Clear();
    }
    private void SceneChange(Levels newLevel)
    {

        switch (newLevel)
        {
            case Levels.Title:
            case Levels.Settings:
            case Levels.Loading:
            case Levels.WorkShop:
            case Levels.Credits:
            case Levels.Win:
                break;
            default:
                SpawnEnemiesForLevel();
                GetAllEnemiesInLevel();
                break;
        }
    }
    private void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStartCombat -= StartCombat;
            GameManager.Instance.OnEndCombat -= EndCombat;
            GameManager.Instance.OnSceneChange -= SceneChange;
        }
    }
}
