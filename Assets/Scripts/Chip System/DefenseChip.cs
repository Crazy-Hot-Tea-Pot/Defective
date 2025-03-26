using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDefenseChip", menuName = "Chip System/Defense Chip")]
public class DefenseChip : NewChip
{    
    [System.Serializable]
    public class DebuffInfo
    {
        public Effects.Debuff debuffType;
        public int amountToDebuff;
        public int upgradedAmountToRemoveBy;
        public bool removeAll;
    }
    [Header("Defense chip")]
    /// <summary>
    /// How much shield the card will give.
    /// </summary>
    public int shieldAmount;

    /// <summary>
    /// The amount to upgrade by.
    /// </summary>
    public int upgradedShieldAmountToApply;

    /// <summary>
    /// Effect to give.
    /// </summary>
    public Effects.SpecialEffects effectToApply = Effects.SpecialEffects.None;

    /// <summary>
    /// The buff to apply.
    /// </summary>
    public Effects.Buff buffToApply=Effects.Buff.None;

    /// <summary>
    /// Amount of that buff to apply.
    /// </summary>
    public int buffStacks = 0;

    /// <summary>
    /// Amount to upgrade the buff amount by.
    /// </summary>
    public int upgradedBuffStacksByAmout;

    /// <summary>
    /// List of Debuffs to remove.
    /// Put the main debuff to remove first and the rest will be if upgraded.
    /// </summary>
    [Tooltip("The first debuff is the main Debuff to remove. Any more will be removed if card is upgraded")]
    public List<DebuffInfo> deBuffsToRemove = new List<DebuffInfo>();

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
                shieldAmount += upgradedShieldAmountToApply;
                buffStacks += upgradedBuffStacksByAmout;

                foreach (DebuffInfo debuffInfo in deBuffsToRemove)
                {
                    debuffInfo.amountToDebuff += debuffInfo.upgradedAmountToRemoveBy;
                }
            }
        }
    }

    public override void OnChipPlayed(PlayerController player)
    {
        base.OnChipPlayed(player);
        
        //Apply worndown effect
        if (player.IsWornDown)
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().ApplyShield(Mathf.FloorToInt(shieldAmount*0.7f));
        else
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().ApplyShield(shieldAmount);

        //ApplyEffect to Player
        if(effectToApply != Effects.SpecialEffects.None)
            player.AddEffect(effectToApply);        

        //Apply buffs to Player
        if(buffToApply != Effects.Buff.None)
            player.AddEffect(buffToApply, buffStacks);

        // Remove specified debuffs will only remove first one if not upgraded
        foreach (DebuffInfo debuffToRemove in deBuffsToRemove)
        {
            try
            {
                player.RemoveEffect(debuffToRemove.debuffType, debuffToRemove.amountToDebuff, debuffToRemove.removeAll);
            }
            catch
            {
                Debug.LogWarning($"Debuff {debuffToRemove.debuffType} not recognized.");
            }                                                                       

            //Break out if not upgraded
            if (!IsUpgraded)
                break;           
        }

        //Play sound for activate
        SoundManager.PlayFXSound(ChipActivate);
    }

    void OnValidate()
    {
        ChipType = TypeOfChips.Defense;
    }

}
