using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationHandler : MonoBehaviour
{
    public Enemy thisScript;


    public void CallIntent(string methodName)   
    {
        thisScript.Invoke(methodName, 0f);
    }
    public void AnimationEndTurnTriggerForNoAnimaton(string method)
    { 
        CallIntent(method);
        StartCoroutine(AnimationEndsAfterAmountOfSeconds(1f));
    }
    public void AnimationEndTurnTrigger()
    {
        thisScript.EndTurn();
    }
    public void AnimationFinishDeathTrigger()
    {
        thisScript.FinishDeath();
    }
    private IEnumerator AnimationEndsAfterAmountOfSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        AnimationEndTurnTrigger();
    }    
}
