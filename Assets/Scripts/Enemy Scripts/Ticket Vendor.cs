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
    private int nextIntentRoll;

    // Start is called before the first frame update
    public override void Start()
    {
        if (EnemyName == null)
            EnemyName = "Ticket Vendor";

        //Add Common Chips Todrop
        DroppedChips = ChipManager.Instance.GetChipsByRarity(NewChip.ChipRarity.Common);

        base.Start();
    }
    public override void CombatStart()
    {
        nextIntentRoll = Random.Range(1, 11);
        base.CombatStart();
    }

    public override void EndTurn()
    {
        nextIntentRoll = Random.Range(1, 11);
        base.EndTurn();
    }
    public override void PerformIntentTrigger(string intentName)
    {
        base.PerformIntentTrigger(intentName);

        switch (intentName)
        {
            case "Halt": 
                Halt(); 
                break;
            case "Confiscate": 
                Confiscate(); 
                break;
            case "Redirect": 
                Redirect(); 
                break;
            default:
                Debug.LogWarning($"Intent '{intentName}' not handled in {EnemyName}.");
                break;
        }
    }
    protected override void PerformIntent()
    {
        base.PerformIntent();

        switch (NextIntent.intentText)
        {
            case "Halt":
                //Halt();
                Animator.SetTrigger("Intent 1");
                break;
            case "Confiscate":
                //Confiscate();
                Animator.SetTrigger("Intent 2");
                break;
            case "Redirect":
                //Redirect();
                Animator.SetTrigger("Intent 3");
                break;
            default:
                Debug.LogWarning("Shouldn't hit here!");
                break;
        }

        StartCoroutine(PrepareToEndTurn());
    }
    protected override (string intentText, IntentType intentType, int value) GetNextIntent()
    {
        if (nextIntentRoll <= 3)
            return ("Halt", IntentType.Attack, 9);
        else if (nextIntentRoll <= 7)
            return ("Confiscate", IntentType.Debuff, 7);
        else
            return ("Redirect", IntentType.Debuff, 7);
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
