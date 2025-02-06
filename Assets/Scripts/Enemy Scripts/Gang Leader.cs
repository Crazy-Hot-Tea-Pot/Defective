using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GangLeader : Enemy
{
    public GameObject Looter1;
    public GameObject Looter2;

    // Start is called before the first frame update
    public override void Start()
    {
        if (EnemyName == null)
            EnemyName = "Looter";
        
        base.Start();
    }

    protected override void PerformIntent()
    {
        if(Looter1.activeInHierarchy || Looter2.activeInHierarchy)
        {
            if (Random.Range(1, 11) < 5)
                Threaten();
            else
                Intimidate();
        }
        //Once Looters are defeated
        else
        {
            if (Random.Range(1, 11) < 4)
                Disorient();
            else
                Cower();
        }
    }
    /// <summary>
    /// Deal 6 Damage, Apply 1 Jam.
    /// 40% chance
    /// </summary>    
    private void Disorient()
    {
        EnemyTarget.GetComponent<PlayerController>().AddEffect(Effects.Debuff.Jam, 1);
        EnemyTarget.GetComponent<PlayerController>().DamagePlayerBy(6);
    }
    /// <summary>
    ///  Gain 15 ShieldBar, Deal 3 Damage.
    ///  60% chance
    /// </summary>    
    private void Cower()
    {
        ApplyShield(15);
        EnemyTarget.GetComponent<PlayerController>().DamagePlayerBy(3);
    }
    /// <summary>
    /// Self and Looters gain 2 Power.
    /// 50% change of this
    /// </summary>
    private void Threaten()
    {
        this.AddEffect(Effects.Buff.Power, 2);

        //Come back and remove this for better logic
        try
        {
            Looter1.GetComponent<Looter>().AddEffect(Effects.Buff.Power, 2);
            Looter2.GetComponent<Looter>().AddEffect(Effects.Buff.Power, 2);
        }
        catch
        {
            Debug.Log("One of looters are dead.");
        }
    }
    /// <summary>
    /// Apply 1 Worn and 1 Drained to Player.
    /// 50% chance
    /// </summary>
    private void Intimidate()
    {
        EnemyTarget.GetComponent<PlayerController>().AddEffect(Effects.Debuff.WornDown, 1);
        EnemyTarget.GetComponent<PlayerController>().AddEffect(Effects.Debuff.Drained, 1);
    }
}
