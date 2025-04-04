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
    public SoundFX ChipHitShield;
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

        float tempDamage = damage;

        // Apply buffs/debuffs to damage
        if (player.IsPowered)
        {
            tempDamage += player.PoweredStacks;
        }

        if (player.IsDrained)
        {
            //Reduce damage by 20% for drained
            tempDamage = Mathf.Round(tempDamage * 0.8f *100f)/100f;
        }

        for(int i = 0; i < numberOfHits; i++)
        {
            Target.TakeDamage(tempDamage);
        }      

        //Play sound for damage
        if (Target.Shield <= 0)
        {
            //Play sound for enemyType
            switch (Target.EnemyIs)
            {
                case Enemy.IsEnemy.Human:
                    SoundManager.PlayFXSound(ChipHitFlesh);
                    break;
                case Enemy.IsEnemy.Robot:
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
