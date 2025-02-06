using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using System.Linq;
using System;
/// <summary>
/// For UI display of Intent
/// </summary>
public class Intent
{
    public string Name { get; set; }
    public Color IntentColor { get; set; }
    public int Damage { get; set; }
    public string AdditionalInfo { get; set; }

    public Intent(string name, Color intentColor, int damage = 0, string additionalInfo = "")
    {
        Name = name;
        IntentColor = intentColor;
        Damage = damage;
        AdditionalInfo = additionalInfo;
    }
}
public class Enemy : MonoBehaviour
{   

    public GameObject enemyTarget;
    /// <summary>
    /// Enemy current Target.
    /// </summary>
    public GameObject EnemyTarget
    {
        get { return enemyTarget; }
        protected set
        {
            enemyTarget = value;
        }
    }

    /// <summary>
    /// Range enemy must be to attack the Player.
    /// </summary>
    public float AttackRange;     

    public float DistanceToPlayer
    {
        get
        {
            distanceToPlayer= Vector3.Distance(transform.position, EnemyTarget.transform.position); ;
            return distanceToPlayer;
        }               
    }

    [Header("Enemy Components")]
    /// <summary>
    /// reference to enemy canvas.
    /// </summary>
    public EnemyUI thisEnemyUI;
    /// <summary>
    /// Reference to combat controller.
    /// </summary>
    public CombatController CombatController;
    public Animator animator;
    public NavMeshAgent agent;
    public GameObject TargetIcon;
    /// <summary>
    /// reference to Player camera.
    /// </summary>
    public Camera playerCamera;

    [Header("Enemy status")]
    #region EnemyStatus
    [SerializeField]
    private string enemyName;

    /// <summary>
    /// Returns name of enemy
    /// </summary>
    public virtual string EnemyName
    {
        get
        {            
                return enemyName;
        }
        protected set
        {
            enemyName = value;            
        }
    }

    private bool inCombat;
    /// <summary>
    /// Is the Enemy in Combat.
    /// </summary>
    public bool InCombat
    {
        get
        {
            return inCombat;
        }
        set
        {
            inCombat = value;            
        }
    }

    /// <summary>
    /// Max Hp of Enemy
    /// </summary>
    public int maxHP;
    private int currentHp;
    /// <summary>
    /// Enemy Current Hp
    /// </summary>
    public int CurrentHP
    {
        get
        {
            return currentHp;
        }
        protected set
        {
            currentHp = value;

            //Update UI for enemy HealthBar
            thisEnemyUI.UpdateHealth(currentHp,maxHP);

            if (currentHp <= 0)
            {
                currentHp = 0;
                Die();
            }
        }
    }

    private int shield;
    /// <summary>
    /// Enemy ShieldBar Amount.
    /// </summary>
    public int Shield
    {
        get
        {
            return shield;
        }
        protected set
        {
            shield = value;

            if (shield > maxShield)
                maxShield = value;

            if (shield <= 0)
            {
                shield = 0;
                maxShield = value;
            }

            thisEnemyUI.UpdateShield(shield,maxShield);
        }
    }
    private int maxShield = 0;
    private bool isTargeted;
    /// <summary>
    /// Is enemy being targeted by Player.
    /// When enemy is targeted by CombatController to change boarder.
    /// </summary>
    public bool IsTargeted
    {
        get
        {
            return isTargeted;
        }
        set
        {
            isTargeted = value;
            TargetIcon.SetActive(value);
        }
    }
    #endregion

    [Header("Status Effects")]
    #region StatusEffects

    [SerializeField]
    private List<Effects.StatusEffect> listOfActiveEffects = new List<Effects.StatusEffect>();

    public List<Effects.StatusEffect> ListOfActiveEffects
    {
        get => listOfActiveEffects;
        set
        {
            listOfActiveEffects = value;

            // Update the UI when effects change
            thisEnemyUI.UpdateEffectsPanel(listOfActiveEffects);
        }
    }


    /// <summary>
    /// Is enemy GalvanizedStacks.
    /// Added for animation or effect later.
    /// </summary>
    public bool IsGalvanized
    {
        get
        {
            if (GalvanizedStacks > 0)            
                return true;            
            else
                return false;
        }
    }
    /// <summary>
    /// Amount of stacks of Galvanize the enemy have.
    /// </summary>
    public int GalvanizedStacks
    {
        get
        {
            return GetStacks(Effects.Buff.Galvanize);
        }
    }
    /// <summary>
    /// Is enemy powered.
    /// Added for animation or effect later.
    /// </summary>
    public bool IsPowered
    {
        get
        {
            if (PowerStacks > 0)
                return true;
            else
                return false;
        }
    }
    /// <summary>
    /// Amount of stacks of Power the enemy have.
    /// </summary>
    public int PowerStacks
    {
        get
        {
            return GetStacks(Effects.Buff.Power);
        }
    }
    /// <summary>
    /// Is enemy Drained.
    /// Added for animation or effect later.
    /// </summary>
    public bool IsDrained
    {
        get
        {
            if(DrainedStacks > 0) 
                return true;
            else
                return false;
        }
    }
    /// <summary>
    /// Amount of stacks of Drained the enemy have.
    /// </summary>
    public int DrainedStacks
    {
        get
        {
            return GetStacks(Effects.Debuff.Drained);
        }
    }        
    #endregion

