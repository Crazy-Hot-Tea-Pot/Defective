using UnityEngine;

public class AnimationEventForwarder : MonoBehaviour
{
    public RoamingAndCombatUiController parent;
   

    public void OnAnimationEventContPrepCombatStart()
    {
        parent.ContPrepCombatStart();
    }
}
