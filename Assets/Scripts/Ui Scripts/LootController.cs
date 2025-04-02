using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LootController : MonoBehaviour, IPointerClickHandler
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
        if (NewChip != null)
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
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (chipinfoDisplay == null && newChip != null)
            {

                chipinfoDisplay = Instantiate(ChipinfoPrefab, UiManager.Instance.transform);


                ChipInfoController controller = chipinfoDisplay.GetComponent<ChipInfoController>();
                
                controller.SetUpChipInfo(newChip);

                controller.animator.SetBool("IsEnlarging", true);
                controller.animator.SetBool("IsShrinking", false);

                controller.TargetPosition = this.transform.position;
                controller.StartPosition = UiManager.Instance.transform.position;
            }
            else if (gearInfoDisplay == null && newItem != null)
            {

                gearInfoDisplay = Instantiate(GearInfoPrefab, UiManager.Instance.transform);

                GearInfoController controller = gearInfoDisplay.GetComponent<GearInfoController>();                
                
                controller.SetUpGearInfo(NewItem);

                controller.animator.SetBool("IsEnlarging", true);
                controller.animator.SetBool("IsShrinking", false);

                controller.TargetPosition = this.transform.position;
                controller.StartPosition = UiManager.Instance.transform.position;
            }
        }
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
