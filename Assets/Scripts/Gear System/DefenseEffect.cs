using UnityEditor.Rendering;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDefenseEffect", menuName = "Gear/Effects/DefenseEffect")]
public class DefenseEffect : ItemEffect
{
    [Header("Armor values")]

    public bool specialEffectOnly;

    [Tooltip("Shield Amount")]
    public int baseShieldAmount;

    public override void Activate(PlayerController player, Item item, Enemy enemy = null)
    {
        int adjustedShieldAmount = baseShieldAmount + item.GetValueIncreaseBy();        

        if (player.SpendEnergy(energyCost))
        {

            // Adjust shield based on debuffs
            if (player.IsWornDown)
            {
                adjustedShieldAmount = Mathf.FloorToInt(adjustedShieldAmount * 0.7f); // Reduce shield by 30%
            }

            player.ApplyShield(adjustedShieldAmount);

            // Apply buffs and debuffs
            base.Activate(player, item, enemy);
        }
        else
        {
            SoundManager.PlayFXSound(item.ItemFailSound);
        }
    }
    public override string GetEffectDescription(Item item)
    {
        if (specialEffectOnly)
        {
            string tempString = "";
            // Corrected Loop
            foreach (Effects.TempBuffs effect in buffToApplyToPlayer)
            {
                tempString += $"Grants {effect.AmountToBuff} {effect.Buff.ToString()}. ";
            }
            // Remove trailing space
            return tempString.Trim();
        }
        else
        {
            int adjustedShield = baseShieldAmount + item.GetValueIncreaseBy();
            return $"Grants {adjustedShield} shield.";
        }
    }

}
