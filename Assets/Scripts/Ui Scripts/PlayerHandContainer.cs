using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerHandContainer : MonoBehaviour
{
    public enum PlayerHandState
    {
        Reveal,
        Hide
    }
    [Header("Chips display settings")]
    // Adjust this to control overlap
    public float overlapOffset = 100f;
    // Optional for slight curving
    public float yOffset = 0f;
    // Ensures proper layering
    public float zOffset = -1f;

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
    private PlayerHandState handStatus = PlayerHandState.Hide;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }
    /// <summary>
    /// Reveal or hide player hand.
    /// </summary>
    /// <param name="WhatStateYouwantItToBe">Reveal or Hide</param>
    public void TogglePanel(PlayerHandState WhatStateYouwantItToBe)
    {        

        if (animator == null)
            animator = GetComponent<Animator>();

        switch (WhatStateYouwantItToBe)
        {
            case PlayerHandState.Reveal:
                if (HandIsVisible == PlayerHandState.Hide)
                {                    
                    SoundManager.PlayFXSound(OpenDeck);
                    animator.SetTrigger("Reveal");
                    HandIsVisible = PlayerHandState.Reveal;
                }
                break;
            case PlayerHandState.Hide:
                if(HandIsVisible == PlayerHandState.Reveal)
                {
                    SoundManager.PlayFXSound(CloseDeck);
                    animator.SetTrigger("Hide");
                    HandIsVisible = PlayerHandState.Hide;                    
                }                
                break;
        }        
    }
    public void FillPlayerHand()
    {
        // first delete all children
        foreach (Transform child in this.transform)
        {
            // Destroy each child
            Destroy(child.gameObject);
        }

        ChipManager.Instance.RefreshPlayerHand();

        for (int i = 0; i < ChipManager.Instance.PlayerHand.Count; i++)
        {
            var chip = ChipManager.Instance.PlayerHand[i];
            GameObject tempNewChip = Instantiate(ChipManager.Instance.chipPrefab, this.transform);

            tempNewChip.GetComponent<Chip>().NewChip = chip;
            tempNewChip.GetComponent<Chip>().SetChipModeTo(Chip.ChipMode.Combat);

            tempNewChip.GetComponent<RectTransform>().sizeDelta = new Vector2(776f, 600f);

            // Apply overlap and positioning
            tempNewChip.transform.localPosition = new Vector3(i * overlapOffset, Mathf.Sin(i * 0.1f) * yOffset, i * zOffset);
        }
        // Select the first chip using the Event System
        if (ChipManager.Instance.PlayerHand.Count > 0)
        {
            GameObject firstChip = this.transform.GetChild(0).gameObject;
            EventSystem.current.firstSelectedGameObject = firstChip;
            EventSystem.current.SetSelectedGameObject(firstChip);
        }

    }
    private void UpdateChipPositions()
    {
        for(int i = 0; i<this.transform.childCount; i++)
        {
            Transform chip = this.transform.GetChild(i);

            chip.localPosition = new Vector3(i * overlapOffset, Mathf.Sin(i * 0.1f) * yOffset, i * zOffset);

            Canvas canvas = chip.GetComponent<Canvas>();
            if(canvas == null)
            {
                canvas = chip.gameObject.AddComponent<Canvas>();
            }
            canvas.overrideSorting = true;
            canvas.sortingOrder = i;
        }
    }
    [ContextMenu("Update Chip layout")]
    private void UpdateChipLayout()
    {
        UpdateChipPositions();
    }
    void OnDestroy()
    {
        
    }
}
