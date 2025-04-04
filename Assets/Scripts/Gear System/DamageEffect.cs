﻿using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDamageEffect", menuName = "Gear/Effects/DamageEffect")]
public class DamageEffect : ItemEffect
{
    [Header("Weapon values")]
    [Tooltip("Damage dealt")]
    public int baseDamage;

    public override void Activate(PlayerController player, Item item, Enemy enemy)
    {
        float adjustedDamage = baseDamage + item.GetValueIncreaseBy();

        if (player.SpendEnergy(energyCost))
        {

            if (player.IsPowered)
                adjustedDamage += player.PoweredStacks;

            if (player.IsDrained)
            {
                //Reduce damage by 20% for drained
                adjustedDamage = Mathf.Round(adjustedDamage * 0.8f * 100f) / 100f;
            }

            //Do Damage to enemy
            enemy?.TakeDamage(adjustedDamage);

            //Any conditions applied after hit with weapon
            if (SpecialConditionEffect)
            {
                switch (Condition)
                {
                    case ConditionEffect.LessThanHalfHealth:

                        if (enemy.CurrentHP <= (enemy.MaxHp / 2))
                        {
                            foreach(Effects.TempDeBuffs debuff in deBuffEffectToApplyToEnemy)
                            {
                                enemy.AddEffect(debuff.DeBuff, debuff.AmountToDeBuff);
                            }
                        }

                    break;
                }
            }
            foreach (Effects.TempDeBuffs tempDeBuffs in deBuffEffectToApplyToEnemy) 
            {
                enemy.AddEffect(tempDeBuffs.DeBuff, tempDeBuffs.AmountToDeBuff);
            }
            // Apply buffs and debuffs
            base.Activate(player, item, enemy);
        }
        else
        {
            SoundManager.PlayFXSound(item.ItemFailSound);

            Debug.Log("Not enough energy to use Weapon.");
        }
    }

    public override void Activate(PlayerController player, Item item, PuzzleRange puzzle)
    {
        int adjustedDamage = baseDamage + item.GetValueIncreaseBy();        

        if (player.SpendEnergy(energyCost))
        {
            //Play Item Effect
            SoundManager.PlayFXSound(item.ItemActivateSound);

            if (player.IsPowered)
                adjustedDamage += player.PoweredStacks;

            if (player.IsDrained)
                adjustedDamage = Mathf.FloorToInt(adjustedDamage * 0.8f); // Reduce damage by 20%

            //Do Damage to enemy
            puzzle?.TakeDamage(adjustedDamage);

        }
        else
        {
            SoundManager.PlayFXSound(item.ItemFailSound);

            Debug.Log("Not enough energy to use Weapon.");
        }
    }

    public override string GetEffectDescription(Item item)
    {
        int adjustedDamage = baseDamage + item.GetValueIncreaseBy();
        return $"Deals {adjustedDamage} damage.";
    }

}