    public GameObject damageTextPrefab;

    [Header("Dropped stuff")]
    /// <summary>
    /// This holds the cards we want to have dropped on death
    /// </summary>
    public List<NewChip> DroppedChips;
    public List<Item> DroppedItems;
    [Range(0,100)]
    public int DroppedScrap;

    private float distanceToPlayer;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        playerCamera = Camera.main;
        thisEnemyUI = this.gameObject.GetComponentInChildren<EnemyUI>();
    }

    // Start is called before the first frame update
    public virtual void Start()
    {
        Initialize();
    }

    public virtual void FixedUpdate()
    {
        
    }
    public virtual void Update()
    {

    }

    /// <summary>
    /// Initialize enemy
    /// </summary>
    public virtual void Initialize()
    {
        CurrentHP = maxHP;
        gameObject.name = EnemyName;
        thisEnemyUI.SetEnemyName(EnemyName);

        CombatController = GameObject.FindGameObjectWithTag("CombatController").GetComponent<CombatController>();
        enemyTarget = GameObject.FindGameObjectWithTag("Player");

    }

    #region Combat

    /// <summary>
    /// Combat start stuff for everyenemy.
    /// </summary>
    public virtual void CombatStart()
    {
        UpdateIntentUI();
    }

    /// <summary>
    /// Call at end of enemyies turn.
    /// </summary>
    public virtual void EndTurn()
    {
        //Remove debuffs by 1
        RemoveEffect(Effects.Debuff.Drained, 1);        
    }

    /// <summary>
    /// Called when round ends
    /// </summary>
    public virtual void RoundEnd()
    {
        if (IsGalvanized)
        {
            Shield += GalvanizedStacks;            
        }
    }

    /// <summary>
    /// Start Combat turn
    /// </summary>
    protected virtual void StartTurn()
    {
        //Remove ShieldBar if there is shield
        if (Shield > 0)
            Shield = 0;

        //Remove Buffs
        RemoveEffect(Effects.Buff.Power, 1);
        RemoveEffect(Effects.Buff.Galvanize, 1);        

        //Look at Player
        //this.gameObject.transform.LookAt(EnemyTarget.transform);

        //Check if Player is in range
        //if (DistanceToPlayer <= AttackRange)
        //{
            //agent.ResetPath();
            PerformIntent();
        //}
        //else
        //{
        //    // move to Player
        //    agent.SetDestination(EnemyTarget.transform.position);
        //}
    }

    /// <summary>
    /// Is called when enemy is attacked by Player.
    /// </summary>
    /// <param name="damage"></param>
    public virtual void TakeDamage(int damage)
    {
        // Plays sound of taking damage
        SoundManager.PlayFXSound(SoundFX.DamageTaken, this.gameObject.transform);

        // if has shield
        if (Shield > 0)
        {
            if (damage >= Shield)
            {
                damage -= Shield;
                Shield = 0;
                Debug.Log("Enemy " + name + "Shield destroyed.");
            }
            else
            {
                // Reduce the shield by the damage amount
                Shield -= damage;
                // No remaining damage to apply to HP
                damage = 0;
            }
        }
        CurrentHP -= damage;

        DisplayDamageTaken(damage);
    }

    /// <summary>
    /// Current CombatEnemies turn to act.
    /// </summary>
    public virtual void MyTurn()
    {
        StartTurn();
    }

    /// <summary>
    /// Call when enemy die.
    /// </summary>
    public virtual void Die()
    {
        SoundManager.PlayFXSound(SoundFX.EnemyDefeated, this.gameObject.transform);

        Debug.Log($"{enemyName} has been defeated!");

        CombatController.LeaveCombat(this.gameObject, DroppedScrap, DroppedChips, DroppedItems);

        EnemyManager.Instance.RemoveEnemy(this.gameObject);
    }

    /// <summary>
    /// Give enemy shield.
    /// </summary>
    /// <param name="shieldAmount"></param>
    public virtual void ApplyShield(int shieldAmount)
    {
        //Restore ShieldBar
        Shield += shieldAmount;

        Debug.Log("Shield Restored: " + shield);
    }

    /// <summary>
    /// Update UI to display next intent
    /// </summary>
    public virtual void UpdateIntentUI()
    {
        var nextIntent = GetNextIntent();
        thisEnemyUI.UpdateIntent(nextIntent);
    }

    /// <summary>
    /// Base Perform Intent.
    /// Make sure the base is run after you perform an action.
    /// </summary>
    protected virtual void PerformIntent()
    {
        if (this.gameObject != null)
        {
            CombatController.EndTurn(this.gameObject);
            UpdateIntentUI();
        }
    }

    /// <summary>
    /// Display damage taken in game.
    /// </summary>
    /// <param name="damage"></param>
    protected virtual void DisplayDamageTaken(int damage)
    {
        // Instantiate the damage text prefab
        // Calculate the position in front of the object
        Vector3 forwardPosition = this.gameObject.transform.position + this.gameObject.transform.forward * 2f + Vector3.up * 1f;

        // Instantiate the damage indicator at the calculated position
        GameObject damageIndicator = Instantiate(damageTextPrefab, forwardPosition, Quaternion.identity);

        // Set the text to display the damage amount
        TextMeshPro textMesh = damageIndicator.GetComponent<TextMeshPro>();
        textMesh.text = $"-{damage}";

        // Ensure the text faces the camera
        damageIndicator.transform.LookAt(Camera.main.transform);
        damageIndicator.transform.Rotate(0, 180, 0); // Correct for backward text
    }


    protected virtual Intent GetNextIntent()
    {
        // Default implementation, overridden in derived classes
        return new Intent("Unknown", Color.gray);
    }

    #endregion

    #region Effects

    #region Add Effects

    /// <summary>
    /// Add Buff to enemy.
    /// </summary>
    /// <param name="buff"></param>
    /// <param name="stacks"></param>
    public void AddEffect(Effects.Buff buff, int stacks)
    {
        AddOrUpdateEffect(buff, stacks);
    }

    /// <summary>
    /// Add debuff to enemy.
    /// </summary>
    /// <param name="debuff"></param>
    /// <param name="stacks"></param>
    public void AddEffect(Effects.Debuff debuff, int stacks)
    {
        AddOrUpdateEffect(debuff, stacks);
    }

    /// <summary>
    /// Add Special Effect to enemy.
    /// </summary>
    /// <param name="specialEffect"></param>
    public void AddEffect(Effects.SpecialEffects specialEffect)
    {
        if (!ListOfActiveEffects.Any(e => e.Effect.Equals(specialEffect)))
        {
            ListOfActiveEffects.Add(new Effects.StatusEffect(specialEffect, 0));
        }
    }

    /// <summary>
    /// Add a new effect or update the stacks on an effect
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="effect"></param>
    /// <param name="stacks"></param>
    private void AddOrUpdateEffect<T>(T effect, int stacks) where T : Enum
    {
        for (int i = 0; i < ListOfActiveEffects.Count; i++)
        {
            var statusEffect = ListOfActiveEffects[i];
            if (statusEffect.Effect.Equals(effect))
            {
                statusEffect.StackCount += stacks;
                // Reassign the updated effect
                ListOfActiveEffects[i] = statusEffect;
                return;
            }
        }

        // Add a new effect if it doesn't exist
        ListOfActiveEffects.Add(new Effects.StatusEffect(effect, stacks));
    }
    #endregion

    #region Remove Effects

    /// <summary>
    /// Remove Buff from Enemy
    /// </summary>
    /// <param name="buff"></param>
    /// <param name="stacks"></param>
    /// <param name="removeAll"></param>
    public void RemoveEffect(Effects.Buff buff, int stacks = 0, bool removeAll = false)
    {
        RemoveOrReduceEffect(buff, stacks, removeAll);
    }

    /// <summary>
    /// Remove Debuff From Enemy
    /// </summary>
    /// <param name="debuff"></param>
    /// <param name="stacks"></param>
    /// <param name="removeAll"></param>
    public void RemoveEffect(Effects.Debuff debuff, int stacks = 0, bool removeAll = false)
    {
        RemoveOrReduceEffect(debuff, stacks, removeAll);
    }

    /// <summary>
    /// Remove Special Effect from Enemy
    /// </summary>
    /// <param name="specialEffect"></param>
    public void RemoveEffect(Effects.SpecialEffects specialEffect)
    {
        ListOfActiveEffects.RemoveAll(e => e.Effect.Equals(specialEffect));
    }

    /// <summary>
    /// Remove or reduce stacks effects for enemy.
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
            if (statusEffect.Effect.Equals(effect))
            {
                if (removeAll || statusEffect.StackCount <= stacks)
                {
                    // Fully remove the effect
                    ListOfActiveEffects.RemoveAt(i);
                    return;
                }

                // Reduce stack count
                statusEffect.StackCount -= stacks;
                ListOfActiveEffects[i] = statusEffect;
                return;
            }
        }
    }
    #endregion

    /// <summary>
    /// Get how many stacks the current Effect has.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="effect"></param>
    /// <returns></returns>
    private int GetStacks<T>(T effect) where T : Enum
    {
        foreach (var statusEffect in ListOfActiveEffects)
        {
            if (statusEffect.Effect.Equals(effect))
            {
                return statusEffect.StackCount;
            }
        }

        // Return 0 if the effect is not present
        return 0;
    }

    #endregion

    public void SetEnemyName(string newName)
    {
        EnemyName = newName;
    }

    [ContextMenu("Test Death")]
    public void TestDeath()
    {
        CurrentHP = 0;
    }
}