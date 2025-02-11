using UnityEngine;
using static Effects;

[CreateAssetMenu(fileName = "NewEquipmentEffect", menuName = "Gear/Effects/EquipmentEffect")]
public class EquipmentEffect : ItemEffect
{
    [Header("Equipment values")]
    public string SpecialEffectName;    

    /// <summary>
    /// If this effect is passive
    /// </summary>
    public bool HasPassiveEffect;

    public Effects.SpecialEffects passiveEffect;

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
            //Play Item Effect
            SoundManager.PlayFXSound(ItemActivate);
            base.Activate(player, item, enemy);
        }
        else
        {
            SoundManager.PlayFXSound(ItemFail);

            Debug.Log("Not enough energy to use Equipment.");
        }
    }
    protected override void Equipped()
    {
        base.Equipped();

        if (HasPassiveEffect && IsEquipped)
        {
                if (!IsPassiveEffectActive)
                {
                    GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().AddEffect(passiveEffect);
                    IsPassiveEffectActive = true;
                }
                else if(IsPassiveEffectActive)
                {
                    GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().RemoveEffect(passiveEffect);
                }            
        }
    }
}
