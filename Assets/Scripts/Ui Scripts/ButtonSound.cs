using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSound : MonoBehaviour, IPointerEnterHandler, ISelectHandler,IPointerClickHandler
{
    public SoundFX ButtonSelectSound;
    public SoundFX ButtonClickSound;

    public void PlaySelectButtonSound()
    {
        SoundManager.PlayFXSound(ButtonSelectSound);
    }
    public void PlayButtonClickSound()
    {
        SoundManager.PlayFXSound(ButtonClickSound);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        PlaySelectButtonSound();
    }

    public void OnSelect(BaseEventData eventData)
    {
        PlaySelectButtonSound();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        PlayButtonClickSound();
    }
}
