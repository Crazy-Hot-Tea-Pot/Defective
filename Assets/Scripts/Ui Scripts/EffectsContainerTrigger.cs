using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EffectsContainerTrigger : MonoBehaviour, IPointerExitHandler
{
    public void OnPointerExit(PointerEventData eventData)
    {
        // Hide the effect description only when leaving the whole container
        UiManager.Instance.CurrentUI.GetComponent<RoamingAndCombatUiController>().HideEffectInfo();
    }
}
