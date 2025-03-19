using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class EffectsInfo : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField]
    private TextMeshProUGUI DisplayEffectAmount;

    public void SetAmountOfEffect(int amount)
    {
        if (amount == 0)
        {
            DisplayEffectAmount.gameObject.SetActive(false);
        }
        else
        {
            DisplayEffectAmount.SetText("("+amount+")");
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        UiManager.Instance.CurrentUI.GetComponent<RoamingAndCombatUiController>().DisplayEffectInfo(this.gameObject.name);
    }
}
