using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LootController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI lootDisplayName;
    public Image lootImage;
    public Button DropButton;
    public Button KeepButton;
    public GameObject ChipinfoPrefab;
    public GameObject GearInfoPrefab;
    public GameObject EffectPrefab;

    public NewChip NewChip
    {
        get
        {
            return newChip;
        }
        set
        {
            newChip = value;
        }
    }
    public Item NewItem
    {
        get
        {
            return newItem;
        }
        set
        {
            newItem = value;
        }
    }

    private Animator animator;
    private NewChip newChip;
    private Item newItem;
    private GameObject chipinfoDisplay;
    private GameObject gearInfoDisplay;

    /// <summary>
    /// Populate chips.
    /// </summary>
    /// <param name="chip"></param>
    public void PopulateLoot(NewChip chip)
    {
        NewChip = chip; 
        lootDisplayName.SetText(NewChip.chipName);
        lootImage.sprite= NewChip.chipImage;

        KeepButton.onClick.AddListener(Keep);
        DropButton.onClick.AddListener(Drop);
    }
    /// <summary>
    /// Populate items
    /// </summary>
    /// <param name="item"></param>
    public void PopulateLoot(Item item)
    {
        NewItem = item;
        lootDisplayName.SetText(NewItem.itemName);
        lootImage.sprite= NewItem.itemImage;

        KeepButton.onClick.AddListener(Keep);
        DropButton.onClick.AddListener(Drop);
    }

    private void Keep()
    {
        if(NewChip != null)
            UiManager.Instance.SelectedChipToReplace(NewChip);
        else
            UiManager.Instance.AddItemToInventory(NewItem);
    }
    private void Drop()
    {
        animator=GetComponent<Animator>();

        animator.SetTrigger("Shrink");

        if(NewChip==null)
            UiManager.Instance.DropLoot(NewItem);
        else
            UiManager.Instance.DropLoot(NewChip);

        Destroy(this.gameObject,1f);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (chipinfoDisplay == null && newChip!=null)
        {

            chipinfoDisplay = Instantiate(ChipinfoPrefab, UiManager.Instance.transform);


            ChipInfoController controller = chipinfoDisplay.GetComponent<ChipInfoController>();

            StartCoroutine(DelayForAnimator(controller));
        }
        else if (gearInfoDisplay == null && newItem != null)
        {

            gearInfoDisplay = Instantiate(GearInfoPrefab, UiManager.Instance.transform);

            GearInfoController controller = gearInfoDisplay.GetComponent<GearInfoController>();

            StartCoroutine(DelayForAnimator(controller));
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (chipinfoDisplay != null && newChip != null)
        {
            ChipInfoController controller = chipinfoDisplay.GetComponent<ChipInfoController>();

            // Animate shrinking
            Vector3 targetPosition = this.transform.position;
            Vector3 startPosition = UiManager.Instance.transform.position;

            controller.Shrink(startPosition, targetPosition);

            // Optional: Delay destruction for animation
            Destroy(chipinfoDisplay, 1f);
        }
        else if (gearInfoDisplay != null && newItem != null)
        {
            GearInfoController controller = gearInfoDisplay.GetComponent<GearInfoController>();

            // Animate shrinking
            Vector3 targetPosition = this.transform.position;
            Vector3 startPosition = UiManager.Instance.transform.position;

            controller.Shrink(startPosition, targetPosition);

            // Optional: Delay destruction for animation
            Destroy(gearInfoDisplay, 1f);
        }
    }

    private IEnumerator DelayForAnimator(ChipInfoController controller)
    {
        yield return null;

        controller.ChipName.SetText(NewChip.chipName + " Chip");
        controller.ChipImage.sprite = NewChip.chipImage;
        controller.ChipType.SetText(NewChip.ChipType.ToString());
        controller.ChipDescription.SetText(NewChip.description);

        //Animate

        Vector3 targetPosition = UiManager.Instance.transform.position;
        Vector3 startPosition = this.transform.position;
        controller.Enlarge(startPosition, targetPosition);
    }
    private IEnumerator DelayForAnimator(GearInfoController controller)
    {
        yield return null;

        controller.GearName.SetText(newItem.itemName);
        controller.GearDescription.SetText(newItem.itemDescription);
        controller.GearImage.sprite = newItem.itemImage;
        controller.GearType.SetText(newItem.itemType.ToString());
        foreach (var effect in newItem.itemEffects)
        {
            GameObject temp = null;
            temp = Instantiate(EffectPrefab, controller.EffectsContainer.transform);
            temp.GetComponent<TextMeshProUGUI>().SetText(effect.ItemEffectDescription);
        }


        //Animate

        Vector3 targetPosition = UiManager.Instance.transform.position;
        Vector3 startPosition = this.transform.position;
        controller.Enlarge(startPosition, targetPosition);
    }
    void OnDestroy()
    {
        DropButton.onClick.RemoveListener(Drop);
        KeepButton.onClick.RemoveListener(Keep);

        if (chipinfoDisplay != null)
            Destroy(chipinfoDisplay);
        if(gearInfoDisplay != null) 
            Destroy(gearInfoDisplay);
    }
}
