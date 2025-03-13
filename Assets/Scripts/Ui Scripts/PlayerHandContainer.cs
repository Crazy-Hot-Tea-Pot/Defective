using UnityEngine;
using UnityEngine.UI;

public class PlayerHandContainer : MonoBehaviour
{
    public enum PlayerHandState
    {
        Open,
        Close
    }

    public GameObject PlayerHand;
    public Button SliderButton;

    [Header("Sounds")]
    public SoundFX OpenDeck;
    public SoundFX CloseDeck;
    public PlayerHandState HandIsVisible
    {
        get
        {
            return handStatus;
        }
        set
        {
            handStatus = value;
        }
    }

    private Animator animator;
    private PlayerHandState handStatus=PlayerHandState.Close;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        SliderButton.onClick.AddListener(OpenOrCloseHandByButtonClick);
    }
    /// <summary>
    /// Open or close player hand.
    /// </summary>
    /// <param name="WhatStateYouwantItToBe">Open or Close</param>
    public void TogglePanel(PlayerHandState WhatStateYouwantItToBe)
    {        

        if (animator == null)
            animator = GetComponent<Animator>();

        switch (WhatStateYouwantItToBe)
        {
            case PlayerHandState.Open:
                if (HandIsVisible == PlayerHandState.Close)
                {                    
                    SoundManager.PlayFXSound(OpenDeck);
                    animator.SetTrigger("SlideInScreen");
                    HandIsVisible = PlayerHandState.Open;
                }
                break;
            case PlayerHandState.Close:
                if(HandIsVisible == PlayerHandState.Open)
                {
                    SoundManager.PlayFXSound(CloseDeck);
                    animator.SetTrigger("SlideOutScreen");
                    HandIsVisible = PlayerHandState.Close;                    
                }                
                break;
        }        
    }
    public void FillPlayerHand()
    {
        // first delete all children
        foreach (Transform child in PlayerHand.transform)
        {
            // Destroy each child
            Destroy(child.gameObject);
        }

        //Change this so it only draws new cards via the combat controller
        //if(GameManager.Instance.CurrentGameMode = GameManager.GameMode.Combat)
        ChipManager.Instance.RefreshPlayerHand();

        foreach (var chip in ChipManager.Instance.PlayerHand)
        {
            GameObject tempNewChip = Instantiate(ChipManager.Instance.chipPrefab, PlayerHand.transform);

            tempNewChip.GetComponent<Chip>().Mode = Chip.ChipMode.Combat;

            tempNewChip.GetComponent<Chip>().NewChip = chip;
        }
    }
    private void OpenOrCloseHandByButtonClick()
    {
        if(HandIsVisible == PlayerHandState.Open)
            TogglePanel(PlayerHandState.Close);
        else
            TogglePanel(PlayerHandState.Open);
    }
    void OnDestroy()
    {
        SliderButton.onClick.RemoveListener(OpenOrCloseHandByButtonClick);
    }
}
