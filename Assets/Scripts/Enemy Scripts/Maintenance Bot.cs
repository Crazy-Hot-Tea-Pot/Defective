using UnityEngine;

public class MaintenanceBot : Enemy
{
    [Header("Custom for Enemy type")]
    private bool repairUsed;

    private int nextIntentRoll;

    public bool RepairUsed
    {
        get
        {
            return repairUsed;
        }
        private set
        {
            repairUsed = value;
        }
    }

    [Header("Sound")]
    public SoundFX RepairBotSound;
    public SoundFX GalvanizeBotSound;
 
    

    // Start is called before the first frame update
    public override void Start()
    {
        if(EnemyName==null)
            EnemyName = "Maintenance Bot";

        DroppedChips.Clear();

        // Get list of all common chips
        var commonChips = ChipManager.Instance.GetChipsByRarity(NewChip.ChipRarity.Common);

        // Ensure at least 2 chips exist before selecting
        if (commonChips.Count >= 2)
        {
            int firstIndex = Random.Range(0, commonChips.Count);
            int secondIndex;

            // Make sure second chip is different
            do
            {
                secondIndex = Random.Range(0, commonChips.Count);
            } while (secondIndex == firstIndex);

            DroppedChips.Add(commonChips[firstIndex]);
            DroppedChips.Add(commonChips[secondIndex]);
        }

        EnemyType = EnemyManager.TypeOfEnemies.GangLeader;

        base.Start();
    }
    public override void EndTurn()
    {
        nextIntentRoll = Random.Range(1, 11);
        base.EndTurn();
    }
    protected override void PerformIntent()
    {
        base.PerformIntent();

        if (CurrentHP <= maxHP / 2 && !RepairUsed)
        {
            //Repair();
            //RepairUsed = true;
            Animator.SetTrigger("Intent 3");
        }
        else
        {           
            if (nextIntentRoll <= 4)
            {
                //Galvanize();
                Animator.SetTrigger("Intent 1");
            }
            else
            {
                //Disassemble();
                Animator.SetTrigger("Intent 2");
            }
        }
    }
    protected override (string intentText, IntentType intentType, int value) GetNextIntent()
    {
        if (CurrentHP <= maxHP / 2 && !RepairUsed)
            return ("Repair", IntentType.Buff, 0);
        else if (nextIntentRoll <= 4)
            return ("Galvanize", IntentType.Buff, 4);
        else
            return ("Disassemble", IntentType.Attack, 9);
    }

    /// <summary>
    /// Deals 9 Damage
    /// and
    /// Apply Worn
    /// </summary>
    public void Disassemble()
    {
        Debug.Log("Maintenance Bot uses Disassemble!");
        EnemyTarget.GetComponent<PlayerController>().DamagePlayerBy(9);
        EnemyTarget.GetComponent<PlayerController>().AddEffect(Effects.Debuff.WornDown, 1);        
    }
    /// <summary>
    /// Gains 4 stacks of Galvanize.
    /// </summary>
    public void Galvanize()
    {
        //PlaySound
        SoundManager.PlayFXSound(GalvanizeBotSound, this.gameObject.transform);

        AddEffect(Effects.Buff.Galvanize, 4);        
    }
    /// <summary>
    /// heals 30% of its Max Hp
    /// </summary>
    public void Repair()
    {        
        //Play sound
        SoundManager.PlayFXSound(RepairBotSound, this.gameObject.transform);

        int tempHealAmount = Mathf.RoundToInt(maxHP * 0.3f);
        CurrentHP = Mathf.Min(CurrentHP + tempHealAmount, maxHP);        
    }
}
