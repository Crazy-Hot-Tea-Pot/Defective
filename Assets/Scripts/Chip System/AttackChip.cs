using UnityEngine;

[CreateAssetMenu(fileName = "NewAttackChip", menuName = "Chip System/Attack Chip")]
public class AttackChip : NewChip
{
    /// <summary>
    /// Amount of Damage Chip will do.
    /// </summary>
    public int damage;
    /// <summary>
    /// Amount of Damage to do when upgraded.
    /// </summary>
    public int upgradedDamageByAmount;    
    public int numberOfHits=1;
    public Effects.Debuff debuffToApply;
    public int debuffStacks = 0;
    public int upgradedDebuffStacksByAmout;

    public override bool IsUpgraded
    {
        get
        {
            return isUpgraded;
        }
        set
        {
            base.IsUpgraded = value;
            if (isUpgraded)
            {
                damage += upgradedDamageByAmount;
                debuffStacks += upgradedDebuffStacksByAmout;
            }
        }
    }

    public override void OnChipPlayed(PlayerController player, Enemy Target)
    {
        base.OnChipPlayed(player,Target);

        int tempDamage = damage;

        // Apply buffs/debuffs to damage
        if (player.IsPowered)
        {
            tempDamage += player.PoweredStacks;
        }

        if (player.IsDrained)
        {
            tempDamage = Mathf.FloorToInt(tempDamage * 0.8f);
        }

        Target.TakeDamage(tempDamage);

        //Now to apply debuffs
        if (debuffStacks > 0)
        {
            Target.AddEffect(debuffToApply, debuffStacks);
        }
    }

    void OnValidate()
    {
        ChipType = TypeOfChips.Attack;
        Debug.Log($"{name}: ChipType set to {ChipType}");
    }

}
