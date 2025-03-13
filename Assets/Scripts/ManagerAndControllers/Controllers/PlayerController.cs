using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static Effects;

// Controller for Player this class is not the input class that is generated.
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private PlayerUiController uiController;

    //Camera in the scene
    private Camera mainCamera;

    // The Select Action from inputAction class.
    private InputAction select;

    [Header("Needed Stuff")]    
    public PlayerInputActions playerInputActions;
    public NavMeshAgent agent;
    public Animator animator;
    public GameObject RipplePrefab;
    public GameObject RippleRunPrefab;
    public GameObject CallScreen;


    [Header("Player stats")]
    #region PlayerStats 
    private int health;
    /// <summary>
    /// Returns PLayer HealthBar
    /// </summary>
    public int Health
    {
        get { return health; }
        private set
        {
            health = value;

            UiManager.Instance.UpdateHealth(Health, MaxHealth);

            if (health > maxHealth)
                health = maxHealth;
            else if (health <= 0)
            {
                health = 0;
                PlayerDie();
            }
        }
    }
    private int maxHealth;
    /// <summary>
    /// Returns max HealthBar
    /// </summary>
    public int MaxHealth
    {
        get { return maxHealth; }
        private set
        {
            maxHealth = value;
        }
    }
    private int shield;
    /// <summary>
    /// Player ShieldBar amount
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

            if (shield > maxShield)
                maxShield = value;

            if (shield <= 0)
            {
                shield = 0;
                maxShield = 100;
            }

            UiManager.Instance.UpdateShield(Shield, MaxShield);
        }
    }
    private int maxShield=100; 
    
    /// <summary>
    /// Max amount of ShieldBar currently.
    /// </summary>
    public int MaxShield
    {
        get
        {
            return maxShield;
        }
        set
        {
            maxShield = value;
        }
    }
    private int energy;
    /// <summary>
    /// Returns PlayerEnergy
    /// </summary>
    public int Energy
    {
        get { return energy; }
        private set
        {
            energy = value;

            if (energy > maxEnergy)
                energy = maxEnergy;
            else if (energy <= 0)
                energy = 0;

            UiManager.Instance.UpdateEnergy(Energy, MaxEnergy);

            UiManager.Instance.UpdateGearButtonsStates(energy);
        }
    }
    private readonly int maxEnergy=50;
    /// <summary>
    /// Returns max energy
    /// </summary>
    public int MaxEnergy
    {
        get { return maxEnergy; }
    }
    private int scrap;
    /// <summary>
    /// Player Scrap
    /// </summary>
    public int Scrap
    {
        get
        {
            return scrap;
        }
        private set
        {
            scrap = value;
            if (scrap < 0)
                scrap = 0;
        }
    }
    #endregion

    [Header("Player Speed")]
    public float walkSpeed = 3.5f;
    public float runSpeed = 7.0f;
    // Speed threshold to detect running
    public float runThreshold = 5.0f;
    // Max time interval for double-click detection
    public float doubleClickTime = 0.3f; 
    private float lastClickTime;
    public SoundFX SoundWhenPlayerClicks;

    private Coroutine rotateCoroutine;


    [Header("Status Effects")]

    #region Effects

    [SerializeField]
    private List<StatusEffect> listOfActiveEffects = new List<StatusEffect>();

    public List<StatusEffect> ListOfActiveEffects
    {
        get
        {
            return listOfActiveEffects;
        }
        private set
        {
            listOfActiveEffects = value;                       
        }
    }   

    #region Buffs   
    /// <summary>
    /// Returns if Player is Galvanized.
    /// </summary>
    public bool IsGalvanized
    {
        get
        {
           if(GalvanizedStacks > 0)
                return true;
           else
                return false;
        }
    }
    /// <summary>
    /// How many GalvanizedStacks
    /// </summary>
    public int GalvanizedStacks
    {
        get
        {
            return GetStacks(Effects.Buff.Galvanize);
        }
    }  
    /// <summary>
    /// Returns if Player is Powered.
    /// </summary>
    public bool IsPowered
    {
        get
        {
            if (PoweredStacks > 0)
                return true;
            else
                return false;
        }
    }
    /// <summary>
    /// Returns how many stacks of power the Player has.
    /// </summary>
    public int PoweredStacks
    {
        get
        {
            return GetStacks(Effects.Buff.Power);
        }
    }
    #endregion

    #region Debuffs    
    /// <summary>
    /// Returns if the Player is drained.
    /// </summary>
    public bool IsDrained
    {
        get
        {
            if (DrainedStacks > 0)
                return true;
            else
                return false;
        }
    }
    /// <summary>
    /// Returns how many stacks of drained the Player has.
    /// </summary>
    public int DrainedStacks
    {
        get
        {
           return GetStacks(Effects.Debuff.Drained);
        }
    }
    /// <summary>
    /// Returns if the Player is in worndown state.
    /// </summary>
    public bool IsWornDown
    {
        get
        {
            if(WornDownStacks>0)
                return true;
            else
                return false;
        }
    }
    /// <summary>
    /// Returns how many stacks of worn down the Player has.
    /// </summary>
    public int WornDownStacks
    {
        get
        {
            return GetStacks(Effects.Debuff.WornDown);
        }
    }
    /// <summary>
    /// Returns if Player is Jammed.
    /// </summary>
    public bool IsJammed
    {
        get
        {
            if(JammedStacks > 0)
                return true;
            else
                return false;
        }
    }

    /// <summary>
    /// Returns how many stacks of jammed the palyer has.
    /// </summary>
    public int JammedStacks
    {
        get
        {
            return GetStacks(Effects.Debuff.Jam);
        }
    }

    /// <summary>
    /// Returns if player is Redirected
    /// </summary>
    public bool IsRedirected
    {
        get
        {
            if (RedirectedStacks > 0)
            {                
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    public int RedirectedStacks
    {
        get
        {
            return GetStacks(Effects.Debuff.Redirect);
        }
    }
    #endregion

    #region Effects
    /// <summary>
    /// Used to apply effect of activing a chip twice.
    /// </summary>
    public bool IsMotivated
    {
        get
        {
            return ListOfActiveEffects.Any(e => e.SpecialEffect.Equals(Effects.SpecialEffects.Motivation));
        }
    }
    /// <summary>
    /// Used to apply effect to not take Damage.
    /// </summary>
    public bool IsImpervious
    {
        get
        {
            return ListOfActiveEffects.Any(e => e.SpecialEffect.Equals(Effects.SpecialEffects.Impervious));
        }
    }
    #endregion

    #endregion
    [Header("Sound")]
    public SoundFX DamageTakenSound;
    public SoundFX GainShieldSound;
    public SoundFX EnterLevelSound;
    // Awake is called when instance is being loaded
    void Awake()
    {
        // assign Player Input class
        playerInputActions = new PlayerInputActions();

        // Automatically finds the camera
        mainCamera = Camera.main;

        agent = GetComponent<NavMeshAgent>();

        GameManager.Instance.OnEndCombat += TriggerPassiveEffectsOnCombatEnd;
    }

    void Start()
    {
       
        agent.updateRotation = false;
        agent.speed = walkSpeed;

        Initialize();

        //CharacterSpeak("Scanning Area, I seem to be in the *InsertSceneName*", false, 0.5f,2f); 
        SoundManager.PlayFXSound(EnterLevelSound);
    }

    /// <summary>
    /// We will do animations in Late Update.
    /// </summary>
    void LateUpdate()
    {
        // Determine if Player is running or walking based on the agent's current speed
        bool isRunning = agent.velocity.magnitude > runThreshold;
        bool isWalking = agent.velocity.magnitude > 0.1f && !isRunning;

        // Set animator bools based on detected movement state
        animator.SetBool("IsWalking", isWalking);
        animator.SetBool("IsRunning", isRunning);

        if(GameManager.Instance.CurrentGameMode==GameManager.GameMode.Combat)
            SmoothRotateTowardsTarget();
    }    

    /// <summary>
    /// Initialize Player
    /// </summary>
    void Initialize()
    {
        health = DataManager.Instance.CurrentGameData.Health;
        MaxHealth = DataManager.Instance.CurrentGameData.MaxHealth;
        Scrap = DataManager.Instance.CurrentGameData.Scraps;
        Energy = MaxEnergy;       
    }

    /// <summary>
    /// Enables
    /// </summary>
    void OnEnable()
    {
        select = playerInputActions.Player.Select;
        select.Enable();
        select.performed += OnClick;
    }

    /// <summary>
    /// Disables
    /// </summary>
    void OnDisable()
    {
        select.Disable();
        select.performed -= OnClick;
    }

    #region PlayerHealth

    /// <summary>
    /// heal Player by amount
    /// </summary>
    /// <param name="amountHeal"></param>
    public void Heal(int amountHeal)
    {
        //Heal the Player using the getter and setter value
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
    /// Changes the max HealthBar value
    /// </summary>
    /// <param name="amount"></param>
    public void UpgradeMaxHealth(int amount)
    {
        MaxHealth += amount;
    }

    /// <summary>
    /// Deal Damage to Player.
    /// </summary>
    /// <param name="damage">Amount of Damage as Int.</param>
    public void DamagePlayerBy(int damage)
    {
        //if Impervious
        if (IsImpervious)
        {
            return;
        }
        else
        {
            int modifiedDamage = damage;

            if (IsWornDown)
            {
                modifiedDamage = Mathf.CeilToInt(damage * 1.3f);
            }
            // if has ShieldAmount
            if (Shield > 0)
            {
                if (modifiedDamage >= Shield)
                {
                    modifiedDamage -= Shield;
                    Shield = 0;
                    Debug.Log(name + "Shield destroyed.");
                }
                else
                {
                    // Reduce the ShieldAmount by the Damage amount
                    Shield -= modifiedDamage;
                    Debug.Log("Shield " + Shield + " Took Damage" + modifiedDamage);
                    // No remaining Damage to apply to HP
                    modifiedDamage = 0;
                }
            }
            Health = Health - modifiedDamage;


            //Play Sound
            SoundManager.PlayFXSound(DamageTakenSound, this.transform);
        }
    }

    #endregion

    #region PlayerShield

    /// <summary>
    /// Give Player baseShieldAmount.
    /// </summary>
    /// <param name="shieldAmount"></param>
    public void ApplyShield(int shieldAmount)
    {
        //Restore ShieldBar
        Shield += shieldAmount;

    }

    #endregion

    #region PlayerEnergy

    /// <summary>
    /// Give Player energy.
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
    /// Will try to spend Energy if not enough energy return false.
    /// </summary>
    /// <param name="energyAmount"></param>
    /// <returns></returns>
    public bool SpendEnergy(int energyAmount)
    {
        if(energyAmount>Energy)
            return false;
        
        Energy-= energyAmount;
        return true;
    }

    #endregion

    #region EffectsMethods

    // Add Effect Methods

    #region AddEffects
    /// <summary>
    /// Add buff to Player.
    /// </summary>
    /// <param name="buff"></param>
    /// <param name="stacks"></param>
    public void AddEffect(Effects.Buff buff, int stacks)
    {
        AddOrUpdateEffect(buff, stacks);
    }
    /// <summary>
    /// Add Debuff to Player
    /// </summary>
    /// <param name="debuff"></param>
    /// <param name="stacks"></param>
    public void AddEffect(Effects.Debuff debuff, int stacks)
    {
        AddOrUpdateEffect(debuff, stacks);
    }
    /// <summary>
    /// Add Special effect to Player
    /// </summary>
    /// <param name="specialEffect"></param>
    public void AddEffect(Effects.SpecialEffects specialEffect)
    {
        //if (!ListOfActiveEffects.Any(e => e.Effect.Equals(specialEffect)))
        //{
        //    ListOfActiveEffects.Add(new Effects.StatusEffect(specialEffect, 0));
        //}
        AddOrUpdateEffect(specialEffect, 1);
    }

    /// <summary>
    /// Add Special Effect to Player
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="effect"></param>
    /// <param name="stacks"></param>
    private void AddOrUpdateEffect<T>(T effect, int stacks) where T : Enum
    {
        for (int i = 0; i < ListOfActiveEffects.Count; i++)
        {
            var statusEffect = ListOfActiveEffects[i];

            if ((statusEffect.BuffEffect.Equals(effect) && effect is Effects.Buff) ||
                (statusEffect.DebuffEffect.Equals(effect) && effect is Debuff) ||
                (statusEffect.SpecialEffect.Equals(effect) && effect is SpecialEffects))
            {
                statusEffect.StackCount += stacks;
                ListOfActiveEffects[i] = statusEffect;                
                return;
            }
        }

        if (effect is Effects.Buff buffEffect)
            ListOfActiveEffects.Add(new StatusEffect(buffEffect, stacks));
        else if (effect is Debuff debuffEffect)
            ListOfActiveEffects.Add(new StatusEffect(debuffEffect, stacks));
        else if (effect is SpecialEffects specialEffect)
            ListOfActiveEffects.Add(new StatusEffect(specialEffect, stacks));

        UiManager.Instance.UpdateEffects(ListOfActiveEffects);        

    }

    #endregion

    // Remove Effect Methods

    #region RemoveEffects
    /// <summary>
    /// Remove buff from Player
    /// </summary>
    /// <param name="buff"></param>
    /// <param name="stacks"></param>
    /// <param name="removeAll"></param>
    public void RemoveEffect(Effects.Buff buff, int stacks = 0, bool removeAll = false)
    {
        RemoveOrReduceEffect(buff, stacks, removeAll);
    }

    /// <summary>
    /// Remove Debuff from Player
    /// </summary>
    /// <param name="debuff"></param>
    /// <param name="stacks"></param>
    /// <param name="removeAll"></param>

    public void RemoveEffect(Effects.Debuff debuff, int stacks = 0, bool removeAll = false)
    {
        RemoveOrReduceEffect(debuff, stacks, removeAll);
    }

    /// <summary>
    /// Remove Speical effect from Player
    /// </summary>
    /// <param name="specialEffect"></param>
    public void RemoveEffect(Effects.SpecialEffects specialEffect)
    {
        ListOfActiveEffects.RemoveAll(e => e.SpecialEffect.Equals(specialEffect));
    }

    /// <summary>
    /// Remove or Reduce Effect on Player.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="effect"></param>
    /// <param name="stacks"></param>
    /// <param name="removeAll"></param>
    private void RemoveOrReduceEffect<T>(T effect, int stacks, bool removeAll = false) where T : Enum
    {
        for (int i = 0; i < ListOfActiveEffects.Count; i++)
        {
            var statusEffect = ListOfActiveEffects[i];
            
            if (effect is Effects.Buff && statusEffect.BuffEffect.Equals(effect))
            {
                if (removeAll || statusEffect.StackCount <= stacks)
                {
                    Debug.Log($"[PlayerController] Removing Buff: {effect}");
                    ListOfActiveEffects.RemoveAt(i);
                    return;
                }

                statusEffect.StackCount -= stacks;
                ListOfActiveEffects[i] = statusEffect;
                return;
            }
            
            if (effect is Effects.Debuff && statusEffect.DebuffEffect.Equals(effect))
            {
                if (removeAll || statusEffect.StackCount <= stacks)
                {
                    Debug.Log($"[PlayerController] Removing Debuff: {effect}");
                    ListOfActiveEffects.RemoveAt(i);
                    return;
                }

                statusEffect.StackCount -= stacks;
                ListOfActiveEffects[i] = statusEffect;
                return;
            }

            if (effect is Effects.SpecialEffects && statusEffect.SpecialEffect.Equals(effect))
            {
                Debug.Log($"[PlayerController] Removing Special Effect: {effect}");
                ListOfActiveEffects.RemoveAt(i);
                return;
            }
        }

        UiManager.Instance.UpdateEffects(listOfActiveEffects);        
    }

    #endregion

    private int GetStacks<T>(T effect) where T : Enum
    {
        foreach (var statusEffect in ListOfActiveEffects)
        {
            if (effect is Effects.Buff && statusEffect.BuffEffect.Equals(effect))
                return statusEffect.StackCount;
            if (effect is Debuff && statusEffect.DebuffEffect.Equals(effect))
                return statusEffect.StackCount;
            if (effect is SpecialEffects && statusEffect.SpecialEffect.Equals(effect))
                return statusEffect.StackCount;
        }
        return 0;
    }

    #endregion

    #region Scrap

    /// <summary>
    /// Adds Scraps
    /// </summary>
    /// <param name="amount"></param>
    public void GainScrap(int amount)
    {
        Scrap += amount;
    }

    /// <summary>
    /// Returns the Scraps stolen or whats left.
    /// </summary>
    /// <param name="amount">the amount of Scraps want to steal</param>
    /// <returns></returns>
    public int TakeScrap(int amount)
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

    #endregion

    #region Movement

    /// <summary>
    /// On click is run everytime the user clicks into the scene.
    /// Using Physics raycast.
    /// </summary>
    private void OnClick(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.CurrentGameMode == GameManager.GameMode.Roaming)
        {
            //Put in Coroutine to cancel out errors
            StartCoroutine(HandleClick());
        }
    }
    private IEnumerator HandleClick()
    {
        // Wait until the end of the frame to ensure UI state is updated
        yield return null;

        // Check if the pointer is over a UI element
        if (EventSystem.current.IsPointerOverGameObject())
        {
            yield break; // Exit if the click is on the UI
        }

        // Create a Ray from the current mouse position
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        // Define LayerMask to exclude the "Player" layer
        int layerMask = LayerMask.GetMask("Ground"); // Add more layers if needed

        // Perform the raycast
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            // Ignore clicks on objects other than the ground
            if (hit.collider.CompareTag("Ground"))
            {
                HandleGroundClick(hit.point);
            }
            else
            {
                Debug.Log("[PlayerController] Click detected on a non-ground object.");
            }
        }
        else
        {
            Debug.Log("[PlayerController] No valid object detected.");
        }
    }
    private void HandleGroundClick(Vector3 clickPoint)
    {
        GameObject tempIndicator;
        float currentTime = Time.time;

        // Double-click detection
        if ((currentTime - lastClickTime) <= doubleClickTime)
        {
            agent.speed = runSpeed;
            tempIndicator = RippleRunPrefab;
        }
        else
        {
            agent.speed = walkSpeed;
            tempIndicator = RipplePrefab;
        }        

        // Create ripple effect at the click point
        GameObject ClickIndicator = Instantiate(tempIndicator, clickPoint, Quaternion.identity);

        // Make the ripple effect face the Player
        ClickIndicator.transform.LookAt(this.transform.position);

        // Destroy the ripple effect after 1 seconds
        Destroy(ClickIndicator, 1f);

        // Move the Player to the clicked position
        agent.SetDestination(clickPoint);
        transform.LookAt(clickPoint);

        // Update the last click time
        lastClickTime = currentTime;
    }

    public void MovePlayerToPosition(Vector3 TargetPosition)
    {
        agent.SetDestination(TargetPosition);
    }
    public void StartSmoothRotate(Vector3 targetPosition)
    {
        if (rotateCoroutine != null)
            StopCoroutine(rotateCoroutine);

        rotateCoroutine = StartCoroutine(SmoothRoatePlayerToTarget(targetPosition));
    }
    private IEnumerator SmoothRoatePlayerToTarget(Vector3 target)
    {
        // Keep rotation only on the horizontal axis
        target.y = transform.position.y;

        Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);

        // Loop until the rotation is almost complete
        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
        {
            // 180 degrees per second rotation speed
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * 180f);
            yield return null;
        }

        // Ensure final rotation is exactly at the target
        transform.rotation = targetRotation;
    }
    /// <summary>
    /// Totate to Target
    /// </summary>
    private void SmoothRotateTowardsTarget()
    {
        if (CombatController.Instance.Target != null)
        {
            Vector3 direction = CombatController.Instance.Target.transform.position - transform.position;
            direction.y = 0; // Keep rotation on the horizontal plane

            if (direction.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 5f); // Smooth rotation
            }
        }
    }
    #endregion

    #region Combat

    /// <summary>
    /// Stuff to do at start of players turn.
    /// </summary>
    public void StartTurn()
    {
        //Remove ShieldAmount
        if (Shield > 0)
            Shield = 0;

        //Remove buffs by 1
        RemoveOrReduceEffect(Effects.Buff.Galvanize, 1);

        UiManager.Instance.ChangeStateOfGear(!IsRedirected);
    }

    /// <summary>
    /// Called Player ends their turn
    /// </summary>
    public void EndTurn()
    {
        //Restore energy to full
        RecoverFullEnergy();

        //Remove debuffs by 1
        RemoveOrReduceEffect(Effects.Debuff.Drained, 1);
        RemoveOrReduceEffect(Effects.Debuff.WornDown, 1);
        RemoveOrReduceEffect(Effects.Debuff.Jam, 1);
        RemoveOrReduceEffect(Effects.Debuff.Redirect, 1);
    }

    /// <summary>
    /// Called when round ends to apply buffs or debuffs.
    /// Buffs Stack don't go away.
    /// Debuffs Stack go away every round.
    /// </summary>
    public void RoundEnd()
    {
        if (GalvanizedStacks > 0)
        {
            ApplyShield(GalvanizedStacks);
        }
    }

    /// <summary>
    /// Anything for player to do at combat end
    /// </summary>
    public void TriggerPassiveEffectsOnCombatEnd()
    {
        foreach (var statusEffect in ListOfActiveEffects)
        {
            switch (statusEffect.SpecialEffect)
            {
                case Effects.SpecialEffects.LuckyTrinket:
                    // Give player +10 scrap
                    GainScrap(10);
                    Debug.Log("Lucky Trinket effect triggered: +10 scrap granted after combat!");
                    break;

                    // Future passive effects can be added here
            }
        }

       RemoveEffect(Effects.Buff.Power,0,true);
    }


    #endregion

    /// <summary>
    /// To make character speak in game world.
    /// </summary>
    /// <param name="message">accepts string format for textmeshpro</param>
    /// <param name="revealByLetter">true if you want to reveal speech by letter or false by word</param>
    /// <param name="howFastToTalk"></param>
    /// <param name="howLongToDisplay">default is 3</param>
    public void CharacterSpeak(string message, bool revealByLetter, float howFastToTalk, float howLongToDisplay = 3f, bool isDialogue = false)
    {
        uiController.PlayerTalk(message, revealByLetter, howFastToTalk, howLongToDisplay, isDialogue);
    }

    /// <summary>
    /// Call when Player is dead.
    /// Does stuff for game over.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void PlayerDie()
    {
        Debug.Log("Player died, game over.");

        //Reset HealthBar, energy and other stuff for now.
        Health = maxHealth;
        Energy = maxEnergy;
        //GainScrap(200);

        // Stop player actions
        this.enabled = false;

        GameManager.Instance.GameOver();

        // Show Game Over UI
        UiManager.Instance.ShowGameOverScreen();        

    }

    void OnDestroy()
    {
        //Unsubscribe to prevent memory leaks
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnEndCombat -= TriggerPassiveEffectsOnCombatEnd;
        }
        select.Disable();
    }






    //DEBUG METHODS
    [ContextMenu("Test Speak")]
    private void TestSpeak()
    {
        CharacterSpeak("I have a voice.\nI realy do have a voice !!", true, 0.1f, 5f);
    }
    [ContextMenu("Test Damage 5")]
    private void TestDamage()
    {
        DamagePlayerBy(5);
    }
    [ContextMenu("Kill Player")]
    private void TestPlayerdeath()
    {
        DamagePlayerBy(1000);
    }
    [ContextMenu("Spend 5 energy")]
    private void TestEnergyUse()
    {
        SpendEnergy(5);
    }
    [ContextMenu("Add 5 energy")]
    private void TestEnergyGain()
    {
        RecoverEnergy(5);
    }
}
