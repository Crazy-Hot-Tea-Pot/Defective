using System.Linq;
using UnityEngine;

public class GangLeader : Enemy
{
    public GameObject Looter1
    {
        get
        {
            return looter1;
        }
        private set
        {
            looter1 = value;
        }
    }
    public GameObject Looter2
    {
        get
        {
            return looter2;
        }
        private set
        {
            looter2 = value;
        }
    }
    public enum GangLeaderPhases
    {
        WithLooters,
        Alone
    }

    public GangLeaderPhases CurrentPhase
    {
        get
        {
            if(Looter1 != null || Looter2!= null)
                return GangLeaderPhases.WithLooters;
            else
                return GangLeaderPhases.Alone;
        }
    }
    private GameObject looter1;
    private GameObject looter2;

    // Start is called before the first frame update
    public override void Start()
    {
        if (EnemyName == null)
            EnemyName = "Gang Leader";

        EnemyType = EnemyManager.TypeOfEnemies.GangLeader;

        base.Start();
    }

    public override void CombatStart()
    {
        Debug.Log("Enemy Combat Start");
        
        Debug.Log("Assigning Looters");
        AssignLooters();

        // Roll first intent at combat start
        nextIntentRoll = Random.Range(1, 11);

        base.CombatStart();
    }

    public override void EndTurn()
    {
        // Roll next intent for the upcoming turn
        nextIntentRoll = Random.Range(1, 11);
        base.EndTurn();
    }


    protected override void SetUpEnemy()
    {
        base.SetUpEnemy();

        switch (Difficulty)
        {
            case EnemyDifficulty.Easy:
                MaxHp = 60;
                break;
            case EnemyDifficulty.Medium:
                MaxHp = 80;
                break;
            case EnemyDifficulty.Hard:
                MaxHp = 100;
                break;
            case EnemyDifficulty.Boss:
                MaxHp = 100;
                break;
        }

        CurrentHP = MaxHp;
    }

    protected override void PerformIntent()
    {
        base.PerformIntent();

        // If the selected intent is a Looter-dependent one
        if (NextIntent.intentText == "Threaten" && (Looter1 == null && Looter2 == null))
        {
            Debug.Log("Looters are gone. Rerolling intent for Gang Leader...");

            // Reroll intent phase
            nextIntentRoll = Random.Range(1, 11);

            // Get new intent (updates display)
            var rerolledIntent = GetNextIntent();
            NextIntent = rerolledIntent;

            // Force update on UI
            EnemyUI enemyUI = EnemyUIObject.GetComponent<EnemyUI>();
            enemyUI.DisplayIntent(rerolledIntent.intentText, rerolledIntent.intentType, rerolledIntent.value);
        }

        // Now perform the actual intent
        switch (NextIntent.intentText)
        {
            case "Threaten":
                //Threaten();
                Animator.SetTrigger("Intent 1");
                break;
            case "Intimidate":
                //Intimidate();
                Animator.SetTrigger("Intent 2");
                break;
            case "Disorient":
                //Disorient();
                Animator.SetTrigger("Intent 3");
                break;
            case "Cower":
                //Cower();
                Animator.SetTrigger("Intent 4");
                break;
            default:
                Debug.LogWarning($"Unknown intent: {NextIntent.intentText}");
                break;
        }
    }
    /// <summary>
    /// Deal 6 Damage, Apply 1 Jam.
    /// 40% chance
    /// </summary>    
    private void Disorient()
    {
        EnemyTarget.GetComponent<PlayerController>().AddEffect(Effects.Debuff.Jam, 1);
        EnemyTarget.GetComponent<PlayerController>().DamagePlayerBy(6);
    }
    /// <summary>
    ///  Gain 15 ShieldBar, Deal 3 Damage.
    ///  60% chance
    /// </summary>    
    private void Cower()
    {
        ApplyShield(15);
        EnemyTarget.GetComponent<PlayerController>().DamagePlayerBy(3);
    }
    /// <summary>
    /// Self and Looters gain 2 Power.
    /// 50% chance of this.
    /// no reference as called by animator
    /// </summary>
    private void Threaten()
    {
        // Gang Leader buffs self
        this.AddEffect(Effects.Buff.Power, 2);

        if (Looter1 != null)
        {
            var enemy = Looter1.GetComponent<Enemy>();
            if (enemy != null)
                enemy.AddEffect(Effects.Buff.Power, 2);
        }

        if (Looter2 != null)
        {
            var enemy = Looter2.GetComponent<Enemy>();
            if (enemy != null)
                enemy.AddEffect(Effects.Buff.Power, 2);
        }
    }
    /// <summary>
    /// Apply 1 Worn and 1 Drained to Player.
    /// 50% chance
    /// </summary>
    private void Intimidate()
    {
        EnemyTarget.GetComponent<PlayerController>().AddEffect(Effects.Debuff.WornDown, 1);
        EnemyTarget.GetComponent<PlayerController>().AddEffect(Effects.Debuff.Drained, 1);
    }

    private void AssignLooters()
    {        
        // Find all Looter enemies in the same combat zone
        var looters = GameObject.FindGameObjectWithTag("CombatController")
            .GetComponent<CombatController>().CombatEnemies
            .Where(enemy => enemy.GetComponent<Enemy>().EnemyType == EnemyManager.TypeOfEnemies.Looter)
            .Take(2)
            .ToList();

        if (looters.Count > 0)
            Looter1 = looters[0];

        if (looters.Count > 1)
            Looter2 = looters[1];

        Debug.Log($"Gang Leader assigned Looters: {Looter1?.name}, {Looter2?.name}");
    }

    protected override (string intentText, IntentType intentType, int value) GetNextIntent()
    {
        if (CurrentPhase == GangLeaderPhases.WithLooters)
        {
            return nextIntentRoll < 5
                ? ("Threaten", IntentType.Buff, 2)
                : ("Intimidate", IntentType.Debuff, 1);
        }
        else
        {
            return nextIntentRoll < 4
                ? ("Disorient", IntentType.Attack, 6)
                : ("Cower", IntentType.Shield, 15);
        }
    }

}
