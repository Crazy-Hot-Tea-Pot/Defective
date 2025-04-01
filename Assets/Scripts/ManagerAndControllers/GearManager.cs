using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearManager : MonoBehaviour
{
    public static GearManager Instance
    {
        get
        {
            return instance;
        }
        private set
        {
            instance = value;
        }
    }

    /// <summary>
    /// What the Player starts with in a new game.
    /// </summary>
    [SerializeField]
    private List<Item> startingGear = new List<Item>();

    /// <summary>
    /// Making it read only to prevent future problems
    /// </summary>
    public IReadOnlyList<Item> StartingGear => startingGear;

    /// <summary>
    /// List of Gear the Player current has.
    /// Max is 10.
    /// </summary>
    public List<Item> PlayerCurrentGear = new List<Item>();

    /// <summary>
    /// All gear in the game. Not what the Player has.
    /// </summary>
    public List<Item> AllGear = new List<Item>();

    private static GearManager instance;
    [SerializeField]
    private int gearLimit = 10;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        LoadAllGear();

        GameManager.Instance.OnStartCombat += StartCombat;
        GameManager.Instance.OnEndCombat += EndCombat;
        GameManager.Instance.OnSceneChange += SceneChange;
    }
    /// <summary>
    /// Add item to Player gear.
    /// </summary>
    /// <param name="newItem">Clone Of item Only!</param>
    public bool Acquire(Item newItem)
    {
        // Check if the player already has this item
        Item existingItem = PlayerCurrentGear.Find(item => item.itemName == newItem.itemName && item.ItemTeir != Item.Teir.Platinum);

        if (existingItem != null)
        {
            
            UpgradeItem(existingItem);                      
            return true;
        }
        else
        {
            // If inventory isn't full, add the item
            if (PlayerCurrentGear.Count < gearLimit)
            {
                PlayerCurrentGear.Add(newItem);
                newItem.IsPlayerOwned = true;
                return true;
            }
            else
            {
                UiManager.Instance.PopUpMessage("Inventory full! Cannot acquire new item.");
                return false;
            }
        }
    }

    /// <summary>
    /// Remove item from Player inventory
    /// </summary>
    /// <param name="itemToRemove"></param>
    public void RemoveItem(Item itemToRemove)
    {
        if(itemToRemove == null) 
            return;

        itemToRemove.IsPlayerOwned = false;
        itemToRemove.IsEquipped = false;

        PlayerCurrentGear.Remove(itemToRemove);
    }

    /// <summary>
    /// Return all gear by itemType
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public List<Item> GetIAllGearByType(Item.ItemType type)
    {
        return AllGear.FindAll(item => item.itemType == type);
    }

    /// <summary>
    /// Equip gear.
    /// Check if the item itemType is already equipped.
    /// </summary>
    /// <param name="item"></param>
    public bool EquipGear(Item item)
    {
        if (item == null)
        {
            return false;
        }

        // Check if an item of the same itemType is already equipped
        foreach (Item itemCheck in PlayerCurrentGear)
        {
            if (itemCheck.itemType == item.itemType && itemCheck.IsEquipped)
            {
                // Unequip the currently equipped item
                itemCheck.IsEquipped = false;
                break;
            }
        }

        // Equip the new item
        item.IsEquipped = true;

        return true;
    }

    /// <summary>
    /// Unequip an item of a specific itemType.
    /// </summary>
    public bool UnequipItem(Item.ItemType type)
    {
        foreach (Item item in PlayerCurrentGear)
        {
            if (item.itemType == type && item.IsEquipped)
            {
                item.IsEquipped = false;

                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Get currently equipped item by itemType.
    /// </summary>
    public Item GetEquippedItem(Item.ItemType type)
    {
        foreach (Item item in PlayerCurrentGear)
        {
            if (item.itemType == type && item.IsEquipped)
            {
                return item;
            }
        }
        return null;
    }
    public void ResetAllGear()
    {
        foreach (var item in AllGear)
        {
            item.ResetToDefault();
        }
    }

    /// <summary>
    /// Load all gear from scriptables
    /// </summary>
    private void LoadAllGear()
    {
        // Load all Item ScriptableObjects in the Resources folder
        AllGear = new List<Item>(Resources.LoadAll<Item>("Scriptables/Gear"));       
    }

    /// <summary>
    /// Upgrade item tear.
    /// </summary>
    /// <param name="item"></param>
    private void UpgradeItem(Item item)
    {
        // Max Tier is Platinum (Tier 4)
        if (item.ItemTeir < Item.Teir.Platinum)
        {
            // Increase tier by one
            item.UpgradeTier();
            UiManager.Instance.PopUpMessage($"{item.itemName} combined to increase to teir {item.ItemTeir}");            
        }
        else
        {
            Debug.Log($"{item.itemName} is already at max tier!");
        }
    }


    private void StartCombat()
    {

    }

    private void EndCombat()
    {

    }

    private void SceneChange(Levels newLevel)
    {
        switch (newLevel)
        {
            case Levels.Title:
                PlayerCurrentGear.Clear();
                break;
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStartCombat -= StartCombat;
            GameManager.Instance.OnEndCombat -= EndCombat;
            GameManager.Instance.OnSceneChange -= SceneChange;
        }
    }
    #region Cheats
    //Cheats
    [ContextMenu("Give Random Gear")]
    private void GiveRandomGear()
    {
        if (AllGear.Count == 0)
        {
            Debug.LogWarning("No gear available to give.");
            return;
        }

        // Select a random item from AllGear
        Item randomItem = AllGear[Random.Range(0, AllGear.Count)];

        // Clone the item to ensure we don't modify the original
        Item newItem = Instantiate(randomItem);

        // Try to acquire the item
        if (Acquire(newItem))
        {
            Debug.Log($"Gave player random gear: {newItem.itemName}");
        }
    }
    [ContextMenu("Give Lucky Trinket")]
    private void GiveluckyTrinket()
    {
        Item item = AllGear.Find(e => e.itemName == "Lucky Trinket");

        Item newItem = Instantiate(item);

        // Try to acquire the item
        if (Acquire(newItem))
        {
            Debug.Log($"Gave player random gear: {newItem.itemName}");
        }
    }
    #endregion
}
