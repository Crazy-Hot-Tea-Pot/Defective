using UnityEngine;

[CreateAssetMenu(fileName = "NewLeechEffect", menuName = "Chip System/Skill Effects/Leech")]
public class LeechEffect : SkillEffects
{
    public int damageAmount;

    protected override void ChipUpgraded()
    {
        base.ChipUpgraded();

        if (IsUpgraded)
            damageAmount += amountToUpgradeBy;                    
    }

    public override void ApplyEffect(PlayerController player, Enemy target, NewChip card)
    {
        if (target != null)
        {            
            float damage = card.IsUpgraded ? damageAmount + 5 : damageAmount;
            target.TakeDamage(damage);
            player.RecoverEnergy(damage);
            Debug.Log("Leech Effect: Dealt " + damage + " damage and recovered energy.");
        }
    }
}
