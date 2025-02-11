using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDamageEffect", menuName = "Gear/Effects/DamageEffect")]
public class DamageEffect : ItemEffect
{
    [Header("Weapon values")]
    [Tooltip("Damage dealt")]
    public int baseDamage;

    public override void Activate(PlayerController player, Item item, Enemy enemy)
    {
        int adjustedDamage = baseDamage + item.GetValueIncreaseBy();

        // decrease for energy cost
        int adjustedEnergyCost = energyCost - item.GetEnergyCostDecreaseBy();

        if (player.SpendEnergy(adjustedEnergyCost))
        {
            //Play Item Effect
            SoundManager.PlayFXSound(ItemActivate);

            if (player.IsPowered)
                adjustedDamage += player.PoweredStacks;

            if(player.IsDrained)
                adjustedDamage = Mathf.FloorToInt(adjustedDamage * 0.8f); // Reduce damage by 20%

            //Do Damage to enemy
            enemy?.TakeDamage(adjustedDamage);

            //Any conditions applied after hit with weapon
            if (SpecialConditionEffect)
            {
                switch (Condition)
                {
                    case ConditionEffect.LessThanHalfHealth:

                        if (enemy.CurrentHP <= (enemy.maxHP / 2))
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
            SoundManager.PlayFXSound(ItemFail);

            Debug.Log("Not enough energy to use Weapon.");
        }
    }
}