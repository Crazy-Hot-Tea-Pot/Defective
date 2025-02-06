using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
/// <summary>
/// Ticket Vendor: 90 HP (Robot Enemy) SUBWAY EXCLUSIVE
/// Intents:
/// Halt: Deal 9 Damage, Apply 1 Worn & 1 Drained. (30%)
/// Confiscate: Deal 7 Damage and Disable 2 of your Chips. (40%)
/// Redirect: Deal 7 Damage and Disable an Ability for a Turn. (30%)
/// </summary>
public class TicketVendor : Enemy
{
    private int intentRandom;
    // Start is called before the first frame update
    public override void Start()
    {
        if (EnemyName == null)
            EnemyName = "Ticket Vendor";

        //Add Common Chips Todrop
        DroppedChips = ChipManager.Instance.GetChipsByRarity(NewChip.ChipRarity.Common);

        base.Start();
    }
    protected override void PerformIntent()
    {

        if (intentRandom <= 3)
            Halt();
        else if (intentRandom <= 7)
            Confiscate();
        else
            Redirect();

        base.PerformIntent();
    }
    protected override Intent GetNextIntent()
    {
        //decide next intent
        intentRandom = Random.Range(1, 11);

        if (intentRandom <= 3)
            return new Intent("Halt", Color.red, 9, "Deals damage and applies Worn/Drained");
        else if (intentRandom <= 7)
            return new Intent("Confiscate", Color.red, 7, "Disables 2 chips");
        else
            return new Intent("Redirect", Color.red, 7, "Disables an ability");
    }
    private void Redirect()
    {
        EnemyTarget.GetComponent<PlayerController>().DamagePlayerBy(7);

        EnemyTarget.GetComponent<PlayerController>().AddEffect(Effects.Debuff.Redirect,1);
    }
    private void Confiscate()
    {
        EnemyTarget.GetComponent<PlayerController>().DamagePlayerBy(7);

        int temp1 = Random.Range(0, ChipManager.Instance.PlayerHand.Count);

        int temp2 = Random.Range(0, ChipManager.Instance.PlayerHand.Count);

        while (temp1 == temp2)
        {
            temp2 = Random.Range(0, ChipManager.Instance.PlayerHand.Count);
        }

        ChipManager.Instance.PlayerHand[temp1].GetComponent<Chip>().NewChip.GetComponent<NewChip>().IsActive = false;
        ChipManager.Instance.PlayerHand[temp2].GetComponent<Chip>().NewChip.GetComponent<NewChip>().IsActive = false;
    }
    private void Halt()
    {
        EnemyTarget.GetComponent<PlayerController>().DamagePlayerBy(9);
        EnemyTarget.GetComponent<PlayerController>().AddEffect(Effects.Debuff.WornDown, 1);
        EnemyTarget.GetComponent<PlayerController>().AddEffect(Effects.Debuff.Drained, 1);
    }
}
