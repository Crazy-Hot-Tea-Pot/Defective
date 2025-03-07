using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationHandler : MonoBehaviour
{
    public Enemy thisScript;


    public void CallMethod(string methodName)
    {
        thisScript.Invoke(methodName, 0f);
    }
    public void AnimationEndTurnTriggerForNoAnimaton(string method)
    {
        CallMethod(method);
        StartCoroutine(EndAfterSecond());
    }
    public void AnimationEndTurnTrigger()
    {
        thisScript.EndTurn();
    }
    public void AnimationFinishDeathTrigger()
    {
        thisScript.FinishDeath();
    }
    private IEnumerator EndAfterSecond()
    {
        yield return new WaitForSeconds(1f);

        AnimationEndTurnTrigger();
    }    
}
