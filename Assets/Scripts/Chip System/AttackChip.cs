using UnityEngine;

[CreateAssetMenu(fileName = "NewAttackChip", menuName = "Chip System/Attack Chip")]
public class AttackChip : NewChip
{
    [Header("Attack Chip")]
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

    [Header("Attack Chip Sounds")]
    public SoundFX ChipHitFlesh;
    public SoundFX ChipHitMetal;

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

        //Play sound for damage
        if (Target.Shield <= 0)
        {
            //Play sound for enemyType
            switch (Target.EnemyType)
            {
                case EnemyManager.EnemyType.Looter:
                case EnemyManager.EnemyType.GangLeader:
                case EnemyManager.EnemyType.TicketVendor:
                case EnemyManager.EnemyType.Inspector:
                    SoundManager.PlayFXSound(ChipHitFlesh);
                    break;
                case EnemyManager.EnemyType.SecurityDrone:
                case EnemyManager.EnemyType.Maintenancebot:
                case EnemyManager.EnemyType.Garbagebot:
                    SoundManager.PlayFXSound(ChipHitMetal);
                    break;                
                default:
                    SoundManager.PlayFXSound(ChipActivate);
                    break;
            }            
        }
        else
            SoundManager.PlayFXSound(ChipHitShield);

        //Now to apply debuffs
        if (debuffStacks > 0)
        {
            Target.AddEffect(debuffToApply, debuffStacks);
        }
    }

    public override void OnChipPlayed(PlayerController player, PuzzleRange Target)
    {
        base.OnChipPlayed(player, Target);
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
    }

    void OnValidate()
    {
        ChipType = TypeOfChips.Attack;       
    }

}
