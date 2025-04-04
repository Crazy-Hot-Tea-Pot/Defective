using System.Collections.Generic;
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

    private float baseDisorientDamage = 6;
    private float baseCowerDamage = 3;

    [Header("Sounds")]
    public BgSound CustomBattleSound;
    private BgSound previousBackgroundSound;
    private bool wasBackgroundPlaying;

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

        // Save current background state
        previousBackgroundSound = SoundManager.GetCurrentBackgroundSound();
        wasBackgroundPlaying = GameObject.Find("BgSound")?.GetComponent<AudioSource>()?.isPlaying ?? false;

        // Play the custom battle sound if set
        if (CustomBattleSound != BgSound.None)
        {
            SoundManager.ChangeBackground(CustomBattleSound);
            Debug.Log($"Playing custom battle sound: {CustomBattleSound}");
        }

        base.CombatStart();
    }

    public override void EndTurn()
    {
        // Roll next intent for the upcoming turn
        nextIntentRoll = Random.Range(1, 11);
        base.EndTurn();
    }

    public override void UpdateIntentUI()
    {
        var ui = EnemyUIObject.GetComponent<EnemyUI>();

        if (NextIntents[0].intentText == "Disorient")
        {
            ui.DisplayIntent(new List<(string, IntentType, int)>
        {
            ("Deal Damage", IntentType.Attack, 6),
            ("Apply Jam", IntentType.Jam, 1)
        });
        }
        else if (NextIntents[0].intentText == "Cower")
        {
            ui.DisplayIntent(new List<(string, IntentType, int)>
        {
            ("Gain Shield", IntentType.Shield, 15),
            ("Deal Damage", IntentType.Attack, 3)
        });
        }
        else if (NextIntents[0].intentText == "Intimidate")
        {
            ui.DisplayIntent(new List<(string, IntentType, int)>
        {
            ("Apply Worn", IntentType.WornDown, 1),
            ("Apply Drained", IntentType.Drained, 1)
        });
        }
        else
        {
            // Fallback to standard UI
            base.UpdateIntentUI();
        }
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
        if (CurrentHP <= 0)
            CurrentHP = MaxHp;
    }

    protected override void PerformIntent()
    {
        base.PerformIntent();

        // If the selected intent is a Looter-dependent one
        if (NextIntents[0].intentText == "Threaten" && (Looter1 == null && Looter2 == null))
        {
            Debug.Log("Looters are gone. Rerolling intent for Gang Leader...");

            // Reroll intent phase
            nextIntentRoll = Random.Range(1, 11);

            // Get new intent (updates display)
            var rerolledIntent = GetNextIntents();
            NextIntents = rerolledIntent;

            // Force update on UI
            EnemyUI enemyUI = EnemyUIObject.GetComponent<EnemyUI>();
            UpdateIntentUI();

        }

        // Now perform the actual intent
        switch (NextIntents[0].intentText)
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
                Debug.LogWarning($"Unknown intent: {NextIntents[0].intentText}");
                break;
        }
    }

    protected override void Die()
    {
        base.Die();

        // Check if the previous sound exists and resume it
        if (previousBackgroundSound != BgSound.None)
        {
            SoundManager.ChangeBackground(previousBackgroundSound);

            if (wasBackgroundPlaying)
            {
                GameObject.Find("BgSound")?.GetComponent<AudioSource>()?.Play();
                Debug.Log($"Resuming previous background sound: {previousBackgroundSound}");
            }
            else
            {
                Debug.Log($"Previous background sound was not playing, remaining silent.");
            }
        }
    }
    /// <summary>
    /// Deal 6 Damage, Apply 1 Jam.
    /// 40% chance
    /// </summary>    
    private void Disorient()
    {
        EnemyTarget.GetComponent<PlayerController>().AddEffect(Effects.Debuff.Jam, 1);
        

        EnemyTarget.GetComponent<PlayerController>().DamagePlayerBy(CalculateFinalDamage(baseDisorientDamage));
    }
    /// <summary>
    ///  Gain 15 ShieldBar, Deal 3 Damage.
    ///  60% chance
    /// </summary>    
    private void Cower()
    {
        ApplyShield(15);

        EnemyTarget.GetComponent<PlayerController>().DamagePlayerBy(CalculateFinalDamage(baseCowerDamage));
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
        {
            Looter1 = looters[0];
            Looter1.GetComponent<Looter>().IsWithLeader = true;
        }

        if (looters.Count > 1)
        {
            Looter2 = looters[1];
            Looter2.GetComponent <Looter>().IsWithLeader = true;
        }

        Debug.Log($"Gang Leader assigned Looters: {Looter1?.name}, {Looter2?.name}");
    }
    protected override List<(string intentText, IntentType intentType, int value)> GetNextIntents()
    {
        if (CurrentPhase == GangLeaderPhases.WithLooters)
        {
            if (nextIntentRoll < 5)
            {
                return new List<(string, IntentType, int)>
            {
                ("Threaten", IntentType.Power, 2)
            };
            }
            else
            {
                return new List<(string, IntentType, int)>
            {
                ("Intimidate", IntentType.WornDown, 1),
                ("Intimidate", IntentType.Drained, 1)
            };
            }
        }
        else
        {
            if (nextIntentRoll < 4)
            {
                return new List<(string, IntentType, int)>
            {
                ("Disorient", IntentType.Attack, 6),
                ("Disorient", IntentType.Jam, 1)
            };
            }
            else
            {
                return new List<(string, IntentType, int)>
            {
                ("Cower", IntentType.Shield, 15),
                ("Cower", IntentType.Attack, 3)
            };
            }
        }
    }

}
