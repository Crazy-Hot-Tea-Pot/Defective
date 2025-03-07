using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Looter : Enemy
{
    [Header("Custom for Enemy type")]
    private int stolenScrap = 0;
    /// <summary>
    /// To keep track of stolen Scraps
    /// </summary>
    public int StolenScrap
    {
        get
        {
            return stolenScrap;
        }
        private set
        {
            stolenScrap = value;
        }
    }

    // To track the number of Swipes performed
    private int swipeCount = 0;

    private bool isShrouded;

    public bool IsShrouded
    {
        get { return isShrouded; }
        private set { isShrouded = value; }
    }

    // Start is called before the first frame update
    public override void Start()
    {
        if (EnemyName == null)
            EnemyName = "Looter";

        swipeCount = 0;
        StolenScrap = 0;

        EnemyType = EnemyManager.TypeOfEnemies.Looter;

        base.Start();
    }
    protected override void PerformIntent()
    {
        base.PerformIntent();

        // Since 100% on first chance just made it this way.

        if (swipeCount < 3) // First three turns are Swipe
        {
            //Swipe();
            Animator.SetTrigger("Intent 1");
        }
        else if (swipeCount == 3 && !IsShrouded) // After three Swipes, do Shroud
        {
            //Shroud();
            Animator.SetTrigger("Intent 2");
        }
        else if (IsShrouded) // After Shroud, perform Escape
        {
            //Escape();
            Animator.SetTrigger("Intent 3");
        }
    }
    /// <summary>
    /// Return all stolen Scraps upon killing
    /// </summary>
    public override void Die()
    {        
        ReturnStolenScrap();

        base.Die();
    }    

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
    }

    public override void CombatStart()
    {
        base.CombatStart();               
    }

    protected override (string intentText, IntentType intentType, int value) GetNextIntent()
    {
        if (swipeCount < 3)
            return ("Swipe", IntentType.Attack, 6);
        else if (swipeCount == 3 && !IsShrouded)
            return ("Shroud", IntentType.Shield, 10);
        else if (IsShrouded)
            return ("Escape", IntentType.Unique, 0);

        return ("Unknown", IntentType.None, 0);
    }

    /// <summary>
    ///  After the 3rd Swipe, perform Shroud
    /// </summary>
    public void Swipe()
    {
        Debug.Log($"{EnemyName} performs Swipe, dealing 4 damage and stealing 5 Scrap.");
        swipeCount++;

        StolenScrap += EnemyTarget.GetComponent<PlayerController>().TakeScrap(5);

        // Empower Swipe
        if (PowerStacks > 0)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().DamagePlayerBy(6 + PowerStacks);
            RemoveEffect(Effects.Buff.Power,0, true);
        }
        // Drained Swipe
        else if (DrainedStacks > 0)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().DamagePlayerBy(Mathf.FloorToInt(6 - 0.8f));
            RemoveEffect(Effects.Debuff.Drained,1, false);            
        }
        // Default Swipe
        else
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().DamagePlayerBy(6);
        }

    }

    public void Shroud()
    {
        Debug.Log($"{EnemyName} performs Shroud, gaining 10 Shield.");
        Shield += 10;

        isShrouded = true;
    }

    public void Escape()
    {
        Debug.Log($"{EnemyName} performs Escape, exiting the fight with {StolenScrap} Scrap.");

        CombatController.LeaveCombat(this.gameObject,0,null,null);

        EnemyManager.Instance.RemoveEnemy(this.gameObject);
    }
    private void ReturnStolenScrap()
    {
        Debug.Log($"{EnemyName} returns {StolenScrap} Scrap upon defeat.");

        EnemyTarget.GetComponent<PlayerController>().GainScrap(StolenScrap);

        // Reset stolen Scraps after returning
        StolenScrap = 0;
    }    
}