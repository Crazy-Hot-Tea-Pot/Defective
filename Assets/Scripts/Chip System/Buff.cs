using UnityEngine;
[CreateAssetMenu(fileName = "NewBuffEffect", menuName = "Chip System/Skill Effects/Buff")]
public class Buff : SkillEffects
{

    public Effects.Buff buffToApply;
    public int amountOfBuffToApply;

    protected override void ChipUpgraded()
    {
        base.ChipUpgraded();
        if(IsUpgraded)           
            amountOfBuffToApply += amountToUpgradeBy;       
    }
    public override void ApplyEffect(PlayerController player)
    {
        player.AddEffect(buffToApply, amountOfBuffToApply);
    }
}
