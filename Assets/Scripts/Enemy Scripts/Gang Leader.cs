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
        base.CombatStart();
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

        if (Looter1.activeInHierarchy || Looter2.activeInHierarchy)
        {
            if (Random.Range(1, 11) < 5)
            {
                //Threaten();
                Animator.SetTrigger("Intent 1");
            }                
            else
            {
                //Intimidate();
                Animator.SetTrigger("Intent 2");
            }
        }
        //Once Looters are defeated
        else
        {
            if (Random.Range(1, 11) < 4)
            {
                //Disorient();
                Animator.SetTrigger("Intent 3");
            }
            else
            {
                //Cower();
                Animator.SetTrigger("Intent 4");
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
    /// 50% change of this
    /// </summary>
    private void Threaten()
    {
        this.AddEffect(Effects.Buff.Power, 2);

        //Come back and remove this for better logic
        try
        {
            Looter1.GetComponent<Enemy>().AddEffect(Effects.Buff.Power, 2);
            Looter2.GetComponent<Enemy>().AddEffect(Effects.Buff.Power, 2);
        }
        catch
        {
            Debug.Log("One of looters are dead.");
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
        if (Looter1.activeInHierarchy || Looter2.activeInHierarchy)
        {
            if (Random.Range(1, 11) < 5)
                return ("Threaten", IntentType.Buff, 2);
            else
                return ("Intimidate", IntentType.Debuff, 1);
        }
        else
        {
            if (Random.Range(1, 11) < 4)
                return ("Disorient", IntentType.Attack, 6);
            else
                return ("Cower", IntentType.Shield, 15);
        }
    }

}
