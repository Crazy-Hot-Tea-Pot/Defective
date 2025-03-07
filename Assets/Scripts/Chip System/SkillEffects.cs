﻿using UnityEngine;
/// <summary>
/// The base class for Skill Effects
/// </summary>
public class SkillEffects : ScriptableObject
{
    protected bool isUpgraded;
    
    public int amountToUpgradeBy;

    public virtual bool IsUpgraded
    {
        get
        {
            return isUpgraded;
        }
        set
        {
            isUpgraded = value;
            ChipUpgraded();
        }
    }
    protected virtual void ChipUpgraded()
    {

    }

    /// <summary>
    /// Default method for Player-only effects
    /// </summary>
    /// <param name="player"></param>
    public virtual void ApplyEffect(PlayerController player)
    {
        Debug.LogWarning("ApplyEffect method needs to be overridden for player-only effects.");
    }
    /// <summary>
    /// Optional method for Player-enemy interaction effects
    /// </summary>
    /// <param name="player"></param>
    /// <param name="target"></param>
    public virtual void ApplyEffect(PlayerController player, Enemy target)
    {
        Debug.LogWarning("ApplyEffect method needs to be overridden for player-enemy interaction effects.");
    }
    /// <summary>
    /// Optional method for Player-enemy interaction effects and need card info
    /// </summary>
    /// <param name="player"></param>
    /// <param name="target"></param>
    /// <param name="card"></param>
    public virtual void ApplyEffect(PlayerController player, Enemy target, NewChip card)
    {
        Debug.LogWarning("ApplyEffect method needs to be overridden for player-enemy interaction effects.");
    }
}
