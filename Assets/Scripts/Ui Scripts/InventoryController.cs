using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class InventoryController : UiController
{
    public Button CloseButton;

    [Header("Chip Stuff")]
    public Button ChipButton;
    public GameObject InventoryChipPrefab;
    public GameObject ChipDisplay;
    public GameObject AttackChipContainer;
    public GameObject DefenseChipContainer;
    public GameObject SkillChipContainer;

    [Header("Gear Stuff")]
    public Button GearButton;
    public GameObject GearPrefab;
    public GameObject GearDisplay;
    public GameObject WeaponGearContainer;
    public GameObject ArmorGearContainer;
    public GameObject EquipmentGearContainer;

    // Start is called before the first frame update
    void Start()
    {
        CloseButton.onClick.AddListener(Close);
        ChipButton.onClick.AddListener(SwitchTabs);
        GearButton.onClick.AddListener(SwitchTabs);
    }

    public override void Initialize()
    {
        Debug.Log("Inventory Ui initialized");
        //Populate chips first
        PopulateChips();
    }
    public void RefreshCurrentInventory()
    {
        if (GearDisplay.activeInHierarchy)
        {
            ClearContainer(WeaponGearContainer);
            ClearContainer(ArmorGearContainer);
            ClearContainer(EquipmentGearContainer);
            PopulateGear();
        }
        else
        {
            ClearContainer(AttackChipContainer);
            ClearContainer(DefenseChipContainer);
            ClearContainer(SkillChipContainer);
            PopulateChips();            
        }
    }

    private void SwitchTabs()
    {
        if (ChipDisplay.activeInHierarchy)
        {
            ChipDisplay.SetActive(false);
            ChipButton.interactable = true;

            GearDisplay.SetActive(true);
            GearButton.interactable=false;

            //Delete children first
            ClearContainer(WeaponGearContainer);
            ClearContainer(ArmorGearContainer);
            ClearContainer(EquipmentGearContainer);


            PopulateGear();
        }
        else
        {
            ChipDisplay.SetActive(!ChipDisplay.activeInHierarchy);
            ChipButton.interactable = false;

            GearDisplay.SetActive(!GearDisplay.activeInHierarchy);
            GearButton.interactable = true;

            ClearContainer(AttackChipContainer);
            ClearContainer(DefenseChipContainer);
            ClearContainer(SkillChipContainer);

            PopulateChips();
        }
    }
    /// <summary>
    /// Populate chips in Inventory
    /// </summary>
    private void PopulateChips()
    {
        List<NewChip> AllChips = new List<NewChip>();
        AllChips.AddRange(ChipManager.Instance.PlayerDeck);
        AllChips.AddRange(ChipManager.Instance.PlayerHand);

        foreach (NewChip chip in AllChips)
        {
            GameObject temp = Instantiate(InventoryChipPrefab);
            switch (chip.ChipType)
            {
                case NewChip.TypeOfChips.Attack:
                    temp.transform.SetParent(AttackChipContainer.transform, false);                    
                    break;
                case NewChip.TypeOfChips.Defense:
                    temp.transform.SetParent(DefenseChipContainer.transform, false);
                    break;
                case NewChip.TypeOfChips.Skill:
                    temp.transform.SetParent(SkillChipContainer.transform, false);
                    break;
            }
            temp.GetComponent<Chip>().Mode = Chip.ChipMode.Inventory;
            temp.GetComponent<Chip>().NewChip = chip;
        }
    }
    private void PopulateGear()
    {        
        foreach (Item item in GearManager.Instance.PlayerCurrentGear)
        {
            GameObject temp = null;

            switch (item.itemType)
            {
                case Item.ItemType.Weapon:
                    temp = Instantiate(GearPrefab, WeaponGearContainer.transform);                    
                    break;
                case Item.ItemType.Armor:
                    temp = Instantiate(GearPrefab, ArmorGearContainer.transform);
                    break;
                case Item.ItemType.Equipment:
                    temp = Instantiate(GearPrefab, EquipmentGearContainer.transform);
                    break;
            }
            temp.GetComponent<GearInventory>().PrefabMode=GearInventory.Mode.Inventory;
            temp.GetComponent<GearInventory>().Item = item;
            
        }
    }

    /// <summary>
    /// Clears container.
    /// </summary>
    /// <param name="container"></param>
    private void ClearContainer(GameObject container)
    {
        foreach (Transform child in container.transform)
        {
            Destroy(child.gameObject);
        }
    }
    /// <summary>
    /// Close Inventory and switch back to roaming Ui.
    /// </summary>
    private void Close()
    {
        GameManager.Instance.UpdateGameMode(GameManager.GameMode.Roaming);
    }
}
