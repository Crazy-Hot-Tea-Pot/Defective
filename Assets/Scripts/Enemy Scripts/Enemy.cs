using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
public class Enemy : MonoBehaviour
{    
    public enum EnemyDifficulty
    {
        None,
        Easy,
        Medium,
        Hard,
        Boss
    }
    public enum IntentType
    {
        None,
        Attack,
        Shield,
        Buff,
        Debuff,
        Unique,
        Galvanize,
        Jam,
        Drained,
        WornDown,
        Power
    }
    public enum IsEnemy
    {
        Human,
        Robot
    }

    [Header("Enemy Components")]
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
    /// Enemy Ui
    /// </summary>
    public GameObject EnemyUIObject;
    /// <summary>
    /// reference to enemy canvas.
    /// </summary>
    public EnemyUI thisEnemyUI;
    /// <summary>
    /// Reference to combat controller.
    /// </summary>
    public CombatController CombatController;
    public GameObject Model;
    public NavMeshAgent agent;    
    public Animator Animator;
    /// <summary>
    /// reference to Player camera.
    /// </summary>
    public Camera playerCamera;

    [Header("Enemy status")]
    #region EnemyStatus

    [SerializeField]
    protected string enemyName;
    protected bool inCombat;
    [SerializeField]
    protected EnemyDifficulty enemyDifficulty;
    [SerializeField]
    protected IsEnemy enemyIs;
    protected EnemyManager.TypeOfEnemies enemyType;
    protected float currentHp;
    /// <summary>
    /// Max Hp of Enemy
    /// </summary>
    protected float maxHp;
    protected float shield;
    protected float maxShield = 0f;
    protected bool isTargeted;
    protected int nextIntentRoll;

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
    /// This Enemies Difficulty
    /// </summary>
    public EnemyDifficulty Difficulty
    {
        get
        {
            return enemyDifficulty;
        }
        set
        {
            enemyDifficulty = value;
            SetUpEnemy();
        }
    }

    /// <summary>
    /// IsEnemy Human Or Robot
    /// </summary>
    public IsEnemy EnemyIs
    {
        get
        {
            return enemyIs;
        }
    }    

    /// <summary>
    /// What type is the enemy.
    /// </summary>
    public EnemyManager.TypeOfEnemies EnemyType
    {
        get
        {
            return enemyType;
        }
        protected set
        {
            enemyType = value;
        }
    }    
    /// <summary>
    /// Enemy Current Hp
    /// </summary>
    public float CurrentHP
    {
        get
        {
            return currentHp;
        }
        protected set
        {
            //Make sure save as 1 decimal place
            currentHp = (float)Math.Round((double)value,1);

            //Update UI for enemy HealthBar
            thisEnemyUI.UpdateHealth(currentHp,MaxHp);

            if (currentHp <= 0)
            {
                currentHp = 0;
                Die();
            }
        }
    }    

    public float MaxHp
    {
        get
        {
            return maxHp;
        }
        protected set
        {
            maxHp = value;
        }
    }
    /// <summary>
    /// Enemy ShieldBar Amount.
    /// </summary>
    public float Shield
    {
        get
        {
            return shield;
        }
        protected set
        {
            shield = (float)Math.Round(value,1);

            if (shield > maxShield)
                maxShield = value;

            if (shield <= 0)
            {
                shield = 0;
                maxShield = value;
            }
            StartCoroutine(WaitAndApplyShield());            
        }
    }    
    
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
            int newLayer;
            isTargeted = value;
            if (isTargeted)
            {
                newLayer = 9;
            }
            else
            {
                newLayer = 8;   
            }

            Model.layer = newLayer;

            foreach(Transform child in Model.transform)
            {
                child.gameObject.layer = newLayer;

                foreach(Transform grandchild in child.transform)
                {
                    grandchild.gameObject.layer = newLayer;
                }
            }
            //foreach (Transform child in transform)
            //{
            //    child.gameObject.layer = newLayer;

            //    foreach (Transform grandchild in child.transform)
            //    {
            //        grandchild.gameObject.layer = newLayer;
            //    };
            //};

