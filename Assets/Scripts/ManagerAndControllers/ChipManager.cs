using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class managers all things chip related.
/// </summary>
public class ChipManager : MonoBehaviour
{
    public static ChipManager Instance
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
    private List<NewChip> startingChips = new();
    /// <summary>
    /// Making it read only to prevent future problems
    /// </summary>
    public IReadOnlyList<NewChip> StartingChips => startingChips;

    /// <summary>
    /// Chips in the players hand.
    /// </summary>
    public List<NewChip> PlayerHand = new();

    /// <summary>
    /// Chips in the players deck
    /// </summary>
    public List<NewChip> PlayerDeck
    {
        get
        {
            return playerDeck;
        }
        private set
        {
            playerDeck = value;
        }
    }

    /// <summary>
    /// Chips Player has used.
    /// </summary>
    public List<NewChip> UsedChips = new();

    /// <summary>
    /// All the chips in the whole game.
    /// </summary>
    public List<NewChip> AllChips
    {
        get
        {
            return allChips;
        }
        private set
        {
            allChips = value;
        }
    }

    public GameObject chipPrefab;

    [SerializeField]
    private int deckLimit = 8;

    [SerializeField]
    private int handLimit = 4;

    private static ChipManager instance;
    private List<NewChip> playerDeck = new();
    private List<NewChip> allChips = new();

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
        Initalize();
        GameManager.Instance.OnStartCombat += StartCombat;
        GameManager.Instance.OnEndCombat += EndCombat;
        GameManager.Instance.OnSceneChange += SceneChange;
    }

    void Initalize()
    {
        LoadAllChips();
    }
    
    /// <summary>
    /// Add chip to deck
    /// </summary>
    /// <param name="CloneOfNewChip">Clone Before Sending!</param>
    public bool AddNewChipToDeck(NewChip CloneOfNewChip)
    {
        if (PlayerDeck.Count < deckLimit)
        {

            PlayerDeck.Add(CloneOfNewChip);
            return true;
        }
        else
        {
            Debug.LogError("Deck full");
            return false;
        }

    }

    /// <summary>
    /// Draws a chip from playerDeck.
    /// </summary>
    /// <param name="draws"></param>
    public void RefreshPlayerHand()
    {
        if (PlayerDeck.Count == 0)
        {
            Debug.LogWarning("Deck is empty!");
            return;
        }

        int tempChipsToDraw = handLimit - PlayerHand.Count;

        while (tempChipsToDraw > 0 && PlayerDeck.Count > 0)
        {
            PlayerHand.Add(PlayerDeck[0]);
            PlayerDeck.RemoveAt(0);
            tempChipsToDraw--;
        }
    }

    /// <summary>
    /// Add chip to use chips.
    /// </summary>
    /// <param name="chipObject"></param>
    public void AddToUsedChips(GameObject chipObject)
    {
        Chip chipComponent = chipObject.GetComponent<Chip>();
        if (chipComponent != null)
        {
            UsedChips.Add(chipComponent.NewChip);
            PlayerHand.Remove(chipComponent.NewChip);
        }

        Destroy(chipObject);
    }

    /// <summary>
    /// Removes Chip from Player deck.
    /// </summary>
    /// <param name="chip"></param>
    public void DeleteChip(NewChip chip)
    {
        // Removes chip from the Player's hand and places it in the used chips
        if (PlayerDeck.Contains(chip))
        {
            PlayerDeck.Remove(chip);
        }
        else
        {
            Debug.LogWarning("[ChipManager] Chip not found in Deck.");
        }
    }

    /// <summary>
    /// Get all chips of a specific type.
    /// </summary>
    /// <param name="chipType">The type of chips to filter by.</param>
    /// <returns>A list of chips matching the specified type.</returns>
    public List<NewChip> GetChipsByType(NewChip.TypeOfChips chipType)
    {
        return AllChips.FindAll(chip => chip.ChipType == chipType);
    }

    /// <summary>
    /// Get all chips of a specific rarity.
    /// </summary>
    /// <param name="chipRarity">The rarity of chips to filter by.</param>
    /// <returns>A list of chips matching the specified rarity.</returns>
    public List<NewChip> GetChipsByRarity(NewChip.ChipRarity chipRarity)
    {
        return AllChips.FindAll(chip => chip.chipRarity == chipRarity);
    }


    /// <summary>
    /// Load all Chip ScriptableObjects in the Resources folder.
    /// </summary>
    private void LoadAllChips()
    {
        AllChips.Clear();       
        AllChips = new List<NewChip>(Resources.LoadAll<NewChip>("Scriptables/Chips"));
    }
    private void StartCombat()
    {
        UsedChips.Clear();
        ShufflePlayerDeck();
    }
    private void ShufflePlayerDeck()
    {
        if (PlayerDeck.Count > 1)
        {
            for (int i = 0; i < PlayerDeck.Count; i++)
            {
                int randomIndex = Random.Range(0, PlayerDeck.Count);
                NewChip temp = PlayerDeck[randomIndex];
                PlayerDeck[randomIndex] = PlayerDeck[i];
                PlayerDeck[i] = temp;
            }
        }
    }
    private void EndCombat()
    {
        // Move unused chips back to the deck
        foreach (var chip in PlayerHand)
        {
            PlayerDeck.Add(chip);
        }
        PlayerHand.Clear();

        // Move used chips back to the deck
        foreach (var chip in UsedChips)
        {
            PlayerDeck.Add(chip);
        }
        UsedChips.Clear();

        ShufflePlayerDeck();
        Debug.Log("[ChipManager] Combat ended. Deck shuffled.");
    }
    private void SceneChange(Levels newLevel)
    {

        switch (newLevel)
        {
            case Levels.Title:
                PlayerHand.Clear();
                PlayerDeck.Clear();
                UsedChips.Clear();

                break;
            case Levels.Tutorial:
            case Levels.Level2:
            case Levels.Level3:
            case Levels.Level4:
                ShufflePlayerDeck();
                break;
        }
    }
    private void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStartCombat -= StartCombat;
            GameManager.Instance.OnEndCombat -= EndCombat;
            GameManager.Instance.OnSceneChange -= SceneChange;
        }
    }
}
