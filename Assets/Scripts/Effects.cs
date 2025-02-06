using System;
using UnityEngine;

/// <summary>
/// Just made this to keep track of Buffs and Debuffs in the game.
/// </summary>
[System.Serializable]
public static class Effects
{
    /// <summary>
    /// Type of Buffs
    /// </summary>
    public enum Buff
    {
        None,
        /// <summary>
        /// Gain shield at the end of your turn, equal to your amount of Galvanize stacks.
        /// </summary>
        Galvanize,
        /// <summary>
        /// Power attacks deal additional damage equal to your amount of power stacks.
        /// </summary>
        Power,
    }

    /// <summary>
    /// Type of Debuffs
    /// </summary>
    public enum Debuff
    {
        None,
        /// <summary>
        /// While drained, your attacks do 20% less damage.
        /// </summary>
        Drained,        
        /// <summary>
        /// While your are Jammed, you may not use Chips.
        /// </summary>
        Jam,
        /// <summary>
        /// While Worn Down, your ShieldBar provides 30% less shield.
        /// </summary>
        WornDown,
        /// <summary>
        /// Disable item use
        /// </summary>
        Redirect
    }

    /// <summary>
    /// Type of Special Effects
    /// </summary>
    public enum SpecialEffects
    {
        None,
        /// <summary>
        /// You take no Damage this Turn.
        /// </summary>
        Impervious,
        /// <summary>
        /// Your next Chip activates twice.
        /// </summary>
        Motivation,
        /// <summary>
        /// Gain 10 Scrap at the end of every combat.
        /// </summary>
        LuckyTrinket
    }

    /// <summary>
    /// General structure to represent a status effect.
    /// </summary>
    [System.Serializable]
    public struct StatusEffect
    {
        // Effect(buff,Debuff or SpecialEffect)
        public Enum Effect;
        // Stack count for buffs/debuffs (if applicable)
        public int StackCount;        

        public StatusEffect(Enum effect, int stackCount)
        {
            Effect = effect;
            StackCount = stackCount;
        }
    }

    // FOR SCRIPTABLES

    /// <summary>
    /// Buff for scritable
    /// </summary>
    [Serializable]
    public class TempBuffs
    {
        public Buff Buff;
        public int AmountToBuff;
    }
    /// <summary>
    /// Debuff for scriptable
    /// </summary>
    [Serializable]
    public class TempDeBuffs
    {
        public Debuff DeBuff;
        public int AmountToDeBuff;
    }

}