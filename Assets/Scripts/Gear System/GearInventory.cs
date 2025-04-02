using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GearInventory : MonoBehaviour, IPointerClickHandler
{
    public enum Mode
    {
        None,
        Inventory,
        Terminal
    }

    public Image GearImage;
    public TextMeshProUGUI GearName;
    public TextMeshProUGUI GearDescription;
    public Button EquipButton;
    public Button ScrapButton;
    public GameObject GearInfoPrefab;
    public GameObject EffectPrefab;
    public GameObject ConfirmationWindow;
    public Mode PrefabMode
    {
        get
        {
            return mode;
        }
        set
        {
            mode = value;
        }
    }

    private GameObject gearInfoDisplay;
    private Mode mode;

    public Item Item
    {
        get
        {
            return item;
        }
        set
        {
            item = value;
        }
    }
    private Item item;
    void Start()
    {
        GearImage.sprite = Item.itemImage;
        GearName.SetText(Item.itemName+" - "+Item.ItemTeir);        

        switch(PrefabMode)
        {
            case Mode.Inventory:
                GearDescription.SetText(Item.itemDescription);
                EquipButton.gameObject.SetActive(true);
                EquipButton.interactable = !Item.IsEquipped;
                EquipButton.onClick.AddListener(EquipGear);
                break;
            case Mode.Terminal:
                GearDescription.SetText("Scrap Value: " + Item.GetScrapValue());
                ScrapButton.gameObject.SetActive(true);
                ScrapButton.onClick.AddListener(VerifyScrapItem);
                if (Item.IsEquipped)
                    ScrapButton.interactable = false;
                break;
            default:
                Debug.LogWarning("Prefab Mode not set!!");
                break;
        }        
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            GameObject infoInScene = GameObject.FindGameObjectWithTag("Info");
            if (gearInfoDisplay == null && infoInScene == null)
            {

                gearInfoDisplay = Instantiate(GearInfoPrefab, UiManager.Instance.transform);

                GearInfoController controller = gearInfoDisplay.GetComponent<GearInfoController>();

                controller.SetUpGearInfo(Item);

                controller.animator.SetBool("IsEnlarging", true);
                controller.animator.SetBool("IsShrinking", false);

                controller.TargetPosition = this.transform.position;
                controller.StartPosition = UiManager.Instance.transform.position;
            }
        }
    }
    private void EquipGear()
    {
        if (GearManager.Instance.EquipGear(Item))
            UiManager.Instance.RefreshInventory();
        else
            Debug.LogWarning("UnExpected error here.");
    }
    /// <summary>
    /// First Confirm with user. Using the Confirmation Window.
    /// </summary>
    private void VerifyScrapItem()
    {
        UiManager.Instance.PopUpMessage("You are about to scrap the item "
            + item.itemName +
            " for the scrap value of "
            + item.GetScrapValue(), ConfirmScrap);       
    }
    private void ConfirmScrap()
    {
        //Add scrap to player
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().GainScrap(Item.GetScrapValue());

        //Remove item from Gear
        GearManager.Instance.RemoveItem(Item);

        //Update player scrap
        UiManager.Instance.UpdateScrapDisplay(GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().Scrap);

        //Update the Item Terminal
        UiManager.Instance.UpdateItemsInTermianl();
    }
    void OnDestroy()
    {
        EquipButton.onClick.RemoveAllListeners();
        ScrapButton.onClick.RemoveAllListeners();

        if (gearInfoDisplay != null)
            Destroy(gearInfoDisplay);
    }
}
