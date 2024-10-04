using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

// Controller for player this class is not the input class that is generated.
public class PlayerController : MonoBehaviour
{
    // Reference to inputAction class that is generated by unity.
    public PlayerInputActions playerInputActions;

    //Camera in the scene
    private Camera mainCamera;

    // The Select Action from inputAction class.
    private InputAction select;

    [SerializeField]
    private bool inCombat;

    // The Deselect Action from inputAction class.
    //private InputAction deSelect;

    // Reference to selected object in the scene that is moveable
    private GameObject MoveableObject;

    [Header("Player stats")]
    [SerializeField]
    private int health;
    private int maxHealth;
    [SerializeField]
    private int shield;
    [SerializeField]
    private int energy;
    private int maxEnergy;
    [SerializeField]
    private int scrap;

    [Header("Status Effects")]
    [Header("Buffs")]
    [SerializeField]
    private bool isGalvanized;
    [SerializeField]
    private int galvanizedStack;
    [SerializeField]
    private bool isPowered;
    [SerializeField]
    private int poweredStacks;

    [Header("DeBuffs")]
    [SerializeField]
    private bool isGunked;
    [SerializeField]
    private int gunkStacks;
    [SerializeField]
    private int amountOfTurnsGunkedLeft;
    [SerializeField]
    private bool isDrained;
    [SerializeField]
    private int drainedStacks;    
    [SerializeField]
    private bool isWornDown;
    [SerializeField]
    private int wornDownStacks;
    [SerializeField]
    private bool isJammed;
    [SerializeField]
    private int jammedStacks;

    [Header("Effects")]
    [SerializeField]
    private bool nextChipActivatesTwice;
    [SerializeField]
    private bool isImpervious;

    /// <summary>
    /// Returns if player is in combat.
    /// </summary>
    public bool InCombat
    {
        get { return inCombat; }
        set
        {
            inCombat = value;
        }
    }
    /// <summary>
    /// Returns PLayer Health
    /// </summary>
    public int Health
    {
        get { return health; }
        private set { 
            health = value;
            if(health > maxHealth)
                health = maxHealth;
        }
    }
    /// <summary>
    /// Player Shield amount
    /// </summary>
    public int Shield
    {
        get
        {
            return shield;
        }
        private set
        {
            shield = value;
        }
    }
    /// <summary>
    /// Returns PlayerEnergy
    /// </summary>
    public int Energy
    {
        get { return energy; }
        private set
        {
            energy = value;
        }
    }
    /// <summary>
    /// Player Scrap
    /// </summary>
    public int Scrap
    {
        get
        {
            return scrap;
        }
        set
        {
            scrap = value;
            if (scrap < 0) 
                scrap = 0;
        }
    }
    /// <summary>
    /// Returns if player is Galvanized.
    /// </summary>
    public bool IsGalvanized
    {
        get
        {
            return isGalvanized;
        }
        private set
        {
            isGalvanized = value;
        }
    }
    /// <summary>
    /// Returns how many stacks of Galvanized the player has.
    /// </summary>
    public int GalvanizedStack
    {
        get => galvanizedStack;
        set
        {
            galvanizedStack = value;
            if (galvanizedStack <= 0)
            {
                IsGalvanized = false;
                galvanizedStack = 0;
            }
            else
                IsGalvanized = true;
        }
    }
    /// <summary>
    /// Returns if player is Powered.
    /// </summary>
    public bool IsPowered { 
        get => isPowered; 
        private set => isPowered = value; 
    }
    /// <summary>
    /// Returns how many stacks of power the player has.
    /// </summary>
    public int PoweredStacks
    {
        get => poweredStacks;
        set
        {
            poweredStacks = value;
            if (poweredStacks <= 0)
            {
                IsPowered = false;
                poweredStacks = 0;
            }
            else
                IsPowered = true;
        }
    }
    /// <summary>
    /// Returns if player is gunked.
    /// </summary>
    public bool IsGunked {
        get => isGunked;
        private set {
         isGunked = value;
        }
    }
    /// <summary>
    /// Returns how many stacks of gunk the player has.
    /// </summary>
    public int GunkStacks
    {
        get => gunkStacks;
        set
        {
            gunkStacks = value;

            if (gunkStacks >= 3)
            {
                IsGunked = true;
                gunkStacks = 0;
                AmountOfTurnsGunkedLeft = 1;
            }
        }
    }
    /// <summary>
    /// Use this to prevent player from attacking when gunked.
    /// </summary>
    public int AmountOfTurnsGunkedLeft
    {
        get => amountOfTurnsGunkedLeft;
        private set
        {
            amountOfTurnsGunkedLeft = value;
            if (amountOfTurnsGunkedLeft <= 0)
                IsGunked = false;
        }
    }
    /// <summary>
    /// Returns how many stacks of drained the player has.
    /// </summary>
    public int DrainedStacks
    {
        get
        {
            return drainedStacks;
        }
        set
        {
            drainedStacks = value;
            if (drainedStacks <= 0)
            {
                IsDrained = false;
                drainedStacks = 0;
            }
            else
                IsDrained = true;
        }
    }
    /// <summary>
    /// Returns if the player is drained.
    /// </summary>
    public bool IsDrained { 
        get => isDrained;
        private set => isDrained = value; 
    }
    /// <summary>
    /// Returns if the player is in worndown state.
    /// </summary>
    public bool IsWornDown { 
        get => isWornDown;
        private set => isWornDown = value; 
    }
    /// <summary>
    /// Returns how many stacks of worn down the player has.
    /// </summary>
    public int WornDownStacks
    {
        get
        {
            return wornDownStacks;
        }
        set
        {
            wornDownStacks = value;
            if (wornDownStacks <= 0)
            {
                IsWornDown = false;
                wornDownStacks = 0;
            }
            else
                IsWornDown = true;
        }
    }
    /// <summary>
    /// Returns if player is Jammed.
    /// </summary>
    public bool IsJammed { 
        get => isJammed;
        private set => isJammed = value; 
    }           
    /// <summary>
    /// Returns how many stacks of jammed the palyer has.
    /// </summary>
    public int JammedStacks
    {
        get
        {
            return jammedStacks;
        }
        set
        {
            jammedStacks = value;
            if (jammedStacks <= 0)
            {
                IsJammed = false;
                jammedStacks = 0;
            }
            else
                IsJammed = true;
        }
    }   
    /// <summary>
    /// Used to apply effect of activing a chip twice.
    /// </summary>
    public bool NextChipActivatesTwice
    {
        get
        {
            return nextChipActivatesTwice;
        }
        private set
        {
            nextChipActivatesTwice = value;
        }
    }
    /// <summary>
    /// Used to apply effect to not take damage.
    /// </summary>
    public bool IsImpervious
    {
        get
        {
            return isImpervious;
        }
        private set
        {
            isImpervious = value;
        }
    }    