            //TargetIcon.SetActive(value);
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

    protected List<(string intentText, IntentType intentType, int value)> NextIntents
    {
        get
        {
            return nextIntents;
        }
        set
        {
            nextIntents = value;
        }
    }

    private float distanceToPlayer;
    protected List<(string intentText, IntentType intentType, int value)> nextIntents;

    [Header("Sound")]
    public SoundFX EnemyDeathSound;
    public SoundFX EnemyDamageTakenSound;

    private Coroutine rotateCoroutine;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        playerCamera = Camera.main;
        thisEnemyUI = this.gameObject.GetComponentInChildren<EnemyUI>();
    }

    // Start is called before the first frame update
    public virtual void Start()
    {
        Initialize();
    }

    /// <summary>
    /// Initialize enemy
    /// </summary>
    protected virtual void Initialize()
    {

        CombatController = GameObject.FindGameObjectWithTag("CombatController").GetComponent<CombatController>();
        enemyTarget = GameObject.FindGameObjectWithTag("Player");

    }
    /// <summary>
    /// Set up enemy based on difficulty.
    /// name is already set in base.
    /// </summary>
    protected virtual void SetUpEnemy()
    {
        EnemyUIObject.SetActive(true);
        gameObject.name = EnemyName;
        thisEnemyUI.SetEnemyName(EnemyName);        
    }
    #region Combat
    protected IEnumerator WaitAndApplyShield()
    {
        while (!EnemyUIObject.activeInHierarchy)
        {
            yield return null;
        }

        thisEnemyUI.UpdateShield(shield, maxShield);
    }
    /// <summary>
    /// Combat start stuff for everyenemy.
    /// </summary>
    public virtual void CombatStart()
    {
        NextIntents = GetNextIntents();
        UpdateIntentUI();
    }

    /// <summary>
    /// Call at end of enemiies turn.
    /// </summary>
    public virtual void EndTurn()
    {
        EnemyUIObject.SetActive(true);

        //Remove debuffs by 1
        RemoveEffect(Effects.Debuff.Drained, 1);        

        //Assign new intent AFTER performing the current one
        NextIntents = GetNextIntents();
        UpdateIntentUI();

        CombatController.EndTurn(this.gameObject);
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
    public virtual void TakeDamage(float damage)
    {

        // Plays sound of taking damage
        SoundManager.PlayFXSound(EnemyDamageTakenSound, this.gameObject.transform);

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

        RotateToPlayer(EnemyTarget.transform.position);

        Animator.SetTrigger("Hit");

        //Track damage
        GameStatsTracker.Instance.ReportDamage(damage);
    }

    /// <summary>
    /// Current CombatEnemies turn to act.
    /// </summary>
    public virtual void MyTurn()
    {
        RotateToPlayer(EnemyTarget.transform.position);

        StartTurn();
    }

    /// <summary>
    /// Call when enemy die.
    /// </summary>
    protected virtual void Die()
    {
        //Update the quest for counting enemies to count it's death
        try
        {
            //Give enemy counter update the enemy name so they can verify it
            foreach (Quest quest in QuestManager.Instance.CurrentQuest)
            {
                Debug.Log("Look at this" + EnemyType.ToString());
                quest.EnemyQuestCounterUpdate(EnemyType.ToString());
            }
        }
        catch
        {

        }
        Animator.SetTrigger("Die");
        
        SoundManager.PlayFXSound(EnemyDeathSound, this.gameObject.transform);
    }

    /// <summary>
    /// Finish death stuff after animation plays
    /// </summary>
    public void FinishDeath()
    {
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
        Animator.SetTrigger("Shield");

        //Restore ShieldBar
        Shield += shieldAmount;

        Debug.Log("Shield Restored: " + shield);

    }

    /// <summary>
    /// Update UI to display next intent
    /// </summary>
    public virtual void UpdateIntentUI()
    {
        thisEnemyUI.DisplayIntent(NextIntents);
    }

    /// <summary>
    /// Base Perform Intent.
    /// Make sure the base is run after you perform an action.
    /// </summary>
    protected virtual void PerformIntent()
    {
        //Don't do anything wait for death
        if (CurrentHP <= 0)
            return;

        EnemyUIObject.SetActive(false);
    }

    /// <summary>
    /// Rotate enemy to player
    /// </summary>
    /// <param name="targetPosition"></param>
    protected void RotateToPlayer(Vector3 targetPosition)
    {
        if (rotateCoroutine != null)
            StopCoroutine(rotateCoroutine);

        rotateCoroutine = StartCoroutine(SmoothRoatePlayerToTarget(targetPosition));
    }
    protected IEnumerator SmoothRoatePlayerToTarget(Vector3 target)
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
    /// Display damage taken in game.
    /// </summary>
    /// <param name="damage"></param>
    protected virtual void DisplayDamageTaken(float damage)
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


    protected virtual List<(string intentText, IntentType intentType, int value)> GetNextIntents()
    {
        return new List<(string, IntentType, int)>
        {
            ("Unknown", IntentType.None, 0)
        };
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
        if (!ListOfActiveEffects.Any(e => e.SpecialEffect == specialEffect))
        {
            ListOfActiveEffects.Add(new Effects.StatusEffect(specialEffect, 1));
        }
    }

    /// <summary>
    /// Add a new effect or update the stacks on an effect
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="effect"></param>
    /// <param name="stacks"></param>
    protected virtual void AddOrUpdateEffect<T>(T effect, int stacks) where T : Enum
    {
        for (int i = 0; i < ListOfActiveEffects.Count; i++)
        {
            var statusEffect = ListOfActiveEffects[i];

            if ((effect is Effects.Buff && statusEffect.BuffEffect.Equals(effect)) ||
                (effect is Effects.Debuff && statusEffect.DebuffEffect.Equals(effect)) ||
                (effect is Effects.SpecialEffects && statusEffect.SpecialEffect.Equals(effect)))
            {
                statusEffect.StackCount += stacks;
                ListOfActiveEffects[i] = statusEffect;
                return;
            }
        }

        if (effect is Effects.Buff buffEffect)
            ListOfActiveEffects.Add(new Effects.StatusEffect(buffEffect, stacks));
        else if (effect is Effects.Debuff debuffEffect)
            ListOfActiveEffects.Add(new Effects.StatusEffect(debuffEffect, stacks));
        else if (effect is Effects.SpecialEffects specialEffect)
            ListOfActiveEffects.Add(new Effects.StatusEffect(specialEffect, 1));
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
        ListOfActiveEffects.RemoveAll(e => e.SpecialEffect == specialEffect);
        Debug.Log($"[Enemy] Removing all instances of Special Effect: {specialEffect}");
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

            if ((effect is Effects.Buff && statusEffect.BuffEffect.Equals(effect)) ||
                (effect is Effects.Debuff && statusEffect.DebuffEffect.Equals(effect)) ||
                (effect is Effects.SpecialEffects && statusEffect.SpecialEffect.Equals(effect)))
            {
                if (removeAll || statusEffect.StackCount <= stacks)
                {
                    ListOfActiveEffects.RemoveAt(i);
                    Debug.Log($"[Enemy] Removing Effect: {effect}");
                    return;
                }

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
            if ((effect is Effects.Buff && statusEffect.BuffEffect.Equals(effect)) ||
                (effect is Effects.Debuff && statusEffect.DebuffEffect.Equals(effect)) ||
                (effect is Effects.SpecialEffects && statusEffect.SpecialEffect.Equals(effect)))
            {
                return statusEffect.StackCount;
            }
        }
        return 0;
    }


    #endregion

    public void SetEnemyName(string newName)
    {
        EnemyName = newName;
    }

    [ContextMenu("Test Death")]
    private void TestDeath()
    {
        CurrentHP = 0;
    }

    [ContextMenu("Take 5 Damage")]
    private void TestHit()
    {
        TakeDamage(5);
    }
}