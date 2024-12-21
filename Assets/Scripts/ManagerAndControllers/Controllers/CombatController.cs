using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CombatController : MonoBehaviour
{
    public class Combadant
    {
        public GameObject combadant;
        public bool attacked;
    }

    public List<Combadant> Combadants
    {
        get
        {
            return combadants;
        }
        set
        {
            combadants = value;
        }
    }
    /// <summary>
    /// Amount of rounds that has passed.
    /// </summary>
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

    // Tracks the index of the combatant whose turn it is
    public int CurrentCombatantIndex
    {
        get
        {
            return currentCombatantIndex;
        }
        private set
        {
            currentCombatantIndex = value;

            if (currentCombatantIndex >= Combadants.Count)
                currentCombatantIndex = Combadants.Count - 1;

            if(Combadants.Count != 0)
                CurrentCombatant = Combadants[currentCombatantIndex].combadant.name;

        }
    }

    /// <summary>
    /// Name of Current person turn.
    /// </summary>
    public string CurrentCombatant
    {
        get
        {
            return currentCombatant;
        }
        private set
        {
            currentCombatant = value;

            if(currentCombatant == "Player")
            {
                //Activate Button so player can end turn
                UiManager.Instance.EndTurnButtonInteractable(true);                                

                // let player know its their turn
                player.GetComponent<PlayerController>().StartTurn();
            }
            else
            {
                UiManager.Instance.EndTurnButtonInteractable(false);                
            }            
        }
    }

    /// <summary>
    /// Current Target
    /// </summary>
    public GameObject Target
    {
        get
        {
            return target;
        }
        private set
        {
            target = value;
        }
    }

    // Reference to inputAction class that is generated by unity.
    public PlayerInputActions playerInputActions;

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

    private List<Combadant> combadants = new();
    private InputAction selectTargetAction;
    private InputAction cycleTargetAction;
    private int roundCounter;
    private string currentCombatant;
    private GameObject target;
    private int currentTargetIndex;
    private int currentCombatantIndex;
    private GameObject player;
    private int scrapLootForCurrentCombat;


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
        // assign player Input class
        playerInputActions = new PlayerInputActions();        
    }

    // Start is called before the first frame update
    void Start()
    {        

        player = GameObject.FindGameObjectWithTag("Player");             

        CurrentCombatantIndex = 0;
        CurrentCombatant = "No Combat Yet";
    }

    /// <summary>
    /// Start Combat
    /// </summary>
    public void StartCombat()
    {
        //Reset loot
        ScrapLootForCurrentCombat = 0;
        ItemsLootForCurrentCombat.Clear();
        NewChipLootForCurrentCombat.Clear();

        GameManager.Instance.StartCombat();
        RoundCounter = 1;

        //Add player to Combadants
        Combadant player = new();
        player.combadant = GameObject.FindGameObjectWithTag("Player");
        player.attacked = false;
        Combadants.Add(player);

        //Set combatEnemies to combat mode in combat zone
        foreach (GameObject combatEnemy in EnemyManager.Instance.CombatEnemies)
        {
            Combadant enemies = new Combadant();
            enemies.combadant = combatEnemy;
            enemies.attacked = false;
            Combadants.Add(enemies);

            combatEnemy.GetComponent<Enemy>().InCombat = true;

            combatEnemy.GetComponent<Enemy>().CombatStart();

        }

        CurrentCombatantIndex = 0;

        //Play Start Combat
        SoundManager.PlayFXSound(SoundFX.BattleStart);

        UiManager.Instance.EndTurnButtonVisibility(true);        
    }

    /// <summary>
    /// When used an action or attack and change status in combat.
    /// </summary>
    /// <param name="gameObject"></param>
    public void TurnUsed(GameObject gameObject)
    {
        //Call End turn on objected
        if (gameObject.tag == "Enemy")
            gameObject.GetComponent<Enemy>().EndTurn();
        else
        {
            gameObject.GetComponent<PlayerController>().EndTurn();
            UiManager.Instance.EndTurnButtonVisibility(true);
        }

        foreach (var combadent in Combadants)
        {
            //Check if gameobject matches
            if(combadent.combadant == gameObject)
            {
                combadent.attacked = true;
                CurrentCombatantIndex++;
            }
        }

        if (AreEnemiesRemaining())
            CheckIfAllAttacked();
        else
            EndCombat();
    }

    /// <summary>
    /// Check if I can make an action in this turn.
    /// Check if it's the current combatant's turn in the list
    /// If it's not this combatant's turn, return false
    /// </summary>
    /// <param name="gameObject"></param>
    public bool CanIMakeAction(GameObject gameObject)
    {        
        if (Combadants[CurrentCombatantIndex].combadant == gameObject)
        {
            return !Combadants[CurrentCombatantIndex].attacked;
        }
        
        return false;
    }

    /// <summary>
    /// Check if combatEnemies are still in combat zone.
    /// </summary>
    /// <returns></returns>
    public bool AreEnemiesRemaining()
    {
        foreach (Combadant combadant in Combadants)
        {
            if (combadant.combadant.CompareTag("Enemy"))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Call this after enemy death to remove enemy from combadant and add their loot.
    /// </summary>
    /// <param name="combadant">the enemy.</param>
    /// <param name="scrapLoot">amount of loot enemy gives.</param>
    /// <param name="chipLoot">chips loot enemy gives</param>
    /// <param name="itemLoot">item loot the enemy gives</param>
    public void RemoveCombadant(GameObject combadant,int scrapLoot, List<NewChip> chipLoot, List<Item> itemLoot)
    {
        // Add loot safely by cloning
        ScrapLootForCurrentCombat += scrapLoot;

        //Add items to loot
        foreach (var item in itemLoot)
            ItemsLootForCurrentCombat.Add(Instantiate(item));
        //Add chips to loot
        foreach (var chip in chipLoot)
            NewChipLootForCurrentCombat.Add(Instantiate(chip));

        //Remove from CombatList
        Combadant combatantToRemove = Combadants.Find(c => c.combadant == combadant);
        if (combatantToRemove != null)
        {
            Combadants.Remove(combatantToRemove);
            Debug.Log($"{combadant.name} has been removed from combat.");
        }

        if (!AreEnemiesRemaining())
            EndCombat();
    }

    /// <summary>
    /// End combat.
    /// leanup and reset
    /// </summary>
    public void EndCombat()
    {

        Combadants.Clear();
        roundCounter = 0;
        CurrentCombatantIndex = 0;
        CurrentCombatant = "No Combat Yet";
        Target = null;

        UiManager.Instance.EndTurnButtonVisibility(false);        

        // Notify the GameManager or other systems
        GameManager.Instance.EndCombat();

        if (ScrapLootForCurrentCombat > 0 ||
            ItemsLootForCurrentCombat != null ||
            NewChipLootForCurrentCombat != null)
        {
            GameManager.Instance.UpdateGameMode(GameManager.GameMode.CombatLoot);
            UiManager.Instance.SendLoot(ScrapLootForCurrentCombat, ItemsLootForCurrentCombat, NewChipLootForCurrentCombat);
        }
    }
    
    /// <summary>
    /// Add enemy to combat 
    /// </summary>
    /// <param name="newCombadant"></param>
    public void AddEnemyToCombat(GameObject newCombadant)
    {
        Combadants.Add(new Combadant { combadant = newCombadant, attacked = false });
    }

    /// <summary>
    /// Set the current target, managing visual feedback
    /// </summary>
    /// <param name="newTarget"></param>
    private void SetTarget(GameObject newTarget)
    {
        // If there's a previous target, remove its selection
        if (target != null)
        {
            Enemy previousEnemy = target.GetComponent<Enemy>();
            if (previousEnemy != null)
            {
                previousEnemy.IsTargeted = false;
            }
        }

        // Set the new target and highlight it
        target = newTarget;
        Enemy newEnemy = target.GetComponent<Enemy>();
        if (newEnemy != null)
        {
            newEnemy.IsTargeted = true;
        }
    }

    // Method to select target by clicking on an enemy
    private void OnSelectTarget(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.CurrentGameMode != GameManager.GameMode.Combat)
        {
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;


        if (Physics.Raycast(ray, out hit))
        {
            // Check if the clicked object is an enemy combatant
            if (hit.collider.CompareTag("Enemy"))
            {
                SetTarget(hit.collider.gameObject);
            }
        }
    }

    // Method to cycle through combatEnemies using Tab key
    private void OnCycleTarget(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.CurrentGameMode != GameManager.GameMode.Combat)
        {
            return;
        }

        if (Combadants.Count == 0) return;

        int startIndex = currentTargetIndex;
        do
        {
            currentTargetIndex = (currentTargetIndex + 1) % Combadants.Count;
            if (Combadants[currentTargetIndex].combadant.CompareTag("Enemy"))
            {
                SetTarget(Combadants[currentTargetIndex].combadant);
                return;
            }
        } while (currentTargetIndex != startIndex); // Loop until we come back to the start
    }

    /// <summary>
    /// Move to next round.
    /// Increase round counter
    /// Reset the current combatant index to the start of the list
    /// reset all objects in scene so they can attack.
    /// </summary>
    private void NextRound()
    {
        RoundCounter++;

        CurrentCombatantIndex = 0;


        foreach (Combadant combadant in Combadants)
        {
            combadant.attacked = false;
            if (combadant.combadant.tag == "Player")
            {
                combadant.combadant.GetComponent<PlayerController>().RoundEnd();
            }
            else if (combadant.combadant.tag == "Enemy")
                combadant.combadant.GetComponent<Enemy>().RoundEnd();
        }

        foreach (NewChip newchip in ChipManager.Instance.PlayerHand)
        {
            newchip.EndRound();
        }

        ChipManager.Instance.RefreshPlayerHand();

        UiManager.Instance.EndTurnButtonVisibility(true);
    }

    /// <summary>
    /// Check if the current combatant has attacked.
    /// Move to the next combatant.
    /// If all combatants have attacked (end of list), reset for the next round.
    /// </summary>
    private void CheckIfAllAttacked()
    {
        foreach (Combadant combadant in Combadants)
        {
            if (!combadant.attacked)
                return;
        }
        NextRound();
    }

    void OnDisable()
    {
        cycleTargetAction.Disable();
        selectTargetAction.Disable();

        cycleTargetAction.performed -= OnCycleTarget;
        selectTargetAction.performed -= OnSelectTarget;
    }
}