    // Awake is called when instance is being loaded
    void Awake()
    {
        // assign player Input class
        playerInputActions = new PlayerInputActions();

        // Automatically finds the camera
        mainCamera = Camera.main;

    }
    void Start()
    {
        //assign Object To Move if empty
        if(MoveableObject == null)
            MoveableObject = GameObject.FindGameObjectWithTag("Player");

        Initialize();
    }
    /// <summary>
    /// Initialize Player
    /// </summary>
    void Initialize()
    {
        Health = 50;
        maxHealth = 50;
        Energy = 50;
        maxEnergy = 50;
        Scrap = 100;

        //loads abilities from folder
        //abilities.AddRange(Resources.LoadAll<Ability>("Abilities"));
    }
    /// <summary>
    /// Enables
    /// </summary>
    private void OnEnable()
    {
        select = playerInputActions.Player.Select;
        select.Enable();
        select.performed += OnClick;

        //deSelect = playerInputActions.Player.DeSelect;
        //deSelect.Enable();
        //deSelect.performed += OnDeselect;
    }
    /// <summary>
    /// Disables
    /// </summary>
    private void OnDisable()
    {
        select.Disable();
        select.performed -= OnClick;

        //deSelect.Disable();
        //deSelect.performed -= OnDeselect;
    }

