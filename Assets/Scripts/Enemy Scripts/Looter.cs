using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Looter : Enemy
{
    [Header("Custom for Enemy type")]
    // To track the number of Swipes performed
    private int swipeCount = 0;
    private bool isShrouded;
    private int stolenScrap = 0;
    private bool isWithLeader = false;
    private float baseSwipeDamage;
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
    public bool IsShrouded
    {
        get { return isShrouded; }
        private set { isShrouded = value; }
    }
    public bool IsWithLeader
    {
        get
        {
            return isWithLeader;
        }
        set
        {
            isWithLeader = value;
        }
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
    protected override void SetUpEnemy()
    {
        base.SetUpEnemy();

        switch (Difficulty)
        {
            case EnemyDifficulty.Easy:
                MaxHp = 30;
                baseSwipeDamage = 6;
                break;
            case EnemyDifficulty.Medium:
                MaxHp = 45;
                baseSwipeDamage = 6;
                break;
            case EnemyDifficulty.Hard:
                MaxHp = 60;
                baseSwipeDamage = 6;
                break;
        }
        if (CurrentHP <= 0)
            CurrentHP = MaxHp;
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
    protected override void Die()
    {        
        ReturnStolenScrap();

        base.Die();
    }    

    public override void CombatStart()
    {
        base.CombatStart();               
    }

    protected override List<(string, IntentType, int)> GetNextIntents()
    {
        if (swipeCount < 3)
            return new() { ("Swipe", IntentType.Attack, 6) };
        else if (swipeCount == 3 && !IsShrouded)
            return new() { ("Shroud", IntentType.Shield, 10) };
        else if (IsShrouded)
        {
            if (IsWithLeader)
            {
                swipeCount = 0;
                return new() { ("Swipe", IntentType.Attack, 6) };
            }
            else
                return new() { ("Escape", IntentType.Unique, 0) };
        }

        return new() { ("Unknown", IntentType.None, 0) };
    }


    /// <summary>
    ///  After the 3rd Swipe, perform Shroud
    /// </summary>
    public void Swipe()
    {
        Debug.Log($"{EnemyName} performs Swipe, dealing 4 damage and stealing 5 Scrap.");
        swipeCount++;

        StolenScrap += EnemyTarget.GetComponent<PlayerController>().TakeScrap(5);

        float adjustedDamage = baseSwipeDamage;
        // Empower Swipe
        if (PowerStacks > 0)        
            adjustedDamage += PowerStacks;                                
        // Drained Swipe
        if (DrainedStacks > 0)
            adjustedDamage += adjustedDamage * 0.8f;
        
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().DamagePlayerBy(adjustedDamage);
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

        CombatController.LeaveCombat(this.gameObject);

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