using UnityEngine;

[CreateAssetMenu(fileName = "NewDefenseEffect", menuName = "Gear/Effects/DefenseEffect")]
public class DefenseEffect : ItemEffect
{
    [Header("Armor values")]
    [Tooltip("Shield Amount")]
    public int baseShieldAmount;

    public override void Activate(PlayerController player, Item item, Enemy enemy = null)
    {
        int adjustedShieldAmount = baseShieldAmount + item.GetValueIncreaseBy();

        // decrease for energy cost
        int adjustedEnergyCost = energyCost - item.GetEnergyCostDecreaseBy();

        if (player.SpendEnergy(adjustedEnergyCost))
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
}
