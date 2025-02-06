using UnityEngine;

public class MaintenanceBot : Enemy
{
    [Header("Custom for Enemy type")]
    private bool repairUsed;

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


        //Add Common Chips Todrop
        DroppedChips = ChipManager.Instance.GetChipsByRarity(NewChip.ChipRarity.Common);

        base.Start();
    }
    protected override void PerformIntent()
    {

        if (CurrentHP <= maxHP / 2 && !RepairUsed)
        {
            Repair();
            RepairUsed = true;
        }
        else
        {           
            if (Random.Range(1, 11) <= 4)
            {
                Galvanize();                
            }
            else
            {
                Disassemble();
            }
        }

        base.PerformIntent();
    }
    protected override Intent GetNextIntent()
    {
        if (CurrentHP <= maxHP / 2 && !RepairUsed)
        {
            return new Intent("Repair", Color.green, 0, "Heals 30% of max HP");
        }
        else if (Random.Range(1, 11) <= 4)
        {
            return new Intent("Galvanize", Color.yellow, 0, "Gains 4 Galvanize");
        }
        else
        {
            return new Intent("Disassemble", Color.red, 9, "Deals damage and applies Worn");
        }
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
