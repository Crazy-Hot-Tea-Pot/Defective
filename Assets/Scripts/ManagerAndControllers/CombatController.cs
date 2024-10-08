using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class Combadant
{
    public GameObject combadant;
    public bool attacked;
}

public class CombatController : MonoBehaviour
{

    [SerializeField]
    private int roundCounter;

    [SerializeField]
    private string currentCombatant;

    [SerializeField]
    private GameObject target;

    private int currentCombatantIndex;
    private int currentTargetIndex;

    public List<Combadant> Combadants = new();


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

    private InputAction selectTargetAction;
    private InputAction cycleTargetAction;

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
        CurrentCombatantIndex = 0;
        CurrentCombatant = "No Combat Yet";
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.InCombat)
        {
            if (AreEnemiesRemaining())
                CheckIfAllAttacked();
            else
                EndCombat();
        }        
    }
    /// <summary>
    /// Check if the current combatant has attacked.
    /// Move to the next combatant.
    /// If all combatants have attacked (end of list), reset for the next round.
    /// </summary>
    private void CheckIfAllAttacked()
    {
        if (Combadants[currentCombatantIndex].attacked)
        {
            currentCombatantIndex++;

            if (currentCombatantIndex >= Combadants.Count)
            {
                NextRound();
            }
        }
    }
    /// <summary>
    /// Start Combat
    /// </summary>
    public void StartCombat()
    {
        RoundCounter = 1;

        //this is a test add to the list.
        Combadant playertest = new();
        playertest.combadant = GameObject.FindGameObjectWithTag("Player");
        playertest.attacked = false;
        Combadants.Add(playertest);

        //Set enemies to combat mode in combat zone
        foreach (GameObject combatEnemy in GameManager.Instance.enemyList)
        {
            Combadant test = new Combadant();
            test.combadant = combatEnemy;
            test.attacked = false;
            Combadants.Add(test);

            combatEnemy.GetComponent<Enemy>().InCombat = true;
        }        
    }
    /// <summary>
    /// When used an action or attack and change status in combat.
    /// </summary>
    /// <param name="gameObject"></param>
    public void TurnUsed(GameObject gameObject)
    {
        foreach (var combadent in Combadants)
        {
            if(combadent.combadant.name == gameObject.name)
            {
                combadent.attacked = true;
            }
        }
    }
    /// <summary>
    /// Check if I can make an action in this turn.
    /// Check if it's the current combatant's turn in the list
    /// If it's not this combatant's turn, return false
    /// </summary>
    /// <param name="gameObject"></param>
    public bool CanIMakeAction(GameObject gameObject)
    {        
        if (Combadants[currentCombatantIndex].combadant == gameObject)
        {
            return !Combadants[currentCombatantIndex].attacked;
        }
        
        return false;
    }
    /// <summary>
    /// Check if enemies are still in combat zone.
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
    /// Call this after enemy death to remove enemy from list of combadants"
    /// </summary>
    /// <param name="combadant"></param>
    public void RemoveCombadant(GameObject combadant)
    {
        Combadant combatantToRemove = Combadants.Find(c => c.combadant == combadant);
        if (combatantToRemove != null)
        {
            Combadants.Remove(combatantToRemove);
            Debug.Log($"{combadant.name} has been removed from combat.");
        }
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

        currentCombatantIndex = 0;


        foreach (Combadant combadant in Combadants)
        {
            combadant.attacked = false;
            if (combadant.combadant.tag == "Player")
                combadant.combadant.GetComponent<PlayerController>().RoundEnd();
            else if(combadant.combadant.tag=="Enemy")
                combadant.combadant.GetComponent<Enemy>().RoundEnd();
        }
    }
    /// <summary>
    /// End combat.
    /// leanup and reset
    /// </summary>
    public void EndCombat()
    {
        Debug.Log("Combat has ended. Only the player remains.");

        Combadants.Clear();
        roundCounter = 0;
        currentCombatantIndex = 0;
        CurrentCombatant = "No Combat Yet";

        // Notify the GameManager or other systems
        GameManager.Instance.EndCombat();
    }

    // Method to select target by clicking on an enemy
    private void OnSelectTarget(InputAction.CallbackContext context)
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Check if the clicked object is an enemy combatant
            Combadant clickedCombatant = hit.collider.GetComponent<Combadant>();
            if (clickedCombatant != null && clickedCombatant.combadant.CompareTag("Enemy"))
            {
                SetTarget(clickedCombatant.combadant);
            }
        }
    }

    // Method to cycle through enemies using Tab key
    private void OnCycleTarget(InputAction.CallbackContext context)
    {
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

    void OnDisable()
    {
        cycleTargetAction.Disable();
        selectTargetAction.Disable();

        cycleTargetAction.performed -= OnCycleTarget;
        selectTargetAction.performed -= OnSelectTarget;
    }
}
