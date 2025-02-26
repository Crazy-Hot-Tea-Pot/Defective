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

        base.Start();
    }
    public override void EndTurn()
    {
        nextIntentRoll = Random.Range(1, 11);
        base.EndTurn();
    }
    public override void PerformIntentTrigger(string intentName)
    {
        base.PerformIntentTrigger(intentName);

        switch (intentName)
        {
            case "Repair":
                Repair();
                repairUsed = true;
                break;
            case "Galvanize":
                Galvanize();
                break;
            case "Disassemble":
                Disassemble();
                break;
            default:
                Debug.LogWarning($"Intent '{intentName}' not handled in {EnemyName}.");
                break;
        }
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
        StartCoroutine(PrepareToEndTurn());
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
    private void Disassemble()
    {
        Debug.Log("Maintenance Bot uses Disassemble!");
        EnemyTarget.GetComponent<PlayerController>().DamagePlayerBy(9);
        EnemyTarget.GetComponent<PlayerController>().AddEffect(Effects.Debuff.WornDown, 1);        
    }
    /// <summary>
    /// Gains 4 stacks of Galvanize.
    /// </summary>
    private void Galvanize()
    {
        //PlaySound
        SoundManager.PlayFXSound(SoundFX.GalvanizeMainenanceBot, this.gameObject.transform);

        AddEffect(Effects.Buff.Galvanize, 4);        
    }
    /// <summary>
    /// heals 30% of its Max Hp
    /// </summary>
    private void Repair()
    {        
        //Play sound
        SoundManager.PlayFXSound(SoundFX.RepairMaintenaceBot, this.gameObject.transform);

        int tempHealAmount = Mathf.RoundToInt(maxHP * 0.3f);
        CurrentHP = Mathf.Min(CurrentHP + tempHealAmount, maxHP);        
    }
}
