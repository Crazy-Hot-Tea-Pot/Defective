using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarbageBot : Enemy
{
    // Store the roll for consistency
    private int nextIntentRoll;
    public override void Start()
    {
        if (EnemyName == null)
            EnemyName = "GarbageBot";

        //Add Rare Chips Todrop
        DroppedChips = ChipManager.Instance.GetChipsByRarity(NewChip.ChipRarity.Rare);

        base.Start();
    }
    public override void CombatStart()
    {
        //Roll once at the start
        nextIntentRoll = Random.Range(1, 11);

        base.CombatStart();
    }
    public override void EndTurn()
    {
        nextIntentRoll = Random.Range(1, 11);
        base.EndTurn();
    }
    protected override void PerformIntent()
    {
        switch (NextIntent.intentText)
        {
            case "Compact":
                Compact();
                break;
            case "Shred":
                Shred();
                break;
            case "PileOn":
                PileOn();
                break;
            default:
                Debug.LogWarning("Should never hit here!");
                break;
        }

        base.PerformIntent();
    }
    protected override (string intentText, IntentType intentType, int value) GetNextIntent()
    {
        if (nextIntentRoll <= 3)
            return ("Compact", IntentType.Attack, 15);
        else if (nextIntentRoll <= 6)
            return ("Shred", IntentType.Attack, 7);
        else
            return ("Pile On", IntentType.Attack, 10);
    }

    /// <summary>
    /// Deals 15 damage.
    /// Has 30% chance
    /// </summary>
    private void Compact()
    {
        Debug.Log("Garbage Bot uses Compact!");
        EnemyTarget.GetComponent<PlayerController>().DamagePlayerBy(15);        
    }
    /// <summary>
    /// Deal 10 Damage
    /// Apply 1 jam
    /// 50% chance
    /// </summary>
    private void PileOn()
    {
        Debug.Log("Garbage Bot uses Pile On!");        

        EnemyTarget.GetComponent<PlayerController>().DamagePlayerBy(10);
        EnemyTarget.GetComponent<PlayerController>().AddEffect(Effects.Debuff.Jam, 1);
    }
    /// <summary>
    /// Gain 7 shield
    /// Deal 7 damage
    /// 30% chance
    /// </summary>
    private void Shred()
    {
        //Play Sound
        SoundManager.PlayFXSound(SoundFX.ShredGarbageBot,this.gameObject.transform);

        ApplyShield(7);
        EnemyTarget.GetComponent<PlayerController>().DamagePlayerBy(7);
    }
}
