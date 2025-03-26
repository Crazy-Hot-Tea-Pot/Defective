using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "BuffDebuffEffect", menuName = "Chip System/Skill Effects/Buff And Debuffs")]
public class BuffAndDebuffsEffect : SkillEffects
{
    public Effects.Buff buffToApply;
    public int amountOfBuffToApply;

    [System.Serializable]
    public class DebuffRemovalInfo
    {
        public Effects.Debuff debuffType;
        public int amountToRemove = 1;
        public int upgradedAmount = 0;
        public bool removeAll = false;
    }

    public List<DebuffRemovalInfo> DebuffsToRemove = new();


    protected override void ChipUpgraded()
    {
        base.ChipUpgraded();
        if (IsUpgraded)
        {
            amountOfBuffToApply += amountToUpgradeBy;
        }
        
    }
    public override void ApplyEffect(PlayerController player)
    {
        // Apply buff
        player.AddEffect(buffToApply, amountOfBuffToApply);

        // Remove debuffs
        foreach (var debuff in DebuffsToRemove)
        {
            int amount = debuff.amountToRemove;

            if (IsUpgraded)
                amount += debuff.upgradedAmount;

            player.RemoveEffect(debuff.debuffType, amount, debuff.removeAll);
        }
    }
}
