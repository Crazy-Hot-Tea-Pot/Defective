using UnityEngine;

public class IntentHandler : StateMachineBehaviour
{    
    [Tooltip("When (in seconds) the intent effect should trigger during the animation.")]
    public float triggerPoint = 0f;

    [Tooltip("Name of the intent this state represents")]
    public string intentName;

    private bool hasTriggered = false;

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float currentTime = stateInfo.length * stateInfo.normalizedTime;

        if(!hasTriggered && currentTime >= triggerPoint) 
        {
            animator.GetComponentInParent<Enemy>()?.PerformIntentTrigger(intentName);
            hasTriggered = true;
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        hasTriggered = false;
    }
}
