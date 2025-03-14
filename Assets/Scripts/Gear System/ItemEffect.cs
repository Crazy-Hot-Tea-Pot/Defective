using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemEffect : ScriptableObject
{

    public bool IsEquipped
    {
        get
        {
            return isEquipped;
        }
        set
        {
            isEquipped = value;

            Equipped();
        }
    }

    public enum ConditionEffect { 
        None,
        /// <summary>
        /// If Enemy has less than 50% hp.
        /// </summary>
        LessThanHalfHealth 
    }
    [Tooltip("Cost to use the ItemEffect")]
    public float energyCost;

    [Space(20)]

    [Tooltip("If effect is only applied on special conditions.")]
    public bool SpecialConditionEffect = false;
    public ConditionEffect Condition = ConditionEffect.None;
    public List<Effects.TempDeBuffs> deBuffEffectToApplyToEnemy = new();


    [Header("Effects if applicable to apply to player on Use")]
    public List<Effects.TempBuffs> buffToApplyToPlayer = new();
    public List<Effects.TempDeBuffs> debuffToApplyToPlayer = new();
    public Effects.SpecialEffects effectToApplyToPlayer;

    [Space(20)]
    public bool HitAllTargets = false;

    private bool isEquipped = false;

    public virtual void Activate(PlayerController player, Item item, Enemy enemy = null)
    {        

            // Apply buffs
            foreach (Effects.TempBuffs buff in buffToApplyToPlayer)
            {
                player.AddEffect(buff.Buff, buff.AmountToBuff);
            }

            // Apply debuffs
            foreach (Effects.TempDeBuffs debuff in debuffToApplyToPlayer)
            {
                player.AddEffect(debuff.DeBuff, debuff.AmountToDeBuff);
            }

            // Apply special effects
            if (effectToApplyToPlayer != Effects.SpecialEffects.None)
            {
                player.AddEffect(effectToApplyToPlayer);
            }        
    }

    //Todo put error sound effect
    public virtual void Activate(PlayerController player, Item item, PuzzleRange puzzle = null)
    {
        //Put a sound here
    }
    public virtual string GetEffectDescription(Item item)
    {
        return "fail";
    }

    protected virtual void Equipped()
    {

    }
}
