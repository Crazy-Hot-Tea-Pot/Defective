using System.Collections.Generic;
using UnityEngine;

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
    public enum EnemyType
    {
        Looter,
        SecurityDrone,
        Maintenancebot,
        TicketVendor,
        Garbagebot,
        GangLeader,
        Inspector
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
    private Dictionary<EnemyType, GameObject> enemyPrefabDict = new();

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

    // Update is called once per frame
    void Update()
    {
        
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
    public GameObject GetEnemyPrefab(EnemyType type)
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
                foreach (EnemyType type in System.Enum.GetValues(typeof(EnemyType)))
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
            Debug.LogError("No current level defined in StoryManager.");
            return;
        }

        // 2. Get all combat zones in the scene
        CombatZone[] combatZones = FindObjectsOfType<CombatZone>();
        if (combatZones.Length == 0)
        {
            Debug.LogError("No CombatZones found in the scene.");
            return;
        }

        List<EnemySpawn> enemySpawns = new List<EnemySpawn>(currentLevel.enemySpawns);
        if (enemySpawns.Count == 0)
        {
            Debug.LogWarning("No enemies are set to spawn for this level.");
            return;
        }

        Debug.Log($"Spawning {enemySpawns.Count} enemies across {combatZones.Length} combat zones.");

        int enemyIndex = 0; // Tracks which enemy from enemySpawns to place next

        // 3. Iterate through Combat Zones and assign enemies
        foreach (CombatZone combatZone in combatZones)
        {
            List<(Vector3 position, EnemyManager.EnemyType type)> availablePositions = combatZone.GetEnemySpawnData();

            int spawnCount = Mathf.Min(enemySpawns.Count - enemyIndex, availablePositions.Count);

            for (int i = 0; i < spawnCount; i++)
            {
                EnemySpawn enemySpawn = enemySpawns[enemyIndex++];
                (Vector3 position, EnemyManager.EnemyType storedType) = availablePositions[i];

                GameObject enemyPrefab = enemySpawn.GetEnemyPrefab();
                if (enemyPrefab != null)
                {
                    GameObject enemy = Instantiate(enemyPrefab, position, Quaternion.identity);
                    enemy.GetComponent<Enemy>().SetEnemyName(enemySpawn.enemyName);
                    enemy.SetActive(true);
                    CombatEnemies.Add(enemy);
                    Debug.Log($"Spawned {enemySpawn.enemyType} at {position}");
                }
                else
                {
                    Debug.LogError($"Enemy prefab for {enemySpawn.enemyType} not found!");
                }

                // If we've assigned all enemies, stop early
                if (enemyIndex >= enemySpawns.Count) return;
            }
        }

        Debug.Log($"Finished spawning enemies. {enemySpawns.Count - enemyIndex} enemies were not placed.");
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
            case Levels.Loading:
            case Levels.WorkShop:
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
