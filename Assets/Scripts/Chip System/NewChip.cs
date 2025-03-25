using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NewChip : ScriptableObject
{
    

    protected bool isUpgraded = false;
    protected bool isActive;   
    protected int disableCounter;
    public GameObject ThisChip
    {
        get;
        set;
    }
    public enum ChipRarity
    {
        Basic,
        Common,
        Rare
    }
    public enum TypeOfChips
    {
        Default,
        Attack,
        Defense,
        Skill
    }
    /// <summary>
    /// This variable decides if the chip is isActive or inactive
    /// </summary>
    public bool IsActive
    {
        get
        {
            return isActive;
        }
        set
        {
            isActive = value;

            if (ThisChip != null)
            {
                Button button = ThisChip.GetComponent<Button>();
                if (button != null)
                {
                    button.interactable = value;
                }
                else
                {
                    Debug.LogError($"No Button component found on {ThisChip.name}.");
                }
            }
            else
            {
                Debug.LogError("ThisChip is not assigned.");
            }

            if (isActive)
                disableCounter = 0;
        }
    }
    /// <summary>
    /// Track how long a card is disabled.
    /// </summary>
    public int DisableCounter
    {
        get
        {
            return disableCounter;
        }
        set
        {
            disableCounter = value;
        }
    }
    /// <summary>
    /// What itemType is the chip.
    /// </summary>
    public TypeOfChips ChipType
    {
        get
        {
            return chipType;
        }
        protected set
        {
            chipType = value;
        }
    }
    /// <summary>
    /// Rariry of the card.
    /// </summary>
    public ChipRarity chipRarity;
    /// <summary>
    /// Name of card
    /// </summary>
    public string chipName;
    /// <summary>
    /// Description of card.
    /// </summary>
    [SerializeField]
    private string StartingDescription;
    /// <summary>
    /// Image for card.
    /// </summary>
    public Sprite chipImage;
    /// <summary>
    /// If this card can be upgraded.
    /// </summary>
    public bool canBeUpgraded;
    /// <summary>
    /// How much it would cost to upgrade this card
    /// </summary>
    public int costToUpgrade;
    /// <summary>
    /// Information to display to Player about card.
    /// </summary>
    public string ChipTip= "<#A20000>*Error missing from Chip Sect Corp database.*</color>";
    /// <summary>
    /// The weight in decimal representing a % that the selection is weighted by. For example 0.50 would be 50% chance of droping
    /// </summary>
    public float ChipRareityWeight;

    /// <summary>
    /// If Chip is upgraded.
    /// </summary>
    public virtual bool IsUpgraded
    {
        get
        {
            return isUpgraded;
        }
        set
        {
            if(canBeUpgraded)
                isUpgraded = value;
            else
            {
                //Give some feedback that chip can't be upgraded.
            }
        }
    }

    /// <summary>
    /// This chip hits all combatEnemies
    /// </summary>
    public bool hitAllTargets;
    /// <summary>
    /// Description of card.
    /// </summary>
    public string ChipDescription
    {
        get
        {
            return GetDynamicDescription();
        }
    }

    [SerializeField]
    private TypeOfChips chipType;

    [Header("Chip Sound")]
    public SoundFX ChipActivate;    

    void OnEnable()
    {
        IsUpgraded = false;
        isActive = false;
        disableCounter = 0;

        if (chipType == TypeOfChips.Default)
        {
            Debug.LogWarning($"{chipName} ChipType not set. Please check the specific chip script.");
        }
    }

    public virtual void OnChipPlayed(PlayerController player)
    {
        
    }

    public virtual void OnChipPlayed(PlayerController player, Enemy Target)
    {
        
    }

    public virtual void OnChipPlayed(PlayerController player, PuzzleRange Target)
    {

    }

    /// <summary>
    /// Any action chip needs to do at end of Turn.
    /// </summary>
    public virtual void EndRound()
    {
        if (IsActive)
        {
            DisableCounter++;

            if (DisableCounter >= 2)
            {
                IsActive = true;
            }
        }
    }
    /// <summary>
    /// Generate a description for chip.
    /// </summary>
    /// <returns></returns>
    public virtual string GetDynamicDescription()
    {
        string description = StartingDescription;

        switch (ChipType) {
            case TypeOfChips.Attack:
                if (this is AttackChip attackChip)
                {
                    int baseDmg = attackChip.damage;
                    int upgradedDmg = attackChip.damage + attackChip.upgradedDamageByAmount;

                    if (IsUpgraded)
                        description += $"\nDeals {baseDmg} damage";
                    else
                        description += $"\nDeals {upgradedDmg} damage";                    

                    if (attackChip.debuffStacks > 0)
                    {
                        description += $"\nApplies {attackChip.debuffToApply} ({attackChip.debuffStacks} stacks)";
                        if (attackChip.IsUpgraded)
                            description += $"{attackChip.debuffStacks + attackChip.upgradedDebuffStacksByAmout} stacks";
                    }
                }
                break;
            case TypeOfChips.Defense:
                if (this is DefenseChip defenseChip)
                {
                    int baseShield = defenseChip.shieldAmount;
                    int upgradedShield = baseShield + defenseChip.upgradedShieldAmountToApply;

                    if (IsUpgraded)
                        description += $"\nGrants {upgradedShield} Shield";
                    else
                        description += $"\nGrants {baseShield} Shield";                    

                    if (defenseChip.buffToApply != Effects.Buff.None)
                    {
                        description += $"\nGrants {defenseChip.buffStacks} {defenseChip.buffToApply}";
                        if (defenseChip.IsUpgraded)
                            description += $"{defenseChip.buffStacks + defenseChip.upgradedBuffStacksByAmout}";
                    }

                    if (defenseChip.deBuffsToRemove.Count > 0)
                    {
                        description += $"\nRemoves {defenseChip.deBuffsToRemove[0].debuffType}";
                        if (defenseChip.IsUpgraded && defenseChip.deBuffsToRemove.Count > 1)
                            description += $"\nAlso removes {defenseChip.deBuffsToRemove[1].debuffType}";
                    }
                }
                break;
            case TypeOfChips.Skill:
                if (this is SkillChip skillChip && skillChip.specialEffect != null)
                {
                    description += $"\n{skillChip.specialEffect.name}";
                    if (skillChip.IsUpgraded)
                        description += $"\n(Upgraded: +{skillChip.specialEffect.amountToUpgradeBy})";
                }
                break;
            default:
                description += "mistake was made.";
                break;
        }

        return description.Trim();
    }

    void OnValidate()
    {
        Debug.Log($"OnValidate called for {name}. ChipType: {chipType}");
    }


}
