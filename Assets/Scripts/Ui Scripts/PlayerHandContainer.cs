using UnityEngine;
using UnityEngine.UI;

public class PlayerHandContainer : MonoBehaviour
{
    public GameObject PlayerHand;
    public Button SliderButton;
    public bool PanelIsVisible
    {
        get
        {
            return isVisible;
        }
        set
        {
            isVisible = value;
        }
    }
    private Animator animator;
    private bool isVisible = false;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        SliderButton.onClick.AddListener(TogglePanel);
    }

    public void TogglePanel()
    {
        isVisible = !isVisible;

        if(!isVisible)
                FillPlayerHand();

        if(animator==null)
            animator = GetComponent<Animator>();

        animator.SetTrigger("Slide");


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
}