    /// <summary>
    /// On click is run everytime the user clicks into the scene.
    /// Using Physics raycast.
    /// Depends on the result it will either:
    /// Assign the selectedObject .
    /// Move the selectedObejct to the position on Ground.
    /// </summary>
    private void OnClick(InputAction.CallbackContext context)
    {
        if (!GameManager.Instance.InCombat)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {

                if (MoveableObject != null && hit.collider.CompareTag("Ground"))
                {
                    NavMeshAgent agent = MoveableObject.GetComponent<NavMeshAgent>();
                    if (agent != null)
                        agent.SetDestination(hit.point);
                }
            }
        }
    }
    /// <summary>
    /// heal player by amount
    /// </summary>
    /// <param name="amountHeal"></param>
    public void Heal(int amountHeal)
    {
            //Heal the player using the getter and setter value
            //This will also check for over healing and correct
            Health += amountHeal;

        //Debug log healing
        Debug.Log("Health Restored: " + health);
        
    }
    /// <summary>
    /// Heal to max hp.
    /// </summary>
    public void FullHeal()
    {
        Health = maxHealth;
    }
    /// <summary>
    /// Give player shield.
    /// </summary>
    /// <param name="shieldAmount"></param>
    public void ApplyShield(int shieldAmount)
    {
        //Restore Shield
        Shield += shieldAmount;

        Debug.Log("Shield Restored: " + shield);
    }
    /// <summary>
    /// Give player energy.
    /// </summary>
    /// <param name="energyAmount"></param>
    public void RecoverEnergy(int energyAmount)
    {
            //Restore energy this will inside of the getter and setter variable check if we are over energizing and correct
            Energy += energyAmount;

        Debug.Log("Energy Restored: " + energy);
    }
    /// <summary>
    /// Recover energy to max.
    /// </summary>
    public void RecoverFullEnergy()
    {
        Energy = maxEnergy;
    }
    /// <summary>
    /// Deal Damage to player.
    /// </summary>
    /// <param name="damage">Amount of Damage as Int.</param>
    public void TakeDamage(int damage)
    {
        //if Impervious
        if (IsImpervious)
        {            
        }
        else
        {
            // if has shield
            if (Shield > 0)
            {
                if (damage >= Shield)
                {
                    damage -= Shield;
                    Shield = 0;
                    Debug.Log(name + "Shield destroyed.");
                }
                else
                {
                    // Reduce the shield by the damage amount
                    Shield -= damage;
                    // No remaining damage to apply to HP
                    damage = 0;
                }
            }
            Health = Health - damage;
        }
    }
    /// <summary>
    /// Apply Skill Effect
    /// </summary>
    /// <param name="effect"></param>
    public void ApplyEffect(Effects.Effect effect)
    {
        switch (effect)
        {
            case Effects.Effect.Motivation:
                NextChipActivatesTwice = true;
                break;
            case Effects.Effect.Impervious:
                IsImpervious = true;
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// Apply Buff to Player
    /// </summary>
    /// <param name="buffToApply"></param>
    /// <param name="buffStacks"></param>
    public void ApplyEffect(Effects.Buff buffToApply, int buffStacks)
    {
        switch (buffToApply)
        {
            case Effects.Buff.Galvanize:
                GalvanizedStack += buffStacks;
                break;
            case Effects.Buff.Power:
                PoweredStacks += buffStacks;
                break;
        }
    }
    /// <summary>
    /// Apply Debuff to player.
    /// </summary>
    /// <param name="deBuffToApply"></param>
    /// <param name="deBuffStacks"></param>
    public void ApplyEffect(Effects.Debuff deBuffToApply, int deBuffStacks)
    {
        switch (deBuffToApply)
        {
            case Effects.Debuff.Gunked:
                GunkStacks += deBuffStacks;
                break;
            case Effects.Debuff.Drained:
                DrainedStacks += deBuffStacks;
                break;
            case Effects.Debuff.WornDown:
                WornDownStacks += deBuffStacks;
                break;
            case Effects.Debuff.Jam:
                JammedStacks += deBuffStacks;
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// Remove an affect from being active.
    /// </summary>
    /// <param name="effect"></param>
    public void RemoveEffect(Effects.Effect effect)
    {
        switch (effect)
        {
            case Effects.Effect.Motivation:
                NextChipActivatesTwice = false;
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// Returns the scrap stolen or whats left.
    /// </summary>
    /// <param name="amount">the amount of scrap want to steal</param>
    /// <returns></returns>
    public int StealScrap(int amount)
    {
        if (Scrap < amount)
        {
            int scrapleft = Scrap;
            Scrap -= amount;
            return scrapleft;
        }
        else
        {
            Scrap = Scrap - amount;
            return amount;
        }
    }
    /// <summary>
    /// Called when round ends to apply buffs or debuffs.
    /// Buffs Stack don't go away.
    /// Debuffs Stack go away every round.
    /// </summary>
    public void RoundEnd()
    {       
        if (galvanizedStack > 0)
        {
            ApplyShield(galvanizedStack);            
        }
        if (isGunked)
        {
            AmountOfTurnsGunkedLeft--;
        }
        drainedStacks--;
        gunkStacks--;
        jammedStacks--;
        wornDownStacks--;

    }

    /// <summary>
    /// Spend energy for card or ability
    /// </summary>
    /// <param name="loss"></param>
    public void PlayedCardOrAbility(int loss)
    {
        //If energy - loss is greater then 0 or equal to 0 then continue
        if(Energy - loss >= 0)
        {
            Energy -= loss;
        }
        //If not then don't be negative
        else
        {
            Energy = 0;
        }
    }
}
