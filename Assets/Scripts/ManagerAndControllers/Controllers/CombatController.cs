using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CombatController : MonoBehaviour
{
    public static CombatController Instance
    {
        get;
        private set;
    }
    public GameObject Player
    {
        get
        {
            return player;
        }
        private set
        {
            player = value;
        }
    }
    public GameObject Target
    {
        get
        {
            return currentTarget;
        }
        private set
        {
            currentTarget = value;
        }
    }
    public List<GameObject> CombatEnemies
    {
        get
        {
            return EnemyManager.Instance.CombatEnemies;
        }        
    }
    /// <summary>
    /// Area bounds of the CombatZone.
    /// </summary>
    public GameObject CombatArea
    {
        get
        {
            return combatArea;
        }
        set
        {
            combatArea = value;
        }
    }

    // Reference to inputAction class that is generated by unity.
    public PlayerInputActions playerInputActions;

    public GameObject CurrentCombadant
    {
        get
        {
            return currentCombatant;
        }
        set
        {
            currentCombatant = value;
        }
    }
    public int RoundCounter
    {
        get
        {
            return roundCounter;
        }
        private set
        {
            roundCounter = value;
        }
    }

    [Header("Current Combat Loot")]
    public List<Item> ItemsLootForCurrentCombat = new();
    public List<NewChip> NewChipLootForCurrentCombat = new();
    public int ScrapLootForCurrentCombat
    {
        get
        {
            return scrapLootForCurrentCombat;
        }
        set
        {
            scrapLootForCurrentCombat = value;
        }
    }

    private Queue<GameObject> turnQueue = new Queue<GameObject>();
    private GameObject player;
    private GameObject currentCombatant;        
    private GameObject currentTarget;
    private InputAction selectTargetAction;
    private InputAction cycleTargetAction;
    private int scrapLootForCurrentCombat;
    private int currentTargetIndex = -1;
    private int roundCounter = 0;
    private GameObject CombatZone;
    [SerializeField]
    private GameObject combatArea;


    [Header("Combat Sounds")]
    public SoundFX CombatStartSound;
    public BgSound CombatSound;
    public SoundFX CombatWinSound;
    private BgSound previousSound;

    void OnEnable()
    {
        selectTargetAction = playerInputActions.PlayerCombat.SelectTarget;
        selectTargetAction.Enable();
        selectTargetAction.performed += OnSelectTarget;

        cycleTargetAction = playerInputActions.PlayerCombat.CycleTarget;
        cycleTargetAction.Enable();
        cycleTargetAction.performed += OnCycleTarget;
    }
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Ensure the PlayerInputActions is initialized
        if (playerInputActions == null)
        {
            playerInputActions = new PlayerInputActions();
        }
    }
    /// <summary>
    /// Starts combat by initializing combatants and setting up the turn order.
    /// </summary>
    public void StartCombat(GameObject CombatZone)
    {
        this.CombatZone = CombatZone;

        GameManager.Instance.StartCombat();

        // Store the previous background sound before changing it
        previousSound = SoundManager.GetCurrentBackgroundSound();

        // Play the combat start sound first, then switch to combat background music
        StartCoroutine(PlayCombatStartThenBG());

        RoundCounter = 1;

        // Setup player and enemies
        Player = GameObject.FindGameObjectWithTag("Player");

        Player.GetComponent<PlayerController>().CombatStart();

        // Prepare turn queue: player always goes first
        turnQueue.Enqueue(Player);
        foreach (var enemy in CombatEnemies)
        {
            turnQueue.Enqueue(enemy);
        }

        //Call start combat for each enemy
        foreach(var enemy in CombatEnemies)
        {
            enemy.GetComponent<Enemy>().CombatStart();
        }

        StartTurn();

        GameStatsTracker.Instance.StartCombatTimer();
    }

    /// <summary>
    /// Ends the turn for the specified combatant and proceeds to the next turn.
    /// </summary>
    /// <param name="combatant">The combatant that ended their turn.</param>
    public void EndTurn(GameObject combatant)
    {                    
        StartTurn();
    }

    /// <summary>
    /// Handles an enemy leaving combat, either by dying.
    /// Adds loot and removes the enemy from the turn queue.
    /// </summary>
    /// <param name="enemy">The enemy leaving combat.</param>
    /// <param name="scrapLoot">Scrap dropped by the enemy.</param>
    /// <param name="newChips">Chips dropped by the enemy.</param>
    /// <param name="items">Items dropped by the enemy.</param>
    public void LeaveCombat(GameObject enemy, int scrapLoot, List<NewChip> newChips, List<Item> items)
    {
        if (!CombatEnemies.Contains(enemy)) 
            return;

        Debug.Log($"{enemy.name} has left combat!");
        CombatEnemies.Remove(enemy);

        // Rebuild the queue without the enemy
        turnQueue = new Queue<GameObject>(CombatEnemies);

        // Add loot
        ScrapLootForCurrentCombat += scrapLoot;

        //Add items to loot
        if(items != null)
            ItemsLootForCurrentCombat.AddRange(items);

        //Add chips to loot
        if(newChips != null)
            NewChipLootForCurrentCombat.AddRange(newChips);

        // Check for combat end
        CheckCombatEnd();
    }
    /// <summary>
    /// Handles an enemy leaving combat by escape.
    /// </summary>
    /// <param name="enemy"></param>
    public void LeaveCombat(GameObject enemy)
    {
        if (!CombatEnemies.Contains(enemy))
            return;

        Debug.Log($"{enemy.name} has left combat!");
        CombatEnemies.Remove(enemy);

        // Rebuild the queue without the enemy
        turnQueue = new Queue<GameObject>(CombatEnemies);

        if (CombatEnemies.Count == 0)
        {
            Debug.Log("All enemies defeated or escaped. Combat ends.");
            EndCombat();
        }
        else
        {
            StartTurn();
        }
    }

    /// <summary>
    /// Adds a new enemy to the combat, initializing them and updating the turn queue.
    /// </summary>
    /// <param name="newEnemy">The enemy GameObject to add to combat.</param>
    public void AddEnemyToCombat(GameObject newEnemy)
    {
        if (newEnemy == null)
        {
            Debug.LogWarning("Attempted to add a null enemy to combat.");
            return;
        }

        // Add the new enemy to the CombatEnemies list
        CombatEnemies.Add(newEnemy);

        // Insert the enemy into the turn queue
        turnQueue.Enqueue(newEnemy);

        // Initialize the enemy for combat
        Enemy enemyComponent = newEnemy.GetComponent<Enemy>();
        if (enemyComponent != null)
        {
            enemyComponent.InCombat = true;
            enemyComponent.CombatStart();
            Debug.Log($"{newEnemy.name} has been added to combat!");
        }
        else
        {
            Debug.LogError($"{newEnemy.name} does not have an Enemy component!");
        }
    }

    /// <summary>
    /// Starts the next combatant's turn.
    /// If all combatants have acted, ends the round and starts a new one.
    /// </summary>
    private void StartTurn()
    {
        if (turnQueue.Count == 0)
        {
            EndRound();
            return;
        }        

        CurrentCombadant = turnQueue.Dequeue();

        if (CurrentCombadant == Player)
        {

            UiManager.Instance.ChangeCombatScreenTemp(true);                        

            player.GetComponent<PlayerController>().StartTurn();
        }
        else if (CombatEnemies.Contains(CurrentCombadant))
        {

            CurrentCombadant.GetComponent<Enemy>().MyTurn();
        }
    }

    /// <summary>
    /// Ends the current round and starts a new one.
    /// Calls RoundEnd() for all combatants and resets the turn queue.
    /// </summary>
    private void EndRound()
    {
        if (Player == null)
            Player = GameObject.Find("Player");

        Player.GetComponent<PlayerController>().RoundEnd();

        foreach (GameObject enemy in CombatEnemies)
        {
            enemy.GetComponent<Enemy>().RoundEnd();
        }

        // Refill the turn queue
        turnQueue.Enqueue(Player);

        foreach (GameObject enemy in CombatEnemies)
        {
            turnQueue.Enqueue(enemy);
        }

        RoundCounter++;

        StartTurn();
    }

    /// <summary>
    /// Checks if combat has ended and handles end-of-combat logic.
    /// </summary>
    private void CheckCombatEnd()
    {       
        if (CombatEnemies.Count == 0)
        {
            Debug.Log("All enemies defeated or escaped. Combat ends.");
            EndCombat();
        }
    }

    /// <summary>
    /// Ends combat and handles cleanup, including notifying the game manager and UI.
    /// </summary>
    private void EndCombat()
    {
        Debug.Log("Combat has ended."); 
        
        GameStatsTracker.Instance.EndCombatTimer();

        SoundManager.PlayFXSound(CombatWinSound);

        // Notify GameManager or handle loot distribution
        GameManager.Instance.EndCombat();

        if (ScrapLootForCurrentCombat > 0 || ItemsLootForCurrentCombat.Count > 0 || NewChipLootForCurrentCombat.Count > 0)
        {
            GameManager.Instance.UpdateGameMode(GameManager.GameMode.CombatLoot);

            UiManager.Instance.SendLoot(ScrapLootForCurrentCombat, ItemsLootForCurrentCombat, NewChipLootForCurrentCombat);
        }
        else
        {
            GameManager.Instance.UpdateGameMode(GameManager.GameMode.Roaming);
        }
        //Add loot to player
        Player.GetComponent<PlayerController>().GainScrap(ScrapLootForCurrentCombat);

        //Delete CombatZone
        Destroy(CombatZone);

        Reset();
    }
    private IEnumerator PlayCombatStartThenBG()
    {
        // Play CombatStart sound effect
        AudioClip combatStartClip = SoundManager.GetSoundFxClip(CombatStartSound);
        if (combatStartClip != null)
        {
            SoundManager.PlayFXSound(CombatStartSound);
            yield return new WaitForSeconds(combatStartClip.length);
        }

        // Change to Combat Background Music
        SoundManager.ChangeBackground(CombatSound);
    }

    /// <summary>
    /// Resets combat-related data to prepare for a new combat encounter.
    /// </summary>
    private void Reset()
    {
        RoundCounter = 0;

        Player = null;
        CombatZone = null;
        turnQueue.Clear();

        //Reset loot
        ScrapLootForCurrentCombat = 0;
        ItemsLootForCurrentCombat.Clear();
        NewChipLootForCurrentCombat.Clear();

        // Restore the previous background sound
        SoundManager.ChangeBackground(previousSound);
    }
    #region Targeting
    /// <summary>
    /// Set the current target, managing visual feedback
    /// </summary>
    /// <param name="newTarget"></param>
    private void SetTarget(GameObject newTarget)
    {
        // If there's a previous target, remove its selection
        if (Target != null)
        {
            Enemy previousEnemy = Target.GetComponent<Enemy>();
            if (previousEnemy != null)
            {
                previousEnemy.IsTargeted = false;
            }
        }

        // Set the new target and highlight it
        Target = newTarget;
        Enemy newEnemy = Target.GetComponent<Enemy>();
        if (newEnemy != null)
        {
            newEnemy.IsTargeted = true;
        }
    }

    /// <summary>
    /// Method to select target by clicking on an enemy
    /// </summary>
    /// <param name="context"></param>
    private void OnSelectTarget(InputAction.CallbackContext context)
    {
        StartCoroutine(HandleTargetSelection());
    }
    private IEnumerator HandleTargetSelection()
    {
        yield return null; // Wait for UI to update this frame

        if (GameManager.Instance.CurrentGameMode != GameManager.GameMode.Combat)
            yield break;

        if (EventSystem.current.IsPointerOverGameObject())
            yield break;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                SetTarget(hit.collider.gameObject);
            }
        }
    }


    /// <summary>
    /// Method to cycle through combatEnemies using Tab key
    /// </summary>
    /// <param name="context"></param>
    private void OnCycleTarget(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.CurrentGameMode != GameManager.GameMode.Combat)
        {
            return;
        }

        if (CombatEnemies.Count == 0) 
            return;


        // Cycle to the next target
        currentTargetIndex = (currentTargetIndex + 1) % CombatEnemies.Count;

        SetTarget(EnemyManager.Instance.CombatEnemies[currentTargetIndex]);
    }
    #endregion

    void OnDisable()
    {
        cycleTargetAction.Disable();
        selectTargetAction.Disable();

        cycleTargetAction.performed -= OnCycleTarget;
        selectTargetAction.performed -= OnSelectTarget;
    }
}
