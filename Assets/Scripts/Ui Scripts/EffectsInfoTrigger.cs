using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EffectsInfoTrigger : MonoBehaviour, IPointerEnterHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        UiManager.Instance.CurrentUI.GetComponent<RoamingAndCombatUiController>().DisplayEffectInfo(this.gameObject.name);
    }
}
