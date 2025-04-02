using UnityEngine;
using static Effects;

[CreateAssetMenu(fileName = "NewEquipmentEffect", menuName = "Gear/Effects/EquipmentEffect")]
public class EquipmentEffect : ItemEffect
{
    [Header("Equipment values")]
    public string SpecialEffectDescription;    

    /// <summary>
    /// If this effect is passive
    /// </summary>
    public bool HasPassiveEffect;

    public SpecialEffects passiveEffect;

    public bool IsPassiveEffectActive
    {
        get
        {
            return isPassiveEffectActive;
        }
        set
        {
            isPassiveEffectActive = value;
        }
    }    

    private bool isPassiveEffectActive = false;

    public override void Activate(PlayerController player, Item item, Enemy enemy = null)
    {
        if (player.SpendEnergy(energyCost))
        {
            LinkedItem = item;

            //Play Item Effect
            base.Activate(player, item, enemy);
        }
        else
        {
            SoundManager.PlayFXSound(item.ItemFailSound);

            Debug.Log("Not enough energy to use Equipment.");
        }
    }
    public override string GetEffectDescription(Item item)
    {
        return $"Special Effect: {SpecialEffectDescription}";
    }

    public override void Equip()
    {
        base.Equip();

        if (HasPassiveEffect && IsEquipped)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().AddEffect(passiveEffect,0,true);
            SoundManager.PlayFXSound(LinkedItem.ItemActivateSound);
            IsPassiveEffectActive = true;
        }
        else if(HasPassiveEffect)
        {            
            SoundManager.PlayFXSound(LinkedItem.ItemDeactivateSound);
            IsPassiveEffectActive = false;
        }                   
    }
}