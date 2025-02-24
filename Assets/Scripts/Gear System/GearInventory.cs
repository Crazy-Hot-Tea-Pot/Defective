using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GearInventory : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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
        GearName.SetText(Item.itemName);        

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
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (gearInfoDisplay == null)
        {

            gearInfoDisplay = Instantiate(GearInfoPrefab, UiManager.Instance.transform);

            GearInfoController controller = gearInfoDisplay.GetComponent<GearInfoController>();

            StartCoroutine(DelayForAnimator(controller));
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (gearInfoDisplay != null)
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
    private IEnumerator DelayForAnimator(GearInfoController controller)
    {
        yield return null;

        controller.GearName.SetText(Item.itemName);
        controller.GearDescription.SetText(Item.itemDescription);
        controller.GearImage.sprite = Item.itemImage;
        controller.GearType.SetText(Item.itemType.ToString());
        foreach(var effect in Item.itemEffects)
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
        GameObject temp = Instantiate(ConfirmationWindow,UiManager.Instance.transform);
        temp.GetComponent<ConfirmationWindow>().SetUpComfirmationWindow("You are about to scrap the item " 
            + item.itemName + 
            " for the scrap value of "
            + item.GetScrapValue(),ConfirmScrap);        
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
