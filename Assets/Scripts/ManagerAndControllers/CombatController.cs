using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
[Serializable]
public class Combadant
{
    public GameObject combadant;
    public bool attacked;
}

public class CombatController : MonoBehaviour
{

    private int roundCounter;

    private string currentCombatant;

    private GameObject target;

    private int currentTargetIndex;

    [Header("Combadents")]
    public List<Combadant> Combadants;
    private int currentCombatantIndex;

    private GameObject player;    
    

    private GameObject endTurnButton;

    


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
                endTurnButton.GetComponent<Button>().interactable =true;

                // let player know its their turn
                player.GetComponent<PlayerController>().StartTurn();
            }
            else
            {
                endTurnButton.GetComponent<Button>().interactable = false;
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

        player = GameObject.FindGameObjectWithTag("Player");

        endTurnButton = GameObject.Find("BtnEndTurn");
        endTurnButton.GetComponent<Button>().onClick.AddListener(() => TurnUsed(player));

        CurrentCombatantIndex = 0;
        CurrentCombatant = "No Combat Yet";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Check if the current combatant has attacked.
    /// Move to the next combatant.
    /// If all combatants have attacked (end of list), reset for the next round.
    /// </summary>
    private void CheckIfAllAttacked()
    {
        foreach(Combadant combadant in Combadants)
        {
            if (!combadant.attacked)
                    return;            
        }
        NextRound();
    }

    /// <summary>
    /// Start Combat
    /// </summary>
    public void StartCombat()
    {
        GameManager.Instance.StartCombat();
        RoundCounter = 1;

        //Add player to Combadants
        Combadant player = new();
        player.combadant = GameObject.FindGameObjectWithTag("Player");
        player.attacked = false;
        Combadants.Add(player);


        //Set enemies to combat mode in combat zone
        foreach (GameObject combatEnemy in GameManager.Instance.enemyList)
        {
            Combadant enemies = new Combadant();
            enemies.combadant = combatEnemy;
            enemies.attacked = false;
            Combadants.Add(enemies);

            combatEnemy.GetComponent<Enemy>().InCombat = true;

            //A loop to grab every card in the combat enemy for dropping cards
            for(int i = 0; i < combatEnemy.GetComponent<Enemy>().dropedCards.Count; i++)
            {
                //Adds the player 
                //PlayerUIManager.Instance.AddChipChoices(combatEnemy.GetComponent<Enemy>().dropedCards[i]);
                GameObject.Find("PlayerUIManager").GetComponent<PlayerUIManager>().AddChipChoices(combatEnemy.GetComponent<Enemy>().dropedCards[i]);
            }
        }

        CurrentCombatantIndex = 0;

        //Play Start Combat
        SoundManager.PlayFXSound(SoundFX.BattleStart);
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
            gameObject.GetComponent<PlayerController>().EndTurn();

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

        foreach(NewChip newchip in GameManager.Instance.playerHand)
        {
            newchip.EndRound();
        }

        //Draws a card based on draws per turn
        GameManager.Instance.DrawChip(GameManager.Instance.drawsPerTurn);
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
        CurrentCombatantIndex = 0;
        CurrentCombatant = "No Combat Yet";

        endTurnButton.SetActive(false);

        //Clear enemy list, selection list in ui and short selection list
        GameManager.Instance.enemyList.Clear();
        // Notify the GameManager or other systems
        GameManager.Instance.EndCombat();

        //Card Pickups at combat end
        //PlayerUIManager.Instance.openDropUI();
        GameObject.Find("PlayerUIManager").GetComponent<PlayerUIManager>().openDropUI();
    }

    // Method to select target by clicking on an enemy
    private void OnSelectTarget(InputAction.CallbackContext context)
    {
        if (!GameManager.Instance.InCombat)
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

    // Method to cycle through enemies using Tab key
    private void OnCycleTarget(InputAction.CallbackContext context)
    {
        if (!GameManager.Instance.InCombat)
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
                previousEnemy.SetTarget(TargetingType.CombatController, false);
            }
        }

        // Set the new target and highlight it
        target = newTarget;
        Enemy newEnemy = target.GetComponent<Enemy>();
        if (newEnemy != null)
        {
            newEnemy.SetTarget(TargetingType.CombatController, true);
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

    void OnDisable()
    {
        cycleTargetAction.Disable();
        selectTargetAction.Disable();

        cycleTargetAction.performed -= OnCycleTarget;
        selectTargetAction.performed -= OnSelectTarget;
    }
}
